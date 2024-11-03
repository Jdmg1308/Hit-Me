using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface HasRangedStates
{
    public bool InRunAwayRange { get; set; }
    public bool InRangedAttackRange { get; set; }

    public void SetInRunAwayRange(bool inRunAwayRange)
    {
        InRunAwayRange = inRunAwayRange;
    }

    public void SetInRangedAttackRange(bool inRangedAttackRange)
    {
        InRangedAttackRange = inRangedAttackRange;   
    }
}
