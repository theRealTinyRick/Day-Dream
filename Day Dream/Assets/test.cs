using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AH.Max.System;
using Sirenix.OdinInspector;

public class test : MonoBehaviour
{
    [SerializeField]
    public UnityEngine.Events.UnityEvent testFire = new UnityEngine.Events.UnityEvent();

    [Button]
	public void Check ()
    {
        testFire.Invoke();
	}
}
