using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public int difficulty = 1; 
    private float nextIncreaseTime = 0.0f;
    private float nextSpawnTime = 0.0f;
    public float increasePeriod = 5f;

    public Text enemyCountText;
    public int defeatCount = 10;
    public float freezeTime = 3f;
    public float spawnPeriod = 3f;

    public List<GameObject> spawnedEnemies;
    public List<GameObject> enemyPrefabs;
    private List<int> enemyHash = new List<int>();

    private bool spawning = true;
    // Start is called before the first frame update
    void Start()
    {
        nextSpawnTime = 0.0f;
        nextIncreaseTime = increasePeriod;
        spawnedEnemies = new List<GameObject>();
        foreach (var e in enemyPrefabs.Select((value, i) => new { i, value }))
        {
            for (var j = 0; j < e.value.GetComponent<Enemy>().diffCoeficient; j++)
            {
                enemyHash.Add(e.i);
            }
        }   
    }
    void Update()
    {
        if (!GameManager.Instance.isDead)
        {
            //increase difficulty
            if (GameManager.Instance.GetGameTime() > nextIncreaseTime)
            {
                nextIncreaseTime += increasePeriod;
                difficulty += 1;
            }

            if (GameManager.Instance.GetGameTime() > nextSpawnTime)
            {
                nextSpawnTime += Random.Range(spawnPeriod, 2 * spawnPeriod) / Mathf.Sqrt(difficulty);
                if (spawning)
                    SpawnEnemy();
            }

            enemyCountText.text = spawnedEnemies.Count.ToString();
            spawnedEnemies.RemoveAll(item => item == null);

            if (spawnedEnemies.Count >= defeatCount)
                GameManager.Instance.CommitDefeat();
        }
    }
    public void KillAll()
    {
        foreach(var e in spawnedEnemies)
        {
            e.GetComponent<Enemy>().CommitDeath();
        }
    }

    public void Freeze()
    {
        foreach (var e in spawnedEnemies)
        {
            StartCoroutine(e.GetComponent<Enemy>().Freeze(freezeTime));
        }
    }

    public void FreezeSpawn()
    {        
            StartCoroutine(FreezeSpawn(freezeTime));
    }    
   
    void SpawnEnemy()
    {
        bool gotPos = false;
        NavMeshHit hit = new NavMeshHit();
        while(!gotPos)
        {
            //Get random point on field
            gotPos = NavMesh.SamplePosition(new Vector3(Random.Range(- 100, 100), 0, Random.Range(-100, 100)), out hit, Mathf.Infinity, 1);
        }
        var myRandomPositionInsideNavMesh = hit.position;

        var enemyIndex = enemyHash[Random.Range(1, enemyHash.Count)-1];
        var newEnemy = Instantiate(enemyPrefabs[enemyIndex], myRandomPositionInsideNavMesh, Quaternion.identity);
        var newEnemyParams = newEnemy.GetComponent<Enemy>();
        newEnemyParams.health += difficulty - 1;
        newEnemyParams.speed += difficulty - 1;
        spawnedEnemies.Add(newEnemy);
    }

    public IEnumerator FreezeSpawn(float fTime)
    {
        spawning = false;
        yield return new WaitForSeconds(fTime);
        spawning = true;
    }
}
