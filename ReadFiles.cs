using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
