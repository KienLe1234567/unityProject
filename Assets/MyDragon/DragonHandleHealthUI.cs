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

    }
    private void Update()
    {
        HealthText.text = DragonBehaviour.hp.ToString();
        if (_mainCamera)
        {
            HealthText.transform.LookAt(_mainCamera.transform);
        }
    }

}
