using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This maze implementation is based on this Got Hub Project https://github.com/willy1989/kruskal
//The original doesn't take weight into account, have tiles with parents and ranks, and the origin and destination vertices for edges weren't set up properly
//I mainly borrowed their grid generation method, but had to update all teh kruskal-related stuff so a minimum spanning tree was outputed

public class GenerateGrid : MonoBehaviour
{

    public GameObject wallPrefab;
    public GameObject tilePrefab;
    public int width;
    public int height;
    public int cycles;

    [HideInInspector] public List<Wall> edges;
    [HideInInspector] public int EdgeIndex = 0;
    [HideInInspector] public GameObject[] tiles;
    private Object[] colours;


    public void Start()
    {
        //Create grid of tiles (vertices)
        tiles = new GameObject[width * height];

        //TEMPORARY - DELETE LATER
        colours = new Object[7];
        colours = Resources.LoadAll("Materials", typeof(Material));


        //This creates all of the coloured tiles
        int counter = 0;
        for (int y = 1; y <= height; y++)
        {
            for (int x = 1; x <= width; x++)
            {

                GameObject tile = Instantiate(tilePrefab, new Vector3((x * 20) - 20f, (y * 20) - 10f, 0), Quaternion.Euler(0, 0, 0));
                tile.name = "Tile-" + (counter + 1);
                tile.GetComponent<Tile>().rank = 0;
                tile.GetComponentInChildren<Text>().text = "0";
                tile.GetComponent<Tile>().parent = tile.GetComponent<Tile>();
                //tile.GetComponent<Tile>().child = tile.GetComponent<Tile>();

                tile.GetComponentInChildren<Text>().text = tile.GetComponent<Tile>().rank.ToString();

                //TEMPORARY - DELETE LATER
                Material mat = (Material)colours[Random.Range(0, colours.Length)];
                tile.GetComponent<SpriteRenderer>().material.shader = mat.shader;
                tile.GetComponent<SpriteRenderer>().material.color = mat.color;

                tiles[counter] = tile;

                counter++;
            }
        }

        edges = new List<Wall>();

        spawnLeftRightBoundaries();
        spawnUpDownBoundaries();
        spawnInnerEdgesLeftRight();
        spawnInnerEdgesUpDown();


        Tile[] vertices = new Tile[tiles.Length];
        for(int i = 0; i < tiles.Length; i++)
        {
            vertices[i] = tiles[i].GetComponent<Tile>();
        }

        KruskalMaze.Maze mazeTree = GetComponent<KruskalMaze>().CreateMaze(KruskalMaze.CreateGraph(edges.ToArray(), vertices.Length), vertices, cycles);
    }

    //OUTER BORDER (not part of tree)
    public void spawnLeftRightBoundaries()
    {
        for (int y = 1; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                if (x == 0 || x == width)
                {
                    //Instantiate(prefab, new Vector3((x * 5) + 2.5f, 0, (z * 5) + 2.5f), Quaternion.Euler(0, 90, 0));
                    GameObject w = Instantiate(wallPrefab, new Vector3((x * 20) -10f,(y * 20) -10f, 0), Quaternion.Euler(0, 0, 0));
                    Destroy(w.GetComponent<Wall>());
                }

            }
        }

    }

    public void spawnUpDownBoundaries()
    {
        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y == 0 || y == height)
                {
                    GameObject w = Instantiate(wallPrefab, new Vector3((x * 20), (y * 20), 0), Quaternion.Euler(0, 0, 90));
                    Destroy(w.GetComponent<Wall>());
                }

            }
        }

    }

    // GENERATING TREE EDGES (grid lines)
    public void spawnInnerEdgesLeftRight()
    {
        EdgeIndex = 0;

        for (int y = 1; y <= height; y++)
        {
            for (int x = 1; x < width; x++)
            {
                GameObject wall = Instantiate(wallPrefab, new Vector3((x * 20) -10f, (y * 20) -10f, 0), Quaternion.Euler(0, 0, 0));

                wall.GetComponent<Wall>().connectedTiles = new Tile[2];
                wall.GetComponent<Wall>().connectedTiles[0] = tiles[EdgeIndex].GetComponent<Tile>();
                wall.GetComponent<Wall>().connectedTiles[1] = tiles[EdgeIndex + 1].GetComponent<Tile>();

                wall.GetComponent<Wall>().origin = tiles[EdgeIndex].GetComponent<Tile>();
                wall.GetComponent<Wall>().destination = tiles[EdgeIndex + 1].GetComponent<Tile>();
                wall.GetComponent<Wall>().weight = Random.Range(0, 100);

                edges.Add(wall.GetComponent<Wall>());

               EdgeIndex++;

            }
            EdgeIndex++;
        }
    }

    public void spawnInnerEdgesUpDown()
    {
        EdgeIndex = 0;

        for (int y = 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject wall = Instantiate(wallPrefab, new Vector3((x * 20),(y * 20), 0), Quaternion.Euler(0, 0, 90));

                wall.GetComponent<Wall>().connectedTiles = new Tile[2];
                wall.GetComponent<Wall>().connectedTiles[0] = tiles[EdgeIndex].GetComponent<Tile>();
                wall.GetComponent<Wall>().connectedTiles[1] = tiles[EdgeIndex + width].GetComponent<Tile>(); //no?

                wall.GetComponent<Wall>().origin = tiles[EdgeIndex].GetComponent<Tile>();
                wall.GetComponent<Wall>().destination = tiles[EdgeIndex + width].GetComponent<Tile>(); //changed
                wall.GetComponent<Wall>().weight = Random.Range(0, 100);

                edges.Add(wall.GetComponent<Wall>());

                EdgeIndex++;

            }

        }
    }
}