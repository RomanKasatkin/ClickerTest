using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    class KillAllBuff : IBaffAction
    {        
        public void PerformBaffAction()
        {
            Debug.Log("Kill granted");
            GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<EnemyManager>().KillAll();
        }
    }
}
