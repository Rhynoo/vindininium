using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vindinium.Behavior
{
    class HuntingBehavior : IBehavior
    {
        RhynoBot bot;
        Path path;
        Hero target;

        public HuntingBehavior(RhynoBot bot, Hero target)
        {
            path = null;
            this.target = target;
            this.bot = bot;
        }

        public void Do()
        {
            path = bot.finder.FindPath(bot.hero.pos.x, bot.hero.pos.y, target.pos.x, target.pos.y);
            if ((path != null) && (path.Count > 0) && (target.life <= bot.hero.life - 20))
            {
                path.Add(new Node(target.pos.x, target.pos.y));
                Console.WriteLine("Path : " + path.ToString());
                bot.FollowPath(path);
            }
            else
            {
                bot.behavior = new MiningBehavior(bot);
                return;
            }
      
        }
    }
}
