using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class FreezeSpawnBuff : IBaffAction
    {
        public void PerformBaffAction()
        {
            Debug.Log("Freeze spawn granted");
            GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<EnemyManager>().FreezeSpawn();
        }
    }
}
