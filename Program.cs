/* Phase 5 : CSV Implementation
 *  
 *  Generate daily CSV files to the same ProgramLogs folder that acts like a structured version of the log.
 *  ProgramEvents_YYYY-MM-DD.csv
 *  Timestamp (ISO format is best: 2026-02-11 14:23:01)
 *  Action (Added, AddAttempt, Replaced, Canceled, Snapshot, Exit, Start)
 *  Name (blank if not applicable)
 *  Age (blank if not applicable)
 *  Category (blank if not applicable)
 *  
 *  
 *  *** Right now you have `FileService.AppendLog(message)`. Add a second method alongside it: `FileService.AppendEventCsv(EventRecord record)`.
 *  
 *  TLDR; When something happens: write a human-readable log line and write a structured CSV row, keeping the same formats as in the text file, but csv friendly.
 *  Note: You can create a class or struct to hold the data for each event, and then implement the AppendEventCsv method to write it to the appropriate CSV file.
 *  Make sure to handle file creation and appending correctly, especially when a new day starts and a new CSV file needs to be created. 
 *  Excel is pretty forgiving with CSV formatting, but be mindful of commas in names or other fields that could disrupt the structure. 
 *  You might want to enclose fields in quotes if they contain commas.
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InternConsoleApp;


namespace InternPractice
{
    class Program
    {
        

        //------------------------------- MAIN METHOD -------------------------------------//
        static void Main(string[] args)
        {
           
                               // adding parse function to connect array to already existing enum.
           var assigned = Enum.GetNames(typeof(AgeCategoryHelper.AgeCategory))
                                 .Select(n => Enum.Parse<AgeCategoryHelper.AgeCategory>(n))
                                 .ToArray()
                                 .ToDictionary(c => c, c => new List<string>());  

            // Thought it'd be best to make it clear for the end user.
            Console.WriteLine("Welcome to the Age Category Assigner!");
            Console.WriteLine("You will be prompted to enter people until every age category has a name assigned.");

            //Log program start
            FileService.AppendLog("User Started Program.");

            //Initial Menu Prompt
            int startMenuChoice = UserInterface.PromptMenuChoice(assigned);
            if (startMenuChoice == 6)
            {
                Console.WriteLine("\nExiting early. Current Snapshot: ");
                AssignmentService.PrintSnapshot(assigned);
                Console.WriteLine();

                //logging early exit
                FileService.AppendLog("User Exited Program Early ");


                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            bool exitRequested = false;

            //MAIN LOOP: continue until all categories are assigned or exit is requested
            while (assigned.Any(kv => kv.Value.Count == 0) && !exitRequested)
            {

                //menuChoice == 1 - proceed to collect person data
                string name = UserInterface.PromptName();
                // get birth year (and validate it)
                int birthYear = UserInterface.PromptBirthYear();

                //Assignment Attempt
                var result = AssignmentService.TryAssignPerson(name, birthYear, assigned);
                
                if (result == AssignmentService.AssignResult.Exit) 
                {  
                    exitRequested = true; 
                    break; 
                }
                if (result == AssignmentService.AssignResult.Decline) 
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
                    int postMenuChoice = UserInterface.PromptMenuChoice(assigned);
                    if (postMenuChoice == 6)
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
                AssignmentService.PrintSnapshot(assigned);

                // write final snapshot to text file on early exit
                FileService.PrintSnapshotToFile(assigned, isFinalSnapShot: true);

                //logging earley exit
                FileService.AppendLog("User Exited Program Early; Final Snapshot Printed. ");


                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            //After all categories are assigned
            Console.WriteLine("\nAll age categories filled. Final Snapshot: \n");
            AssignmentService.PrintSnapshot(assigned);

            FileService.AppendLog("User Filled All Age Categories. Final Snapshot Printed And Program Exited.");

            //write final snapshot to text file
            FileService.PrintSnapshotToFile(assigned, isFinalSnapShot: true);

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            return; // Added return statement to explicitly indicate the end of Main method
        }
        //-------------------------------- END OF MAIN METHOD --------------------------------//
       
    }
}

