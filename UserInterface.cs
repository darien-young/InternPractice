using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace InternConsoleApp
{
    public static class UserInterface
    {

        // MENU FUNCTION -- Shows menu. if user selects 2, s    hows snapshot and re-prompts. returns 1 or 3.
        public static int PromptMenuChoice(Dictionary<AgeCategoryHelper.AgeCategory, List<string>> assigned)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Press 1 To Continue.");
                Console.WriteLine("Press 2 To View A Snapshot Of The Category List.");
                Console.WriteLine("Press 3 To View The Program Log File (Raw).");
                Console.WriteLine("Press 4 To View The Program Log File (Parsed).");
                Console.WriteLine("Press 5 To View The Program Log File (Parsed Filtered By Action Type).");
                Console.WriteLine("Press 6 To Exit The Program.");
                string postChoice = (Console.ReadLine() ?? "").Trim();

                if (postChoice == "1")
                {
                    return 1; //continue requested
                }
                else if (postChoice == "2")
                {
                    AssignmentService.PrintSnapshot(assigned);

                    //Log snapshot request
                    FileService.AppendLog($"Snapshot Requested: {AssignmentService.SnapshotForLog(assigned)}");

                    //Appending Snapshot to CategorySnapshot.txt
                    FileService.PrintSnapshotToFile(assigned);

                    //loop back to menu after showing snapshot
                    continue;

                }
                else if (postChoice == "3")
                {
                    //Log file request
                    FileService.AppendLog("User Viewed Program Log File.");

                    //Show log file contents in console
                    string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    string logsFolder = Path.Combine(desktop, "ProgramLogs");
                    string date = DateTime.Now.ToString("yyyy-MM-dd");
                    string fullPath = Path.Combine(logsFolder, $"ProgramLog_{date}.txt");

                    ReadFiles.ReadAndDisplayFile(fullPath);

                    //loop back to menu after showing log file
                    continue;
                }
                else if (postChoice == "4")
                {
                    string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    string logsFolder = Path.Combine(desktop, "ProgramLogs");
                    string date = DateTime.Now.ToString("yyyy-MM-dd");
                    string fullPath = Path.Combine(logsFolder, $"ProgramLog_{date}.txt");

                    ReadFiles.DisplayParsedLogs(fullPath);
                    continue;
                }
                else if (postChoice == "5")
                {
                    string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    string logsFolder = Path.Combine(desktop, "ProgramLogs");
                    string date = DateTime.Now.ToString("yyyy-MM-dd");
                    string fullPath = Path.Combine(logsFolder, $"ProgramLog_{date}.txt");

                    ReadFiles.PromptAndDisplayLogFilter(fullPath);
                    continue;
                }

                else if (postChoice == "6")
                {
                    
                    return 6; //exit requested
                }
                else
                {
                    Console.WriteLine("Invalid Input. Please enter 1, 2, 3, 4, or 6.");
                    FileService.AppendLog("User Entered Invalid Input At Prompt Menu");

                }
            }
        }


        //NAME FUNCTION -- Function to prompt for name inputs
        public static string PromptName()
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
                    FileService.AppendLog("User Entered Invalid Name Input.");
                    continue;
                }

                return nameInput;
            }
        }



        //BIRTH YEAR FUNCTION -- Function to prompt for birth year input
        public static int PromptBirthYear()
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
                    Console.WriteLine("\nInvalid Input. Please use Numbers Only (No letters or symbols)\n");
                    FileService.AppendLog("User Entered Invalid Birth Year.");
                    continue;
                }

                if (BirthYear > currentYear)
                {
                    Console.WriteLine("\nInvalid Input. This year has not happened yet -_-\n");
                    FileService.AppendLog("User Entered Invalid Birth Year.");
                    continue;
                }

                if (BirthYear < currentYear - 130)
                {
                    Console.WriteLine("\nInvalid Input. No one lives that long these days.\n");
                    FileService.AppendLog("User Entered Invalid Birth Year.");
                    continue;
                }

                //This is of course assuming the user's birthday is jan 1st, since we only ask for year
                return BirthYear;
            }

        }
    }
}