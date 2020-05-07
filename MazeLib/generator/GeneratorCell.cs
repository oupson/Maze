using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace MazeLib.generator
{
    public class GeneratorCell
    {
        public int X;
        public int Y;
        public Wall Topwall;
        public Wall BottomWall;
        public Wall LeftWall;
        public Wall RightWall;
        public bool PartOfMaze = false;

        public List<Wall> GetWalls()
        {
            return new List<Wall> { Topwall, BottomWall, LeftWall, RightWall };
        }
    }
}