using System.IO;

#nullable enable
namespace MazeTest.CommandLine
{
    class GeneratorArgs
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public int Seed { get; set; }
        public bool WriteMaze { get; set; }
        public FileInfo? Output { get; set; }
        public bool WriteJson { get; set; }
        public FileInfo? JsonOutput { get; set; }

        public override string ToString()
        {
            return $"[Generate] :" +
                $"\n\tWidth : {Width}," +
                $"\n\tHeight : {Height}," +
                $"\n\tCellSize : {CellSize}," +
                $"\n\tSeed : {Seed}," +
                $"\n\tWrite Maze: {WriteMaze}," +
                $"\n\tOutput : {Output?.FullName ?? "null"}," +
                $"\n\tWrite Json : {WriteJson}," +
                $"\n\tJsonOutput : {JsonOutput?.FullName ?? "null"}";
        }
    }
}
#nullable disable