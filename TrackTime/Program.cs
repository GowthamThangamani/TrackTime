using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    // Importing the necessary function to get idle time
    [DllImport("user32.dll")]
    public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [StructLayout(LayoutKind.Sequential)]
    public struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    private static string logFilePath = "activity_log.txt"; // Log file path
    private static DateTime lastStartTime;
    private static DateTime lastInputTime;
    private static int maxIdleTimeInMinutes = 1; // Maximum idle time in minutes before stopping tracking

    static void Main()
    {
        // Check if log file exists and read the last total time if it does
        double totalTime = 0;
        if (File.Exists(logFilePath))
        {
            string[] logLines = File.ReadAllLines(logFilePath);
            if (logLines.Length > 0)
            {
                string lastLine = logLines[logLines.Length - 1];
                if (lastLine.StartsWith("Total Time"))
                {
                    // Extract the last total time from the last line
                    string[] parts = lastLine.Split(':');
                    if (parts.Length > 1 && double.TryParse(parts[1].Trim(), out double previousTotalTime))
                    {
                        totalTime = previousTotalTime;
                    }
                }
            }
        }

        while (true)
        {
            var lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO));

            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint idleTime = (uint)Environment.TickCount - lastInputInfo.dwTime;
                var idleTimeInSeconds = idleTime / 1000;

                Console.Clear();
                Console.WriteLine($"Idle Time: {idleTimeInSeconds} seconds");

                if (idleTimeInSeconds == 0)
                {
                    // System is active
                    if (lastStartTime == default)
                    {
                        lastStartTime = DateTime.Now;
                        Console.WriteLine("System is active! Tracking start time.");
                    }

                    // Update last input time to the current time (system is active)
                    lastInputTime = DateTime.Now;
                }
                else
                {
                    // If the system is idle, calculate the idle duration and check for max idle time
                    if (lastStartTime != default)
                    {
                        // Calculate the time since last input
                        TimeSpan idleDuration = DateTime.Now - lastInputTime;
                        if (idleDuration.TotalMinutes >= maxIdleTimeInMinutes)
                        {
                            // Max idle time exceeded, stop tracking
                            DateTime endTime = DateTime.Now;
                            TimeSpan activeDuration = endTime - lastStartTime;

                            // Log the activity and total time
                            LogActivity(lastStartTime, endTime, activeDuration);

                            totalTime += activeDuration.TotalSeconds;

                            // Reset start time
                            lastStartTime = default;

                            Console.WriteLine("Max idle time reached, ending tracking.");
                        }
                    }
                }

                Thread.Sleep(1000); // Check every second
            }
        }
    }

    static void LogActivity(DateTime startTime, DateTime endTime, TimeSpan duration)
    {
        string logEntry = $"Start Time: {startTime}, End Time: {endTime}, Duration: {duration.TotalSeconds} seconds";

        // Append to the log file
        File.AppendAllText(logFilePath, logEntry + Environment.NewLine);

        // After logging, append the total time at the end
        string totalTimeLog = $"Total Time: {GetTotalTime()} seconds";
        File.AppendAllText(logFilePath, totalTimeLog + Environment.NewLine);
    }

    static double GetTotalTime()
    {
        double totalTime = 0;

        if (File.Exists(logFilePath))
        {
            string[] logLines = File.ReadAllLines(logFilePath);
            foreach (var line in logLines)
            {
                if (line.StartsWith("Duration"))
                {
                    // Extract the duration from the log line
                    string[] parts = line.Split(':');
                    if (parts.Length > 1 && double.TryParse(parts[1].Trim().Split(' ')[0], out double duration))
                    {
                        totalTime += duration;
                    }
                }
            }
        }

        return totalTime;
    }
}
