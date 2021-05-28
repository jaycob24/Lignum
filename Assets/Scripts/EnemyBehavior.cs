using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
class EnemyBehavior : MonoBehaviour
{
    // дефолтное состояние
    public EnemyStates currentState = EnemyStates.Patrolling;
    private GameObject player;
    public Animator animator;

    public Vector3[] waypoints;
    public int indexOfWaypoint;
    private NavMeshAgent navMeshAgent;
    public bool isNowAttack;

    private AudioSource _audioSource;
    public AudioClip attackToEnemyAudioClips;
    public AudioClip attackToAirAudioClips;
    public AudioClip attackToSwordAudioClips;
    
    IEnumerator Start()
    {
        // to visually distinguish the initial animation speeds of multiple enemies
        animator.enabled = false;
        yield return new WaitForSeconds(Random.Range(.1f, .9f));
        animator.enabled = true;
        
        _audioSource = GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");
        //Debug.Log(player);
        navMeshAgent = GetComponent<NavMeshAgent>();
        indexOfWaypoint = Random.Range(0, waypoints.Length);
    }

    public int health = 100;
    public AudioClip death;
    public void Damage(int damageCount)
    {
        health -= damageCount;

        if (health <= 0)
        {
            animator.SetBool("isRun", false);
            animator.SetBool("isAttack", false);
            
            animator.SetBool("isDeath", true);
            
            
            GetComponent<AudioSource>().PlayOneShot(death, .2f);
            
            // turn everything off in every way)0
            navMeshAgent.speed = 0;
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = true;
            this.enabled = false;
        }
    }
    
    private void FixedUpdate()
    {
        player ??= GameObject.FindWithTag("Player");
        if(player is null)
            return;
        
        if(health <= 0)
            return;
        
        switch (currentState)
        {
            case EnemyStates.Patrolling:
                Patrooling();
                break;
            case EnemyStates.Attack:
                if (!isNowAttack)
                    StartCoroutine(Attack());
                break;
            case EnemyStates.Run:
                Run();
                break;
        }
    }

    void Patrooling()
    {
       var position = transform.position;
       var distanceBetweenPlayer = Vector3.Distance(position, player.transform.position);
       var distanceBetweenWaypoints = Vector3.Distance(position, waypoints[indexOfWaypoint]);

       if (distanceBetweenPlayer > 10)
       {
           // ковыляем с точки на точку
           if (distanceBetweenWaypoints < 2)
           {
               // next waypoint
               indexOfWaypoint = Random.Range(0, waypoints.Length);
           }
           else
           {
               // walk
               if (navMeshAgent.destination != waypoints[indexOfWaypoint])
               {
                   navMeshAgent.SetDestination(waypoints[indexOfWaypoint]);
                   
                   // todo remove this lines
                   navMeshAgent.speed = 0.4f;
                   animator.SetBool("isWalk", true);
               }
           }   
       }
       else
       {
           // todo remove this lines
           navMeshAgent.speed = 4;
           animator.SetBool("isWalk", false);
           
           currentState = EnemyStates.Run;
       }
       
       //Debug.Log("position " + position);
       //Debug.Log("distanceBetweenPlayer " + distanceBetweenPlayer);
       //Debug.Log("distanceBetweenWaypoints " + distanceBetweenWaypoints);
    }

    IEnumerator Attack()
    {
        var position = transform.position;
        var distanceBetweenPlayer = Vector3.Distance(position, player.transform.position);
        if (distanceBetweenPlayer > 2f)
        {
            currentState = EnemyStates.Patrolling;
            yield return null;
        }

        // fill my yea)
        transform.LookAt(player.transform);
        
        isNowAttack = true;

        navMeshAgent.isStopped = true;
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(1.4f);
        
        // sound
        if (player.GetComponent<PlayerController>().isNowAttack)
        {
            _audioSource.PlayOneShot(attackToSwordAudioClips);
            player.GetComponent<PlayerController>().Damage(10);
        }
        else
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 1.65f)
            {
                _audioSource.PlayOneShot(attackToEnemyAudioClips);
                player.GetComponent<PlayerController>().Damage(10);
            }
            else
            {
                _audioSource.PlayOneShot(attackToAirAudioClips);
            }
        }
        
        yield return new WaitForSeconds(.6f);
        
        animator.SetBool("isAttack", false);
        navMeshAgent.isStopped = false;
        
        isNowAttack = false;
    }

    void Run()
    {
        var position = transform.position;
        var distanceBetweenPlayer = Vector3.Distance(position, player.transform.position);
        if (distanceBetweenPlayer > 2f)
        {
            if (navMeshAgent.destination != player.transform.position)
            {
                animator.SetBool("isRun", true);
                navMeshAgent.SetDestination(player.transform.position);
            }

            if (distanceBetweenPlayer > 8f)
            {
                currentState = EnemyStates.Patrolling;
            }
        }
        else
        {
            animator.SetBool("isRun", false);
            currentState = EnemyStates.Attack;
        }
    }
}
enum EnemyStates
{
    Patrolling,
    Run,
    Attack
}