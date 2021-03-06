﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MazeLib.generator
{
    public class Wall
    {
        public bool isOpen = false;
        public bool isEdge = false;
        public Tuple<int, int>[] cells = new Tuple<int, int>[2];

        public static bool operator ==(Wall left, Wall right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Wall left, Wall right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Wall w)
            {
                return (cells[0] == w.cells[0] && cells[1] == w.cells[1]) || (cells[0] == w.cells[1] && cells[1] == w.cells[0]);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return isOpen.GetHashCode() + isEdge.GetHashCode() + cells.GetHashCode();
        }
    }
}
