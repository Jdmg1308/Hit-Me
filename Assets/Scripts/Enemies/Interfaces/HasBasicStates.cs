using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface HasBasicStates
{
    public bool InChaseRange { get; set; }
    public bool InAttackRange { get; set; }

    public void SetInChaseRange(bool inChaseRange)
    {
        InChaseRange = inChaseRange;
    }

    public void SetInAttackRange(bool inAttackRange)
    {
        InAttackRange = inAttackRange;
    }
}
