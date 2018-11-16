using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorDesigner.Runtime;
using Sirenix.OdinInspector;

public class ToolsComponent : MonoBehaviour {

    [ShowInInspector]
    [SerializeField]
    protected List<Tools> characterTools = new List<Tools>();

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

    public void UseTool(int element, out UseableComponent useableComponent)
    {
        useableComponent = characterTools[element].GetComponent<UseableComponent>();

        useableComponent.Use();
    }
}
