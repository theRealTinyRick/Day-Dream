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
        // mesh.GetComponent<Rigidbody>().MovePosition(mesh.transform.position + direction * speed * Time.fixedDeltaTime);
        mesh.transform.Translate(direction * speed * Time.deltaTime);

        if(Vector3.Distance(mesh.transform.position, destination.position) <=  /*speed * Time.fixedDeltaTime*/ .2f){
            SetDestination(destination == aPos ? bPos : aPos);
        }
    }

    // void OnDrawGizmos(){
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireCube(aPos.position, new Vector3(2,1,2));

    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(bPos.position, new Vector3(2,1,2));
    // }

    void SetDestination(Transform dest){
        destination = dest;
        direction = (destination.position - mesh.transform.position)/*.normalized*/;
    }

}
