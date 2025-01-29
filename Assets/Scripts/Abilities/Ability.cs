using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability System/New Ability", order = 0)]
public class Ability : ScriptableObject
{
    public string AbilityName;
    public float Cooldown;
    public int ManaCost;

    public AbilityAction Action; // Reference to an action (e.g., attack, heal)

    public void Activate(PlayerController controller)
    {
        if (Action != null)
        {
            Action.Use(controller);
        }
    }
}
