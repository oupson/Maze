using System.Drawing;

namespace MazeLib
{
    public interface IExplorer
    {
        public Cell CurrentCell { get; set; }
        public int MovementCount { get; set; }
        public bool Arrived();
        public void Explore(Maze maze);
        public void RenderPath(Maze maze, Graphics graphics);
    }
}
