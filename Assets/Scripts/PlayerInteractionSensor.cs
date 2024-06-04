using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionSensor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<IPickable>() != null)
        {
            other.gameObject.GetComponent<IPickable>().PickUp();
        }
    }
}
