using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Actions/Swing")]
public class SwingAction : AbilityAction
{
    public override void Use(PlayerController controller)
    {
        Debug.Log("Swing Attack!");
        // Implement attack logic, like applying damage to nearby enemies
    }
}
