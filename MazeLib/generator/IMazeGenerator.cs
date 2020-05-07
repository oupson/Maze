namespace MazeLib.generator
{
    public interface IMazeGenerator
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Maze GenerateMaze();
    }
}
 