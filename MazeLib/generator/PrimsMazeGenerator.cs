using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MazeLib.generator
{
    public class PrimsMazeGenerator : IMazeGenerator
    {
        private readonly Random random;

        public int Width { get; set; }
        public int Height { get; set; }

        public PrimsMazeGenerator(int width, int height)
        {
#if DEBUG
            var seed = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().GetHashCode();
            Console.WriteLine("Seed is : {0}", seed);
            random = new Random(seed);
#else
            random = new Random();
#endif

            Width = width;
            Height = height;
        }

        public PrimsMazeGenerator(int seed, int width, int height)
        {
            random = new Random(seed);
            Width = width;
            Height = height;
        }

        private GeneratorCell[][] GenerateInternalMaze()
        {
            var maze = new GeneratorCell[Height][];
            for (int row = 0; row < Height; row++)
            {
                maze[row] = new GeneratorCell[Width];
                for (int column = 0; column < Width; column++)
                {
                    var cell = new GeneratorCell
                    {
                        X = column,
                        Y = row
                    };

                    if (row == 0)
                    {
                        cell.Topwall = new Wall
                        {
                            isEdge = true
                        };
                    }
                    else
                    {
                        cell.Topwall = maze[row - 1][column].BottomWall;
                        cell.Topwall.cells[0] = new Point(row, column);
                        cell.Topwall.cells[1] = new Point(row - 1, column);
                    }

                    cell.BottomWall = new Wall();
                    if (row == Height - 1)
                    {
                        cell.BottomWall.isEdge = true;
                    } else
                    {
                        cell.BottomWall.cells[0] = new Point(row, column);
                        cell.BottomWall.cells[1] = new Point(row + 1, column);
                    }

                    if (column == 0)
                    {
                        cell.LeftWall = new Wall
                        {
                            isEdge = true
                        };
                    }
                    else
                    {
                        cell.LeftWall = maze[row][column - 1].RightWall;
                        cell.LeftWall.cells[0] = new Point(row, column);
                        cell.LeftWall.cells[1] = new Point(row, column - 1);
                    }

                    cell.RightWall = new Wall();
                    if (column == Width - 1)
                    {
                        cell.RightWall.isEdge = true;
                    } else
                    {
                        cell.RightWall.cells[0] = new Point(row, column);
                        cell.RightWall.cells[1] = new Point(row, column + 1);
                    }

                    maze[row][column] = cell;
                }
            }

            //Console.WriteLine(JsonConvert.SerializeObject(maze, Formatting.Indented));
            var wallList = new List<Wall>();
            wallList.AddRange(maze[0][0].GetWalls());
            maze[0][0].PartOfMaze = true;
            while (wallList.Count > 0)
            {
                var wall = wallList[random.Next(0, wallList.Count)];
                if (wall.isEdge)
                {
                    wallList.Remove(wall);
                    continue;
                }
                var cell0 = maze[wall.cells[0].X][wall.cells[0].Y];
                var cell1 = maze[wall.cells[1].X][wall.cells[1].Y];
                if (cell0.PartOfMaze && !cell1.PartOfMaze)
                {
                    cell1.PartOfMaze = true;
                    wallList.AddRange(cell1.GetWalls().Where((w) => w != wall));
                    wall.isOpen = true;
                }
                else if (!cell0.PartOfMaze && cell1.PartOfMaze)
                {
                    cell0.PartOfMaze = true;
                    wallList.AddRange(cell0.GetWalls().Where((w) => w != wall));
                    wall.isOpen = true;
                }
                wallList.Remove(wall);
            }
            return maze;
        }

        public Maze GenerateMaze()
        {
            var result = new Maze(Width, Height);
            var maze = GenerateInternalMaze();
            for (int row = 0; row < maze.Length; row++)
            {
                for (int col = 0; col < maze[row].Length; col++)
                {
                    var cell = maze[row][col];
                    result.cells[row][col] = new Cell {
                        Row = row,
                        Column = col,
                        HasTopWall = cell.Topwall.isEdge || !cell.Topwall.isOpen,
                        HasBottomWall = cell.BottomWall.isEdge || !cell.BottomWall.isOpen,
                        HasLeftWall = cell.LeftWall.isEdge || !cell.LeftWall.isOpen,
                        HasRightWall = cell.RightWall.isEdge || !cell.RightWall.isOpen
                    };
                }
            }
            return result;
        }
    }
}
