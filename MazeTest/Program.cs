using MazeLib;
using MazeLib.generator;
using MazeTest.Explorers;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;

namespace MazeTest
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine(string.Join(", ", args));
#endif
            // C# args does not contain the path
            var stats = false;
            var iter = 100;
            if (args.Length >= 1)
            {
                if (args[0] == "stats")
                {
                    stats = true;
                }
                if (args.Length >= 2 & stats)
                {
                    iter = Convert.ToInt32(args[1]);
                }
            }
            if (!stats)
            {
                var maze = new PrimsMazeGenerator(20, 20).GenerateMaze();
                maze.CellFactorDrawing = 100;
                maze.GenerateBitmap().Save("maze.png");
                File.WriteAllText("maze.json", JsonConvert.SerializeObject(maze, Formatting.Indented));
                maze.Explorers.Add(new LeftHandExplorer
                {
                    ColorPen = Pens.Red,
                });
                maze.Explorers.Add(new RightHandExplorer
                {
                    ColorPen = Pens.Blue
                });
                maze.FullExplore("explored.gif", 100);
                Console.WriteLine(JsonConvert.SerializeObject(maze.Explorers[0], Formatting.Indented));
            }
            else
            {
                StatistiqueProgram.DoStat(iter);
            }
        }
    }
}
