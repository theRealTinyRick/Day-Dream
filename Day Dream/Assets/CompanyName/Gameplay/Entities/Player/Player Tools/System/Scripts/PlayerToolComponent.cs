/*
Author: Aaron Hines
Description: This component is responsiple for spawning and despawning tools as well as keeping track of the tools 
*/
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;
using AH.Max;

public class PlayerToolComponent : SerializedMonoBehaviour
{
    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public IdentityType currentTool;

    private GameObject currentToolObject;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public IdentityType previousTool;

    private GameObject previousToolObject;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public List<IdentityType> tools = new List<IdentityType>();

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private Dictionary<Handedness, Transform> IKMap = new Dictionary<Handedness, Transform>();

    [TabGroup(Tabs.Events)]
    [SerializeField]
    private ToolChangedEvent toolChangedEvent = new ToolChangedEvent();

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        currentTool = tools[0];
        SetCurrent(currentTool);
    }

    public void SetCurrent(IdentityType tool)
    {
        if(currentTool != null)
        {
            previousTool = currentTool;
        }

        if(currentToolObject != null)
        {
            previousToolObject = currentToolObject;
        }

        currentTool = tool;

        currentToolObject = SpawnManager.Instance.Spawn(currentTool, GetIKPoint(currentTool.handedness));

        if(previousToolObject != null)
        {
            SpawnManager.Instance.Despawn(previousToolObject.GetComponentInChildren<Entity>());
        }

        if(toolChangedEvent != null)
        {
            toolChangedEvent.Invoke(currentTool);
        }
    }

    public void GoToPrevious()
    {
        if(previousTool != null)
        {
            SetCurrent(previousTool);
            return;
        }

        Debug.Log("There is no previous tool");
    }

    private Transform GetIKPoint(Handedness handedness)
    {
        Transform _ikPoint;

        if(IKMap.TryGetValue(handedness, out _ikPoint))
        {
            return _ikPoint;
        }

        Debug.LogWarning("");
        return null;
    }
}
