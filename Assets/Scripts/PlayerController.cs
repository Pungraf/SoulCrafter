using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Collider interactionRange;
    // Start is called before the first frame update
    void Start()
    {
        WorldManager.Instance.Player = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
