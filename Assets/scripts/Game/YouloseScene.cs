using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YouloseScene : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Button youlose;
    void Start()
    {
        youlose.onClick.AddListener(() => SceneManager.LoadScene("Game", LoadSceneMode.Single));
    }
}
