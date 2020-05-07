using Newtonsoft.Json;
using System.Drawing;

namespace MazeLib
{
    /// <summary>
    /// Interface for a Maze Explorer.
    /// </summary>
    public interface IExplorer
    {
        /// <summary>
        /// The current cell. Must be updated for each deplacements.
        /// </summary>
        public Cell CurrentCell { get; set; }

        /// <summary>
        /// The maze the explorer is exploring.
        /// </summary>
        [JsonIgnore]
        public Maze Maze { get; set; }

        /// <summary>
        /// Pen used to draw the explorer
        /// </summary>
        [JsonIgnore]
        public Pen ColorPen { get; set; }

        /// <summary>
        /// The number of deplacement the explorer madded. Must be updated for each deplacements 
        /// </summary>
        /// <remarks>
        /// Rotation is a deplacement.
        /// </remarks>
        public int MovementCount { get; set; }

        /// <summary>
        /// Function called to know if the explorer have find the way out.
        /// </summary>
        /// <returns>True if the explorer have find a way out.</returns>
        public bool Arrived()
        {
            return (CurrentCell.Column == Maze.Width - 1) && (CurrentCell.Row == Maze.Height - 1);
        }

        /// <summary>
        /// Function called at every turns, the explorer must make only one deplacement (1 cell) or rotation.
        /// </summary>
        public void Explore();

        /// <summary>
        /// Function called to render the path of the robot. Must at least draw the robot.
        /// </summary>
        /// <param name="graphics">The maze.</param>
        public void RenderPath(Graphics graphics)
        {
            graphics.DrawRectangle(
                ColorPen ?? Pens.Red,
                CurrentCell.Column * Maze.CellFactorDrawing + Maze.CellFactorDrawing / 10,
                CurrentCell.Row * Maze.CellFactorDrawing + Maze.CellFactorDrawing / 10,
                Maze.CellFactorDrawing * 4 / 5,
                Maze.CellFactorDrawing * 4 / 5
            );
        }

        /// <summary>
        /// Function called to setup the explorer
        /// </summary>
        /// <param name="maze">The maze that will be explored</param>
        /// <param name="startingCell">The starting cell</param>
        public void Setup(Maze maze, Cell startingCell)
        {
            this.Maze = maze;
            this.CurrentCell = startingCell;
            this.MovementCount = 0;
        }
    }
}
