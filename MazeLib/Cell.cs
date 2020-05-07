using System;
using System.Collections.Generic;
using System.Text;

namespace MazeLib
{
    public class Cell
    {
        public int Row;
        public int Column;
        public bool HasTopWall;
        public bool HasBottomWall;
        public bool HasLeftWall;
        public bool HasRightWall;
    }
}
