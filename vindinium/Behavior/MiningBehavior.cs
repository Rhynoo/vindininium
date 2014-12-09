﻿using System;
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
		Hero targetToHunt;

		public MiningBehavior (RhynoBot bot)
		{
			this.bot = bot;
			path = null;
		}

		public void CheckTransitions ()
		{
			if (isDrinkingConditionCompleted ()) {
				bot.behavior = new DrinkingBehavior (bot);
				return;
			} else if (isFleeingConditionCompleted ()) {//fleeing
				bot.behavior = new FleeingBehavior (bot);
				return;
			} else if (isHuntingConditionCompleted ()) {//Hunting
				bot.behavior = new HuntingBehavior (bot, targetToHunt);
				return;
			} else
				return;
		}

		public void Do ()
		{   
			if (path != null) {
				if (path.Count > 0) {
					Console.WriteLine ("Path : " + path.ToString ());
					bot.FollowPath (path);
				} else
					path = null;
			}

			if (path == null) {
				Mine mine = null;
				for (int i = 0; i < bot.serverStuff.mines.Count; i++) {
					mine = bot.serverStuff.mines.ElementAt (i);
					if (mine.id != bot.hero.id)
						break;
				}
				Console.WriteLine ("Mine : (" + mine.x + ", " + mine.y + ")");
				if ((mine != null) && (mine.id != bot.hero.id)) {
					path = bot.finder.FindPath (bot.hero.pos.x, bot.hero.pos.y, mine.x, mine.y);
				}
			}
			if (path == null)
				bot.serverStuff.moveHero (Direction.Stay);
		}

		private bool isDrinkingConditionCompleted ()
		{
			return (bot.hero.life <= RhynoBot.LIFE_LIMIT);
		}

		private bool isFleeingConditionCompleted ()
		{
			foreach (Hero hero in bot.serverStuff.heroes) {
				if ((hero.life > bot.hero.life + 20) && (hero.distanceToMyHero <= FleeingBehavior.MAX_FLEE_DISTANCE)) {
					return true;
				}
			}
			return false;
		}

		private bool isHuntingConditionCompleted ()
		{
			foreach (Hero hero in bot.serverStuff.heroes) {
				if ((hero.mineCount > bot.hero.mineCount) && (bot.hero.life > (hero.life + 20)) && (hero.distanceToMyHero <= HuntingBehavior.MAX_HUNTING_DISTANCE)) {
					targetToHunt = hero;
					return true;
				}
			}
			return false;
		}
	}
}
