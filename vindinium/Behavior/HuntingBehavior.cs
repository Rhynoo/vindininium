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

        public const int SPAWN_POINT_LIMIT = 2;
        public const int MAX_HUNTING_DISTANCE = 7;

        public HuntingBehavior(RhynoBot bot, Hero target)
        {
            path = null;
            this.target = target;
            this.bot = bot;
        }
		
		public override string ToString ()
		{
			return "Hunting";
		}

        public void CheckTransitions()
        {
            if (isDrinkingConditionCompleted())
            {
                bot.behavior = new DrinkingBehavior(bot);
                return;
            }
            else if (isFleeingConditionCompleted())//fleeing
            {
                bot.behavior = new FleeingBehavior(bot);
                return;
            }
            else if (isMiningConditionCompleted())//Hunting
            {
                bot.behavior = new MiningBehavior(bot);
                return;
            }
            else return;
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

        private bool isDrinkingConditionCompleted()
        {
            return (bot.hero.life <= RhynoBot.LIFE_LIMIT);
        }

		private bool isFleeingConditionCompleted ()
		{
			if ((target.life > bot.hero.life + 20) && (bot.hero.life < 40) && (target.distanceToMyHero <= FleeingBehavior.MAX_FLEE_DISTANCE)) {
				return true;
			}
			return false;
		}

        private bool isMiningConditionCompleted()
        {
            //target strong || target near base || target dead
            if ((target.life > bot.hero.life + 20) || (target.distanceToSpawnPoint <= SPAWN_POINT_LIMIT)) 
                return true;
            else
                return false;
        }
    }
}
