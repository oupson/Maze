using MazeLib.Explorers;
using MazeLib.generator;
using MazeTest.CommandLine;
using Newtonsoft.Json;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Drawing;
using System.IO;

namespace MazeTest
{
    class Program
    {
        static int Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("Program was called with : [{0}]", string.Join(", ", args));
#endif

            #region generator
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
                new Option<int>(
                    "--cell-size",
                    getDefaultValue: () => 10,
                    "Size of the cells of the maze"
                ),
                new Option<int>(
                    "--seed",
                    getDefaultValue: () => -1,
                    "Seed for the maze"
                ),
                new Option<bool>(
                    "--write-maze",
                    getDefaultValue: () => true,
                    description: "Output the generated bitmap. Default is true"
                ),
                new Option<FileInfo>(
                    new string[] { "--output", "-o" },
                    getDefaultValue: () => null,
                    description: "Output file for the maze"
                ),
                new Option<bool>(
                    "--write-json",
                    getDefaultValue: () => false,
                    description: "Output the generated json maze representation. False by default"
                ),
                new Option<FileInfo>(
                    new string[] { "--json-output", "-j" },
                    getDefaultValue: () => null,
                    description: "Output file for the generated json maze representation"
                )
            };

            generatorCommand.Handler = CommandHandler.Create<GeneratorArgs>((args) =>
            {
#if DEBUG
                Console.WriteLine(args.ToString());
#endif
                var date = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
                if (args.WriteMaze && args.Output == null)
                    args.Output = new FileInfo($"maze-{date}.png");
                if (args.WriteJson && args.JsonOutput == null)
                    args.JsonOutput = new FileInfo($"maze-{date}.json");

                var maze = args.Seed switch
                {
                    -1 => new PrimsMazeGenerator(args.Width, args.Height).GenerateMaze(),
                    _ => new PrimsMazeGenerator(args.Seed, args.Width, args.Height).GenerateMaze()
                };

                maze.CellSize = args.CellSize;
                if (args.WriteMaze)
                    maze.GenerateBitmap().Save(args.Output.FullName);

                if (args.WriteJson)
                {
                    File.WriteAllText(args.JsonOutput.FullName, JsonConvert.SerializeObject(maze, Formatting.Indented));
                }
            });
            #endregion

            #region explore
            var exploreCommand = new Command("explore", "Maze explorer")
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
                new Option<int>(
                    "--cell-size",
                    getDefaultValue: () => 10,
                    "Size of the cells of the maze"
                ),
                new Option<int>(
                    "--seed",
                    getDefaultValue: () => -1,
                    "Seed for the maze"
                ),
                new Option<int>(
                    "--delay",
                    getDefaultValue: () => 100,
                    description: "Delay for the frame of the gif animation"
                ),
                new Option<FileInfo>(
                    new string[] { "--output", "-o" },
                    getDefaultValue: () => null,
                    description: "Output file for the explore animation"
                ),
                new Option<bool>(
                    "--write-json",
                    getDefaultValue: () => false,
                    description: "Output the generated json maze representation. False by default"
                ),
                new Option<FileInfo>(
                    new string[] { "--json-output", "-j" },
                    getDefaultValue: () => null,
                    description: "Output file for the generated json maze representation"
                ),
                new Option<bool>(
                    "--write-maze",
                    getDefaultValue: () => false,
                    description: "Write the generated maze."
                ),
                new Option<FileInfo>(
                    "--maze-output",
                    getDefaultValue: () => null,
                    description: "Output file for the maze"
                )
            };

            exploreCommand.Handler = CommandHandler.Create<ExplorerArgs>((args) =>
            {
#if DEBUG
                Console.WriteLine(args.ToString());
#endif

                var date = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
                if (args.Output == null)
                    args.Output = new FileInfo($"maze-{date}.gif");
                if (args.WriteMaze && args.MazeOutput == null)
                    args.MazeOutput = new FileInfo($"maze-{date}.png");
                if (args.WriteJson && args.JsonOutput == null)
                    args.JsonOutput = new FileInfo($"maze-{date}.json");

                var maze = args.Seed switch
                {
                    -1 => new PrimsMazeGenerator(args.Width, args.Height).GenerateMaze(),
                    _ => new PrimsMazeGenerator(args.Seed, args.Width, args.Height).GenerateMaze()
                };

                maze.CellSize = args.CellSize;

                if (args.WriteMaze)
                    maze.GenerateBitmap().Save(args.MazeOutput.FullName);

                if (args.WriteJson)
                    File.WriteAllText(args.JsonOutput.FullName, JsonConvert.SerializeObject(maze, Formatting.Indented));

                maze.Explorers.Add(new LeftHandExplorer
                {
                    ColorPen = Pens.Red,
                });
                maze.Explorers.Add(new RightHandExplorer
                {
                    ColorPen = Pens.Blue
                });

                maze.FullExplore(args.Output.FullName, args.Delay);

#if DEBUG
                foreach (var ex in maze.Explorers)
                {
                    Console.WriteLine("{0} : {1}", ex.GetType(), JsonConvert.SerializeObject(ex, Formatting.Indented));
                }
#endif
            });
            #endregion

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
                generatorCommand,
                exploreCommand
            };

            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
