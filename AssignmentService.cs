using System;
using System.Text;

namespace InternConsoleApp
{
    public static class AssignmentService
    {

        // ASSIGNMENT RESULT ENUM
        public enum AssignResult { Assigned, Decline, Exit }

        //ASSIGN PERSON FUNCTION -- Now handles multiple names per category
        // and prints the category-dependent message
        // If category already assigned, user can add, replace , or cancel
        public static AssignResult TryAssignPerson(string name, int birthYear, Dictionary<AgeCategoryHelper.AgeCategory, List<string>> assigned)
        {
            int age = AgeCategoryHelper.CalculateAge(birthYear);
            AgeCategoryHelper.AgeCategory category = AgeCategoryHelper.GetCategory(age);

            var list = assigned[category];

            if (list.Count > 0)
            {
                //Logging when existing category is detected
                FileService.AppendLog($"User attempted To Add '{name}' (Age {age}) To '{AgeCategoryHelper.PrettyCategoryName(category)}' — Existing Entries Detected; Prompting Add/Replace/Cancel.");
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
                FileService.AppendLog($"User Added '{name}' (Age {age}) To '{AgeCategoryHelper.PrettyCategoryName(category)}' Category.");
            }

            //Age Category Message now sepparated into separate function
            Console.WriteLine();
            PrintCategoryMessage(category, name);


            // let caller print messages based on category
            return AssignResult.Assigned;
        }


        //EXISTING CATEGORY PROCESSING FUNCTION - handles add/replace/cancel logic
        public static AssignResult ProcessExistingCategory(string name, AgeCategoryHelper.AgeCategory category, List<string> list, Dictionary<AgeCategoryHelper.AgeCategory, List<string>> assigned, int age)
        {
            Console.WriteLine($"\nThe category {category} already has {list.Count} entr{(list.Count == 1 ? "y" : "ies")}: {string.Join(",", list)}");

            while (true)
            {
                Console.WriteLine("Choose: \n[A] Add This Name, \n[R] Replace All Names With This Name, or \n[C] Cancel And Re-enter Name");
                string choice = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

                if (choice == "c" || choice == "cancel")
                {
                    Console.WriteLine("Canceled name assignment...");
                    FileService.AppendLog($"User Canceled Assigning '{name}' (Age {age}) To The '{AgeCategoryHelper.PrettyCategoryName(category)}' Category.");
                    return AssignResult.Decline;
                }

                if (choice == "r" || choice == "replace")
                {
                    list.Clear();
                    list.Add(name);
                    FileService.AppendLog($"User Added '{name}' (Age {age}) To The '{AgeCategoryHelper.PrettyCategoryName(category)}' Category, Replacing All Other Names In The Category.");
                    return AssignResult.Assigned;
                }
                else if (choice == "a" || choice == "add")
                {
                    list.Add(name);
                    FileService.AppendLog($"User Added '{name}' (Age {age}) To '{AgeCategoryHelper.PrettyCategoryName(category)}' Category.");
                    return AssignResult.Assigned;
                }
                else
                {
                    Console.WriteLine("\nInvalid Choice. Please enter a, r, or c.");
                    //keeps user in a/r/c menu until valid input, just like the main menu.
                    //Logs invalid input
                    FileService.AppendLog("User Entered Invalid Input At Existing Assignment Menu.");

                }
            }
        }


        //CATEGORY MESSAGE FUNCTION - Specified category message is now its own function for clarity
        public static void PrintCategoryMessage(AgeCategoryHelper.AgeCategory category, string name)
        {
            switch (category)
            {
                case AgeCategoryHelper.AgeCategory.Infant:
                    Console.WriteLine($"Googoo, {name},\ngoo ga goo goo googooga.");
                    break;
                case AgeCategoryHelper.AgeCategory.Child:
                    Console.WriteLine($"Hi, {name},\nlet's go play outside.");
                    break;
                case AgeCategoryHelper.AgeCategory.Teenager:
                    Console.WriteLine($"Yo, {name},\nlet's go to high school.");
                    break;
                case AgeCategoryHelper.AgeCategory.YoungAdult:
                    Console.WriteLine($"Hey, {name},\nlet's go out for a drink");
                    break;
                case AgeCategoryHelper.AgeCategory.Adult:
                    Console.WriteLine($"Hello, {name},\nlet's go do our taxes.");
                    break;
                case AgeCategoryHelper.AgeCategory.Senior:
                    Console.WriteLine($"Good day, {name},\nlet's go write our will!");
                    break;
            }
        }


        //SNAPSHOT FOR LOG FUNCTION - compact snapshot string
        public static string SnapshotForLog(Dictionary<AgeCategoryHelper.AgeCategory, List<string>> assigned)
        {
            var parts = new List<string>();
            foreach (AgeCategoryHelper.AgeCategory cat in Enum.GetValues(typeof(AgeCategoryHelper.AgeCategory)))
            {
                assigned.TryGetValue(cat, out var list);
                if (list == null || list.Count == 0)
                {
                    parts.Add($" {AgeCategoryHelper.PrettyCategoryName(cat)} -> (empty)");
                }
                else
                {
                    parts.Add($" {AgeCategoryHelper.PrettyCategoryName(cat)} -> [{string.Join(",", list)}]");
                }
            }
            return string.Join(",", parts);
        }

        // SNAPSHOT FUNCTION to print current snapshot of assigned categories in Console
        public static void PrintSnapshot(Dictionary<AgeCategoryHelper.AgeCategory, List<string>> assigned)
        {
            Console.WriteLine("\n -- Category Snapshot --");
            foreach (var kv in assigned)
            {
                string assignedName = kv.Value.Count == 0 ? "(empty)" : string.Join(",", kv.Value);
                Console.WriteLine($"{(int)kv.Key}: {kv.Key} => {assignedName}");
            }
        }

    }
}
