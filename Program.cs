using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Configuration;

namespace TextSearch
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                //Stops the application from ending unless prompted by the user
                var exitApp = new bool();


                //Will loop forever until the user types; 'x' (see below for where this is assigned)
                while (!exitApp)
                {
                    //Clear the screen
                    Console.Clear();

                    //Display default path value in app.config file
                    var defaultPath = ConfigurationManager.AppSettings["DefaultPath"];
                    WriteSummary("T E X T  - S E A R C H -  P R O G R A M");

                    //Ask user for a file path or use default
                    WritePrompt($"Type file path to search or press Enter key to use '{defaultPath}' as the default path:");
                    var path = Console.ReadLine().Trim();
                    path = string.IsNullOrEmpty(path) ? defaultPath : path;

                    //Ask user for file name using wildcards if necessary
                    WritePrompt("Type file name or fuzzy search using '*' as a wildcard character (e.g. *my file*.* or *.txt - *.* is the default):");
                    var filter = Console.ReadLine();
                    filter = string.IsNullOrEmpty(filter) ? "*.*" : filter;

                    //Ask user for text to search for
                    WritePrompt("Type text to search for:");
                    var searchValue = Console.ReadLine();

                    //Let user know what's about to happen...
                    WriteInfo($"Searching for text: '{searchValue}' within files named: '{filter}' and located in: '{path}', please wait...", true);

                    //Create and start a stopwatch for the stats purposes
                    var sw = new Stopwatch();
                    sw.Start();

                    //Search folder, subfolders and files for the text and add each of the results to a list...
                    var results = (from file in Directory.EnumerateFiles(path, filter, SearchOption.AllDirectories)
                                   from line in File.ReadLines(file)
                                   where line.Contains(searchValue)
                                   select new
                                   {
                                       FileName = file
                                   }).ToList();

                    //Filter the results so we don't show duplicates
                    var resultsFiltered = results.GroupBy(x => x.FileName).Select(y => y).ToList();
                    
                    //Stop the stopwatch
                    sw.Stop();

                    //Display results summary as a headline, just to be dramatic!
                    WriteInfo("S E A R C H - R E S U L T S:", true);
                    WriteSummary($"Searched for   : {searchValue}");
                    WriteSummary($"Total text hits: {results.Count()}");
                    WriteSummary($"Total file hits: {resultsFiltered.Count()}");
                    WriteSummary($"Processing time: {ConvertMilliseconds(sw.ElapsedMilliseconds)}");

                    //Display results header information
                    WriteInfo($"No.   Path", true);
                    WriteInfo("----- ----");

                    //Display the results (without duplicates)
                    var count = 0;
                    foreach (var result in resultsFiltered)
                    {
                        count++;
                        WriteOutput(count.ToString("D5") + " " + result.Key);
                    }

                    //Prompt user to continue with a new search or exit
                    WritePrompt("Press 'x' and Enter to close the app, or press any other key and Enter to continue: ", true);

                    //If user entered 'x' then exit the application
                    if (Console.ReadLine() == "x") exitApp = true;
                }

            }
            catch (Exception e)
            {
                //Using 'ToString()' on an error object returns the full information including the message and the call stack, this is very handy for resolving issues.
                WriteError(e.ToString(), true);
            }
        }

        private static string ConvertMilliseconds(long ms)
        {
            //Taken from: https://stackoverflow.com/a/9994060
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            string answer = string.Format("{0:D2} minutes, {1:D2} seconds and {2:D3} milliseconds",
                                    t.Minutes,
                                    t.Seconds,
                                    t.Milliseconds);
            return answer;
        }

        private static void WriteInfo(string message, bool newLine = false)
        {
            if (newLine) NewLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            ResetConsoleForeColour();
        }

        private static void WritePrompt(string message, bool newLine = false)
        {
            if (newLine) NewLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            ResetConsoleForeColour();
        }

        private static void WriteOutput(string message, bool newLine = false)
        {
            if (newLine) NewLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            ResetConsoleForeColour();
        }

        private static void WriteError(string message, bool newLine = false)
        {
            if (newLine) NewLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            ResetConsoleForeColour();
        }

        private static void WriteSummary(string message, bool newLine = false)
        {
            if (newLine) NewLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            ResetConsoleForeColour();
        }

        private static void ResetConsoleForeColour()
        {
            //Always reset console font colour back to neutral white
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void NewLine()
        {
            //Always reset console font colour back to neutral white
            Console.WriteLine(Environment.NewLine);
        }
    }
}
