using MazeLib;
using MazeLib.Explorers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace MazeApp
{
    class UserExplorer : IExplorer
    {
        public Cell CurrentCell { get; set; }
        public int MovementCount { get; set; }
        public Pen ColorPen { get; set; }
        public Maze Maze { get; set; }

        private Cell NextCell;
        public void Explore()
        {
            _renderNext = false;
            CurrentCell = NextCell;
            MovementCount++;
            isAwaiting = true;
        }


        private bool _renderNext = false;
        public bool CanRenderNext()
        {
            return (this as IExplorer).Arrived() || _renderNext;
        }

        public bool isAwaiting = true;
        public void OnKeyPress(Key key)
        {
            switch (key)
            {
                case Key.Up:
                    if (CurrentCell.HasTopWall)
                    {
                        return;
                    }
                    else
                    {
                        NextCell = Maze.cells[CurrentCell.Row - 1][CurrentCell.Column];
                        break;
                    }
                case Key.Down:
                    if (CurrentCell.HasBottomWall)
                    {
                        return;
                    }
                    else
                    {
                        NextCell = Maze.cells[CurrentCell.Row + 1][CurrentCell.Column];
                        break;
                    }
                case Key.Left:
                    if (CurrentCell.HasLeftWall)
                    {
                        return;
                    }
                    else
                    {
                        NextCell = Maze.cells[CurrentCell.Row][CurrentCell.Column - 1];
                        break;
                    }
                case Key.Right:
                    if (CurrentCell.HasRightWall)
                    {
                        return;
                    }
                    else
                    {
                        NextCell = Maze.cells[CurrentCell.Row][CurrentCell.Column + 1];
                        break;
                    }
            }
            _renderNext= true;
            isAwaiting = false;
        }

        public void Setup(Maze maze, Cell startingCell)
        {
            this.Maze = maze;
            this.CurrentCell = startingCell;
            this.MovementCount = 0;
        }
    }
}
