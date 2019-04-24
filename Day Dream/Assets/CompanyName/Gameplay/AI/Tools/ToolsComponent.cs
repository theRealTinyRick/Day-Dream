using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorDesigner.Runtime;
using Sirenix.OdinInspector;
using AH.Max;

public class ToolsComponent : MonoBehaviour {

    [ShowInInspector]
    [SerializeField]
    protected List<Tools> characterTools = new List<Tools>();

    private Tools currentlyInUse;

    private UseableComponent currentUseableComponent;

    public List<Tools> GetCharacterTools()
    {
        return characterTools;
    }

    public Tools GetTool(int element)
    {
        return characterTools[element];
    }

    public UseableComponent GetToolsUseable(int element)
    {
        return characterTools[element].GetComponent<UseableComponent>();
    }

    public void UseTool(int element, bool forceUse)
    {
        if(currentlyInUse != null)
        {
            if(forceUse)
            {
                CancelTool();
            }
            else if(currentUseableComponent.inUse)
            {
                currentUseableComponent = null;
                return;
            }
        }
        Tools _tool = characterTools[element];

        currentUseableComponent = _tool.GetComponent<UseableComponent>();

        currentlyInUse = _tool;

        currentUseableComponent.Use();
    }

    public void CancelTool()
    {
        if(currentlyInUse == null)
        {
            return;
        }

        currentlyInUse.GetComponent<UseableComponent>().CancelUse();

        currentlyInUse = null;
    }
}
