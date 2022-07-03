using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IClickable
{
    public int diffCoeficient;
    public AudioSource deathSound;

    public int score;
    public int health = 1;
    public float speed = 1f;

    private bool isDead = false;
    private bool move = true;
    private NavMeshAgent agent;
    private ParticleSystem particles;
    private Animator anim;
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        anim.SetBool("isWalk", move);
        particles = gameObject.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<ParticleSystem>();
        isDead = false;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (!agent.pathPending && move)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        GenerateAgentTarget();
                    }
                }
            }
        }
        else
            agent.isStopped = true; ;
        
    }
    public void DoClickAction()
    {
        GetDamage();
    }

    public IEnumerator Freeze(float fTime)
    {
        Debug.Log("Frze");
        var color = gameObject.GetComponentInChildren<Renderer>().material.color;
        gameObject.GetComponentInChildren<Renderer>().material.color = Color.gray;
        move = false;
        anim.SetBool("isWalk", move);
        var saveWay = agent.destination;
        agent.isStopped = true;
            yield return new WaitForSeconds(fTime);
        if(!isDead)
        {
            Debug.Log("Unfreeze");
            agent.isStopped = false;
            gameObject.GetComponentInChildren<Renderer>().material.color = color;
            move = true;
            anim.SetBool("isWalk", move);
        }
        
    }

    void GenerateAgentTarget()
    {
        var walkRadius = Random.Range(0, 10);
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;
        agent.SetDestination(finalPosition);
    }

    void GetDamage()
    {
        if(!isDead)
        {
            particles.Play();
            GameManager.Instance.tapSound.Play();
            Debug.Log("Health-");
            health -= 1;
            if (health <= 0)
                CommitDeath();
        }        
    }

    public void CommitDeath()
    {
        isDead = true;
        StartCoroutine(DeathSequence());
    }

    public IEnumerator DeathSequence()
    {
        anim.SetTrigger("isDead");
        deathSound.Play();
        while (deathSound.isPlaying)
            yield return null;
        GameManager.Instance.GainScore(score);
        GameObject.Destroy(gameObject);
    }
}
