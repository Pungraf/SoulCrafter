using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulTree : Food
{
    protected override void InvokeUpdate()
    {
        currentNutritiousness += counterUpdateSampling * Nutritiousness / sekPerDay;
        base.InvokeUpdate();
    }
}
