using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Transform thisThing;
    public Transform target;

	void Update ()
    {
        thisThing.LookAt(target.position);
	}
}
