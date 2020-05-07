using MazeLib;
using MazeLib.generator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MazeTest
{
    class LeftHandExplorer : IExplorer
    {
        public Cell CurrentCell { get; set; }
        public int MovementCount { get; set; }
        private bool isArrived = false;
        [JsonIgnore]
        public Pen color;

        private int CurrentOrientation = 0;

        public bool Arrived()
        {
            return isArrived;
        }


        /**
         *      0
         *  90      270
         *      180
         */
        private bool HasLeftWall()
        {
            return CurrentOrientation switch
            {
                0 => CurrentCell.HasLeftWall,
                90 => CurrentCell.HasBottomWall,
                180 => CurrentCell.HasRightWall,
                270 => CurrentCell.HasTopWall,
                // UNREACHABLE
                _ => false,
            };
        }

        private bool CantGoForward()
        {
            return CurrentOrientation switch
            {
                0 => CurrentCell.HasTopWall,
                90 => CurrentCell.HasLeftWall,
                180 => CurrentCell.HasBottomWall,
                270 => CurrentCell.HasRightWall,
                // UNREACHABLE
                _ => false,
            };
        }

        private void MoveForward(Maze maze)
        {
            if (CantGoForward())
            {
                return;
            }
            CurrentCell = CurrentOrientation switch
            {
                0 => maze.cells[CurrentCell.Row - 1][CurrentCell.Column],
                90 => maze.cells[CurrentCell.Row][CurrentCell.Column - 1],
                180 => maze.cells[CurrentCell.Row + 1][CurrentCell.Column],
                270 => maze.cells[CurrentCell.Row][CurrentCell.Column + 1],
                _ => null
            };
        }

        private bool MustGoForward = false;
        public void Explore(Maze maze)
        {
            if (CurrentCell == null)
            {
                CurrentCell = maze.cells[0][0];
            }

            if (MustGoForward)
            {
                MoveForward(maze);
                MustGoForward = false;
                MovementCount += 1;
                return;
            }

            if (HasLeftWall())
            {
                if(CantGoForward())
                {
                    CurrentOrientation = (CurrentOrientation - 90) % 360;
                    if (CurrentOrientation < 0)
                    {
                        CurrentOrientation += 360;
                    }
                    MovementCount += 1;
                    return;
                }
                MoveForward(maze);
            }
            else
            {
                CurrentOrientation = (CurrentOrientation + 90) % 360;
                MustGoForward = true;
            }

            MovementCount += 1;

            isArrived = CurrentCell.Column == maze.Width - 1 && CurrentCell.Row == maze.Height - 1;
        }

        public void RenderPath(Maze maze, Graphics graphics)
        {
            var explorer = new Bitmap(maze.CellFactorDrawing, maze.CellFactorDrawing);
            using (var gr = Graphics.FromImage(explorer))
            {
                gr.TranslateTransform(maze.CellFactorDrawing / 2, maze.CellFactorDrawing / 2);
                gr.RotateTransform(-CurrentOrientation);
                gr.TranslateTransform(-maze.CellFactorDrawing / 2, -maze.CellFactorDrawing / 2);
                gr.DrawRectangle(
                    color,
                    0,
                    0,
                    maze.CellFactorDrawing - 1,
                    maze.CellFactorDrawing - 1
                );
                gr.DrawLine(
                    color,
                    maze.CellFactorDrawing / 2,
                    0,
                    maze.CellFactorDrawing / 2,
                    maze.CellFactorDrawing / 2
                );
            }
            graphics.DrawImage(explorer, CurrentCell.Column * maze.CellFactorDrawing, CurrentCell.Row * maze.CellFactorDrawing);
        }
    }
}