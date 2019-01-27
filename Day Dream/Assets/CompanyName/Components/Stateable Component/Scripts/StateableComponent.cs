using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

[System.Serializable]
public class State
{
    public GameObject model;
    public float value;
}

public class StateableComponent : MonoBehaviour
{
    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private List<State> states = new List<State>();

    private State currentState;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private float currentValue;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private float minimumValue;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private float maximumValue;

    [TabGroup(Tabs.Events)]
    [SerializeField]
    StateChangedEvent stateChangedEvent = new StateChangedEvent();

    private void Start ()
    {
        Evaluate();
	}

    public void Evaluate()
    {
        foreach(State _state in states)
        {
            if(currentValue >= _state.value)
            {
                currentState = _state;
                SetStateUp();
                return;
            }
        }

        if(stateChangedEvent != null)
        {
            stateChangedEvent.Invoke(currentState.model, currentState.value);
        }
    }

    private void SetStateUp()
    {
        if(currentState == null)
        {
            return;
        }

        foreach(State _state in states)
        {
            if(_state == currentState)
            {
                _state.model.SetActive(true);
            }
            else
            {
                _state.model.SetActive(false);
            }
        }
    }

    public void SetValue(float value)
    {
        currentValue = value;
        Evaluate();
    }
}
