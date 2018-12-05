using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;

[System.Serializable]
public class Stat
{
    public StatType statType;

    ///<Summary>
    /// The amount shown in the inspector to rep the statType
    ///</Summary>
    [TabGroup(Tabs.Stats)]
    [SerializeField]
    private float amount;
    public float Amount
    {
        get
        {
            return amount;
        }
        set
        {
            amount = value;
        }
    }

    ///<Summary>
    ///
    ///</Summary>
    [TabGroup(Tabs.Stats)]
    [SerializeField]
    private float minimumAmount;
    public float MinimumAmount
    {
        get 
        {
            return minimumAmount;
        }
        set
        {
            minimumAmount = value;
        }
    }

    ///<Summary>
    ///
    ///</Summary>
    [TabGroup(Tabs.Stats)]
    [SerializeField]
    private float maximumAmount;
    public float MaximumAmount
    {
        get 
        {
            return maximumAmount;
        }
        set
        {
            maximumAmount = value;
        }
    }

    [SerializeField]
    ///<Summary>
    ///
    ///</Summary>
    public void Add()
    {
        Amount ++;
    }

    public void Subtract()
    {
        Amount --;
    }

    public void Add(float amountToAdd)
    {
        Amount += amountToAdd;
    }

    public void Subtract(float amountToSubtract)
    {
        Amount -= amountToSubtract;
    }

    public void Reset()
    {
        Amount = MaximumAmount;
    }

    public void RemoveAll()
    {
        Amount = MinimumAmount;
    }
}
