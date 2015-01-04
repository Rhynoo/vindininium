using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vindinium.Behavior
{
    class DrinkingBehavior : IBehavior
    {
        RhynoBot bot;
        Path path;

        public DrinkingBehavior(RhynoBot bot)
        {
            path = null;
            this.bot = bot;
        }
		
		public override string ToString ()
		{
			return "Drinking";
		}

        public void CheckTransitions()
        {
            if ((path != null) && (path.Count == 0) && (bot.hero.life > 90))
            {
                bot.behavior = new MiningBehavior(bot);
                return;
            }
			if (isFleeingConditionCompleted()) 
			{
				bot.behavior = new FleeingBehavior (bot);
				return;
			}
        }

        public void Do()
        {
            if (path != null)
            {
                if (path.Count > 0)
                {
                    Console.WriteLine("Path : " + path.ToString());
                    bot.FollowPath(path);
                }
                else
                    path = null;
            }

            if (path == null)
            {
                Tavern tavern = bot.serverStuff.taverns.ElementAt(0);
                Console.WriteLine("Tavern : (" + tavern.x + ", " + tavern.y + ")");
                path = bot.finder.FindPath(bot.hero.pos.x, bot.hero.pos.y, tavern.x, tavern.y);
                if (path != null)
                {
                    path.Add(new Node(tavern.x, tavern.y));
                    Console.WriteLine("Path : " + path.ToString());
                }
            }
        }

		private bool isFleeingConditionCompleted ()
		{
			Hero nearestEnemy = bot.serverStuff.heroes [0];
            Tavern nearestTavern = bot.serverStuff.taverns.ElementAt(0);
            int distTavernX = (nearestTavern.x - bot.hero.pos.x);
			int distTavernY = (nearestTavern.y - bot.hero.pos.y);
			int distEnemyX = (nearestEnemy.pos.x - bot.hero.pos.x);
			int distEnemyY = (nearestEnemy.pos.y - bot.hero.pos.y);

            if(!(((distEnemyX > 0) ^ (distTavernX > 0)) || ((distEnemyY > 0) ^ (distTavernY > 0))) && 
                (nearestEnemy.life > bot.hero.life + 20) && 
                (bot.hero.life < 40) && 
                (nearestEnemy.distanceToMyHero <= nearestTavern.distance))
            {
				return true;
			}
			return false;
		}
    }
}
