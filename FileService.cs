using System;

namespace InternConsoleApp
{
    public static class FileService
    {

        //log file name used on Desktop
        public const string LogFilePrefix = "ProgramLog_";
        public const string SnapshotFileName = "CategorySnapshot.txt";


        //APPENDLOG FUNCTION: a single log line (timestamped to the log file on the Desktop
        public static void AppendLog(string message)
        {
            try
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                string logsFolder = Path.Combine(desktop, "ProgramLogs");
                Directory.CreateDirectory(logsFolder);

                // Now creating new date-based filename
                string date = DateTime.Now.ToString("yyy-MM-dd");
                string fileName = $"ProgramLog_{date}.txt";
                string fullPath = Path.Combine(logsFolder, fileName);


                // Timestamp for inside log line
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:sstt");
                string line = $"{timestamp} - {message}";

                File.AppendAllLines(fullPath, new[] { line });
            }
            catch (Exception ex)
            {
                //Don't throw from logger; if logging fails, keep running.
                Console.WriteLine("Log Error: " + ex.Message);
            }
        }


        //PRINT TO TEXTFILE FUNCTION -- Function to print snapshot to the CategorySnapshot text file           //added bool to change snapshot title
        public static void PrintSnapshotToFile(Dictionary<AgeCategoryHelper.AgeCategory, List<string>> assigned, bool isFinalSnapShot = false)
        {
            try
            {
                // Get desktop path
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                // Use same ProgramLogs folder as logs
                string logsFolder = Path.Combine(desktop, "ProgramLogs");
                Directory.CreateDirectory(logsFolder);

                // Date-based snapshot filename
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string datedFileName = $"CategorySnapshot_{date}.txt";
                string fullPath = Path.Combine(logsFolder, datedFileName);


                var lines = new List<string>();

                // add separator when file already exists
                if (File.Exists(fullPath))
                {
                    lines.Add(string.Empty);
                    lines.Add("\n------------------------------\n");
                }

                //alters title depending on if program is closing
                string title = isFinalSnapShot ? "Final Snapshot" : "Category Snapshot";
                lines.Add($"-- {title} ({DateTime.Now:yyyy-MM-dd HH:mm:ss}) --");
                foreach (var kv in assigned)
                {
                    string assignedName = kv.Value.Count == 0 ? "(empty)" : string.Join(",", kv.Value);
                    lines.Add($"{(int)kv.Key}: {kv.Key} => {assignedName}");
                }
                //now appends instead of ovewriting; REMOVED console writing message.
                File.AppendAllLines(fullPath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing snapshot to file: {ex.Message}");
            }
        }
    }
}