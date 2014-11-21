using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vindinium
{
    class RhynoBot
    {
        private ServerStuff serverStuff;
        private Tile[][] board = null;
        private Random random = new Random();
        private Hero hero;
        private Path path = null;
        private Pathfinder finder = null;

        private Tile goal = 0;
        private Pos lastPos = null;

        private Pos spawnPoint = null;

        private static int LIFE_LIMIT = 25;

        private void Init()
        {
            Console.Out.WriteLine("> RhynoBot fight !");
            serverStuff.createGame();
            if (serverStuff.errored == false)
            {
                ShowWebPage();
            }
            goal = Tile.GOLD_MINE_NEUTRAL;
        }

        public RhynoBot(ServerStuff serverStuff)
        {
            this.serverStuff = serverStuff;
        }

        public void Run()
        {
            Init();

            while (serverStuff.finished == false && serverStuff.errored == false)
            {
                Console.Out.WriteLine("-- New Turn : " + serverStuff.currentTurn);
                UpdateDatas();

                if (serverStuff.currentTurn <= 4)
                {
                    spawnPoint = hero.pos;
                    lastPos = spawnPoint;
                }

                Console.Out.WriteLine("Hero at : " + hero.pos.x + " " + hero.pos.y);

                DidIDied();

                DisplayGoal();

                if (goal != Tile.TAVERN)
                {
                    if (hero.life <= LIFE_LIMIT)
                    {
                        path = null;
                        goal = Tile.TAVERN;
                    }
                }
                if (path != null)
                {
                    if (path.Count > 0)
                    {
                        Console.WriteLine("Path : " + path.ToString());
                        FollowPath(path);
                    }
                    else path = null;
                }
                if (path == null)
                {
                    //i need a beer
                    if (hero.life <= LIFE_LIMIT)
                    {
                        Tavern tavern = serverStuff.taverns.ElementAt(0);
                        Console.WriteLine("Tavern : (" + tavern.x + ", " + tavern.y + ")");
                        path = finder.FindPath(hero.pos.x, hero.pos.y, tavern.x, tavern.y);
                        if (path != null)
                        {
                            path.Add(new Node(tavern.x, tavern.y));
                            Console.WriteLine("Path : " + path.ToString());
                            goal = Tile.TAVERN;
                        }
                    }
                    else
                    {
                        Mine mine = null;
                        for (int i = 0; i < serverStuff.mines.Count; i++)
                        {
                            mine = serverStuff.mines.ElementAt(i);
                            if (mine.id != hero.id)
                                break;
                        }
                        Console.WriteLine("Mine : (" + mine.x + ", " + mine.y + ")");
                          if ((mine != null) && (mine.id != hero.id))
                        {
                            path = finder.FindPath(hero.pos.x, hero.pos.y, mine.x, mine.y);
                            goal = Tile.GOLD_MINE_NEUTRAL;
                        }
                    }
                }
                if (path == null) serverStuff.moveHero(Direction.Stay);

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

        private void DisplayGoal()
        {
            if (goal == Tile.GOLD_MINE_NEUTRAL)
                Console.Out.WriteLine("Goal : Mine");
            else if (goal == Tile.TAVERN)
                Console.Out.WriteLine("Goal : Tavern");
            else
                Console.Out.WriteLine("No goal");
        }

        private void DidIDied()
        {
            if(Utils.Distance(hero.pos, lastPos) > 2)
            {
                path = null;
                goal = Tile.GOLD_MINE_NEUTRAL;
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

        private void MoveTowards(int x, int y)
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
