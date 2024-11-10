using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    public Unit HeldUnit;

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();
        if(unit != null && HeldUnit == null)
        {
            CaptureUnit(unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();
        if(HeldUnit == unit)
        {
            HeldUnit = null;
        }
    }

    private void CaptureUnit(Unit capturedUnit)
    {
        GameObject unitGO = capturedUnit.gameObject;

        capturedUnit.controller.Brain.ClearAllPeristentBehaviours();
        Vector3 destination = transform.position;
        capturedUnit.controller.ForceBehaviour(BaseBehaviour.Behaviour.Incapacitated);

        HeldUnit = capturedUnit;

        unitGO.transform.position = transform.position;
    }
}
