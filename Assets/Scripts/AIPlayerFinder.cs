using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPlayerFinder : MonoBehaviour
{
    public float viewDistance = 100f;
    public float hFOV = 60; //half of FOV, for 120 FOV, halfFOV=60
    public bool alert = false; //true if seeing player
    //public bool playerInFOV = false;
    public float alertDistance = 30f;
    public GameObject mapManagerObject;
    private MapManagerComponent mapManager;
    MapManagerComponent.Tile playerTile;
    private Vector2 waypoint;
    public int waypointsTries=5;
    public int generateWaypointTries = 100;
    private AIPatrolComponent aiPatrolComponent;
    LayerMask maskObstacles;
    //Vector3 originRayCast;
    //Vector3 destinationRayCast;

    public bool GenerateWaypoint(MapManagerComponent.Tile tile, int numberOfTries, out Vector2 genWaypoint)
    {
        int i = 0;

        do{
            genWaypoint = new Vector2(Random.Range(tile.x1, tile.x2), Random.Range(tile.z1, tile.z2));
            i++;
            if (i > numberOfTries) return false;
        } while(!CheckIfWalkable(ref genWaypoint, maskObstacles));

        return true;
    }

    public static Vector2 GenerateWaypoint(MapManagerComponent.Tile tile, LayerMask maskObstacles, int numberOfTries)
    {
        int i = 0;
        Vector2 waypointGen;
        do
        {
            waypointGen = new Vector2(Random.Range(tile.x1, tile.x2), Random.Range(tile.z1, tile.z2));
            i++;
            if (i > numberOfTries)
                break;
        } while (!CheckIfWalkable(ref waypointGen, maskObstacles));
        return waypointGen;
    }

    public static bool CheckIfWalkable(ref Vector2 vec, LayerMask maskObstacles)
    {
        Vector3 originRayCast = new Vector3(vec.x, 5.0f, vec.y);
        Vector3 destinationRayCast = new Vector3(vec.x, -5.0f, vec.y);
        if (Physics.Raycast(originRayCast, destinationRayCast, 20.0f, maskObstacles))
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
        //TODO goto tile before searching, not necesarry?
        //TODO getobject tile, check if player in the same tile, to increase difficulty, make AI smarter
        SearchTileForPlayer(waypointsTries);
    }

    private bool IsPlayerInFOV(float halfFOV)
    {
        Vector3 targetDirection = mapManager.GetPlayerPosition() - transform.position;
        float angleToPlayer = (Vector3.Angle(targetDirection, transform.forward));
        RaycastHit hit;

        if (angleToPlayer >= -halfFOV && angleToPlayer <= halfFOV)
        {
            //Debug.Log("Player in angle");
            //Debug.Log(this.transform.position + " , " + this.transform.forward);
            //Debug.DrawLine(this.transform.position + 2 * this.transform.forward, mapManager.GetPlayerPosition() + this.transform.forward, Color.red, 0.5f);
            if (Physics.Raycast(this.transform.position+2*this.transform.forward, mapManager.GetPlayerPosition()+ this.transform.forward, out hit, viewDistance))
            {
                //Debug.Log("Player in sight!");
                //playerInFOV = true;
                if(hit.transform.name == "Player")
                    return true;
            }  
        }
        //playerInFOV = false;
        return false;
    }

    private void FollowPlayer()
    {
        aiPatrolComponent.GoToPoint(mapManager.GetPlayerPosition());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            mapManager.KillPlayer();
    }



    // Start is called before the first frame update
    void Start()
    {
        aiPatrolComponent = GetComponent<AIPatrolComponent>();
        mapManager = mapManagerObject.GetComponent<MapManagerComponent>();
        maskObstacles = LayerMask.GetMask("Obstacles");
    }

    // Update is called once per frame
    void Update()
    {       
        //Debug.Log(Vector3.Distance(this.transform.position, mapManager.GetPlayerPosition()));
        if (IsPlayerInFOV(hFOV)) //if enemy sees the player
        {
            if (!alert) //if enemy not alarmed
            {
                gameObject.GetComponent<Renderer>().material.color = Color.magenta; //change to alarmed color
                alert = true; //set status to alarmed
            }
            FollowPlayer();
        }
        else //if enemy doesnt see the player
        {
            if (alert) //if alarmed
            {
                if (Vector3.Distance(this.transform.position, mapManager.GetPlayerPosition()) > alertDistance) //if not close enough to player
                {
                    alert = false; //disable alarmed
                    gameObject.GetComponent<Renderer>().material.color = Color.red; //change color back
                    aiPatrolComponent.ClearNavList(); //stop following the player
                }
                else FollowPlayer(); //if still close enough to player, follow
            }
            else //if not alarmed
            {
                //if all points searched, check where the player is and generate new waypoints
                if (aiPatrolComponent.GetNavPointsCount() == 0)
                {
                    if (mapManager.GetPlayerTile(out playerTile))
                    {
                        GoToPlayerTile(playerTile);
                    }
                }
            }
            
        }
        //Debug.Log(aiPatrolComponent.GetNavPointsCount());
        
    }
}
