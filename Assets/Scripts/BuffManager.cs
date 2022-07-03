
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuffManager : MonoBehaviour
{
    public EnemyManager spawnManager;
    public float buffChance;
    public float buffInterval;
    public List<GameObject> buffPrefabs;

    private float nextBuffTime = 0.0f;    

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextBuffTime && !GameManager.Instance.isDead)
        {
            nextBuffTime += (buffInterval + spawnManager.difficulty);
            if(spawnManager.difficulty > 1)
            {
                if(Random.Range(0,100) < buffChance)
                    SpawnBuff();
            }
                
        }
    }

    private void SpawnBuff()
    {
        bool gotPos = false;
        NavMeshHit hit = new NavMeshHit();
        while (!gotPos)
        {
            gotPos = NavMesh.SamplePosition(new Vector3(Random.Range(-90, 90), 0, Random.Range(-90, 90)), out hit, Mathf.Infinity, 1);
        }

        var myRandomPositionInsideNavMesh = hit.position;
        myRandomPositionInsideNavMesh.y = 3;
        var enemyIndex = Random.Range(0, buffPrefabs.Count);
        Instantiate(buffPrefabs[enemyIndex], myRandomPositionInsideNavMesh, Quaternion.identity);
    }
}
