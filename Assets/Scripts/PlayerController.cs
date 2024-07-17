using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerPackManager packManager;
    public PlayerPackManager PackManager { get { return packManager; } }
    // Start is called before the first frame update
    void Start()
    {
        WorldManager.Instance.Player = this.gameObject;
        packManager = GetComponent<PlayerPackManager>();
    }
}
