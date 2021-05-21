using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

class EnemyBehavior : MonoBehaviour
{
    // дефолтное состояние
    public EnemyStates currentState = EnemyStates.Patrolling;
    private GameObject player;
    public Animator animator;

    public Vector3[] Waypoints;
    public int indexOfWaypoint;
    private NavMeshAgent navMeshAgent;
    private bool isNowAttack;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        Debug.Log(player);
        navMeshAgent = GetComponent<NavMeshAgent>();
        indexOfWaypoint = Random.Range(0, Waypoints.Length);

        //currentWaypoint.transform.position = new Vector3(Random.Range(0f,5f), -5, Random.Range(0f,5f));
        //navMeshAgent.SetDestination(new Vector3(5,5,5));
    }

    private void Update()
    {
        player ??= GameObject.FindWithTag("Player");
        
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
       var distanceBetweenWaypoints = Vector3.Distance(position, Waypoints[indexOfWaypoint]);

       if (distanceBetweenPlayer > 5)
       {
           // ковыляем с точки на точку
           if (distanceBetweenWaypoints < 2)
           {
               // next waypoint
               var tempValue = indexOfWaypoint;
               while (indexOfWaypoint == tempValue)
               {
                   indexOfWaypoint = Random.Range(0, Waypoints.Length);   
               }
           }
           else
           {
               // walk
               if(navMeshAgent.destination != Waypoints[indexOfWaypoint])
                   navMeshAgent.SetDestination(Waypoints[indexOfWaypoint]);
               
               // todo remove this line
               animator.SetBool("isRun", true);
           }   
       }
       else
       {
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
        
        distanceBetweenPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceBetweenPlayer < 1.65f)
            player.GetComponent<PlayerController>().Damage(10);
        
        animator.SetBool("isAttack", false);
        navMeshAgent.isStopped = false;
        
        // todo атакуем
        // todo включаем анимацию атаки
        // todo если убили, гг
        // todo если убежал, то бежим за ним
        
        isNowAttack = false;
    }

    void Run()
    {
        animator.SetBool("isRun", true);
        
        var position = transform.position;
        var distanceBetweenPlayer = Vector3.Distance(position, player.transform.position);
        if (distanceBetweenPlayer > 2f)
        {
            if(navMeshAgent.destination != player.transform.position)
                navMeshAgent.SetDestination(player.transform.position);
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