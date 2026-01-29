// MAX NOTE: Current Commit of code RUNS but is UNFINSIHED.
// Yet to add loop that forces completion of array before exitting.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

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
            // Phase 2:
            // Instead of asking for an age, ask for the user's Birth Year and calculate the age based on the current year dynamically before output.

            // Darien Comment: Good as is, with the only asterisk being when user inputs the current year as their birth year, the program doesn't enter the switch.

            // Phase 3: Based on user's age, assign the user a category and print a different message for each category.
            // The categories are as follows: Child, Teenager, Young Adult, Adult, Senior.
            // For the purpose of initial development and testing, you can define the logic as you see fit.

            // Darien Comment: Having a constructor and private class is good practice.

            // Phase 3.1: Now, I'd like you to refine the system by using an array of strings to hold the categories.
            // For each user input, determine the appropriate category index and use it to fetch the category from the array.
            // Print a message that includes the user's name and their assigned category.
            // For each input, the person's name should be saved into the array, and the program should loop until all categories have a name assigned.
            // At any point, the end user should be able to see a snapshot of the current state of the array,
            // and if the program is exited early, it will print out the current state of the array.



            var categories = Enum.GetNames(typeof(AgeCategory))
                                 .Select(n => Enum.Parse<AgeCategory>(n))
                                 .ToArray();
            var assigned = categories.ToDictionary(c => c, c => string.Empty);  

            Console.WriteLine("Welcome to the Age Category Assigner!");
            Console.WriteLine("You will be prompted to enter people until every age category has anmae assigned.");
            Console.WriteLine("At the name prompt you can type 'snapshot' to view current assignments or 'exit' to quit early.\n");

            bool exitRequested = false;


            while (assigned.Any(kv => string.IsNullOrWhiteSpace(kv.Value)) && !exitRequested)

            {
                // Ask for user's name
                Console.Write("Enter your name: ");
                string nameInput = (Console.ReadLine() ?? "").Trim();

                if (string.Equals(nameInput, "snapshot", StringComparison.OrdinalIgnoreCase))
                {
                    PrintSnapshot(assigned);
                    continue;
                }

                if (string.Equals(nameInput, "exit", StringComparison.OrdinalIgnoreCase))
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
                            continue;
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


                    Console.WriteLine();
                    Console.WriteLine("Final snapshot:");
                    PrintSnapshot(assigned);

                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();


                    return; // Added return statement to explicitly indicate the end of Main method
                }
            }
        }
                   // still needs two keystrokes on this laptop to exit. 

        private static void PrintSnapshot(Dictionary<AgeCategory,string> assigned)
        {
            Console.WriteLine("\n -- Category Snapshot --");
            foreach (var kv in assigned)
            {
                string assignedName = string.IsNullOrWhiteSpace(kv.Value) ? "(empty)" : kv.Value;
                Console.WriteLine($"{(int)kv.Key}: {kv.Key} => {assignedName}");
            }
        }
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

