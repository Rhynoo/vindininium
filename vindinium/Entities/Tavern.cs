using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vindinium
{
    public class Tavern : IComparable<Tavern>
    {
        public int x { get; private set; }
        public int y { get; private set; }

        public int distance { get; set; }

        public Tavern(int x, int y)
        {
            this.x = x;
            this.y = y;
            distance = 0;
        }

        public void UdpateDistance(Pos pos, Pathfinder finder)
        {
            Path path = finder.FindPath(pos.x, pos.y, x, y);
            if (path != null)
                distance = path.Count;
            else
                distance = int.MaxValue;
            //distance = Utils.Distance(pos, new Pos(x, y));
        }

        public int CompareTo(Tavern obj)
        {
            if (distance < obj.distance)
            {
                return -1;
            }
            else if (distance > obj.distance)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
