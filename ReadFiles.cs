using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*  Phase 1: Read & Display File Contents
 * - Open text file, and print each line to the console.
 * - Count the total number of lines and display it at the end (this should exclude blank spaces and empty lines).
 * - Handle potential exceptions (e.g., file not found, access denied) gracefully, providing informative error messages to the user.
 * - Ensure that the file is properly closed after reading, even if an error occurs (consider using 'using' statements for resource management).
 * - This should be incorporated into the main program flow, allowing users the option to either read the programlog file, or proceed to add new entries without reading the file first.
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
