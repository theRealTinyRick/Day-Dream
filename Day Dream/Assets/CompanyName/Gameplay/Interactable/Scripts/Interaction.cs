using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class Interaction
{
    [TabGroup(Tabs.Properties)]
    public IInteractionFilter[] filters;

    [TabGroup(Tabs.Events)]
    public InteractionStartedEvent interactionStartedEvent = new InteractionStartedEvent();

    [TabGroup(Tabs.Events)]
    public InteractionCompletedEvent interactionCompletedEvent = new InteractionCompletedEvent();

    public KeyCode[] keyCodes;

    public bool EvaluateFilters()
    {
        foreach(IInteractionFilter _filter in filters)
        {
            if(!_filter.Filter())
            {
                return false;
            }
        }

        return true;
    }

    public void ExecuteInteraction()
    {
        if(interactionStartedEvent != null)
        {
            interactionStartedEvent.Invoke();
        }
    }

    public bool EvaluateInput()
    {
        if(keyCodes != null)
        {
            foreach(KeyCode _code in keyCodes)
            {
                if(Input.GetKeyDown(_code))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
