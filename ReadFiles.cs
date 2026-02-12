using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InternConsoleApp
{
    internal class ReadFiles
    {
        // PRINT RAW LOGS IN CONSOLE FUNCTION
        public static void ReadAndDisplayFile(string filePath)
        {
            try
            {
                if(!File.Exists(filePath))
                {
                    Console.WriteLine("Error: Log File Not Found.");
                    return;
                }
                int lineCount = 0;

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string? line;
                    Console.WriteLine("--- Program Log File Contents: ---");
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            Console.WriteLine(line);
                            lineCount++;
                        }
                    }
                }
                Console.WriteLine($"--- End of Log File. Total Non-Empty Lines: {lineCount} ---");
                Console.WriteLine("---------------------------------------------------\n");
            }
            catch (UnauthorizedAccessException) 
            {
                Console.WriteLine("Error: Access to the log file is denied.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");

            }
        }
        
        //  LOG PARSER FUNCTION
        public static List<LogEntry> ParseLogFile(string filePath)
        {
            var results = new List<LogEntry>();

            if (!File.Exists(filePath))
                return results;

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var entry = new LogEntry();

                // Time Stamp Extraction
                var timeMatch = Regex.Match(line, @"\d{2}:\d{2}:\d{2}");
                if (timeMatch.Success)
                    entry.TimeStamp = timeMatch.Value;

                // Action Detection
                // Action Detection
                if (line.Contains("attempted", StringComparison.OrdinalIgnoreCase))
                    entry.Action = "AddAttempt";
                else if (line.Contains("Replacing", StringComparison.OrdinalIgnoreCase))
                    entry.Action = "ReplacedBy";
                else if (line.Contains("Added", StringComparison.OrdinalIgnoreCase))
                    entry.Action = "Added";
                else if (line.Contains("Canceled", StringComparison.OrdinalIgnoreCase) || line.Contains("Cancelled", StringComparison.OrdinalIgnoreCase))
                    entry.Action = "Canceled";
                else if (line.Contains("Snapshot", StringComparison.OrdinalIgnoreCase))
                    entry.Action = "Snapshot";
                else if (line.Contains("Exited", StringComparison.OrdinalIgnoreCase) || line.Contains("Exit", StringComparison.OrdinalIgnoreCase))
                    entry.Action = "Exit";
                else if (line.Contains("Started Program", StringComparison.OrdinalIgnoreCase))
                    entry.Action = "Start";
                else if (line.Contains("Invalid", StringComparison.OrdinalIgnoreCase))
                    entry.Action = "Invalid";
                else
                    entry.Action = "Other";


                // Name extraction inside ''
                var nameMatch = Regex.Match(line, @"'([^']+)'");
                if (nameMatch.Success)
                    entry.Name = nameMatch.Groups[1].Value;

                // Age extraction inside (Age ##)
                var ageMatch = Regex.Match(line, @"Age\s+(\d+)");
                if (ageMatch.Success)
                    entry.Age = ageMatch.Groups[1].Value;

                // Category extraction inside last ''
                var catMatches = Regex.Matches(line, @"'([^']+)'");
                if (catMatches.Count > 1)
                    entry.Category = catMatches[catMatches.Count - 1].Groups[1].Value;

                results.Add(entry);
            }

            return results;
        }

        //  CSV PARSER FUNCTION
        public static List<LogEntry> ParseCsvFile(string filePath)
        {
            var results = new List<LogEntry>();

            if (!File.Exists(filePath))
                return results;

            var lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                string line = lines[i];

                var parts = Regex.Matches(line, @"(?<=^|,)(?:""([^""]*)""|([^,]*))")
                         .Cast<Match>()
                         .Select(m => m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value)
                         .ToArray();

                if (parts.Length < 5)
                    continue;

                results.Add(new LogEntry
                {
                    TimeStamp = parts[0],
                    Action = parts[1],
                    Name = parts[2],
                    Age = parts[3],
                    Category = parts[4]
                });
            }

            return results;
        }



        // PRINT PARSED LOGS IN CONSOLES FUNCTION
        public static void DisplayParsedLogs(string filePath)
        {
            var logs = ParseLogFile(filePath);

            Console.WriteLine("\n| Timestamp |   Action   |    Name     | Age |  Category   |");
              Console.WriteLine("|-----------|------------|-------------|-----|-------------|");

            foreach (var l in logs)
            {
                Console.WriteLine($"| {l.TimeStamp,-9} | {l.Action,-10} | {l.Name,-11} | {l.Age,-3} | {l.Category,-11} |");
            }
        }

        // ================= DISPLAY CSV EVENTS =================
        public static void DisplayParsedCsv(string filePath)
        {
            var logs = ParseCsvFile(filePath);

            Console.WriteLine("\n| Timestamp           |   Action   |    Name     | Age |  Category   |");
            Console.WriteLine("|---------------------|------------|-------------|-----|-------------|");

            foreach (var l in logs)
            {
                Console.WriteLine($"| {l.TimeStamp,-19} | {l.Action,-10} | {l.Name,-11} | {l.Age,-3} | {l.Category,-11} |");
            }

            Console.WriteLine($"\nTotal CSV Records: {logs.Count}\n");
        }


        // ACTION FILTER MEMU FUNCTION
        public static void PromptAndDisplayLogFilter(string filePath)
        {
            string actionFilter = "INVALID";

            while (actionFilter == "INVALID")
            {
                Console.WriteLine("[A] Added");
                Console.WriteLine("[T] Add Attempt");
                Console.WriteLine("[R] Replaced");
                Console.WriteLine("[C] Canceled");
                Console.WriteLine("[S] Snapshot");
                Console.WriteLine("[E] Exit");
                Console.WriteLine("[B] Start (Begin)");
                Console.WriteLine("[I] Invalid Input");
                Console.WriteLine("[O] Other");
                Console.WriteLine("[X] Show ALL Logs");
                Console.Write("Choice: ");

                string actionChoice = (Console.ReadLine() ?? "").Trim().ToLower();

                actionFilter = actionChoice switch
                {
                    "a" or "added" => "Added",
                    "t" or "attempt" or "addattempt" => "AddAttempt",
                    "r" or "replaced" or "replacedby" => "ReplacedBy",
                    "c" or "canceled" or "cancelled" => "Canceled",
                    "s" or "snapshot" => "Snapshot",
                    "e" or "exit" => "Exit",
                    "b" or "start" or "begin" => "Start",
                    "i" or "invalid" => "Invalid",
                    "o" or "other" => "Other",
                    "x" or "all" => "ALL",
                    _ => "INVALID"
                };

                if (actionFilter == "INVALID")
                {
                    Console.WriteLine("Invalid selection. Please enter A, T, R, C, S, E, B, I, O, or X (or full word).");
                }
            }

            FileService.AppendLog($"User Filtered Logs By Action: {actionFilter}");

            DisplayParsedLogsByAction(filePath, actionFilter);
        }

        // CSV ACTION FILTER MENU =================
        public static void PromptAndDisplayCsvFilter(string filePath)
        {
            string actionFilter = "INVALID";

            while (actionFilter == "INVALID")
            {
                Console.WriteLine("\nSelect CSV Event Action Filter:");
                Console.WriteLine("[A] Added");
                Console.WriteLine("[T] Add Attempt");
                Console.WriteLine("[R] Replaced");
                Console.WriteLine("[C] Canceled");
                Console.WriteLine("[S] Snapshot");
                Console.WriteLine("[E] Exit");
                Console.WriteLine("[B] Start (Begin)");
                Console.WriteLine("[I] Invalid Input");
                Console.WriteLine("[X] Show ALL Events");
                Console.WriteLine("You may also type the full word (e.g., 'added', 'exit', 'snapshot').");
                Console.Write("Choice: ");

                string actionChoice = (Console.ReadLine() ?? "").Trim().ToLower();

                actionFilter = actionChoice switch
                {
                    "a" or "added" => "Added",
                    "t" or "attempt" or "addattempt" => "AddAttempt",
                    "r" or "replaced" or "replacedby" => "ReplacedBy",
                    "c" or "canceled" or "cancelled" => "Canceled",
                    "s" or "snapshot" => "Snapshot",
                    "e" or "exit" => "Exit",
                    "b" or "start" or "begin" => "Start",
                    "i" or "invalid" => "Invalid",
                    "x" or "all" => "ALL",
                    _ => "INVALID"
                };

                if (actionFilter == "INVALID")
                {
                    Console.WriteLine("Invalid selection. Please enter A, T, R, C, S, E, B, I, or X (or full word).");
                }
            }

            FileService.AppendLog($"User Filtered CSV Events By Action: {actionFilter}");
            FileService.AppendEventCsv(new EventRecord { Action = "FilterCSV" });

            if (actionFilter == "ALL")
            {
                DisplayParsedCsv(filePath);
            }
            else
            {
                DisplayCsvByAction(filePath, actionFilter);
            }
        }



        // DATE RANGE HELPER FUNCTION
        public static List<string> PromptDateRangeAndGetFiles(string filePrefix, string extension)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string logsFolder = Path.Combine(desktop, "ProgramLogs");
            if (!Directory.Exists(logsFolder))
                Directory.CreateDirectory(logsFolder);

            List<string> files = new();

            while (true)
            {
                Console.WriteLine("\nSelect Date Range:");
                Console.WriteLine("[T] Today Only");
                Console.WriteLine("[R] Specify Date Range");
                Console.WriteLine("[A] All Available Dates");
                Console.Write("Choice: ");

                string choice = (Console.ReadLine() ?? "").Trim().ToLower();

                if (choice == "t")
                {
                    string today = DateTime.Now.ToString("yyyy-MM-dd");
                    string file = Path.Combine(logsFolder, $"{filePrefix}_{today}.{extension}");
                    if (File.Exists(file))
                        files.Add(file);
                    break;
                }
                else if (choice == "a")
                {
                    files = Directory.GetFiles(logsFolder, $"{filePrefix}_*.{extension}")
                                     .OrderBy(f => f)
                                     .ToList();
                    break;
                }
                else if (choice == "r")
                {
                    DateTime start, end;
                    while (true)
                    {
                        Console.Write("Start Date (YYYY-MM-DD): ");
                        string startInput = (Console.ReadLine() ?? "").Trim();
                        if (!DateTime.TryParseExact(startInput, "yyyy-MM-dd",
                                System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.None,
                                out start))
                        {
                            Console.WriteLine("Invalid start date. Use YYYY-MM-DD format.");
                            continue;
                        }

                        Console.Write("End Date (YYYY-MM-DD): ");
                        string endInput = (Console.ReadLine() ?? "").Trim();
                        if (!DateTime.TryParseExact(endInput, "yyyy-MM-dd",
                                System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.None,
                                out end))
                        {
                            Console.WriteLine("Invalid end date. Use YYYY-MM-DD format.");
                            continue;
                        }

                        if (end < start)
                        {
                            Console.WriteLine("End date cannot be earlier than start date.");
                            continue;
                        }

                        if (start > DateTime.Now || end > DateTime.Now)
                        {
                            Console.WriteLine("Dates cannot be in the future.");
                            continue;
                        }

                        // All checks passed
                        break;
                    }

                    files = GetFilesInRange(filePrefix, extension, start, end);
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter T, R, or A.");
                }
            }

            if (files.Count == 0)
                Console.WriteLine("No files found for the selected date range.");

            return files;
        }

        public static string PromptAndGetActionFilter(bool includeAllOption = true, bool isCsv = false)
            {
            string actionFilter = "INVALID";
            while (actionFilter == "INVALID")
            {
                Console.WriteLine(isCsv ? "\nSelect CSV Event Action Filter:" : "\nSelect Log Action Filter:");
                Console.WriteLine("[A] Added");
                Console.WriteLine("[T] Add Attempt");
                Console.WriteLine("[R] Replaced");
                Console.WriteLine("[C] Canceled");
                Console.WriteLine("[S] Snapshot");
                Console.WriteLine("[E] Exit");
                Console.WriteLine("[B] Start (Begin)");
                Console.WriteLine("[I] Invalid Input");
                Console.WriteLine("[O] Other");
                if (includeAllOption)
                    Console.WriteLine("[X] Show ALL");

                Console.Write("Choice: ");
                string input = (Console.ReadLine() ?? "").Trim().ToLower();

                actionFilter = input switch
             {
                   "a" or "added" => "Added",
                  "t" or "attempt" or "addattempt" => "AddAttempt",
                  "r" or "replaced" or "replacedby" => "ReplacedBy",
                  "c" or "canceled" or "cancelled" => "Canceled",
                  "s" or "snapshot" => "Snapshot",
                  "e" or "exit" => "Exit",
                  "b" or "start" or "begin" => "Start",
                  "i" or "invalid" => "Invalid",
                   "o" or "other" => "Other",
                   "x" or "all" when includeAllOption => "ALL",
                   _ => "INVALID"
                 };

             if (actionFilter == "INVALID")
                 Console.WriteLine("Invalid selection. Please enter one of the listed options.");
            }

         FileService.AppendLog($"User Filtered {(isCsv ? "CSV Events" : "Logs")} By Action: {actionFilter}");
         if (isCsv) FileService.AppendEventCsv(new EventRecord { Action = "FilterCSV" });

         return actionFilter;
        }


        // RAW LOG DISPLAY WITH ACTION FILTER
        public static void ReadAndDisplayFileByAction(string filePath, string actionFilter)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error: Log File Not Found.");
                return;
            }

            var filteredLines = new List<string>();
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Determine the action in the line
                string lineAction = "Other";
                if (line.Contains("attempted", StringComparison.OrdinalIgnoreCase))
                    lineAction = "AddAttempt";
                else if (line.Contains("Replacing", StringComparison.OrdinalIgnoreCase))
                    lineAction = "ReplacedBy";
                else if (line.Contains("Added", StringComparison.OrdinalIgnoreCase))
                    lineAction = "Added";
                else if (line.Contains("Canceled", StringComparison.OrdinalIgnoreCase) || line.Contains("Cancelled", StringComparison.OrdinalIgnoreCase))
                    lineAction = "Canceled";
                else if (line.Contains("Snapshot", StringComparison.OrdinalIgnoreCase))
                    lineAction = "Snapshot";
                else if (line.Contains("Exited", StringComparison.OrdinalIgnoreCase) || line.Contains("Exit", StringComparison.OrdinalIgnoreCase))
                    lineAction = "Exit";
                else if (line.Contains("Started Program", StringComparison.OrdinalIgnoreCase))
                    lineAction = "Start";
                else if (line.Contains("Invalid", StringComparison.OrdinalIgnoreCase))
                    lineAction = "Invalid";

                if (lineAction.Equals(actionFilter, StringComparison.OrdinalIgnoreCase))
                    filteredLines.Add(line);
            }

            if (filteredLines.Count == 0)
            {
                Console.WriteLine($"No RAW logs found for action: {actionFilter}");
                return;
            }

            Console.WriteLine($"--- RAW LOGS FILTERED BY ACTION: {actionFilter} ---");
            foreach (var l in filteredLines)
                Console.WriteLine(l);

            Console.WriteLine($"--- End of Filtered RAW Logs. Total Lines: {filteredLines.Count} ---");
        }


        // NEW FUNCTION — get files within a date range
        public static List<string> GetFilesInRange(string filePrefix, string extension, DateTime start, DateTime end)
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string logsFolder = Path.Combine(desktop, "ProgramLogs");

                if (!Directory.Exists(logsFolder))
                    return new List<string>();

                var files = Directory.GetFiles(logsFolder, $"{filePrefix}_*{extension}")
                                     .Where(f =>
                                     {
                                         string name = Path.GetFileNameWithoutExtension(f);
                                         string datePart = name.Split('_').Last();
                                         if (DateTime.TryParseExact(datePart, "yyyy-MM-dd",
                                             System.Globalization.CultureInfo.InvariantCulture,
                                             System.Globalization.DateTimeStyles.None,
                                             out DateTime fileDate))
                                         {
                                             return fileDate >= start && fileDate <= end;
                                         }
                                         return false;
                                     })
                                     .OrderBy(f => f)
                                     .ToList();
            return files;
            }

        // PRINT PARSED LOGS BASED ON DATE AND ACTION
        public static void DisplayParsedLogsByAction(string filePath, string actionFilter)
        {
            var logs = ParseLogFile(filePath);

            // Normalize filter input
            actionFilter = actionFilter.Trim().ToLower();

            // Filter Logs
            var filtered = logs.Where(l => l.Action.ToLower() == actionFilter).ToList();

            if(filtered.Count == 0)
            {
                Console.WriteLine($"No Logs Found With Action: {actionFilter}");
                return;
            }

            Console.WriteLine($"\nFiltered Logs (Action = {actionFilter})");
            Console.WriteLine("| Timestamp |   Action   |    Name     | Age |  Category   |");
            Console.WriteLine("|-----------|------------|-------------|-----|-------------|");

            foreach (var l in filtered)
            {
                Console.WriteLine($"| {l.TimeStamp,-9} | {l.Action,-10} | {l.Name,-11} | {l.Age,-3} | {l.Category,-11} |");
            }
                
        }

        public static void DisplayCsvByAction(string filePath, string actionFilter)
        {
            var logs = ParseCsvFile(filePath);
            actionFilter = actionFilter.Trim().ToLower();

            var filtered = logs.Where(l => l.Action.ToLower() == actionFilter).ToList();

            if (!filtered.Any())
            {
                Console.WriteLine($"No CSV records found for action: {actionFilter}");
                return;
            }

            Console.WriteLine($"\nFiltered CSV Records (Action = {actionFilter})");
            Console.WriteLine("| Timestamp           |   Action   |    Name     | Age |  Category   |");
            Console.WriteLine("|---------------------|------------|-------------|-----|-------------|");

            foreach (var l in filtered)
            {
                Console.WriteLine($"| {l.TimeStamp,-19} | {l.Action,-10} | {l.Name,-11} | {l.Age,-3} | {l.Category,-11} |");
            }
        }





    }

}




