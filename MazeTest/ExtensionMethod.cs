using System;

namespace MazeTest
{
    public static class ExtensionMethod
    {
        public static T Let<T>(this T o, Func<T, T> action)
        {
            o = action.Invoke(o);
            return o;
        }
    }
}
