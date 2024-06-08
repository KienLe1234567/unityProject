using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DragonHandleHealthUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text HealthText;

    private Camera _mainCamera;

    public override void OnNetworkSpawn()
    {
        _mainCamera = GameObject.FindObjectOfType<Camera>();
        DragonDataManager.Instance.OnDragonHealthChanged += InstanceOnDragonHealthChangedServerRpc;

    }
    public override void OnNetworkDespawn()
    {
        DragonDataManager.Instance.OnDragonHealthChanged -= InstanceOnDragonHealthChangedServerRpc;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InstanceOnDragonHealthChangedServerRpc()
    {
        SetHealthTextClientRpc();
    }

    [ClientRpc]
    void SetHealthTextClientRpc()
    {
        HealthText.text = DragonDataManager.Instance.hp.Value.ToString();
    }
    private void Update()
    {
        //HealthText.text = DragonBehaviour.hp.ToString();
        SetHealthTextClientRpc();

        if (_mainCamera)
        {
            HealthText.transform.LookAt(_mainCamera.transform);
        }
    }

}
