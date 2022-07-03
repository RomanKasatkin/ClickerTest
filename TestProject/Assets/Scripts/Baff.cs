using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    KillAll,
    Freeze,
    FreezeSpawn
}
public class Baff : MonoBehaviour, IClickable
{
    public AudioSource pickupSound;
    public float lifeEndurance;
    public BuffType buffType;
    public IBaffAction buffActionScript;

    private bool isGranted = false;
    private ParticleSystem particles;
    // Start is called before the first frame update
    void Start()
    {
        particles = gameObject.GetComponentInChildren<ParticleSystem>();
        isGranted = false;
        switch (buffType)
        {
            case BuffType.KillAll:
                {
                    buffActionScript = new KillAllBuff();
                    break;
                }
            case BuffType.Freeze:
                {
                    buffActionScript = new FreezeBuff();
                    break;
                }
            case BuffType.FreezeSpawn:
                {
                    buffActionScript = new FreezeSpawnBuff();
                    break;
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeEndurance <= 0 && !isGranted)
        {
            GameObject.Destroy(gameObject);
        }
        else 
        {
            lifeEndurance -= Time.deltaTime;
        }
    }
    public void DoClickAction()
    {
        if (!isGranted)
        {
            isGranted = true;
            particles.Play();
            StartCoroutine(PickupSequence());
        }
    }

    public IEnumerator PickupSequence()
    {
        pickupSound.Play();
        while (pickupSound.isPlaying)
            yield return null;
        buffActionScript.PerformBaffAction();
        GameObject.Destroy(gameObject);
    }
}
