using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vindinium
{
    public static class Utils
    {
        /// <summary>
        /// Manhattan distance between two position
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static int Distance(Pos pos1, Pos pos2)
        {
            return (int)(Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y));
        }
    }
}
