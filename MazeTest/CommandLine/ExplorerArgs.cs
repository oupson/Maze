using System.IO;

#nullable enable
namespace MazeTest.CommandLine
{
    class ExplorerArgs
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public int Seed { get; set; }
        public int Delay { get; set; }
        public FileInfo? Output { get; set; }
        public bool WriteJson { get; set; }
        public FileInfo? JsonOutput { get; set; }
        public bool WriteMaze { get; set; }
        public FileInfo? MazeOutput { get; set; }


        public override string ToString()
        {
            return $"[Generate] :" +
                $"\n\tWidth : {Width}," +
                $"\n\tHeight : {Height}," +
                $"\n\tCellSize : {CellSize}," +
                $"\n\tSeed : {Seed}," +
                $"\n\tDelay : {Delay}," +
                $"\n\tOutput : {Output?.FullName ?? "null"}," +
                $"\n\tWrite Json : {WriteJson}," +
                $"\n\tWrite Maze : {WriteMaze}," +
                $"\n\tJsonOutput : {JsonOutput?.FullName ?? "null"}," +
                $"\n\tMaze Output : {MazeOutput}";
        }
    }
}
#nullable disable