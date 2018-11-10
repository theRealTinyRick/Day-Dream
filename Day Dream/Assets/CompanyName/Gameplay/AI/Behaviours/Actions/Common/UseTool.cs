using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Custom/Tool")]

    public class UseTool : Action
    {

        public SharedGameObject agent;

        public int tool;

        public override void OnStart()
        {
            

        }

        
    }
}