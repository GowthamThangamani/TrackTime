using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Threading;

class Program
{
    [DllImport("user32.dll")]
    public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [StructLayout(LayoutKind.Sequential)]
    public struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    private static string logFilePath = "activity_log.txt";
    private static DateTime sessionStartTime;
    private static DateTime lastInputTime;
    private static int maxIdleTimeInSeconds = 5 * 60; // Adjusted for easier testing
    private static int idleCount = 0; // Count of idle occurrences
    private static TimeSpan totalActiveTimeToday = TimeSpan.Zero;

    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Show total time taken based on date");
            Console.WriteLine("2. Record time");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ShowTotalTime();
                    break;
                case "2":
                    RecordTime();
                    Console.ReadKey();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    static void ShowTotalTime()
    {
        if (!File.Exists(logFilePath))
        {
            Console.WriteLine("No data available.");
        }
        else
        {
            var logData = File.ReadAllLines(logFilePath)
                .Select(line => line.Split(','))
                .Where(parts => parts.Length == 3)
                .Select(parts => new
                {
                    Date = DateTime.Parse(parts[0]).Date,
                    Duration = TimeSpan.FromSeconds(double.Parse(parts[2]))
                })
                .GroupBy(entry => entry.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    TotalDuration = group.Aggregate(TimeSpan.Zero, (sum, entry) => sum + entry.Duration)
                });

            Console.WriteLine("\nTotal Time by Date:");
            foreach (var entry in logData)
            {
                Console.WriteLine(@$"{entry.Date:MMM dd, yyyy} - {entry.TotalDuration:hh\:mm\:ss}");
            }
        }
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
    static void RecordTime()
    {
        DateTime currentDate = DateTime.Now.Date;
        idleCount = 0;
        totalActiveTimeToday = GetTotalTimeForToday(currentDate);

        sessionStartTime = DateTime.Now;
        lastInputTime = DateTime.Now;

        bool isRunning = true;

        while (isRunning)
        {
            var lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO));

            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint idleTimeMilliseconds = (uint)Environment.TickCount - lastInputInfo.dwTime;
                var idleTimeInSeconds = idleTimeMilliseconds / 1000;

                if (idleTimeInSeconds >= maxIdleTimeInSeconds)
                {
                    // Log active time if idle for too long
                    LogActiveTime(true);
                }
                else
                {
                    // User is active, update lastInputTime and continue
                    lastInputTime = DateTime.Now;
                }

                Console.Clear();
                Console.WriteLine(@$"Running Time: {(DateTime.Now - sessionStartTime):hh\:mm\:ss}");
                Console.WriteLine($"Idle Time: {(idleTimeInSeconds >= maxIdleTimeInSeconds ? idleTimeInSeconds : 0):F0} sec");
                Console.WriteLine($"{currentDate:MMMM dd, yyyy}");
                Console.WriteLine($"Idle Count: {idleCount}");
                Console.WriteLine(@$"Total Time Taken ({currentDate:MMMM dd, yyyy}): {totalActiveTimeToday + (DateTime.Now - sessionStartTime):hh\:mm\:ss}");

                // Check if user pressed 'S' to stop
                Console.WriteLine("Press 'S' to stop the timer and log the time.");
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S)
                {
                    isRunning = false; // Stop the loop when 'S' is pressed
                }

                Thread.Sleep(1000);
            }
        }

        // Log the active time when stopped
        LogActiveTime();
        Console.WriteLine("\nTimer stopped. Time logged.");
    }



    static TimeSpan GetTotalTimeForToday(DateTime date)
    {
        if (!File.Exists(logFilePath))
            return TimeSpan.Zero;

        return File.ReadAllLines(logFilePath)
            .Where(line => line.StartsWith(date.ToString("yyyy-MM-dd")))
            .Select(line => line.Split(',')[2])
            .Select(duration => TimeSpan.FromSeconds(double.Parse(duration)))
            .Aggregate(TimeSpan.Zero, (total, next) => total.Add(next));
    }


    private static void LogActiveTime(bool isToVerifyThreshold = false)
    {
        DateTime now = DateTime.Now;
        TimeSpan activeDuration = now - sessionStartTime;

        // Log active time only if it's greater than idle time threshold
        if (isToVerifyThreshold == false || activeDuration.TotalSeconds > maxIdleTimeInSeconds)
        {
            string logEntry = $"{sessionStartTime:yyyy-MM-dd HH:mm:ss},{now:yyyy-MM-dd HH:mm:ss},{activeDuration.TotalSeconds}";
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);

            totalActiveTimeToday += activeDuration;
        }

        sessionStartTime = now; // Reset session start time
    }
}
