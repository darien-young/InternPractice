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
                Console.WriteLine("Press 2 To View Category Snapshot.");
                Console.WriteLine("Press 3 To View RAW Program Logs.");
                Console.WriteLine("Press 4 To View PARSED Program Logs.");
                Console.WriteLine("Press 5 To View CSV Event Logs.");
                Console.WriteLine("Press 6 To Exit.");

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

                    // Appending to event record CSV
                    FileService.AppendEventCsv(new EventRecord { Action = "Snapshot" });

                    //Appending Snapshot to CategorySnapshot.txt
                    FileService.PrintSnapshotToFile(assigned);

                    //loop back to menu after showing snapshot
                    continue;

                }
                else if (postChoice == "3") // RAW Program Logs
                {
                    FileService.AppendLog("User Requested RAW Program Logs (Date Range + Action Filter).");
                    FileService.AppendEventCsv(new EventRecord { Action = "ViewRawLog" });

                    var files = ReadFiles.PromptDateRangeAndGetFiles("ProgramLog", "txt");

                    if (files.Count == 0)
                    {
                        Console.WriteLine("No RAW log files found for the selected date range.");
                        continue;
                    }

                    string actionFilter = ReadFiles.PromptAndGetActionFilter(includeAllOption: true);

                    foreach (var file in files)
                    {
                        FileService.AppendLog($"User Viewed RAW Log File: {Path.GetFileName(file)}");
                        FileService.AppendEventCsv(new EventRecord { Action = "ViewRawLogFile", Name = Path.GetFileName(file) });

                        Console.WriteLine($"\n=== RAW LOG FILE: {Path.GetFileName(file)} ===");

                        if (actionFilter == "ALL")
                            ReadFiles.ReadAndDisplayFile(file);
                        else
                            ReadFiles.ReadAndDisplayFileByAction(file, actionFilter); // New helper for filtering RAW logs by action
                    }
                    continue;
                }
                else if (postChoice == "4") // PARSED Program Logs
                {
                    FileService.AppendLog("User Requested PARSED Program Logs (Date Range + Action Filter).");
                    FileService.AppendEventCsv(new EventRecord { Action = "ViewParsedLog" });

                    var files = ReadFiles.PromptDateRangeAndGetFiles("ProgramLog", "txt");

                    if (files.Count == 0)
                    {
                        Console.WriteLine("No PARSED log files found for the selected date range.");
                        continue;
                    }

                    string actionFilter = ReadFiles.PromptAndGetActionFilter(includeAllOption: true);

                    foreach (var file in files)
                    {
                        FileService.AppendLog($"User Parsed Log File: {Path.GetFileName(file)}");
                        FileService.AppendEventCsv(new EventRecord { Action = "ParseLogFile", Name = Path.GetFileName(file) });

                        Console.WriteLine($"\n=== PARSED LOG FILE: {Path.GetFileName(file)} ===");

                        if (actionFilter == "ALL")
                            ReadFiles.DisplayParsedLogs(file);
                        else
                            ReadFiles.DisplayParsedLogsByAction(file, actionFilter);
                    }
                    continue;
                }
                else if (postChoice == "5") // CSV Event Logs
                {
                    FileService.AppendLog("User Requested CSV Event Logs (Date Range + Action Filter).");
                    FileService.AppendEventCsv(new EventRecord { Action = "ViewCSV" });

                    var files = ReadFiles.PromptDateRangeAndGetFiles("ProgramEvents", "csv");

                    if (files.Count == 0)
                    {
                        Console.WriteLine("No CSV event files found for the selected date range.");
                        continue;
                    }

                    string actionFilter = ReadFiles.PromptAndGetActionFilter(includeAllOption: true, isCsv: true);

                    foreach (var file in files)
                    {
                        FileService.AppendLog($"User Parsed CSV File: {Path.GetFileName(file)}");
                        FileService.AppendEventCsv(new EventRecord { Action = "ParseCSVFile", Name = Path.GetFileName(file) });

                        Console.WriteLine($"\n=== CSV EVENT FILE: {Path.GetFileName(file)} ===");

                        if (actionFilter == "ALL")
                            ReadFiles.DisplayParsedCsv(file);
                        else
                            ReadFiles.DisplayCsvByAction(file, actionFilter);
                    }
                    continue;
                }

                else if (postChoice == "6")
                {
                    
                    return 6; //exit requested
                }
                else
                {
                    Console.WriteLine("Invalid Input. Please enter  a number from 1 to 6.");
                    FileService.AppendLog("User Entered Invalid Input At Prompt Menu");

                    // Appending to event record CSV
                    FileService.AppendEventCsv(new EventRecord { Action = "Invalid" });
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
                    // Appending to event record CSV
                    FileService.AppendEventCsv(new EventRecord { Action = "Invalid" });
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
                    // Appending to event record CSV
                    FileService.AppendEventCsv(new EventRecord { Action = "Invalid" });
                    continue;
                }

                if (BirthYear > currentYear)
                {
                    Console.WriteLine("\nInvalid Input. This year has not happened yet -_-\n");
                    FileService.AppendLog("User Entered Invalid Birth Year.");
                    // Appending to event record CSV
                    FileService.AppendEventCsv(new EventRecord { Action = "Invalid" });
                    continue;
                }

                if (BirthYear < currentYear - 130)
                {
                    Console.WriteLine("\nInvalid Input. No one lives that long these days.\n");
                    FileService.AppendLog("User Entered Invalid Birth Year.");
                    // Appending to event record CSV
                    FileService.AppendEventCsv(new EventRecord { Action = "Invalid" });
                    continue;
                }

                //This is of course assuming the user's birthday is jan 1st, since we only ask for year
                return BirthYear;
            }


        }

        // DATE VALIDATION FUNCTION
        public static DateTime PromptValidDate(string promptMessage)
        {
            while (true)
            {
                Console.Write(promptMessage);
                string input = (Console.ReadLine() ?? "").Trim();

                // Try to parse using exact format yyyy-MM-dd
                if (!DateTime.TryParseExact(input, "yyyy-MM-dd",
                                           System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.None,
                                           out DateTime result))
                {
                    Console.WriteLine("Invalid date format. Please enter a date in the format YYYY-MM-DD (e.g., 2026-02-12).");
                    FileService.AppendLog($"User Entered Invalid Date Format: '{input}'");
                    FileService.AppendEventCsv(new EventRecord { Action = "Invalid" });
                    continue; // retry
                }

                // Check for impossible future date
                if (result > DateTime.Now)
                {
                    Console.WriteLine("Invalid date. You cannot specify a future date.");
                    FileService.AppendLog($"User Entered Future Date: '{input}'");
                    FileService.AppendEventCsv(new EventRecord { Action = "Invalid" });
                    continue; // retry
                }

                // Check for impossible past date (before logging ever existed)
                DateTime earliestLogDate = new DateTime(2026, 2, 4); // adjust to when your first logs exist
                if (result < earliestLogDate)
                {
                    Console.WriteLine($"Invalid date. Logs only exist starting {earliestLogDate:yyyy-MM-dd}.");
                    FileService.AppendLog($"User Entered Too Early Date: '{input}'");
                    FileService.AppendEventCsv(new EventRecord { Action = "Invalid" });
                    continue; // retry
                }

                // Valid date
                return result;
            }
        }

        // DATE-RANGE VALIDTION FUNCTION
        public static (DateTime startDate, DateTime endDate) PromptValidDateRange()
        {
            while (true)
            {
                Console.WriteLine("\nSpecify the date range for the logs:");

                DateTime startDate = PromptValidDate("Start Date (YYYY-MM-DD): ");
                DateTime endDate = PromptValidDate("End Date (YYYY-MM-DD): ");

                // Ensure start <= end
                if (startDate > endDate)
                {
                    Console.WriteLine("Invalid range. Start date cannot be after end date. Please try again.");
                    FileService.AppendLog($"User Entered Invalid Date Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
                    FileService.AppendEventCsv(new EventRecord { Action = "Invalid" });
                    continue; // retry
                }

                return (startDate, endDate);
            }
        }




    }
}   