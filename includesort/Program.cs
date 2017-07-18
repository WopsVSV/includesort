using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace includesort
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "includesort";
            Console.WriteLine("Created by Sorin Vijoaica (Wops)\n");

            // Checks if the user just wants help
            if (args.Length == 1 && args[0] == "-h")
            {
                Console.WriteLine(Properties.Resources.Help);
                Console.ReadKey(true);
                return;
            }

            // Evaluates the arguments to set the flags
            ParseArguments(args);

            // Executes the sorting
            Sort();
        }


        private static void Sort()
        {
            string[] extensions = { ".cpp", ".c", ".h", ".hpp", ".cc" };

            string pathToCheck = string.IsNullOrEmpty(Flags.Path)
                ? Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                : Flags.Path;

            SearchOption option = Flags.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var files = System.IO.Directory.GetFiles(pathToCheck, "*.*", option)
                .Where(f => extensions.Any(p => f.ToLower().EndsWith(p)))
                .ToArray();

            foreach (var file in files)
            {
                // Init
                includes.Clear();
                var includeStadfxh = false;
                var firstIncludeIndex = -1;

                // We assume the source file is not terribly huge so we can use ReadAllLines
                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    // Case: before any includes
                    if (lines[i].StartsWith("#include"))
                    {
                        if (firstIncludeIndex == -1)
                            firstIncludeIndex = i;

                        if (lines[i].ToLower().Contains("stdafx.h"))
                        {
                            includeStadfxh = true;
                            continue;
                        }

                        ParseInclude(lines[i]);
                    }
                }

                includes.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase));

                int index = firstIncludeIndex;
                if (includeStadfxh)
                {
                    lines[index] = "#include \"stdafx.h\"";
                    index++;
                }

                // Add rest
                foreach (var includeStatement in includes)
                {
                    if (includeStatement.Symbol == '<')
                    {
                        lines[index++] = $"#include <{includeStatement.Name}>";
                    }
                    else
                    {
                        lines[index++] = $"#include \"{includeStatement.Name}\"";
                    }
                }

                System.IO.File.WriteAllLines(file, lines);
                Console.WriteLine("Sorted " + file);
            }

            Console.WriteLine("\nFinished sorting!");
            Console.ReadKey(true);
        }

        private static List<IncludeStatement> includes = new List<IncludeStatement>();

        private static void ParseInclude(string includeStr)
        {
            // First, delete "#include " and symbols
            includeStr = includeStr.Replace("#include ", string.Empty);

            char symbol = '"';
            if (includeStr.Contains("\""))
                symbol = '"';
            else if (includeStr.Contains("<") && includeStr.Contains(">"))
                symbol = '<';

            includeStr = includeStr.Replace("\"", string.Empty);
            includeStr = includeStr.Replace("<", string.Empty);
            includeStr = includeStr.Replace(">", string.Empty);

            // Now we are left with the name, let's create the include statement
            IncludeStatement include = new IncludeStatement(includeStr, symbol);

            includes.Add(include);
        }

        private static void ParseArguments(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-r")
                    Flags.Recursive = true;

                if (args[i] == "-p")
                {
                    try
                    {
                        if (Directory.Exists(args[i + 1]))
                        {
                            Flags.Path = args[i + 1];
                        }
                    }
                    catch { Console.WriteLine("Could not parse any path after -p"); }
                }
            }
        }
  
    }
}
