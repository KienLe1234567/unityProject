using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using static UnityEngine.UI.GridLayoutGroup;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class DragonBehaviour : NetworkBehaviour
{
    public static float hp = 100;

    public static event Action OnApproachingPlayer;
    public static event Action OnDragonDead;
    public static event Action OnDragonHealthChanged;
    private const float MOVE_SPEED = 0.01f;
    private const float MOVE_THRESHOLD = 0.5f;
    private Animator _dragonAnimator;
    private AnimatorControllerParameter allParams;
    public bool isWin = false;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        //Debug.Log("Dragon spawned");
        _dragonAnimator = GetComponentInChildren<NetworkObject>().GetComponent<Animator>();
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
            OnDragonAttackClientRpc();
            return;
        }

        if (distance <= MOVE_SPEED)
        {
            // Dragon move towards but not too close to the player (MOVE_THRESHOLD away from the player)
            transform.LookAt(playerPosition);
            float distanceToMove = distance - MOVE_THRESHOLD;
            transform.position = Vector3.MoveTowards(dragonPosition, playerPosition, distanceToMove);
        }
        else
        {
            transform.LookAt(playerPosition);
            if (distance > 50)
            {
                OnDragonFlyClientRpc();
                transform.position = Vector3.MoveTowards(dragonPosition, new Vector3(playerPosition.x - 0.5f, playerPosition.y, playerPosition.z - 0.5f), distance);
            }
            else
            {
                OnDragonAttackClientRpc();
                transform.position = Vector3.MoveTowards(dragonPosition, playerPosition, MOVE_SPEED);

            }

        }

    }


    IEnumerator set1sec()
    {
        yield return new WaitForSeconds(1);
        //isDie = true;
        //Debug.Log("Dragon spawned");
    }

    [ClientRpc]
    public void OnDragonDeadClientRpc()
    {
        _dragonAnimator.SetBool("fly", false);
        _dragonAnimator.SetBool("walk", false);
        _dragonAnimator.SetBool("attack", false);
        _dragonAnimator.SetBool("die", true);
    }
    [ClientRpc]
    public void OnDragonFlyClientRpc()
    {
        _dragonAnimator.SetBool("fly", true);
        _dragonAnimator.SetBool("walk", false);
        _dragonAnimator.SetBool("attack", false);
        _dragonAnimator.SetBool("die", false);
    }

    [ClientRpc]
    public void OnDragonWalkClientRpc()
    {
        _dragonAnimator.SetBool("fly", false);
        _dragonAnimator.SetBool("walk", true);
        _dragonAnimator.SetBool("attack", false);
        _dragonAnimator.SetBool("die", false);
    }

    [ClientRpc]
    public void OnDragonAttackClientRpc()
    {
        _dragonAnimator.SetBool("fly", false);
        _dragonAnimator.SetBool("walk", false);
        _dragonAnimator.SetBool("attack", true);
        _dragonAnimator.SetBool("die", false);
    }

    void OnFlyForwardNearestPlayer()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");

        if (gameObjects.Length != 0)
        {
            Transform nearestPlayer = gameObjects[0].transform;
            for (int i = 1; i < gameObjects.Length; i++)
            {
                Vector3 playerPosition = gameObjects[i].GetComponent<NetworkObject>().transform.position;

                if (Vector3.Distance(transform.position, playerPosition) < Vector3.Distance(transform.position, nearestPlayer.position))
                {
                    nearestPlayer = gameObjects[i].transform;
                }
            }

            MoveTowardsPlayer(nearestPlayer.position);
        } else
        {
            _dragonAnimator.SetBool("fly", false);
            _dragonAnimator.SetBool("walk", false);
            _dragonAnimator.SetBool("attack", false);
            _dragonAnimator.SetBool("die", false);
        }
    }
    public async void LoadSceneYouWin()
    {
        isWin = true;
        await Task.Delay(4000);
        SceneManager.LoadScene("youWin", LoadSceneMode.Single);
    }

    public void AttractDragon(Vector3 playerPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, playerPosition, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        if (DragonDataManager.Instance.hp.Value <= 0)
        {

            if (!isWin)
            {
                OnDragonDeadClientRpc();
                LoadSceneYouWin();
            }
        }
        else
        {
            OnFlyForwardNearestPlayer();
        }

    }
}