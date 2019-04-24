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

        public bool priorityUse;

        private UseableComponent toolsUseable;

        private ToolsComponent agentsToolComponent;

        public override void OnStart()
        {
            if (agentsToolComponent == null)
            {
                agentsToolComponent = agent.Value.GetComponentInChildren<ToolsComponent>();
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (agent == null || agentsToolComponent == null)
            {
                return TaskStatus.Failure;
            }

            toolsUseable = agentsToolComponent.GetToolsUseable(tool);

            if (toolsUseable != null && !toolsUseable.inUse)
            {
                agentsToolComponent.UseTool(tool, priorityUse);
            }
            Debug.Log("using tool!");

            toolsUseable = null;
            return TaskStatus.Success;
        }
        
        public override void OnConditionalAbort()
        {
            agentsToolComponent.CancelTool();
        }

    }
}