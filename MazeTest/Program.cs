using MazeLib;
using MazeLib.generator;
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
            Console.WriteLine("Hello World!");
            var maze = new PrimsMazeGenerator(100, 10, 10).GenerateMaze();
            maze.GenerateBitmap().Save("maze.png");
            File.WriteAllText("maze.json", JsonConvert.SerializeObject(maze, Formatting.Indented));
            maze.Explorers.Add(new LeftHandExplorer
            {
                color = Pens.Red,
            });
            maze.FullExplore("explored.gif");
            Console.WriteLine(JsonConvert.SerializeObject(maze.Explorers[0], Formatting.Indented));
        }
    }
}
