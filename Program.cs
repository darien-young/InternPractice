/* Phase 3.2 - Cleaner Logic Flow 
             Separate Concerns into 3 functions:

            PromptName()
            PromptBirthYear()
            PromptMenuChoice() - [return 1, 2, or 3]

            Rules:
                -PromptMenuChoice() only accepts specific integers.
                -PromptName() accepts names.

            Program Shows Menu:
                -at the very start(optional)
                - after each successful assignment(required)
                - snapshot choice loops back to menu

            Acceptance Criteria:
                -The user is never asked to type “2” or “3” at the “Enter your name” prompt.
                - If overwrite is declined, program returns to the menu(or restarts person input cleanly—your choice, just be consistent).
                - Snapshot works from the menu reliably.


            Comments, Functions:
             - You're calculating age from birth year in multiple places. Consolidate that logic.
             - The overwrite prompt logic is a bit nested. Consider refactoring for clarity. Hint: early returns or separate functions might help. 
                Use a boolean to give the system a clear path forward. 
            */

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace InternConsoleApp
{
    class Program
    {

        //------------------------------- MAIN METHOD -------------------------------------//
        static void Main(string[] args)
        {
           
                                // adding parse function to connect array to already existing enum.
           var categories = Enum.GetNames(typeof(AgeCategory))
                                 .Select(n => Enum.Parse<AgeCategory>(n))
                                 .ToArray();
            var assigned = categories.ToDictionary(c => c, c => string.Empty);  

            // Thought it'd be best to make it clear for the end user.
            Console.WriteLine("Welcome to the Age Category Assigner!");
            Console.WriteLine("You will be prompted to enter people until every age category has a name assigned.");
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
            while (assigned.Any(kv => string.IsNullOrWhiteSpace(kv.Value)) && !exitRequested)
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

                // Assigned -> print category-dependent message
                int age = CalculateAge(birthYear);
                AgeCategory category = GetCategory(age);

                //Boolean for AgeCategory-dependent Message
                Console.WriteLine();
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

                    //if all categories assigned after this, break to avoid extra prompt
                    if (!assigned.Any(kv => string.IsNullOrWhiteSpace(kv.Value)))
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
                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            //After all categories are assigned
            Console.WriteLine("\nAll age categories filled. Final Snapshot: \n");
            PrintSnapshot(assigned);
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

        // ASSIGNMENT RESULT FUNCTION
        private enum AssignResult { Assigned, Decline, Exit}

        private static AssignResult TryAssignPerson(string name, int birthYear, Dictionary<AgeCategory,string> assigned)
        {
            int age = CalculateAge(birthYear);
            AgeCategory category = GetCategory(age);

            if (!string.IsNullOrWhiteSpace(assigned[category]))
            {
                Console.WriteLine($"The category '{category}' is already assigned to '{assigned[category]}'. Would you like to overwrite it? [y/n]");
                string answer = (Console.ReadLine() ?? "").Trim();
                if (!string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase))
                {
                    // User declined overwrite
                    Console.WriteLine("Not Overwriting. Returning to menu...");

                    //show prompt menu
                    int fallback = PromptMenuChoice(assigned);
                    if (fallback == 3) return AssignResult.Exit; //user chose to exit from menu
                    return AssignResult.Decline; //user declined overwrite
                }
            }

            assigned[category] = name;
            // let caller print messages based on category
            return AssignResult.Assigned;
        }




        // MENU FUNCTION -- Shows menu. if user selects 2, shows snapshot and re-prompts. returns 1 or 3.
        private static int PromptMenuChoice(Dictionary<AgeCategory,string> assigned)
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

        //NAME FUNCTION -- Function to prompt for name input
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
        private static void PrintSnapshot(Dictionary<AgeCategory,string> assigned)
        {
            Console.WriteLine("\n -- Category Snapshot --");
            foreach (var kv in assigned)
            {
                string assignedName = string.IsNullOrWhiteSpace(kv.Value) ? "(empty)" : kv.Value;
                Console.WriteLine($"{(int)kv.Key}: {kv.Key} => {assignedName}");
            }
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

