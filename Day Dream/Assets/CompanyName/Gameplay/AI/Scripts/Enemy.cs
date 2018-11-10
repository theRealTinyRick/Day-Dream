using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

using AH.Max;

public class Enemy : Entity
{
    public BehaviorTree entityBehaviourTree;

    protected override void Enable()
    {
        if (entityBehaviourTree == null)
        {
            entityBehaviourTree = GetComponent<BehaviorTree>();
        }
        if (entityBehaviourTree != null)
        {
            entityBehaviourTree.GetVariable("Agent").SetValue(this.gameObject);
        }
        else
        {
            Debug.LogError("No behaviour tree assigned to agent: " + this.gameObject);
        }
    }
}
