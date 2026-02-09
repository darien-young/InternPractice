using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/*  Phase 2: Simple Parsing
 *      Given a log line:
 *          Example log lines:
 *              2026-02-04 08:20:55AM - User attempted to add 'Kyle' (Age 15) to 'Teenager'
 *              
 *      * detect whether the line contains "attempted", "added", or "cancelled"
 *      * extract the name(s) (if present) inside ''
 *      * extract the category name
 *      * extract the age (if present) inside ()
 *      * For example, for the above log line, the output would be:
 *      
 *      | Timestamp | Action     | Name | Age | Category |
 *      | --------- | ---------- | ---- | --- | -------- |
 *      | 08:20:55  | AttemptAdd | Kyle | 15  | Teenager |
 *      
 *      Note, the user should have the option to see both parsed and unparsed log lines, and the program 
 *      should handle cases where some information is missing (e.g., no age provided).
 * 
 *      02/09/2026 : Let's implement the functionality for new log files to generate based on the date.
 *      For example, the first time it runs on 2026-02-09, it should create a log file named inclusive of the current date `2026_02_09` and write all logs for that day to that file.
 *      The following day, it should create a new log file named `2026_02_10` and write all logs for that day to that file, and so on.
 *      
 *      You may edit the overall current code structure to follow coding best practices (ease of reading, classes having their own segregated files, etc.,
 *      and you may also add additional functionality if you think it would be useful (e.g., allowing the user to specify a date range for which to display logs, 
 *      or allowing the user to filter logs by action type).
 */

namespace InternPractice
{
    internal class ReadFiles
    {
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

        public static void DisplayParsedLogs(string filePath)
        {
            var logs = ParseLogFile(filePath);

            Console.WriteLine("\n| Timestamp | Action     | Name    | Age | Category    |");
              Console.WriteLine("|------------|-----------|---------|-----|-------------|");

            foreach (var l in logs)
            {
                Console.WriteLine($"| {l.TimeStamp,-9} | {l.Action,-10} | {l.Name,-7} | {l.Age,-3} | {l.Category,-11} |");
            }
        }



    }

}




