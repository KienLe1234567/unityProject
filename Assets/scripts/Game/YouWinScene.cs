using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class YouWinScene : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Button youwin;
    void Start()
    {
        youwin.onClick.AddListener(() => SceneManager.LoadScene("LoadScene", LoadSceneMode.Single)); 
    }
}
