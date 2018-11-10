using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class ToolsComponent : MonoBehaviour {

    [ShowInInspector]
    protected List<Tools> characterTools = new List<Tools>();

    public List<Tools> GetCharacterTools()
    {
        return characterTools;
    }

    public Tools GetTool(int element)
    {
        return characterTools[element];
    }
}
