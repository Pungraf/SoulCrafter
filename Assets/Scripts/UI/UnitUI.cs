using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUI : MonoBehaviour
{
    [SerializeField] GameObject controlledUIGo;

    public void SetControlledUI(bool enabled)
    {
        controlledUIGo.SetActive(enabled);
    }
}
