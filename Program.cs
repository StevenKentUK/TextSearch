using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Configuration;

namespace TextSearch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                bool endApp = false;

                while (!endApp)
                {
                    Console.Clear();
                    var defaultPath = ConfigurationManager.AppSettings["DefaultPath"];

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Default path = {defaultPath}");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Enter path to search or press Enter (or space then Enter) to use default path:");
                    Console.ForegroundColor = ConsoleColor.White;
                    var path = Console.ReadLine();
                    path = path.Trim();
                    if (string.IsNullOrEmpty(path))
                    {
                        path = defaultPath;
                    }

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Enter part of file name:");
                    Console.ForegroundColor = ConsoleColor.White;
                    var fileName = Console.ReadLine();
                    var filter = "*" + fileName + "*.*";

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Enter text to search for:");
                    Console.ForegroundColor = ConsoleColor.White;
                    var searchValue = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Searching for: '{searchValue}', using file filter: '{filter}' and path: '{path}'.  Please wait...");

                    var sw = new Stopwatch();
                    sw.Start();
                    var results = (from file in Directory.EnumerateFiles(path, filter, SearchOption.AllDirectories)
                                   from line in File.ReadLines(file)
                                   where line.Contains(searchValue)
                                   select new
                                   {
                                       FileName = file
                                   }).ToList();

                    var resultsFiltered = results.GroupBy(x => x.FileName).Select(y => y).ToList();
                    sw.Stop();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Found '{searchValue}', {results.Count()} times within {resultsFiltered.Count()} files in {ConvertMilliseconds(sw.ElapsedMilliseconds)}");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Listing files:");
                    Console.ForegroundColor = ConsoleColor.White;
                    var count = 0;
                    foreach (var result in resultsFiltered)
                    {
                        count++;
                        Console.WriteLine("#" + count.ToString("D5") + " - " + result.Key);
                    }
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("Press 'x' and Enter to close the app, or press any other key and Enter to continue: ");
                    if (Console.ReadLine() == "x") endApp = true;
                }

            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.WriteLine(e.ToString());
            }
        }

        private static string ConvertMilliseconds(long ms)
        {
            //Taken from: https://stackoverflow.com/a/9994060
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            string answer = string.Format("{0:D2} minutes, {1:D2} seconds and {2:D3} millisecsonds",
                                    t.Minutes,
                                    t.Seconds,
                                    t.Milliseconds);
            return answer;
        }
    }
}
