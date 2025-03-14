using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/Transitions/In Sight Range")]
public class InSightRangeTransition : AiTransition
{
    public override bool Decision(AiTransitionBlackboard transitionBlackboard)
    {
        return transitionBlackboard.InSightRange;
    }
}
