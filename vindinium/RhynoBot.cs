using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vindinium.Behavior;

namespace vindinium
{
    class RhynoBot
    {
        public ServerStuff serverStuff {get; set;}
        private Tile[][] board = null;
        private Random random = new Random();
        public Hero hero {get; set;}

        public Pathfinder finder { get; set; }

        private Pos lastPos = null;

        public static int LIFE_LIMIT = 25;

        public IBehavior behavior{get; set;}

        private void Init()
        {
            Console.Out.WriteLine("> RhynoBot fight !");
            serverStuff.createGame();
            if (serverStuff.errored == false)
            {
                ShowWebPage();
            }
        }

        public RhynoBot(ServerStuff serverStuff)
        {
            this.serverStuff = serverStuff;
            this.behavior = new MiningBehavior(this);
        }

        public void Run()
        {
            Init();

            while (serverStuff.finished == false && serverStuff.errored == false)
            {
                Console.Out.WriteLine("-- New Turn : " + serverStuff.currentTurn + " : " + this.behavior);
                UpdateDatas();
								
				Console.Write ("Mines : ");
				foreach (Mine mine in serverStuff.mines) {
					Console.Write (mine.distance + " - ");
				}
				Console.WriteLine ();
				Console.Write ("Tavernes : ");
				foreach (Tavern tavern in serverStuff.taverns) {
					Console.Write (tavern.distance + " - ");
				}
				Console.WriteLine ();
				Console.Write ("Heros : ");
				foreach (Hero h in serverStuff.heroes) {
					Console.Write (h.distanceToMyHero + " - ");
				}
				Console.WriteLine ();
				
                if (serverStuff.currentTurn <= 4)
                {
                    lastPos = hero.spawnPos;
                }

                Console.Out.WriteLine("Hero at : " + hero.pos.x + " " + hero.pos.y);

                AmIDead();

                behavior.CheckTransitions();

                behavior.Do();

                lastPos = hero.pos;

                Console.Out.WriteLine("-- Completed turn : " + serverStuff.currentTurn);
                if (serverStuff.errored)
                {
                    Console.Out.WriteLine("error: " + serverStuff.errorText);
                }
                Console.Out.WriteLine();
            }

            Console.Out.WriteLine("> RhynoBot finished fight");
        }

        private void AmIDead()
        {
            if(Utils.Distance(hero.pos, lastPos) > 2)
            {
                behavior = new MiningBehavior(this);
            }
        }

        private void UpdateDatas()
        {
            hero = serverStuff.myHero;
            board = serverStuff.board;
            finder = new Pathfinder(serverStuff.board);

            foreach (Mine mine in serverStuff.mines)
            {
                mine.UdpateDistance(hero.pos, finder);
            }
            serverStuff.mines.Sort();

            foreach (Tavern tavern in serverStuff.taverns)
            {
                tavern.UdpateDistance(hero.pos, finder);
            }
            serverStuff.taverns.Sort();

            foreach (Hero h in serverStuff.heroes)
            {
                h.UdpateDistance(hero.pos, finder);
                h.UpdateSpawnDistance(finder);
            }
            serverStuff.heroes.Sort();
        }

        private void ShowWebPage()
        {
            //opens up a webpage so you can view the game, doing it async so we dont time out
            new Thread(delegate()
            {
                System.Diagnostics.Process.Start(serverStuff.viewURL);
            }).Start();
        }

        /**
         * Fait suivre un chemin par le personnage
         * @param path le chemin à suivre
         */
        public void FollowPath(Path path)
        {
            if (path != null && (path.Count > 0))
            {
                Node step = path.ElementAt(0);
                if ((hero.pos.x == path.X) && (hero.pos.y == path.Y))
                {
                    path.Remove(step);
                    if (path.Count > 0)
                        step = path.ElementAt(0);
                    else
                    {
                        path = null;
                    }
                }
                if (path != null)
                {
                    MoveTowards(path.X, path.Y);
                    // use/attack of a near case i.e. mine or tavern so remove it from the path
                    if (IsTileactivable(board[path.Y][path.X]))
                        path.Remove(step);
                }
            }
            else serverStuff.moveHero(Direction.Stay);
        }

        private bool IsTileactivable(Tile tile)
        {
            switch (tile)
            {
                case Tile.GOLD_MINE_1:
                case Tile.GOLD_MINE_2:
                case Tile.GOLD_MINE_3:
                case Tile.GOLD_MINE_4:
                case Tile.GOLD_MINE_NEUTRAL:
                case Tile.TAVERN:
                    return true;
                case Tile.FREE:
                default:
                    return false;
            }
        }

        public void MoveTowards(int x, int y)
        {
            if (hero.pos.y > y)
            {
                serverStuff.moveHero(Direction.West);
            }
            else if (hero.pos.y < y)
            {
                serverStuff.moveHero(Direction.East);
            }
            else if (hero.pos.x < x)
            {
                serverStuff.moveHero(Direction.South);
            }
            else if (hero.pos.x > x)
            {
                serverStuff.moveHero(Direction.North);
            }
            else
            {
                serverStuff.moveHero(Direction.Stay);
            }
        }

        private void MoveRandom()
        {
            switch (random.Next(0, 6))
            {
                case 0:
                    serverStuff.moveHero(Direction.East);
                    break;
                case 1:
                    serverStuff.moveHero(Direction.North);
                    break;
                case 2:
                    serverStuff.moveHero(Direction.South);
                    break;
                case 3:
                    serverStuff.moveHero(Direction.Stay);
                    break;
                case 4:
                    serverStuff.moveHero(Direction.West);
                    break;
            }
        }
    }
}
