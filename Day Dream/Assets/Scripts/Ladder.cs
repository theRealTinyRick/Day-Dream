using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour {

    public Transform bottomPos;
    public Transform topPos;
    public Transform endPos;

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerManager.instance.ladder = gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerManager.instance.ladder = null;
        }
    }
}
