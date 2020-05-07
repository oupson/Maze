﻿using MazeLib.generator;
using MazeTest.Explorers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MazeTest
{
    class StatistiqueProgram
    {
        public static void DoStat(int iterations = 100)
        {
#if DEBUG
            Console.WriteLine("DoStat({0})", iterations);
#endif
            if (iterations <= 0)
                throw new Exception("Error, iterations must be > 0");

            System.IO.File.Create("statistique.txt").Close();
            foreach (var width in new List<int> { 10, 20, 30, 50, 75, 100, 500}) {
                var maze = NextMaze(width, width);
                float leftAverage = maze.Item1;
                float rightAverage = maze.Item2;

                for (int i = 1; i < iterations; i++)
                {
                    maze = NextMaze(width, width);
                    leftAverage = (i * leftAverage + maze.Item1) / (i + 1);
                    rightAverage = (i * rightAverage + maze.Item2) / (i + 1);
                }
                System.IO.File.AppendAllText("statistique.txt", string.Format("[{0}x{1}] Left Hand Average : {2}; Right Hand Average : {3}\n", width, width, leftAverage, rightAverage));
                Console.WriteLine("{0} vs {1}", leftAverage, rightAverage);
            }
        }

        private static Tuple<int, int> NextMaze(int width, int height)
        {
            var maze = new PrimsMazeGenerator(width, width).GenerateMaze();
            maze.ExploreTimeout = -1;

            var leftExplorer = new LeftHandExplorer();
            var rightExplorer = new RightHandExplorer();

            maze.Explorers.Add(leftExplorer);
            maze.Explorers.Add(rightExplorer);

            maze.FullExplore();
            return new Tuple<int, int>(leftExplorer.MovementCount, rightExplorer.MovementCount);
        }
    }
}
