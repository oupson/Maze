using MazeLib.Explorers;
using MazeLib.generator;
using Newtonsoft.Json;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MazeTest
{
    class Program
    {
        static int Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("Program was called with : [{0}]", string.Join(", ", args));
#endif
            var generatorCommand = new Command("generate", description: "Maze generator")
            {
                new Option<int>(
                    "--width",
                    getDefaultValue: () => 10,
                    description: "Width of the maze"
                ),
                new Option<int>(
                    "--height",
                    getDefaultValue: () => 10,
                    description: "Height of the maze"
                ),
                new Option<FileInfo>(
                    new string[] { "--output", "-o" },
                    getDefaultValue: () => null
                ),
                new Option<FileInfo>(
                    new string[] { "--gif-output", "-g" },
                    getDefaultValue: () => null
                ),
                new Option<FileInfo>(
                    new string[] { "--json-output", "-j" },
                    getDefaultValue: () => null
                ),
                new Option<int>(
                    "--delay",
                    getDefaultValue: () => 100
                )
            };
            generatorCommand.Handler = CommandHandler.Create<int, int, FileInfo, FileInfo, FileInfo, int>((width, height, output, gifOutput, jsonOutput, delay) =>
            {
#if DEBUG
                Console.WriteLine("Generate : {{ Width : {0}, Height : {1}, Output : {2}, GifOutput : {3}, JsonOutput : {4}, Delay : {5} }}", width, height, output?.FullName ?? "null", gifOutput?.FullName ?? "null", jsonOutput?.FullName ?? "null", delay );
#endif

                var date = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
                if (output == null)
                    output = new FileInfo($"maze-{date}.png");
                if (gifOutput == null)
                    gifOutput = new FileInfo($"explored-{date}.gif");

                var maze = new PrimsMazeGenerator(width, height).GenerateMaze();
                maze.CellFactorDrawing = 100;
                maze.GenerateBitmap().Save(output.FullName);

                if (jsonOutput != null)
                {
                    File.WriteAllText(jsonOutput.FullName, JsonConvert.SerializeObject(maze, Formatting.Indented));
                }

                maze.Explorers.Add(new LeftHandExplorer
                {
                    ColorPen = Pens.Red,
                });
                maze.Explorers.Add(new RightHandExplorer
                {
                    ColorPen = Pens.Blue
                });

                maze.FullExplore(gifOutput.FullName, delay);

#if DEBUG
                foreach (var ex in maze.Explorers)
                {
                    Console.WriteLine("{0} : {1}", ex.GetType(), JsonConvert.SerializeObject(ex, Formatting.Indented));
                }
#endif
            });

            var statCommand = new Command("stats", description: "Run stat programm")
                {
                    new Option<int>(
                        "--iter",
                        getDefaultValue: () => 100,
                        description: "Number of maze created for each size of maze"
                    ),
                    new Option<FileInfo>(
                        new string[]{"--output", "-o"},
                        getDefaultValue: () => null,
                        description: "Output file for statistiques"
                    )
                };
            statCommand.Handler = CommandHandler.Create<int, FileInfo>((iter, output) =>
            {
#if DEBUG
                Console.WriteLine("Stats : {{ Iter : {0}, Output : {1} }}", iter, output?.FullName ?? "null");
#endif
                StatsProgram.DoStat(iter, output);
            });

            var rootCommand = new RootCommand
            {
                statCommand,
                generatorCommand
            };

            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
