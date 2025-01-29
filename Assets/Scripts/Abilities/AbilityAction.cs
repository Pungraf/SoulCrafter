using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityAction : ScriptableObject
{
    public abstract void Use(PlayerController controller);
}
