using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    [SerializeField] Transform aPos;
    [SerializeField] Transform bPos;
    [SerializeField] GameObject mesh;
    [SerializeField] float speed;

    public Vector3 direction;
    Transform destination;
    
    void Start(){
        SetDestination(aPos);
    }

    void FixedUpdate(){
        mesh.transform.Translate(direction * speed * Time.deltaTime);

        if(Vector3.Distance(mesh.transform.position, destination.position) <=  /*speed * Time.fixedDeltaTime*/ .2f){
            SetDestination(destination == aPos ? bPos : aPos);
        }
    }

    void SetDestination(Transform dest){
        destination = dest;
        direction = (destination.position - mesh.transform.position)/*.normalized*/;
    }

}
