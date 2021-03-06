using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManagerComponent : MonoBehaviour
{
    public GameObject player;
    public GameObject floor;
    public GameObject coinPrefab;
    public int coinsAmount;
    private float mapWidth;
    private float mapHeight;
    private float tileWidth;
    private float tileHeight;
    public int tilesNumberWide=3;
    public int tilesNumberHigh=3;
    private Tile[,] tiles;

    //map boundaries
    private Tile floorTile;

    private LayerMask maskObstacles;

    public struct Tile
    {
        public float x1, x2, z1, z2;

        public Tile(float x1, float x2, float z1, float z2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.z1 = z1;
            this.z2 = z2;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        maskObstacles = LayerMask.GetMask("Obstacles");

        mapWidth = 10 * floor.transform.localScale.x;
        mapHeight = 10 * floor.transform.localScale.z;

        tileWidth = mapWidth / tilesNumberWide;
        tileHeight = mapHeight / tilesNumberHigh;
        
        //TODO check if right (1 pixel difference?)
        //map boundaries
        floorTile = new Tile(
            floor.transform.position.x - (mapWidth / 2),
            floor.transform.position.x + (mapWidth / 2),
            floor.transform.position.z - (mapWidth / 2),
            floor.transform.position.z + (mapWidth / 2)
            );

        //tiles array
        tiles = new Tile[tilesNumberWide, tilesNumberHigh];

        for (int x = 0; x < tilesNumberWide; x++)
            for (int y = 0; y < tilesNumberHigh; y++)
                tiles[x, y] = new Tile(
                    floorTile.x1+x*tileWidth, floorTile.x1 + x*tileWidth + tileWidth,
                    floorTile.z1 + y * tileHeight, floorTile.z1 + y * tileHeight + tileHeight);

        SpawnCoins(coinsAmount);
    }

    public bool GetPlayerTile(out Tile tile)
    {
        return GetObjectTile(out tile, player);
    }

    public bool GetObjectTile(out Tile tile, GameObject ob)
    {
        for (int x = 0; x < tilesNumberWide; x++)
            for (int y = 0; y < tilesNumberHigh; y++)
            {
                //Debug.Log(tiles[x, y].x1 + "-" + tiles[x, y].x2 + " x " + tiles[x, y].z1 + "-" + tiles[x, y].z2 + " object: " + ob.transform.position.x + "x" + ob.transform.position.z);
                if (
                    ob.transform.position.x > tiles[x, y].x1
                    && ob.transform.position.x < tiles[x, y].x2
                    && ob.transform.position.z > tiles[x, y].z1
                    && ob.transform.position.z < tiles[x, y].z2
                    )
                {
                    tile = tiles[x, y];
                    return true;
                }
            }


        tile = new Tile();
        return false;
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public void KillPlayer()
    {
        player.SetActive(false);
    }

    public void SpawnCoins(int amount)
    {
        Vector2 coinPosition;
        for(int i = 0; i < amount; i++)
        {
            coinPosition = AIPlayerFinder.GenerateWaypoint(floorTile,maskObstacles,60000);
            if(coinPosition!= default(Vector2))
                Instantiate(coinPrefab, new Vector3(coinPosition.x, 0.5f, coinPosition.y), coinPrefab.transform.rotation);
        }
        
    }
}
