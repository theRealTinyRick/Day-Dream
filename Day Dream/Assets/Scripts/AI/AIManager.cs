using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour {

	public static AIManager instance;

    public List<GameObject> allSpawnedEnemies = new List<GameObject>();
    public List<GameObject> mobbedEnemies = new List<GameObject>();
    public List<GameObject> activeEnemies = new List<GameObject>();
    [SerializeField] private float switchTimer;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(SwitchTimer());
    }

    IEnumerator SwitchTimer()
    {
        yield return new WaitForSeconds(switchTimer);
        SwitchActiveEnemies();
        StartCoroutine(SwitchTimer());
    }

    void SwitchActiveEnemies()
    {
        activeEnemies.Clear();

        if(mobbedEnemies.Count>0)
        {
            float numberOfEnemiesToAttack = mobbedEnemies.Count / 3;
            if (numberOfEnemiesToAttack < 1)
                numberOfEnemiesToAttack = 1;

            while (activeEnemies.Count < numberOfEnemiesToAttack)
            {
                GameObject newAdd;
                newAdd = mobbedEnemies[Random.Range(0, mobbedEnemies.Count)];
                if (!activeEnemies.Contains(newAdd)){
                    activeEnemies.Add(newAdd);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other){
        if (other.tag == "LockOnTarget"){
            if (!mobbedEnemies.Contains(other.gameObject)){
                EnemyBase eBase = other.GetComponent<EnemyBase>();
                if(eBase && eBase.isAggro && eBase.currentState != EnemyBase.State.Dead){
                    mobbedEnemies.Add(other.gameObject);
                }else{
                    mobbedEnemies.Remove(other.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other){
        if (other.tag == "LockOnTarget"){
            mobbedEnemies.Remove(other.gameObject);
        }
    }
}
