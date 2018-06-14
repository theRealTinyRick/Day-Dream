using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    [SerializeField] private Transform aPos;
    [SerializeField] private Transform bPos;
    [SerializeField] private GameObject mesh;
    [SerializeField] private float speed;
    [SerializeField] private float timeToWait;

    public float range;

    public enum Dir {A, B, Paused };
    public Dir currentDirection = Dir.B;

    private void Update()
    {
        if (currentDirection == Dir.B)
        {
            mesh.transform.position = Vector3.MoveTowards(mesh.transform.position, bPos.position, speed * Time.deltaTime);
            if (Vector3.Distance(mesh.transform.position, bPos.position) < range)
            {
                StartCoroutine(SwitchDir(Dir.A));
                mesh.transform.position = bPos.position;
            }
        }
        else if (currentDirection == Dir.A)
        {
            mesh.transform.position = Vector3.MoveTowards(mesh.transform.position, aPos.position, speed * Time.deltaTime);
            if (Vector3.Distance(mesh.transform.position, aPos.position) < range)
            {
                mesh.transform.position = aPos.position;
                StartCoroutine(SwitchDir(Dir.B));
            }
        }
        else
            return;
    }

    IEnumerator SwitchDir(Dir switchTo)
    {
        currentDirection = Dir.Paused;
        yield return new WaitForSeconds(timeToWait);
        currentDirection = switchTo;

    }

}
