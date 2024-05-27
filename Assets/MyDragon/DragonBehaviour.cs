using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DragonBehaviour : NetworkBehaviour
{
    public float hp = 100;

    public static event Action OnApproachingPlayer;
    public static event Action OnDragonDead;

    private const float MOVE_SPEED = 0.02f;
    private const float MOVE_THRESHOLD = 0.01f;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        //Debug.Log("Dragon spawned");
    }
    void Start()
    {
        
    }

    // Dragon transforms to the position of the player
    private void MoveTowardsPlayer(Vector3 playerPosition)
    {
        // if the player is close to the dragon, the dragon will not move
        // Get the Dragon's position which is the prefab named "Blue"
        Vector3 dragonPosition = GetComponent<NetworkObject>().transform.position;

        // Get the distance between the player and the dragon
        float distance = Vector3.Distance(playerPosition, dragonPosition);
        if (distance < MOVE_THRESHOLD)
        {
            return;
        }

        if (distance <= MOVE_SPEED)
        {
            // Dragon move towards but not too close to the player (MOVE_THRESHOLD away from the player)
            transform.LookAt(playerPosition);
            float distanceToMove = distance - MOVE_THRESHOLD;
            transform.position = Vector3.MoveTowards(dragonPosition, playerPosition , distanceToMove);
        }
        else
        {
            transform.LookAt(playerPosition);
            if (distance > 50)
            {
                transform.position = Vector3.MoveTowards(dragonPosition, playerPosition, 1000f);
            }
            else
            {
                transform.position = Vector3.MoveTowards(dragonPosition, playerPosition, MOVE_SPEED);
            }
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            hp -= 10;
        }
    }

    void OnFlyForwardNearestPlayer()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");

        if (gameObjects.Length != 0)
        {   Transform nearestPlayer = gameObjects[0].transform;
            for (int i = 1; i < gameObjects.Length; i++)
            {
                Vector3 playerPosition = gameObjects[i].GetComponent<NetworkObject>().transform.position;

                if (Vector3.Distance(transform.position, playerPosition) < Vector3.Distance(transform.position, nearestPlayer.position))
                {
                    nearestPlayer = gameObjects[i].transform;
                }
            }

            MoveTowardsPlayer(nearestPlayer.position);
        }
    }

    public void AttractDragon(Vector3 playerPosition)
    {
       transform.position = Vector3.MoveTowards(transform.position, playerPosition, 100f);
    }   

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
        {
            StartGameAR.DragonDead();
        }

        OnFlyForwardNearestPlayer();
        
    }
}
