using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerGravityComponent : MonoBehaviour
{
    [SerializeField]
    private float fallMultiplier;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = transform.root.GetComponentInChildren<Rigidbody>();
    }

	void FixedUpdate ()
    {
        if (_rigidbody.velocity.y  < 0)
        {
         	_rigidbody.velocity += Vector3.up *  Physics.gravity.y  * (fallMultiplier - 1) * Time.deltaTime;

        }
        else if (_rigidbody.velocity.y  > 0 )
        {
         	_rigidbody.velocity += Vector3.up * Physics.gravity.y *2 * Time.deltaTime;
        }
    }
}
