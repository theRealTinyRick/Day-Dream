using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour {

    private enum Type { Continuous, Triggered};
    [SerializeField] private Type thisType = Type.Continuous;

    [SerializeField] private float speed;

    private void Update()
    {
        if(thisType == Type.Continuous)
        {
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
        else
        {
            //trigger logic
        }
    }
}
