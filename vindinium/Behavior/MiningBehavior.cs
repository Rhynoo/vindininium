using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vindinium.Behavior
{
    class MiningBehavior:IBehavior
    {
        RhynoBot bot;
        Path path;

        public MiningBehavior(RhynoBot bot)
        {
            this.bot = bot;
            path = null;
        }

        public void Do()
        {
            if (bot.hero.life <= RhynoBot.LIFE_LIMIT)
            {
                bot.behavior = new FleeingBehavior(bot);
                return;
            }
            
            if (path != null)
            {
                if (path.Count > 0)
                {
                    Console.WriteLine("Path : " + path.ToString());
                    bot.FollowPath(path);
                }
                else path = null;
            }

            if (path == null)
            {
                Mine mine = null;
                for (int i = 0; i < bot.serverStuff.mines.Count; i++)
                {
                    mine = bot.serverStuff.mines.ElementAt(i);
                    if (mine.id != bot.hero.id)
                        break;
                }
                Console.WriteLine("Mine : (" + mine.x + ", " + mine.y + ")");
                if ((mine != null) && (mine.id != bot.hero.id))
                {
                    path = bot.finder.FindPath(bot.hero.pos.x, bot.hero.pos.y, mine.x, mine.y);
                }
                
            }
            if (path == null) bot.serverStuff.moveHero(Direction.Stay);
        }
    }
}
