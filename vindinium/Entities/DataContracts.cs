using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace vindinium
{
    [DataContract]
    class GameResponse
    {
        [DataMember]
        internal Game game;

        [DataMember]
        internal Hero hero;

        [DataMember]
        internal string token;

        [DataMember]
        internal string viewUrl;

        [DataMember]
        internal string playUrl;
    }

    [DataContract]
    class Game
    {
        [DataMember]
        internal string id;

        [DataMember]
        internal int turn;

        [DataMember]
        internal int maxTurns;

        [DataMember]
        internal List<Hero> heroes;

        [DataMember]
        internal Board board;

        [DataMember]
        internal bool finished;
    }

    [DataContract]
    class Hero : IComparable<Hero>
    {
        [DataMember]
        internal int id;

        [DataMember]
        internal string name;

        [DataMember]
        internal int elo;

        [DataMember]
        internal Pos pos;

        [DataMember]
        internal int life;

        [DataMember]
        internal int gold;

        [DataMember]
        internal int mineCount;

        [DataMember]
        internal Pos spawnPos;

        [DataMember]
        internal bool crashed;

        public int strength { get; private set; }

        public int distanceToSpawnPoint { get; private set; }

        public int distanceToMyHero { get; private set; }

        public void UdpateDistance(Pos myPos, Pathfinder finder)
        {
            Path path = finder.FindPath(myPos.x, myPos.y, pos.x, pos.y);
            if (path != null)
                distanceToMyHero = path.Count;
            else
                distanceToMyHero = int.MaxValue;
        }

        public void UpdateSpawnDistance(Pathfinder finder)
        {
            Path path = finder.FindPath(pos.x, pos.y, spawnPos.x, spawnPos.y);
            if (path != null)
                distanceToSpawnPoint = path.Count;
            else
                distanceToSpawnPoint = int.MaxValue;
        }

        public int CompareTo(Hero obj)
        {
            if (distanceToMyHero < obj.distanceToMyHero)
            {
                return -1;
            }
            else if (distanceToMyHero > obj.distanceToMyHero)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    [DataContract]
    public class Pos
    {
        [DataMember]
        internal int x;

        [DataMember]
        internal int y;

        public Pos(int x1, int y1)
        {
            this.x = x1;
            this.y = y1;
        }

        public override bool Equals(Object obj)
        {
            Pos pos = (Pos)obj;
            if ((pos.x == x) && (pos.y == y))
            {
                return true;
            }
            else return false;
        }
    }

    [DataContract]
    class Board
    {
        [DataMember]
        internal int size;

        [DataMember]
        internal string tiles;
    }
}
