using System;
using Unity.Netcode;
using UnityEngine;

public class HeadHick : NetworkBehaviour
{
    // Start is called before the first frame update
    public static event Action<(ulong from, ulong to)> OnHitPlayer;
    void OnTriggerEnter(Collider collision) 
    {
        Debug.Log("fuckingshit0");
        if (IsServer)
        {
            if (collision.transform.TryGetComponent(out NetworkObject networkObject))
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Debug.Log("Dragon head has Collision to player");
                    (ulong, ulong) hehe = new(4, networkObject.OwnerClientId);
                    OnHitPlayer?.Invoke(hehe);
                    return;
                }
                Debug.Log("fuckingshit1");
            }
            else
            {
                Debug.Log("fuckingshit2");
                return;
            }
        }
    }

}
