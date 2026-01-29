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
        enum AgeCategory
        {
            Infant, //0-2
            Child, //3-12  
            Teenager,//13-17
            YoungAdult,//18-24
            Adult,//25-64
            Senior//65+
        }
        static void Main(string[] args)
        {
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

            */
                                // adding parse function to connect array to already existing enum.
                                var categories = Enum.GetNames(typeof(AgeCategory))
                                 .Select(n => Enum.Parse<AgeCategory>(n))
                                 .ToArray();
            var assigned = categories.ToDictionary(c => c, c => string.Empty);  

            // Thought it'd be best to make it clear for the end user.
            Console.WriteLine("Welcome to the Age Category Assigner!");
            Console.WriteLine("You will be prompted to enter people until every age category has a name assigned.");
            Console.WriteLine("Press 1 To Continue, 2 to View a Snapshot of the Category List, and 3 to Exit.\n");

            bool exitRequested = false;

            // Loop until all categories are assigned or exit is requested
            while (assigned.Any(kv => string.IsNullOrWhiteSpace(kv.Value)) && !exitRequested)
            {
                // Ask for user's name
                Console.Write("\nEnter your name: ");
                string nameInput = (Console.ReadLine() ?? "").Trim();

                if (string.Equals(nameInput, "2", StringComparison.OrdinalIgnoreCase))
                {
                    PrintSnapshot(assigned);
                    continue;
                }

                if (string.Equals(nameInput, "3", StringComparison.OrdinalIgnoreCase))
                {
                    exitRequested = true;
                    break;
                }

                //Checking for only letter inputs
                if (string.IsNullOrWhiteSpace(nameInput) || !nameInput.All(char.IsLetter))
                {
                    Console.WriteLine("Invalid Input. Please use letters only (no numbers or symbols)");
                    continue;
                }

                string name = nameInput;

                //Setting age integer to then be updated by string parsing
                //Changing the ageinput to Birth Year input
                int age;
                int BirthYear;
                var currentYear = DateTime.Now.Year;

                while (true)
                {
                    // Ask for user's age
                    Console.Write("Enter your Birth Year: ");
                    String BirthYearInput = (Console.ReadLine() ?? string.Empty).Trim();

                    // Parse BirthYear string to int safely
                    DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

                    if (!int.TryParse(BirthYearInput, out BirthYear))
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

                    //This is of course assuming the user's birthday is jan 1st, since we only ask for year.
                    age = currentDate.Year - BirthYear;
                    if (age < 0)
                    {
                        Console.WriteLine("Invalid age. Please Reenter Birth Year");
                        continue;
                    }


                    // Fetching Age to Determine Age Category. Set in static block at the bottom
                    // max: determining category and index
                    AgeCategory category = GetCategory(age);

                    //If an age category is already assigned, ask to overwrite
                    if (!string.IsNullOrWhiteSpace(assigned[category]))
                    {
                        Console.WriteLine($"The category '{category}' is already assigned to '{assigned[category]}'. Would you like to overwrite it? [y/n]");
                        string answer = (Console.ReadLine() ?? "").Trim();
                        if (!string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Not overwriting. Move to next input.\n");
                            break;
                        }
                    }

                    assigned[category] = name;

                   

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


                    //showing PROMPT MENU After Each Successful Assignment
                    while (true)
                    {
                        Console.WriteLine();
                        Console.WriteLine ("Press 1 to Continue, 2 to View a Snapshot of the Category List, and 3 to Exit.");
                        string postChoice = (Console.ReadLine() ?? "").Trim();

                        if (postChoice == "1")
                        {
                            break; //continue to next name input
                        }
                        else if (postChoice == "2")
                        {
                            PrintSnapshot(assigned);
                        }
                        else if (postChoice == "3")
                        {
                            exitRequested = true;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid Input. Please enter 1, 2, or 3.");
                        }
                    }

                    // After handling menu choice, break birth year loop if exit requested
                    
                    //successfully assigned current name; break inner birth year loop to return to outer name loop
                    break;
                }//end of inner birthyear loop
            }//end of outer name loop

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

        // Function to print current snapshot of assigned categories
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

