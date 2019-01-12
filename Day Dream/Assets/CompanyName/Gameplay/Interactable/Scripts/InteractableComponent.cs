using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max;
using AH.Max.System;

public class InteractableComponent : SerializedMonoBehaviour
{
    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private List <IdentityType> interactableIdentities;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private LayerMask layers;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private List <Interaction> interactions;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    private List<Interaction> validInteractions = new List<Interaction>();

    public void OnTriggerStay(Collider other)
    {
        if(ValidateCollider(other))
        {
            Evaluate();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (ValidateCollider(other))
        {
            Clear();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Evaluate()
    {
        foreach(Interaction _interaction in interactions)
        {
            if(_interaction.EvaluateFilters())
            {
                if(!validInteractions.Contains(_interaction))
                {
                    validInteractions.Add(_interaction);
                }
            }
            else
            {
                if (validInteractions.Contains(_interaction))
                {
                    validInteractions.Remove(_interaction);        
                }
            }
        }
    }

    /// <summary>
    /// Clears out all the interactions
    /// </summary>
    public void Clear()
    {
        validInteractions.Clear();
    }

    private bool ValidateCollider(Collider other)
    {
        if (LayerMaskUtility.Contains(layers, other.gameObject.layer))
        {
            Entity _entity = other.transform.root.GetComponentInChildren<Entity>();
            if (_entity != null)
            {
                if (interactableIdentities.Contains(_entity.IdentityType))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
