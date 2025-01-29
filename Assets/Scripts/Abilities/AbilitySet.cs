using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ability Set")]
public class AbilitySet : ScriptableObject
{
    public Ability[] abilities;

    public Ability GetAbilityByName(string abilityName)
    {
        foreach (var ability in abilities)
        {
            if (ability.AbilityName == abilityName)
            {
                return ability;
            }
        }
        Debug.LogWarning($"Ability '{abilityName}' not found in {name}");
        return null; // Return null if no ability matches
    }
}
