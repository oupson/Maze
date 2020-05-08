using System.Drawing;

namespace MazeLib.Explorers
{
    public class LeftHandExplorer : IExplorer
    {
        public Cell CurrentCell { get; set; }
        public int MovementCount { get; set; }
        public Pen ColorPen { get; set; }
        public Maze Maze { get; set; }

        private int CurrentOrientation = 0;

        /**
         *      0
         *  90      270
         *      180
         * Return true if there is a left wall. It depend of the rotation
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

        /**
         *      0
         *  90      270
         *      180
         * Return true if there is a wall in front of the robot.
         */
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
        public void Explore()
        { 
            if (MustGoForward)
            {
                MoveForward(Maze);
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
                MoveForward(Maze);
            }
            else
            {
                CurrentOrientation = (CurrentOrientation + 90) % 360;
                MustGoForward = true;
            }

            MovementCount += 1;
        }

        public void RenderPath(Graphics graphics)
        {
            var explorer = new Bitmap(Maze.CellSize, Maze.CellSize);
            using (var gr = Graphics.FromImage(explorer))
            {
                gr.TranslateTransform(Maze.CellSize / 2, Maze.CellSize / 2);
                gr.RotateTransform(-CurrentOrientation);
                gr.TranslateTransform(-Maze.CellSize / 2, -Maze.CellSize / 2);
                gr.DrawRectangle(
                    ColorPen ?? Pens.Red,
                    Maze.CellSize / 10,
                    Maze.CellSize / 10,
                    Maze.CellSize - Maze.CellSize / 5,
                    Maze.CellSize - Maze.CellSize / 5
                );
                gr.DrawLine(
                    ColorPen ?? Pens.Red,
                    Maze.CellSize / 2,
                    Maze.CellSize / 10,
                    Maze.CellSize / 2,
                    Maze.CellSize / 2
                );
            }
            graphics.DrawImage(explorer, CurrentCell.Column * Maze.CellSize, CurrentCell.Row * Maze.CellSize);
        }

        public void Setup(Maze maze, Cell startingCell)
        {
            this.Maze = maze;
            this.CurrentCell = startingCell;
            this.MovementCount = 0;
            this.CurrentOrientation = 0;
        }
    }
}