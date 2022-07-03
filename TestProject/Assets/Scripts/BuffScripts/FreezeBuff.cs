using UnityEngine;

namespace Assets.Scripts
{
    class FreezeBuff : IBaffAction
    {
        public void PerformBaffAction()
        {
            Debug.Log("Freeze granted");
            GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<EnemyManager>().Freeze();
        }
    }
}
