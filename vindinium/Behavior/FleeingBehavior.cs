using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vindinium.Behavior
{
	class FleeingBehavior : IBehavior
	{
		RhynoBot bot;
		Path path;

		public FleeingBehavior (RhynoBot bot)
		{
			this.bot = bot;
			this.path = null;
		}

		public void Do ()
		{
			Hero nearestEnemy = bot.serverStuff.heroes [1];
			avoidEnemy (nearestEnemy);
			return;
		}
		
		public void avoidEnemy (Hero nearestEnemy)
		{
			
			if (nearestEnemy.distanceToMyHero < 4) {
				path = null;
				int distanceX = (nearestEnemy.pos.x - bot.hero.pos.x);
				int distanceY = (nearestEnemy.pos.y - bot.hero.pos.y);
				bot.MoveTowards (bot.hero.pos.x - distanceX, bot.hero.pos.y - distanceY);
			} else {
				bot.behavior = new DrinkingBehavior(bot);
			}
		}
	}
}
