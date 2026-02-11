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
                if (line.Contains("attempted"))
                    entry.Action = "AddAttempt";
                else if (line.Contains("Replacing") || line.Contains("replacing"))
                    entry.Action = "ReplacedBy";
                else if (line.Contains("Added") || line.Contains("added"))
                    entry.Action = "Added";
                else if (line.Contains("Canceled") || line.Contains("canceled"))
                    entry.Action = "Canceled";
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

        // PRINT PARSED LOGS IN CONCOLES FUNCTION
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

        // ACTION FILTER MEMU FUNCTION
        public static void PromptAndDisplayLogFilter(string filePath)
        {
            string actionFilter = "INVALID";

            while (actionFilter == "INVALID")
            {
                Console.WriteLine("\nSelect Log Action Filter:");
                Console.WriteLine("[A] Added");
                Console.WriteLine("[T] Add Attempt");
                Console.WriteLine("[R] Replaced");
                Console.WriteLine("[C] Canceled");
                Console.WriteLine("[O] Other");
                Console.WriteLine("[X] Show ALL Logs");
                Console.WriteLine("You may also type the full word (e.g., 'added', 'canceled', 'all').");
                Console.Write("Choice: ");

                string actionChoice = (Console.ReadLine() ?? "").Trim().ToLower();

                actionFilter = actionChoice switch
                {
                    "a" or "added" => "Added",
                    "t" or "attempt" or "addattempt" => "AddAttempt",
                    "r" or "replaced" or "replacedby" => "ReplacedBy",
                    "c" or "canceled" or "cancelled" => "Canceled",
                    "o" or "other" => "Other",
                    "x" or "all" => "ALL",
                    _ => "INVALID"
                };

                if (actionFilter == "INVALID")
                {
                    Console.WriteLine("Invalid selection. Please enter A, T, R, C, O, or X (or full word).");
                }
            }

            FileService.AppendLog($"User Filtered Logs By Action: {actionFilter}");

            DisplayParsedLogsByAction(filePath, actionFilter);
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
        



    }

}




