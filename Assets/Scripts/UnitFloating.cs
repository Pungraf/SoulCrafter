using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFloating : MonoBehaviour
{
    private UnitController unitController;
    private float currentYPosition;

    [SerializeField] private float floatingSpeed = 1f;
    [SerializeField] private float floatingAmp = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        unitController = transform.parent.GetComponent<UnitController>();
        currentYPosition = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3();
        if (unitController.CurrentBehaviourState == UnitController.BehaviourState.Copulating)
        {
            newPosition = new Vector3(this.transform.position.x, currentYPosition + (floatingAmp * 2) * Mathf.Sin((floatingSpeed * 4 ) * Time.time), this.transform.position.z);
        }
        else
        {
            newPosition = new Vector3(this.transform.position.x, currentYPosition + floatingAmp * Mathf.Sin(floatingSpeed * Time.time), this.transform.position.z);

        }
        this.transform.position = newPosition;
    }
}
