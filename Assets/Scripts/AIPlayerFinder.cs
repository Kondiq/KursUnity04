using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPlayerFinder : MonoBehaviour
{
    public float viewDistance = 10f;
    public GameObject mapManagerObject;
    private MapManagerComponent mapManager;
    MapManagerComponent.Tile playerTile;
    private Vector2 waypoint;
    public int waypointsTries=5;
    public int generateWaypointTries = 100;
    private AIPatrolComponent aiPatrolComponent;
    LayerMask mask;
    Vector3 originRayCast;
    Vector3 destinationRayCast;

    public bool GenerateWaypoint(MapManagerComponent.Tile tile, int numberOfTries, out Vector2 genWaypoint)
    {
        int i = 0;

        do{
            genWaypoint = new Vector2(Random.Range(tile.x1, tile.x2), Random.Range(tile.z1, tile.z2));
            i++;
            if (i > numberOfTries) return false;
        } while(!CheckIfWalkable(ref genWaypoint));

        return true;
    }

    public bool CheckIfWalkable(ref Vector2 vec)
    {
        originRayCast = new Vector3(vec.x, 5.0f, vec.y);
        destinationRayCast = new Vector3(vec.x, -5.0f, vec.y);
        if (Physics.Raycast(originRayCast, destinationRayCast, 20.0f, mask))
        {
            return false;
        }
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
        mask = LayerMask.GetMask("Obstacles");
    }

    // Update is called once per frame
    void Update()
    {
        //TODO testing will change later
        if (aiPatrolComponent.GetNavPointsCount()==0)
        {
            if (mapManager.GetPlayerTile(out playerTile))
            {
                GoToPlayerTile(playerTile);
            }
        }
        
    }
}
