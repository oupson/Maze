using System.Drawing;

namespace MazeLib.Explorers
{
    public class RightHandExplorer : IExplorer
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
         * Return true if there is a right wall. It depend of the rotation.
         */
        private bool HasRightWall()
        {
            return CurrentOrientation switch
            {
                0 => CurrentCell.HasRightWall,
                90 => CurrentCell.HasTopWall,
                180 => CurrentCell.HasLeftWall,
                270 => CurrentCell.HasBottomWall,
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

            if (HasRightWall())
            {
                if (CantGoForward())
                {
                    CurrentOrientation = (CurrentOrientation + 90) % 360;
                    MovementCount += 1;
                    return;
                }
                MoveForward(Maze);
            }
            else
            {
                CurrentOrientation = (CurrentOrientation - 90) % 360;
                if (CurrentOrientation < 0)
                {
                    CurrentOrientation += 360;
                }
                MustGoForward = true;
            }

            MovementCount += 1;
        }

        public void RenderPath(Graphics graphics)
        {
            var explorer = new Bitmap(Maze.CellFactorDrawing, Maze.CellFactorDrawing);
            using (var gr = Graphics.FromImage(explorer))
            {
                gr.TranslateTransform(Maze.CellFactorDrawing / 2, Maze.CellFactorDrawing / 2);
                gr.RotateTransform(-CurrentOrientation);
                gr.TranslateTransform(-Maze.CellFactorDrawing / 2, -Maze.CellFactorDrawing / 2);
                gr.DrawRectangle(
                    ColorPen ?? Pens.Red,
                    Maze.CellFactorDrawing / 10,
                    Maze.CellFactorDrawing / 10,
                    Maze.CellFactorDrawing - Maze.CellFactorDrawing / 5,
                    Maze.CellFactorDrawing - Maze.CellFactorDrawing / 5
                );
                gr.DrawLine(
                    ColorPen ?? Pens.Red,
                    Maze.CellFactorDrawing / 2,
                    Maze.CellFactorDrawing / 10,
                    Maze.CellFactorDrawing / 2,
                    Maze.CellFactorDrawing / 2
                );
            }
            graphics.DrawImage(explorer, CurrentCell.Column * Maze.CellFactorDrawing, CurrentCell.Row * Maze.CellFactorDrawing);
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