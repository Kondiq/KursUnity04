using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPlayerFinder : MonoBehaviour
{
    public GameObject mapManagerObject;
    private MapManagerComponent mapManager;
    MapManagerComponent.Tile playerTile;
    private Vector2 waypoint;
    public int waypointsTries=5;
    public int generateWaypointTries = 100;
    private AIPatrolComponent aiPatrolComponent;

    bool first = true;

    public bool GenerateWaypoint(MapManagerComponent.Tile tile, int numberOfTries, out Vector2 genWaypoint)
    {
        int i = 0;

        do{
            genWaypoint = new Vector2(Random.Range(tile.x1, tile.x2), Random.Range(tile.z1, tile.z2));
            i++;
            if (i > numberOfTries) return false;
        } while(!CheckIfWalkable(genWaypoint));

        return true;
    }

    public bool CheckIfWalkable(Vector2 vec)
    {
        return true;
    }

    public void SearchTileForPlayer(int tries)
    {
        //number of waypoints to search
        for (int i = 0; i < tries; i++)
        {
            if (GenerateWaypoint(playerTile, generateWaypointTries, out waypoint))
            {
                Vector3 navPoint = new Vector3(waypoint.x, 1.0f, waypoint.y);
                aiPatrolComponent.AddNavPoint(ref navPoint);
            }
        }
    }

    public void GoToPlayerTile(MapManagerComponent.Tile tile)
    {
        //TODO goto tile before searching
        SearchTileForPlayer(waypointsTries);
    }

    // Start is called before the first frame update
    void Start()
    {
        aiPatrolComponent = GetComponent<AIPatrolComponent>();
        mapManager = mapManagerObject.GetComponent<MapManagerComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO testing will change later
        if (first)
        {
            first = false;
            if (mapManager.GetPlayerTile(out playerTile))
            {
                GoToPlayerTile(playerTile);
            }
        }
        
    }
}
