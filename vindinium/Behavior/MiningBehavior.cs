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

        public MiningBehavior (RhynoBot bot)
		{
			this.bot = bot;
			path = null;
		}
		
		public override string ToString ()
		{
			return "Mining";
		}

        public void Do ()
		{
			// Si l'ennemi le + proche est + puissant que moi et trop proche, fuire
			Hero nearestEnemy = bot.serverStuff.heroes [0];
			if (nearestEnemy.distanceToMyHero < 5 && nearestEnemy.life > bot.hero.life) {
				bot.behavior = new FleeingBehavior (bot);
				return;
			}
			// Si l'ennemi est proche, moins puissant mais possède plus
			// d'un tiers des mines, l'attaquer
			if (nearestEnemy.distanceToMyHero < 5 && 
			    nearestEnemy.life < bot.hero.life - 20 &&
			    nearestEnemy.mineCount > bot.serverStuff.mines.Count / 3) {
				bot.behavior = new HuntingBehavior (bot, nearestEnemy);
				return;
			}
			// Si la vie descend trop pendant le minage, on va boire
			if (bot.hero.life <= RhynoBot.LIFE_LIMIT) {
				bot.behavior = new DrinkingBehavior (bot);
				return;
			}
			// Si un chemin vers une mine est déjà défini, le suivre
			if (path != null) {
				if (path.Count > 0) {
					Console.WriteLine ("Path : " + path.ToString ());
					bot.FollowPath (path);
				} else
					path = null;
			}
			// Si l'on n'a rien à faire
			if (path == null) {
				Mine mine = null;
				// Chercher la mine qui n'est pas à nous la plus proche
				for (int i = 0; i < bot.serverStuff.mines.Count; i++) {
					mine = bot.serverStuff.mines.ElementAt (i);
					if (mine.id != bot.hero.id)
						break;
				}
				Console.WriteLine ("Mine : (" + mine.x + ", " + mine.y + ")");
				// Aller à la mine
				if ((mine != null) && (mine.id != bot.hero.id)) {
					path = bot.finder.FindPath (bot.hero.pos.x, bot.hero.pos.y, mine.x, mine.y);
				}
                
			}
			// Si toutes les mines sont à nous, rester immobile
			if (path == null)
				bot.serverStuff.moveHero (Direction.Stay);
		}
    }
}
