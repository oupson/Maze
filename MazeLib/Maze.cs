using MazeLib.Explorers;
using System;
using System.Collections.Generic;
using System.Drawing;


// TODO ONLY DRAW MAZE THE FIRST TIME
namespace MazeLib
{
    public class Maze
    {
        public int Width;
        public int Height;
        public Cell[][] cells;
        public int CellSize = 10;

        public int ExploreTimeout = 100_000;

        public List<IExplorer> Explorers = new List<IExplorer>();

        public Maze(int width, int height)
        {
            Width = width;
            Height = height;
            cells = new Cell[height][];
            for (int row = 0; row < height; row++)
            {
                cells[row] = new Cell[width];
            }
        }

        public void FullExplore()
        {
            Setup();
            var iter = 0;
            while (NotFullyExplored() && HasNotTimedOut(iter++))
            {
                Console.Write("\r[{0}:{1}]", iter, ExploreTimeout);
                ExploreWithoutDrawing();
            }
            Console.WriteLine();
        }

        public void FullExplore(string filename, int delay = 500)
        {
            Setup();
            var gifWritter = new GifWriter(filename, delay, 0);
            var iter = 0;
            while(NotFullyExplored() && HasNotTimedOut(iter++))
            {
                Console.Write("\r[{0}:{1}]", iter, ExploreTimeout);
                gifWritter.WriteFrame(Explore());
            }
            gifWritter.Dispose();
            Console.WriteLine();
        }

        public void Setup()
        {
            if (Explorers.Count == 0)
                throw new Exception("Error : Need at least 1 explorer");
            foreach (var ex in Explorers)
            {
                ex.Setup(this, cells[0][0]);
            }
        }

        public Bitmap RenderCurrent()
        {
            var btm = GenerateBitmap();
            using (var graphics = Graphics.FromImage(btm))
            {
                foreach (var ex in Explorers)
                {
                    ex.RenderPath(graphics);
                }
            }
            return btm;
        }

        public Bitmap Explore()
        {
            var btm = GenerateBitmap();
            using (var graphics = Graphics.FromImage(btm))
            {
                foreach (var ex in Explorers)
                {
                    if (!ex.Arrived())
                    {
                        ex.Explore();
                    }
                    ex.RenderPath(graphics);
                }
            }
            return btm;
        }

        public void ExploreWithoutDrawing()
        {
            foreach (var ex in Explorers)
            {
                if (!ex.Arrived())
                {
                    ex.Explore();
                }
            }
        }

        private bool HasNotTimedOut(int iter)
        {
            return (ExploreTimeout == -1) || (iter < ExploreTimeout);
        }

        public bool NotFullyExplored()
        {
            var res = false;
            foreach (IExplorer ex in Explorers)
            {
                res |= !ex.Arrived();
            }
            return res;
        }

        private Bitmap _mazeBitmap = null;
        public Bitmap GenerateBitmap()
        {
            if (_mazeBitmap == null)
            {
                _mazeBitmap = new Bitmap(Width * CellSize, Height * CellSize);

                using (var graphics = Graphics.FromImage(_mazeBitmap))
                {
                    graphics.Clear(Color.White);
                    graphics.DrawLine(Pens.Black, 0, 0, Width * CellSize, 0);
                    graphics.DrawLine(Pens.Black, 0, 0, 0, Height * CellSize);
                    graphics.DrawLine(Pens.Black, 0, Height * CellSize - 1, Width * CellSize, Height * CellSize - 1);
                    graphics.DrawLine(Pens.Black, Width * CellSize - 1, 0, Width * CellSize - 1, Height * CellSize);
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            if (cells[y][x].HasBottomWall)
                            {
                                graphics.DrawLine(Pens.Black, x * CellSize, y * CellSize + CellSize, x * CellSize + CellSize, y * CellSize + CellSize);
                            }
                            if (cells[y][x].HasRightWall)
                            {
                                graphics.DrawLine(Pens.Black, x * CellSize + CellSize, y * CellSize, x * CellSize + CellSize, y * CellSize + CellSize);
                            }
                        }

                    }
                }
            }
            return (Bitmap)_mazeBitmap.Clone();
        }
    }
}
