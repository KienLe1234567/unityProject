using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

public class DragonDataManager : NetworkBehaviour
{
    public static DragonDataManager Instance;
    public NetworkVariable<int> hp;
    public bool isDie = false;
    public event Action OnDragonDead;
    public event Action OnDragonHealthChanged;
    public event Action OnFly;
    public event Action OnAttack;
    public event Action OnWalk;
    public event Action OnDie;

    private void Awake()
    {
        hp = new NetworkVariable<int>(100);
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }

        Instance = this;
    }
    private void BulletDataOnHitDragon()
    {
        if (IsServer)
        {
            if (hp.Value <= 0)
            {
                OnDragonDead?.Invoke();
            }
            else
            {
                hp.Value = hp.Value - 10;
                Debug.Log("Dragon was hit by bullet");
            }
        }
        SyncReduceDragonHealthClientRpc();
    }
    IEnumerator set1sec()
    {
        yield return new WaitForSeconds(1);
        isDie = true;
    }

    [ClientRpc]
    void SyncReduceDragonHealthClientRpc()
    {
        OnDragonHealthChanged?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        OnDragonDead += () =>
        {
            StartCoroutine(set1sec());
        };

        BulletData.OnHitDragon += BulletDataOnHitDragon;
        
    }

    private void OnDisable()
    {
        BulletData.OnHitDragon -= BulletDataOnHitDragon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
