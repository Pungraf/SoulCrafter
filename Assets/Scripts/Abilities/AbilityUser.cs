using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUser : MonoBehaviour
{
    public AbilitySet abilitySet; // Assign an AbilitySet in the Inspector

    private float[] abilityCooldowns;

    private void Start()
    {
        if (abilitySet != null)
        {
            abilityCooldowns = new float[abilitySet.abilities.Length];
        }
    }

    private void Update()
    {
        // Reduce cooldowns over time
        for (int i = 0; i < abilityCooldowns.Length; i++)
        {
            if (abilityCooldowns[i] > 0)
            {
                abilityCooldowns[i] -= Time.deltaTime;
            }
        }

        // Example Input Handling
        if (Input.GetKeyDown(KeyCode.Alpha1)) { UseAbility(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { UseAbility(1); }
    }

    public void UseAbility(int index)
    {
        if (index < 0 || index >= abilitySet.abilities.Length) return;

        Ability ability = abilitySet.abilities[index];

        if (abilityCooldowns[index] <= 0)
        {
            ability.Activate(GetComponent<PlayerController>());
            abilityCooldowns[index] = ability.Cooldown;
        }
        else
        {
            Debug.Log($"{ability.AbilityName} is on cooldown!");
        }
    }
}
