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

        public const int MAX_FLEE_DISTANCE = 5;

		public FleeingBehavior (RhynoBot bot)
		{
			this.bot = bot;
			this.path = null;
		}
		
		public override string ToString ()
		{
			return "Fleeing";
		}

        public void CheckTransitions()
        {
            //TODO
        }

		public void Do ()
		{
			Hero nearestEnemy = bot.serverStuff.heroes [0];
			Tavern nearestTavern = bot.serverStuff.taverns [0];
			// Si la bière est proche
			if (nearestTavern.distance < 8) {
				int distTavernX = (nearestTavern.x - bot.hero.pos.x);
				int distTavernY = (nearestTavern.y - bot.hero.pos.y);
				int distEnemyX = (nearestEnemy.pos.x - bot.hero.pos.x);
				int distEnemyY = (nearestEnemy.pos.y - bot.hero.pos.y);
				// Si la bière et l'ennemi ne sont pas dans la même direction
				if (((distEnemyX > 0) ^ (distTavernX > 0)) || ((distEnemyY > 0) ^ (distTavernY > 0))) {
					bot.behavior = new DrinkingBehavior (bot);
					return;
				}
				// Sinon, ne pas aller vers la bière et continuer à fuire
			}
			// Si un ennemi est proche
			avoidEnemy (nearestEnemy);
		}
		
		private void avoidEnemy (Hero nearestEnemy)
		{
			path = null;
			int distanceX = (nearestEnemy.pos.x - bot.hero.pos.x);
			int distanceY = (nearestEnemy.pos.y - bot.hero.pos.y);
			Pos direction = chooseDirection (distanceX, distanceY);
			Console.WriteLine ("Fuite de l'ennemi '" + nearestEnemy.name + 
			                   "' (" + nearestEnemy.pos.x + "," + nearestEnemy.pos.y + ") dans la direction" +
			                   "("+direction.x+","+direction.y+")");
			bot.MoveTowards (bot.hero.pos.x + direction.x, bot.hero.pos.y + direction.y);
		}

		private Pos chooseDirection (int distanceX, int distanceY)
		{
			/*Boolean xDirection = Math.Abs (distanceX) < Math.Abs (distanceY);
			Tile[][] board = bot.serverStuff.board;
			Pos direction = new Pos ();
			if (xDirection && distanceX < 0 && board [bot.hero.pos.x + 1] [bot.hero.pos.y] == Tile.FREE) {
				// Fuite direction X > 0
				direction.x = 1;
				direction.y = 0;
				return direction;
			}
			if (xDirection && distanceX > 0 && board [bot.hero.pos.x - 1] [bot.hero.pos.y] == Tile.FREE) {
				// Fuite direction Y < 0
				direction.x = -1;
				direction.y = 0;
				return direction;
			}
			if (!xDirection && distanceY < 0 && board [bot.hero.pos.x] [bot.hero.pos.y + 1] == Tile.FREE) {
				// Fuite direction Y > 0
				direction.x = 0;
				direction.y = 1;
				return direction;
			}
			if (!xDirection && distanceY > 0 && board [bot.hero.pos.x] [bot.hero.pos.y - 1] == Tile.FREE) {
				// Fuite direction Y < 0
				direction.x = 0;
				direction.y = -1;
			}*/
			
			Boolean xDirection = Math.Abs (distanceX) < Math.Abs (distanceY);
			Tile[][] board = bot.serverStuff.board;
			Pos direction = new Pos (0, 0);
			
			// Partir dans la direction opposée.
			direction.x = xDirection ? (distanceX < 0 ? 1 : -1) : 0;
			direction.y = xDirection ? 0 : (distanceY < 0 ? 1 : -1);
			// Si la direction souhaitée est bouchée
			if (directionBouchee (direction)) {
				direction.x = xDirection ? 0 : (distanceX < 0 ? 1 : -1);
				direction.y = xDirection ? (distanceY < 0 ? 1 : -1) : 0;
			}
			// Si cette deuxième direction est bouchée (on est dans un coin)
			if (directionBouchee (direction)) {
				// Ne pas bouger
				direction.x = 0;
				direction.y = 0;
			}
			
			return direction;
		}

		private bool directionBouchee (Pos direction)
		{
			return bot.hero.pos.x + direction.x < 0 ||
			    bot.hero.pos.x + direction.x >= bot.serverStuff.board.Length ||
			    bot.hero.pos.y + direction.y < 0 ||
			    bot.hero.pos.y + direction.y >= bot.serverStuff.board.Length ||
			    bot.serverStuff.board [bot.hero.pos.x + direction.x] [bot.hero.pos.y + direction.y] != Tile.FREE;
		}
	}
}
