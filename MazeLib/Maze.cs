﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace MazeLib
{
    public class Maze
    {
        public int Width;
        public int Height;
        public Cell[][] cells;
        public int CellFactorDrawing = 10;

        public int ExploreTimeout = 1_000;

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

        public void FullExplore(string filename)
        {
            if (Explorers.Count == 0)
                throw new System.Exception("Error : Need at least 1 explorer");
            var gifWritter = new GifWriter(filename, 100, 0);
            var iter = 0;
            while(NotFullyExplored() && iter++ < ExploreTimeout)
            {
                Console.Write("\r[{0}:{1}]", iter, ExploreTimeout);
                gifWritter.WriteFrame(Explore());
            }
            gifWritter.Dispose();
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
                        ex.Explore(this);
                    }
                    ex.RenderPath(this, graphics);
                }
            }
            return btm;
        }

        private bool NotFullyExplored()
        {
            var res = false;
            foreach (IExplorer ex in Explorers)
            {
                res |= !ex.Arrived();
            }
            return res;
        }

        public Bitmap GenerateBitmap()
        {
            var bitmap = new Bitmap(Width * CellFactorDrawing, Height * CellFactorDrawing);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.DrawLine(Pens.Black, 0, 0, Width * CellFactorDrawing, 0);
                graphics.DrawLine(Pens.Black, 0, 0, 0, Height * CellFactorDrawing);
                for (int y = 0; y < Height; y ++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (cells[y][x].HasBottomWall)
                        {
                            graphics.DrawLine(Pens.Black, x * CellFactorDrawing, y * CellFactorDrawing + CellFactorDrawing, x * CellFactorDrawing + CellFactorDrawing, y * CellFactorDrawing + CellFactorDrawing);
                        }
                        if (cells[y][x].HasRightWall)
                        {
                            graphics.DrawLine(Pens.Black, x * CellFactorDrawing + CellFactorDrawing, y * CellFactorDrawing, x * CellFactorDrawing + CellFactorDrawing, y * CellFactorDrawing + CellFactorDrawing);
                        }
                    }

                }
            }
            return bitmap;
        }


    }
}