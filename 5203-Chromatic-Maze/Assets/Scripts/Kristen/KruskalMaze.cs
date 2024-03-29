using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/* Kruskal's Algorithm
 * 1. Convert each edge into a set for keeping track of its source and destination vertex and its weight (starting rank is 0 and starting parent value is -1 because they're all in their own set so no parent atm)
 * 2. Sort edges of graph in increasing order of their edge weights
 * 3. Create three distinct sets for maintaining all vertexes, keeping track of the hierarchy and ranking of each vertex.
 *    For each insertaion of an edge in the MST, the rank and parent of each vertex will be updated
 * 4. Add every vertex to the MST that doesn't create a cycle. If a vertex shares a parent with a previously added vertex, it will not be added because this will create a loop
*/

/* Detailed step by step of above after sorting edges
 * 1. Take a look at the first edge (the one with minimum edge cost). In order to insert this edge, we need the absolute value of both parents of the two vertices that make the edge.
 * If their parent values are the same (they should both be -1 at this point), we can add this edge to the MSP. The source and destination vertices of the edge depends on the ranking of the two vertices (i.e. either going from A -> B or B-> A where A and B are the two vertices and the child points to the parent).
 * If they have the same rank, you can have the connection go in either direction (A->B or B->A), but if one ranks higher than the other, the lower ranking vertex should be the source vertex and the highier ranking vertex shoudl be the destination vertex (i.e. ARank = 0 BRank = 1, therefore A->B)
 * Updating ranks:
 *      If you added an edge with vertices of the same rank, you'll have to update their rank (i.e. if you added A->B where B is now the parent, you have to +1 to the rank of B)
 *      You will also have to update the rankings of any ascenstors of B by +1. This is if it's been assigned a parent and doesn't have the intial -1 parent value
 * Updating the parents:
 *      If vertices had same rank: if we add the edge A->B, B will become A's parent and B's parent will become whatver A's parent was previously. The rank of B will increase by one as stated above.
 *      If vertices had diff. ranks: if we add A->B where B has the higher ranking, B becomes A's parent (B's parent stays the same)
 *      This process is repeated, going down the list of edges (that were sorted in ascending order) until you've added |V|-1 edges where V = number of vertices.
 *          Therefore the loop starts at E = 0 and loops until E = V-1
 *      The MST is now formed, but we must discard any edges we didn't look at.
 *      Discard any edges where a child and parent make up the edge
*/

//Above Explanation is based on this video: https://youtu.be/XP9dOgM4ctQ
//Example of Kruskal's Algorithm in C# (I took graph structure from this): https://www.programmingalgorithms.com/algorithm/kruskal's-algorithm/

public class KruskalMaze : MonoBehaviour
{
    public struct Maze
    {
        public LongestPath LP; //longest path struct that is the solution path
        public Graph tree; //A tree with cycles
        public List<Tile> deadends; //dead end tiles not a part of a cycle
        public List<Tile> rankZero; //All tiles ranked 0 (dead ends and cycled tiles)
        public Tile[] tiles; //The tile grid (from bottom left at index 0 to top right)
        public List<Tile> tileList; //Same as "tiles" but in list form
        public int h; //height of maze grid
        public int w; //width of maze grid
    }

    public struct LongestPath
    {
        public Tile entrance; //start of path
        public Tile exit; //end fo path
        public int length; //length of path
        public Tile[] path; //the tiles of the path starting at the entrance tile
    }

    public struct Graph
    {
        public int numEdges;
        public int numVertices;
        public Wall[] edges; //All edges of graph that have a wall
    }

    public static Graph CreateGraph(Wall[] walls, int v)
    {
        Graph graph = new Graph();
        graph.numVertices = v;
        graph.edges = walls;
        graph.numEdges = walls.Length;
        return graph;
    }

    public Maze CreateMaze(Graph graph, Tile[] subset, int cycles)
    {
        int vCount = graph.numVertices;
        //Tile[] subset = graph.vertices;

        //1. Sort the given edges by weight
        Array.Sort(graph.edges, delegate (Wall a, Wall b)
        {
            return a.weight.CompareTo(b.weight);
        });

        //2. Create maze (new tree, longest path, entrance, exit)
        Maze maze = RemoveEdges(graph, subset, cycles);

        //Print(graph.edges, graph.numEdges);
        Debug.Log("entrance: " +  maze.LP.entrance + ", exit: " + maze.LP.exit + ", length: " + maze.LP.length + ", deadend count: " + maze.deadends.Count);
        return maze; //mst with cycles
    }

    //Creates traversable tree in structure of maze (not mst) and disbales the walls, sets exit
    private Maze RemoveEdges(Graph graph, Tile[] subset, int cycles)
    {
        Wall[] edgeArray = graph.edges;
        int VCount = subset.Length;

        //2.1 Make a list of all edges of graph
        List<Wall> allEdges = new List<Wall>(edgeArray);

        //2.2 Creates a list that will become the spanning tree (list of walls that will be removed/edges apart of solution path)
        List<Wall> mst = new List<Wall>();

        int index = 0; //to cycle through edges
            int e = 0; //used as index for mstree
            while (e < VCount - 1)
            {
                Wall nextEdge = edgeArray[index++]; //why ++?
                Tile O = nextEdge.origin;
                Tile D = nextEdge.destination;

            if (Tile.GetRootParent(O) != Tile.GetRootParent(D))
                {
                    mst.Add(nextEdge);
                    nextEdge.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    e++;
                    allEdges.Remove(nextEdge);
                    Union(subset, O, D);
                }
            }

        //2.4 Remove the temporary lsit of edges from the main edge list
        foreach (Wall byeEdge in mst)
        {
            if(byeEdge != null)
            {
                byeEdge.disableEdge(); //remove the wall (The wall that's being disabled is the wall perpendicular to the imaginary edge (so the edge's origin and destination are the centres of two tiles))
            } 
        }

        //2.5reset ranks to 0
        ResetRanks(subset);


        //2.6 Update Ranks and set the exit to the root node
        List<Tile> leafs = GetDeadEnds(subset);
        int childrenIndex = 0;
        int path = 0;

        LongestPath LP = new LongestPath();
        LP = RanksAndPath(leafs, subset, childrenIndex, LP, path);
        LP.entrance.transform.Find("triangle-WHITE").gameObject.GetComponent<SpriteRenderer>().enabled = true;
        LP.exit.transform.Find("triangle-BLACK").gameObject.GetComponent<SpriteRenderer>().enabled = true;
        LP.path = GetLongestPath(LP.entrance, LP.length);

        //2.6 shuffle the mst edge list
        for (int i = 0; i < mst.Count; i++)
        {
            Wall temp = mst[i];
            int randomIndex = UnityEngine.Random.Range(i, mst.Count);
            mst[i] = mst[randomIndex];
            mst[randomIndex] = temp;
        }

        //2.7 Add cycles and create graph for maze
        Maze maze = new Maze();
        maze = AddCycles(cycles, mst, allEdges, leafs, LP);
        maze.tree.numVertices = graph.numVertices;
        maze.LP = LP;
        maze.tiles = subset;
        maze.tileList = subset.ToList();
        maze.h = GenerateGrid.hght;
        maze.w = GenerateGrid.wdth;

        return maze;
    }

    //Combine two tile subsets into the same subset
    public static void Union(Tile[] tiles, Tile o, Tile d)
    {

        if(Tile.GetRootParent(o).rank != Tile.GetRootParent(d).rank)
        {
            if (o.rank < d.rank) { //if O parent rank < D parent rank
                if (o.parent == o){
                    o.parent = d;
                    d.children.Add(o);
                    UpdateRanks(d);
                }
                else{
                    List<Tile> fromOtoRoot = new List<Tile>();
                    SwitchRoot(fromOtoRoot, o, d, tiles);
                }
            }
            else if (o.rank > d.rank)
            {
                if (d.parent == d){
                    d.parent = o;
                    o.children.Add(d);
                    UpdateRanks(o);
                }
                else{
                    List<Tile> fromOtoRoot = new List<Tile>();
                    SwitchRoot(fromOtoRoot, d, o, tiles);
                }
            }
            else //equal ranks
            {
                if (o.parent == o){
                    o.parent = d;
                    d.children.Add(o);
                    UpdateRanks(d);
                }
                else{
                    List<Tile> fromOtoRoot = new List<Tile>();
                    SwitchRoot(fromOtoRoot, o, d, tiles);
                }
            }
        }
        else //ranks are equal
        {
            if (o.parent == o)
            { //o and d have rank zero, each in own subset
                o.parent = d;
                d.children.Add(o);
                d.rank++;
            }
            else
            {
                List<Tile> fromOtoRoot = new List<Tile>();
                SwitchRoot(fromOtoRoot, o, d, tiles);
            }
        }
    }

    private static void UpdateRanksSpecial(Tile d, int startingRank)
    {
        d.rank = startingRank;
        if (d.parent != d)
        {
            UpdateRanksSpecial(d.parent, d.rank + 1);
        }
    }

    private static void UpdateRanks(Tile t) //***this updates the root's rank more than it has to (add condition)
    {
        t.rank++;
        if (t.parent != t)
        {
            UpdateRanks(t.parent);
        }
    }

    //When combining two subsets, this updates the child-parent relationships so everything pionts to one root node properly
    public static void SwitchRoot(List<Tile> fromOtoRoot, Tile o, Tile d, Tile[] tiles)
    {
        fromOtoRoot.Add(o);

        if (o.parent != o)
        {
            SwitchRoot(fromOtoRoot, o.parent, d, tiles);
        }
        else
        {
            fromOtoRoot.Reverse();
            
            int startingRank = fromOtoRoot[0].rank; //rank of old root
            fromOtoRoot[0].parent = fromOtoRoot[1];
            fromOtoRoot[0].children.Remove(fromOtoRoot[1]);
            fromOtoRoot[1].children.Add(fromOtoRoot[0]);

            //bool hasChild = false;

            //check if old root has any other children (if not, change its child to itself)
            //for(int i =0; i < tiles.Length; i++)
            //{
                //if(tiles[i] != fromOtoRoot[1] && tiles[i].parent == fromOtoRoot[0])
                //{
                //    hasChild = true;
                //    fromOtoRoot[0].children.Add(tiles[i]);
                //    break;
                //}
            //}

            //if(hasChild == false)
            //{
            //    fromOtoRoot[0].children.Add(fromOtoRoot[0]);
            //}

            for (int i = 1; i < fromOtoRoot.Count; i++)
            {
                fromOtoRoot[i].rank = startingRank + i;
                //fromOtoRoot[i].GetComponentInChildren<Text>().text = fromOtoRoot[i].rank.ToString();

                if (i < fromOtoRoot.Count - 1) //update parents except for the oriignal origin
                {
                    fromOtoRoot[i].parent = fromOtoRoot[i + 1];
                    fromOtoRoot[i].children.Remove(fromOtoRoot[i + 1]);
                    fromOtoRoot[i + 1].children.Add(fromOtoRoot[i]);
                }
            }
            Tile oldOrigin = fromOtoRoot[fromOtoRoot.Count - 1];
            oldOrigin.parent = d; //set origin's parent to destination
            d.children.Add(oldOrigin);

            UpdateRanksSpecial(d, oldOrigin.rank + 1);
        }
       
    }

    //Returns a list of the leaf nodes
    private static List<Tile> GetDeadEnds(Tile[] tiles)
    {
        
        List<Tile> deadends = new List<Tile>();

        for (int t = 0; t < tiles.Length; t++)
        {
            if (tiles[t].children.Count == 0) //double check //tiles[t].children.Contains(tiles[t]) || 
            {
                deadends.Add(tiles[t]);
            }
        }
        return deadends;
    }

    private static void ResetRanks(Tile[] tiles)
    {
        foreach (Tile t in tiles)
        {
            t.rank = 0;
            //t.parent.GetComponentInChildren<Text>().text = t.parent.rank.ToString();
        }
    }

    //Updates all ranks
    private static LongestPath RanksAndPath(List<Tile> leafs, Tile[] tiles, int childrenIndex, LongestPath LP, int LPathLength)
    {
        int n = 1; //current path length
        int leafIndex = Array.IndexOf(tiles, leafs[childrenIndex]);
        Tile child = tiles[leafIndex]; //the leaf node
        Tile entrance = child;
        
        while (child.parent != child) //not root node
        {
            if (child.parent.rank - child.rank <= 0)
            {
                child.parent.rank = child.rank + 1;
                //child.parent.GetComponentInChildren<Text>().text = child.parent.rank.ToString();
            }
            n++;
            child = child.parent;
        }

        if(n > LPathLength) //update longest path length value
        {
            
            LPathLength = n;
            LP.length = n; //entrance
            LP.entrance = entrance; //entrance
        }

        if (childrenIndex < leafs.Count - 1)
        {
            return RanksAndPath(leafs, tiles, childrenIndex + 1, LP, LPathLength);
        }
        else
        {
            LP.exit = child; //exit (root node)
            return LP;
        }
    }

    public Tile[] GetLongestPath(Tile startPoint, int length)
    {
        Tile[] pathh = new Tile[length];

        for (int i = 0; i < length; i++)
        {
            pathh[i] = startPoint;
            startPoint = startPoint.parent;
        }

        return pathh;
    }


    //get the path length from leaf to any part of solution path
    //prioritize adding cycles to the longer paths

    private Maze AddCycles(int cycles, List<Wall> tree, List<Wall> EdgesWithWalls, List<Tile> leafs, LongestPath longestP)
    {
        Maze maze = new Maze();
        Graph graph = new Graph();
        maze.rankZero = leafs;

        if (cycles == 0){
            maze.deadends = leafs;
            graph.edges = tree.ToArray();
            graph.numEdges = tree.Count;
            maze.tree = graph;
            return maze;
        }

        int addedCycles = 0;
        List<Wall> newTree = new List<Wall>(tree);
        List<Tile> newDends = new List<Tile>(leafs);

        //Ensuring cycles won't be added at entrance and exit points (unless another deadend is beside it and all other deadends wiht longer distance from solution path don't work)
        if (leafs.Contains(longestP.entrance)) {
            leafs.Remove(longestP.entrance);
        }
        if (leafs.Contains(longestP.exit)){
            leafs.Remove(longestP.exit);
        }

        //1. Create a list of dead-ends and the path length from the dead-end to the solution path
        //Dictionary<Tile, int> dict = new Dictionary<Tile, int>();
        List<KeyValuePair<Tile, int>> orderedList = new List<KeyValuePair<Tile, int>>();

        //2. add all the tiles (dead-ends) and paths to the dictioanry
        for (int i = 0; i < leafs.Count; i++)
        {  
            orderedList.Add(new KeyValuePair<Tile, int>(leafs[i], GetPathLength(leafs[i], longestP.path)));
            //dict.Add(leafs[i], GetPathLength(leafs[i], longestP.path));
        }

        //3. Order list from longest path (to solution path) to shortest
        //List<KeyValuePair<Tile, int>> orderedList = dict.ToList();
        orderedList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
        orderedList.Reverse();

        //4. Get a list of the dead ends only (in order of path length)
        Dictionary<Tile, int> orderedDict = orderedList.ToDictionary(x => x.Key, x => x.Value);
        List<Tile> orderedLeafs = new List<Tile>(orderedDict.Keys.ToList()); //list of dead-ends in order

        //4. Go through ordered list of dead-ends
        for (int i = 0; i < orderedLeafs.Count; i++)
        {
            Tile leaf = orderedLeafs[i];
            Tile parent = orderedLeafs[i].parent;
            Tile other;

            String leafNumS = leaf.name.Substring(leaf.name.IndexOf("-") + 1, leaf.name.Length - leaf.name.IndexOf("-") - 1);
            String parentNumS = parent.name.Substring(parent.name.IndexOf("-") + 1, parent.name.Length - parent.name.IndexOf("-") - 1);
            int leafNum = int.Parse(leafNumS);
            int parentNum = int.Parse(parentNumS);

            /* if leaf is one of four corners, ignore and move ont next
             * if leaf is a central tile OR both leaf and its parent or on the border, remove the wall opposite of the leaf-parent edge
             * if leaf is a border tile (but parent isn't), remove adjacent wall* with longest path to the solution path on its other side
             *     *if the list of adjacent walls count = 1, ignore this deadend and move onto next
             */

            //Leaf is one of four corners, it can only have one wall that we don't want to remove, so we won't do anything in this loop
            if (leafNum != 1 && leafNum != GenerateGrid.wdth && leafNum != (GenerateGrid.wdth * GenerateGrid.hght - GenerateGrid.wdth + 1) && leafNum != (GenerateGrid.wdth * GenerateGrid.hght))
            {
                if (leaf.border == false || (leaf.border == true && parent.border == true))
                {
                    foreach (Wall edge in tree) //edges without walls
                    {
                        if (edge.origin == leaf || edge.destination == leaf)
                        {
                            if (edge.origin == leaf)
                            {
                                if (edge.destination == parent)
                                {
                                    //find the opposite edge to the parent in same row
                                    if (leafNum + 1 == parentNum) //parent beside leaf
                                    {
                                        other = GameObject.Find("Tile-" + (leafNum - 1).ToString()).GetComponent<Tile>();
                                    }
                                    else if (leafNum - 1 == parentNum)
                                    {
                                        other = GameObject.Find("Tile-" + (leafNum + 1).ToString()).GetComponent<Tile>();
                                    }
                                    else if (parentNum > leafNum) //parent above leaf
                                    {
                                        //get tile below leaf (tile number - width)
                                        other = GameObject.Find("Tile-" + (leafNum - GenerateGrid.wdth).ToString()).GetComponent<Tile>();
                                    }
                                    else //leaf above parent
                                    {
                                        //get tile above leaf (tile number + width)
                                        other = GameObject.Find("Tile-" + (leafNum + GenerateGrid.wdth).ToString()).GetComponent<Tile>();
                                    }

                                    foreach (Wall e in EdgesWithWalls)
                                    {
                                        if ((e.origin == leaf && e.destination == other) || (e.origin == other && e.destination == leaf))
                                        {
                                            //Make tiles on either side of removed wall each other's children (so cycle is traversable)
                                            e.origin.children.Add(e.destination);
                                            e.destination.children.Add(e.origin);

                                            e.disableEdge(); //remove wall
                                            e.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                                            Debug.Log("removed edge " + e.name);
                                            e.tag = "cycle";
                                            EdgesWithWalls.Remove(e);
                                            newTree.Add(e); //add edge to the tree (no longer a mst)
                                            addedCycles++;
                                            newDends.Remove(leaf);
                                            break;
                                        }
                                    }

                                    if (orderedLeafs.Contains(other))
                                    { //if parent is a dead-end remove from list and don't check it
                                        orderedLeafs.Remove(other);
                                        newDends.Remove(other);
                                        i++;
                                    }
                                }
                            }
                            else //leaf is destination
                            {
                                if (edge.origin == parent)
                                {
                                    //find the opposite edge to the parent in same row
                                    if (leafNum + 1 == parentNum) //parent beside leaf
                                    {
                                        other = GameObject.Find("Tile-" + (leafNum - 1).ToString()).GetComponent<Tile>();
                                    }
                                    else if (leafNum - 1 == parentNum)
                                    {
                                        other = GameObject.Find("Tile-" + (leafNum + 1).ToString()).GetComponent<Tile>();
                                    }
                                    else if (parentNum > leafNum) //parent above leaf
                                    {
                                        //get tile below leaf (tile number - width)
                                        other = GameObject.Find("Tile-" + (leafNum - GenerateGrid.wdth).ToString()).GetComponent<Tile>();
                                    }
                                    else //leaf above parent
                                    {
                                        //get tile above leaf (tile number + width)
                                        other = GameObject.Find("Tile-" + (leafNum + GenerateGrid.wdth).ToString()).GetComponent<Tile>();
                                        //this tile doesnt exist
                                    }

                                    foreach (Wall e in EdgesWithWalls)
                                    {
                                        if ((e.origin == leaf && e.destination == other) || (e.origin == other && e.destination == leaf))
                                        {
                                            e.origin.children.Add(e.destination);
                                            e.destination.children.Add(e.origin);

                                            e.disableEdge(); //remove wall
                                            e.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                                            Debug.Log("removed edge " + e.name);
                                            e.tag = "cycle";
                                            EdgesWithWalls.Remove(e);
                                            newTree.Add(e); //add edge to the tree (no longer a mst)
                                            addedCycles++;
                                            newDends.Remove(leaf);
                                            break;
                                        }
                                    }

                                    if (orderedLeafs.Contains(other))
                                    { //if parent is a dead-end remove from list and don't check it
                                        orderedLeafs.Remove(other);
                                        newDends.Remove(other);
                                        i++;
                                    }
                                }
                            }
                           break; //maybe here??
                        }
                    }
                }
                else //deadend on border, therefore, remove wall with longest path on the other side of it
                {
                    Dictionary<Wall, int> adjacentWalls = new Dictionary<Wall, int>();

                    foreach (Wall edge in EdgesWithWalls)
                    {
                        if (edge.origin == leaf)
                        {
                            String otherNumS = edge.destination.name.Substring(edge.destination.name.IndexOf("-") + 1, edge.destination.name.Length - edge.destination.name.IndexOf("-") - 1);
                            int otherNum = int.Parse(otherNumS);

                            //if other tile isn't a corner
                            if (otherNum != 1 && otherNum != GenerateGrid.wdth && otherNum != (GenerateGrid.wdth * GenerateGrid.hght - GenerateGrid.wdth + 1) && otherNum != (GenerateGrid.wdth * GenerateGrid.hght))
                            {
                                adjacentWalls.Add(edge, GetPathLength(edge.destination, longestP.path));
                            }     
                        }
                        if (edge.destination == leaf)
                        {
                            String otherNumS = edge.origin.name.Substring(edge.origin.name.IndexOf("-") + 1, edge.origin.name.Length - edge.origin.name.IndexOf("-") - 1);
                            int otherNum = int.Parse(otherNumS);

                            //if other tile isn't a corner
                            if (otherNum != 1 && otherNum != GenerateGrid.wdth && otherNum != (GenerateGrid.wdth * GenerateGrid.hght - GenerateGrid.wdth + 1) && otherNum != (GenerateGrid.wdth * GenerateGrid.hght))
                            {
                                adjacentWalls.Add(edge, GetPathLength(edge.origin, longestP.path));
                            }
                        }
                    }

                    if (adjacentWalls.Count > 1) //if there's only one adjacent wall, don't remove it
                    {
                        List<KeyValuePair<Wall, int>> adjList = adjacentWalls.ToList();
                        adjList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value)); //sort by path length
                        adjList.Reverse();

                        Dictionary<Wall, int> orderedAdj = adjList.ToDictionary(x => x.Key, x => x.Value);
                        List<Wall> orderedAdjacent = new List<Wall>(orderedAdj.Keys.ToList()); //list of adjecent walls in order of path length

                        orderedAdjacent[0].origin.children.Add(orderedAdjacent[0].destination);
                        orderedAdjacent[0].destination.children.Add(orderedAdjacent[0].origin);

                        orderedAdjacent[0].disableEdge(); //remove wall
                        orderedAdjacent[0].gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        Debug.Log("removed edge " + orderedAdjacent[0].name);
                        orderedAdjacent[0].tag = "cycle";
                        EdgesWithWalls.Remove(orderedAdjacent[0]);
                        newTree.Add(orderedAdjacent[0]); //add edge to the tree (no longer a mst)
                        addedCycles++;

                        if(newDends.Contains(orderedAdjacent[0].origin))
                        {
                            newDends.Remove(orderedAdjacent[0].origin);
                        }
                        if (newDends.Contains(orderedAdjacent[0].destination))
                        {
                            newDends.Remove(orderedAdjacent[0].destination);
                        }
                    }
                }

                if (addedCycles == cycles)
                {
                    maze.deadends = newDends;
                    graph.edges = tree.ToArray();
                    graph.numEdges = tree.Count;
                    maze.tree = graph;
                    return maze; //exit function
                }
            }
        }

        //If there wasn't enough dead-ends, remove random wall
        for (int c = addedCycles; c < cycles; c++)
        {
            Wall randEdge = EdgesWithWalls[UnityEngine.Random.Range(0, EdgesWithWalls.Count)];
            Debug.Log("remove random wall");
            randEdge.origin.children.Add(randEdge.destination);
            randEdge.destination.children.Add(randEdge.origin);
            randEdge.disableEdge(); //remove a random wall
            randEdge.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Debug.Log("removed edge " + randEdge.name);
            randEdge.tag = "cycle";
            tree.Add(randEdge); //add the random edge to the tree (no longer a mst)
            EdgesWithWalls.Remove(randEdge);

            if (newDends.Contains(randEdge.origin)){
                newDends.Remove(randEdge.origin);
            }
            if (newDends.Contains(randEdge.destination)){
                newDends.Remove(randEdge.destination);
            }
        }
        maze.deadends = newDends;
        graph.edges = tree.ToArray();
        graph.numEdges = tree.Count;
        maze.tree = graph;
        return maze;
    }

    //Getting the length from a tile to the solution path
    public int GetPathLength(Tile startPoint, Tile[] longestP)
    {
        List<Tile> lp = new List<Tile>(longestP);

        int length = 0;

        while(lp.Contains(startPoint) == false)
        {
            startPoint = startPoint.parent;
            length++;
        }

        return length;
    }


    private static void Print(Wall[] result, int e)
    {
        for (int i = 0; i < e; ++i)
        {
            Debug.Log(Tile.GetRootParent(result[i].destination));
            //Debug.Log(result[i].origin + "--" + result[i].destination + "O rank ==" + result[i].origin.rank + "D rank ==" + result[i].destination.rank);
        }
    }

}