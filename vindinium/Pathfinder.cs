using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vindinium
{
    public class Pathfinder
    {
        /**
         * Matrice contenant les valeurs des cases de l'environnement : -1 présence
         * d'obstacles, [0,1] sinon
         **/
        private Tile[][] map;
        /** Liste de noeuds à explorer **/
        private List<Node> OpenList;
        /** Liste de noeuds déjà explorés **/
        private List<Node> CloseList;

        private Node[,] nodes;

        int tx;
        int ty;

        /**
         * Crée et initialise une instance de pathfinder utilisé pour l'algorithme A*
         * 
         * @param m la matrice de l'environnement
         */
        public Pathfinder(Tile[][] m) 
        {
		    map = m;
		    OpenList = new List<Node>();
		    CloseList = new List<Node>();
		    nodes = new Node[m.Length, m.Length];
		    for (int x = 0; x < m.Length; x++) {
			    for (int y = 0; y < m.Length; y++) {
				    nodes[x, y] = new Node(x, y);
			    }
		    }
	    }

        /**
         * Cherche le meilleur chemin entre 2 cases
         * @param sx abscisse de la case source
         * @param sy ordonnée de la case source
         * @param tx abscisse de la case destination
         * @param ty ordonnée de la case destinationy
         * @return Une liste de Cases représentant le chemin à emprunter
         */
        public Path FindPath(int sx, int sy, int tx, int ty)
        {
            OpenList.Clear();
            CloseList.Clear();
            OpenList.Add(nodes[sx, sy]);
            nodes[tx, ty].parent = null;
            nodes[sx, sy].cost = 0;

            this.tx = tx;
            this.ty = ty;

            while ((OpenList.Count() != 0))
            {
                // On recupère le premier noeud de la liste
                Node current = OpenList.First();
                if (current == nodes[tx, ty])
                {
                    // s'il s'agit du noeud destination, on s'arette
                    break;
                }
                OpenList.Remove(current);
                OpenList.Sort();

                CloseList.Add(current);
                // le noeud est maintenant exploré

                // On va explorer les noeuds voisins
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        if ((Math.Abs(x)+Math.Abs(y)) != 1)
                        {
                            continue;
                        }
                        int xp = x + current.x;
                        int yp = y + current.y;
                        //Si le neoud voisin est valide
                        if (IsValid(xp, yp))
                        {
                            int nextStepCost = current.cost + 1;
                            Node neighbour = nodes[xp, yp];

                            // On le reevalue s'il a deja été évalué
                            if (nextStepCost < neighbour.cost)
                            {
                                if (OpenList.Contains(neighbour))
                                {
                                    OpenList.Remove(neighbour);
                                    OpenList.Sort();
                                }
                                if (CloseList.Contains(neighbour))
                                {
                                    CloseList.Remove(neighbour);
                                }
                            }
                            // et on l'ajoute à la liste d'exploration s'il n'est contenu dans aucune des listes
                            if (!OpenList.Contains(neighbour)
                                    && !(CloseList.Contains(neighbour)))
                            {
                                neighbour.cost = nextStepCost;
                                OpenList.Add(neighbour);
                                OpenList.Sort();
                                neighbour.parent = current;
                            }
                        }
                    }
                }
            }

            if (nodes[tx, ty].parent == null)
            {
                // Si le noeud destination n'a pas de parent, il n'y a pas de solution
                return null;
            }
            //Sinon on recupère le chemin à parcourir par backtracking
            Path path = new Path();
            Node target = nodes[tx, ty];
            while (target != nodes[sx, sy])
            {
                path.Prepend(target.x, target.y);
                target = target.parent;
            }

            return path;
        }

        /**
         * Vérifie la validité d'une case (si elle est navigable)
         * 
         * @param xp
         *            l'abscisse de la case
         * @param yp
         *            l'ordonnée de la case
         * @return {@code true} si la case est valide, {@code false} sinon
         */
        private bool IsValid(int xp, int yp)
        {
            if ((xp == tx) && (yp == ty)) return true;
            bool notvalid = (xp < 0 || (xp > map.Length - 1)
                    || (yp > map.Length - 1) || (yp < 0) || (map[yp][xp] != Tile.FREE));
            return !notvalid;
        }
    }
    /**
        * Classe interne représentant un noeud dans le cadre d'A*
        */

    public class Node : IComparable
    {
        /** Cout du noeud */
        public int cost { get; set; }
        /** abscisse du noeud */
        public int x { get; set; }
        /** Ordonnée du noeud */
        public int y { get; set; }
        /** Noeud parent */
        public Node parent { get; set; }

        /**
            * Constructeur de la classe interne Noeud
            * 
            * @param x
            *            l'abscisse du noeud
            * @param y
            *            l'ordonnée du noeud
            */
        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
            parent = null;
        }

        public override string ToString()
        {
            return "("+x+", "+y+")";
        }

        /**
            * Méthode de comparaison d'un noeud avec un autre en fonction du cout
            * 
            * @return -1 si le cout du noeud est inférieur à celui du noeud
            *         comparé, 1 s'il est supérieur et 0 s'il sont égaux
            */
        int IComparable.CompareTo(Object other)
        {
            Node o = (Node)other;

            if (cost < o.cost)
            {
                return -1;
            }
            else if (cost > o.cost)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
    public class Path : List<Node>
    {
        /** Représente une liste de cases */

        /**
         * Retourne l'abscisse de la case dont l'index est spécifié
         * 
         * @param index l'index de la case
         * @return l'abscisse de la case
         */
        public int X
        {
            get
            {
                return this.ElementAt(0).x;
            }
        }

        /**
         * Retourne l'ordonnée de la case dont l'index est spécifié
         * 
         * @param index l'index de la case
         * @return l'ordonnée de la case
         */
        public int Y
        {
            get
            {
                return this.ElementAt(0).y;
            }
        }

        /**
         * Ajoute en tête de liste une case  
         * 
         * @param x l'abscisse de la case
         * @param y l'ordonnée de la case
         */
        public void Prepend(int x, int y)
        {
            this.Insert(0, new Node(x, y));
        }

        public override String ToString()
        {
            string s = "";
            foreach (Node step in this)
            {
                s += step;
            }
            return s;
        }
    }
}
