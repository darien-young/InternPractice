/* Phase 3.5 - Log Creation
 * 
 * Comments:
     *Good improvement: logs are now file-only, snapshot events log correctly, and the audit trail captures key actions (start/add/snapshot/attempted add). 
     *Your log captures the prompt event, but it doesn’t capture the decision outcome. In this run, Fiona was cancelled — we need a log line for cancel (I haven't tested replace).
     *
     *Example:  2026-02-04 08:20:45AM - Snapshot Requested: Infant -> (empty),Child -> [Lianna],Teenager -> [Neji],Young Adult -> [Dar],Adult -> [Drew],Senior -> [George]
                2026-02-04 08:20:55AM - User attempted to add 'Kyle' (Aged 15) to 'Teenager' — existing entries detected; prompting add/replace/cancel.
                2026-02-04 08:20:59AM - User added 'Kyle' (Aged 15) to 'Teenager ' Category.
                2026-02-04 08:21:19AM - User attempted to add 'Fiona' (Aged 26) to 'Adult' — existing entries detected; prompting add/replace/cancel.
                2026-02-04 08:21:37AM - User added 'Sam' (Age 2) to 'Infant' Category.

    *Next additions:
    *Split output into two files: ProgramLog.txt for event logs and CategorySnapshots.txt for snapshot reports.
    *Standardize log wording (use one “Age X” format, and always use 'PrettyCategoryName'; remove the extra space in 'Teenager ' Category).
    *Add explicit log lines on exit and on completion, plus “Final Snapshot: …” so the audit trail is complete even without the snapshot report block.

   
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace InternConsoleApp
{
    class Program
    {
        //log file name used on Desktop
        private const string LogFileName = "CategorySnapshot.txt";
        private const string SnapshotFileName = "CategorySnapshot.txt";
        private const bool EchoLogsToConsole = false;


        //------------------------------- MAIN METHOD -------------------------------------//
        static void Main(string[] args)
        {
           
                               // adding parse function to connect array to already existing enum.
           var categories = Enum.GetNames(typeof(AgeCategory))
                                 .Select(n => Enum.Parse<AgeCategory>(n))
                                 .ToArray();
            var assigned = categories.ToDictionary(c => c, c => new List<string>());  

            // Thought it'd be best to make it clear for the end user.
            Console.WriteLine("Welcome to the Age Category Assigner!");
            Console.WriteLine("You will be prompted to enter people until every age category has a name assigned.");

            //Log program start
            AppendLog("\n\n User Started Program.");

            //Initial Menu Prompt
            int startMenuChoice = PromptMenuChoice(assigned);
            if (startMenuChoice == 3)
            {
                Console.WriteLine("\nExiting early. Current Snapshot: ");
                PrintSnapshot(assigned);
                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            bool exitRequested = false;

            //MAIN LOOP: continue until all categories are assigned or exit is requested
            while (assigned.Any(kv => kv.Value.Count == 0) && !exitRequested)
            {

                //menuChoice == 1 - proceed to collect person data
                string name = PromptName();
                // get birth year (and validate it)
                int birthYear = PromptBirthYear();

                //Assignment Attempt
                var result = TryAssignPerson(name, birthYear, assigned);
                
                if (result == AssignResult.Exit) 
                {  
                    exitRequested = true; 
                    break; 
                }
                if (result == AssignResult.Decline) 
                {
                    
                    continue;
                } //returns to menu


                

               
                    //if all categories assigned after this, break to avoid extra prompt
                    if (!assigned.Any(kv => kv.Value.Count == 0))
                    {
                        //all categories assigned - break birthyear loop and let outer loop end
                        break;
                    }

                    //SHOW MENU after each successful assignment
                    int postMenuChoice = PromptMenuChoice(assigned);
                    if (postMenuChoice == 3)
                    {
                        exitRequested = true;
                        break;
                    }
                    //otherwise continue to next loop iteration
            }

            //after loop ends, either all categories assigned or exit requested
            if (exitRequested)
            {
                Console.WriteLine("\nExiting early. Current Snapshot: ");
                PrintSnapshot(assigned);

                // write final snapshot to text file on early exit
                PrintSnapshotToFile(assigned, "CategorySnapshot.txt");

                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            //After all categories are assigned
            Console.WriteLine("\nAll age categories filled. Final Snapshot: \n");
            PrintSnapshot(assigned);

            //write final snapshot to text file
            PrintSnapshotToFile(assigned, "CategorySnapshot.txt");

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            return; // Added return statement to explicitly indicate the end of Main method
        }
        //-------------------------------- END OF MAIN METHOD --------------------------------//



        //-------------------------------- HELPER FUNCTIONS ----------------------------------//

        // ENUM FOR AGE CATEGORIES
        enum AgeCategory
        {
            Infant, //0-2
            Child, //3-12  
            Teenager,//13-17
            YoungAdult,//18-24
            Adult,//25-64
            Senior//65+
        }

        // ASSIGNMENT RESULT ENUM
        private enum AssignResult { Assigned, Decline, Exit}

        //ASSIGN PERSON FUNCTION -- Now handles multiple names per category
        // and prints the category-dependent message
        // If category already assigned, user can add, replace , or cancel
        private static AssignResult TryAssignPerson(string name, int birthYear, Dictionary<AgeCategory, List<string>> assigned)
        {
            int age = CalculateAge(birthYear);
            AgeCategory category = GetCategory(age);

            var list = assigned[category];

            if (list.Count > 0)
            {
                //Logging when existing category is detected
                AppendLog($"User attempted to add '{name}' (Aged {age}) to '{PrettyCategoryName(category)}' — existing entries detected; prompting add/replace/cancel.");
                //delegate overwrite processing to separate function
                var overwriteResult = ProcessExistingCategory(name, category, list, assigned, age);
                if (overwriteResult != AssignResult.Assigned)
                {
                    //either Decline (go back to menu) or Exit (user chose exit)
                    return overwriteResult;
                }
            
            }
            else
            {
                //no existing names, just add
                list.Add(name);
                AppendLog($"User added '{name}' (Age {age}) to '{PrettyCategoryName(category)}' Category.");
            }

            //Age Category Message now sepparated into separate function
            Console.WriteLine();
            PrintCategoryMessage(category, name);


            // let caller print messages based on category
            return AssignResult.Assigned;
        }


        //EXISTING CATEGORY PROCESSING FUNCTION - handles add/replace/cancel logic
        private static AssignResult ProcessExistingCategory(string name, AgeCategory category, List<string> list, Dictionary<AgeCategory, List<string>> assigned, int age)
        { 
            Console.WriteLine($"The category {category} already has {list.Count} entr{(list.Count == 1 ? "y" : "ies")}: {string.Join(",", list)}");
            Console.WriteLine("Choose: (a)dd this name, (r)replace all names with this name, or (c)ancel to main menu");
            string choice = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            
            if (choice == "c" || choice == "cancel")
            {
                Console.WriteLine("Canceled. Returning to menu...");
                // fallback to menu
                int fallback = PromptMenuChoice(assigned);
                if (fallback == 3) return AssignResult.Exit;
                return AssignResult.Decline;
            }
            
            if (choice == "r" || choice == "replace")
            {
                list.Clear();
                list.Add(name);
                AppendLog($"User added '{name}' (Aged {age}) to '{PrettyCategoryName(category)} ' Category.");
                return AssignResult.Assigned;
            }
            else if (choice == "a" || choice == "add")
            {
                list.Add(name);
                AppendLog($"User added '{name}' (Aged {age}) to '{PrettyCategoryName(category)} ' Category.");
                return AssignResult.Assigned;
            }
            else
            {
                Console.WriteLine("Invalid Choice. Returning to menu...");
                    // fallback to menu
                    int fallback = PromptMenuChoice(assigned);
                    if (fallback == 3) return AssignResult.Exit;
                    return AssignResult.Decline;
        }
    }

        //CATEGORY MESSAGE FUNCTION - Specified category message is now its own function for clarity
        private static void PrintCategoryMessage(AgeCategory category, string name)
        {
            switch (category)
            {
                case AgeCategory.Infant:
                    Console.WriteLine($"Googoo, {name},\ngoo ga goo goo googooga.");
                    break;
                case AgeCategory.Child:
                    Console.WriteLine($"Hi, {name},\nlet's go play outside.");
                    break;
                case AgeCategory.Teenager:
                    Console.WriteLine($"Yo, {name},\nlet's go to high school.");
                    break;
                case AgeCategory.YoungAdult:
                    Console.WriteLine($"Hey, {name},\nlet's go out for a drink");
                    break;
                case AgeCategory.Adult:
                    Console.WriteLine($"Hello, {name},\nlet's go do our taxes.");
                    break;
                case AgeCategory.Senior:
                    Console.WriteLine($"Good day, {name},\nlet's go write our will!");
                    break;
            }
        }


        // MENU FUNCTION -- Shows menu. if user selects 2, shows snapshot and re-prompts. returns 1 or 3.
        private static int PromptMenuChoice(Dictionary<AgeCategory, List<string>> assigned)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Press 1 to Continue, 2 to View a Snapshot of the Category List, and 3 to Exit.");
                string postChoice = (Console.ReadLine() ?? "").Trim();

                if (postChoice == "1")
                {
                    return 1; //continue requested
                }
                else if (postChoice == "2")
                {
                    PrintSnapshot(assigned);
                    //Log snapshot request
                    AppendLog($"Snapshot Requested: {SnapshotForLog(assigned)}");
                    //loop back to menu after showing snapshot
                    continue;

                                    }
                else if (postChoice == "3")
                {
                    return 3; //exit requested
                }
                else
                {
                    Console.WriteLine("Invalid Input. Please enter 1, 2, or 3.");
                }
            }
        }

        //NAME FUNCTION -- Function to prompt for name inputs
        private static string PromptName()
        {
            while (true)
            {
                // Ask for user's name
                Console.Write("\nEnter your name: ");
                string nameInput = (Console.ReadLine() ?? "").Trim();

                //Checking for only letter inputs
                if (string.IsNullOrWhiteSpace(nameInput) || !nameInput.All(char.IsLetter))
                {
                    Console.WriteLine("Invalid Input. Please use letters only (no numbers or symbols)");
                    continue;
                }

                return nameInput;
            }
        }

        //CALCULATE AGE FUNCTION -- Function to calculate age from birth year
        private static int CalculateAge(int birthYear)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            int age = currentDate.Year - birthYear;
            return age;
        }

        //BIRTH YEAR FUNCTION -- Function to prompt for birth year input
        private static int PromptBirthYear()
        {
        var currentYear = DateTime.Now.Year;
        while (true)
        {
            // Ask for user's age
            Console.Write("Enter your Birth Year: ");
            String BirthYearInput = (Console.ReadLine() ?? string.Empty).Trim();

            // Parse BirthYear string to int safely
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

            if (!int.TryParse(BirthYearInput, out int BirthYear))
            {
                Console.WriteLine("Invalid Input. Please use Numbers Only (No letters or symbols)");
                continue;
            }

            if (BirthYear > currentYear)
            {
               Console.WriteLine("Invalid Input. This year has not happened yet -_-");
                continue;
            }

            if (BirthYear < currentYear - 130)
            {
                Console.WriteLine("Invalid Input. No one lives that long these days.");
                continue;
            }

            //This is of course assuming the user's birthday is jan 1st, since we only ask for year
            return BirthYear;
        }

    }

        // SNAPSHOT FUNCTION to print current snapshot of assigned categories
        private static void PrintSnapshot(Dictionary<AgeCategory,List<string>> assigned)
        {
            Console.WriteLine("\n -- Category Snapshot --");
            foreach (var kv in assigned)
            {
                string assignedName = kv.Value.Count == 0 ? "(empty)" : string.Join(",", kv.Value);
                Console.WriteLine($"{(int)kv.Key}: {kv.Key} => {assignedName}");
            }
        }

        //PRINT TO TEXTFILE FUNCTION -- Function to print snapshot to text file
        private static void PrintSnapshotToFile(Dictionary<AgeCategory,List<string>> assigned, string fileName)
        {
            try
            {
                // Get desktop path
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string fullPath = Path.Combine(desktop, fileName);

                var lines = new List<string>();

                // add separator when file already exists
                if (File.Exists(fullPath))
                {
                    lines.Add(string.Empty);
                    lines.Add("\n------------------------------\n");
                }

                lines.Add($"-- Category Snapshot ({DateTime.Now:yyyy-MM-dd HH:mm:ss}) --");
                foreach (var kv in assigned)
                {
                    string assignedName = kv.Value.Count == 0 ? "(empty)" : string.Join(",", kv.Value);
                    lines.Add($"{(int)kv.Key}: {kv.Key} => {assignedName}");
                }
                //now appends instead of ovewriting; REMOVED console writing message.
                File.AppendAllLines(fullPath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing snapshot to file: {ex.Message}");
            }
        }
        
        //APPENDLOG FUNCTION: a single log line (timestamped to the log file on the Desktop
        private static void AppendLog(string message)
        {
            try
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string fullPath = Path.Combine(desktop, LogFileName);
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:sstt");
                string line = $"{timestamp} - {message}";
                File.AppendAllLines(fullPath, new[] { line });
            }
            catch
            {
                //Don't throw from logger; if logging fails, keep runnign.
            }    
        }

        //SNAPSHOT FOR LOG FUNCTION - compact snapshot string
        private static string SnapshotForLog(Dictionary<AgeCategory, List<string>> assigned)
        {
            var parts = new List<string>();
            foreach (AgeCategory cat in Enum.GetValues(typeof(AgeCategory)))
            {
                assigned.TryGetValue(cat, out var list);
                if (list == null || list.Count == 0)
                {
                    parts.Add($"{PrettyCategoryName(cat)} -> (empty)");
                }
                else
                {
                    parts.Add($"{PrettyCategoryName(cat)} -> [{string.Join(",", list)}]");
                }
            }
            return string.Join(",", parts);
        }

        //ENUM NAME CONVERSION FUNCTION - converts names like "YoungAdult" to "Young Adult" 
        private static string PrettyCategoryName(AgeCategory cat)
        {
            string s = cat.ToString();
            var sb = new StringBuilder();
            foreach (char ch in s)
            {
                if (char.IsUpper(ch) && sb.Length > 0) sb.Append(' ');
                sb.Append(ch);
            }
            return sb.ToString();
        }



        // Function to determine age category based on age
        private static AgeCategory GetCategory(int age)
            {
                if (age >= 0 && age <= 2) return AgeCategory.Infant;
                if (age >= 3 && age <= 12) return AgeCategory.Child;
                if (age >= 13 && age <= 17) return AgeCategory.Teenager;
                if (age >= 18 && age <= 24) return AgeCategory.YoungAdult;
                if (age >= 25 && age <= 64) return AgeCategory.Adult;
                if (age >= 65) return AgeCategory.Senior;
                return AgeCategory.Adult; // Default case, should not reach here
        }
    }
}

