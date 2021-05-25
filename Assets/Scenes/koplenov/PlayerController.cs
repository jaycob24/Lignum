using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private Camera _camera;
    private NavMeshAgent _navMeshAgent;
    public Animator animator;

    public UI ui;

    public GameObject PointClick;
    private Vector3 point;

    private GameObject FocusedEnemy;
    public bool isNowAttack;

    private AudioSource _audioSource;
    public AudioClip attackToEnemyAudioClips;
    public AudioClip attackToAirAudioClips;
    public AudioClip attackToSwordAudioClips;
    
    public AudioClip fireWallAudioClips;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        PointClick = Instantiate(PointClick, Vector3.zero + Vector3.up / 2, Quaternion.identity);
        PointClick.transform.Rotate(90, 0, 0);

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;

        // set default parms
        defaultFireWallScale = FireWall.transform.localScale;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                switch (hitInfo.collider.tag)
                {
                    case "Enemy":
                        FocusedEnemy = hitInfo.collider.gameObject;
                        Debug.LogWarning("Click to enemy!");
                        if (!isNowAttack)
                            StartCoroutine(Attack());
                        break;
                    default:
                        _navMeshAgent.SetDestination(hitInfo.point);

                        //Instantiate(PointClick, hitInfo.point + Vector3.up/2, Quaternion.identity).transform.Rotate(90,0,0);
                        PointClick.transform.position = hitInfo.point + Vector3.up / 2;
                        break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("FireBall");
            if(!isNowAttack)
                StartCoroutine(UseFireWall());
        }
    }

    public GameObject FireWall;
    public Vector3 defaultFireWallScale;
    IEnumerator UseFireWall()
    {
        if (ui.GetMana() < 0.3)
            yield break;
        
        // такова цена огня..
        DamageMana(30f);

        _audioSource.PlayOneShot(fireWallAudioClips);
        
        isNowAttack = true;
        
        FireWall.transform.localScale = defaultFireWallScale;
        FireWall.transform.position = transform.position + Vector3.down;
        FireWall.SetActive(true);
        
        while (true)
        {
            yield return new WaitForFixedUpdate();
            
            if (FireWall.transform.localScale.magnitude < new Vector3(4, 4, 4).magnitude)
            {
                FireWall.transform.localScale *= 1.2f;
                FireWall.transform.Rotate(0, 60 * Time.deltaTime, 0);
                FireWall.transform.position = transform.position + Vector3.down;
            }
            else
            {
                break;
            }
        }
        
        yield return new WaitForSeconds(.85f);

        FireWall.SetActive(false);
        
        isNowAttack = false;
        yield return null;
    }

    private void FixedUpdate()
    {
        //animator.SetBool("isRun", _navMeshAgent.velocity.magnitude > 1 );
        animator.SetBool("isRun", _navMeshAgent.remainingDistance > 2 /*&& _navMeshAgent.angularSpeed*/);

        #region Demo

        if (Input.GetKey(KeyCode.F)) ui.DamageHealth(ui.GetHealth() - 0.02f);
        if (Input.GetKey(KeyCode.R)) ui.DamageHealth(ui.GetHealth() + 0.02f);
        if (Input.GetKey(KeyCode.G)) ui.DamageMana(ui.GetMana() - 0.02f);
        if (Input.GetKey(KeyCode.T)) ui.DamageMana(ui.GetMana() + 0.02f);

        #endregion
    }
    
    private void Damage(EnemyBehavior enemyBehavior)
    {
        enemyBehavior.Damage(50);
    }
    
    IEnumerator Attack()
    {
        var enemyBehavior = FocusedEnemy.GetComponent<EnemyBehavior>();

        isNowAttack = true;

        _navMeshAgent.isStopped = true;
        animator.SetBool("isAttack", true);
        
        // fill my yea)
        transform.LookAt(FocusedEnemy.transform);
        yield return new WaitForSeconds(.85f);
        transform.LookAt(FocusedEnemy.transform);
        
        // sound
        if (enemyBehavior.isNowAttack)
        {
            _audioSource.PlayOneShot(attackToSwordAudioClips);
            Damage(enemyBehavior);
        }
        else
        {
            if (Vector3.Distance(transform.position, FocusedEnemy.transform.position) < 1.65f)
            {
                _audioSource.PlayOneShot(attackToEnemyAudioClips);
                Damage(enemyBehavior);
            }
            else
            {
                _audioSource.PlayOneShot(attackToAirAudioClips);
            }
        }


        animator.SetBool("isAttack", false);
        _navMeshAgent.isStopped = false;

        isNowAttack = false;
    }

    public void Damage(float i)
    {
        ui.DamageHealth(ui.GetHealth() - i/100);
    }
    public void DamageMana(float i)
    {
        ui.DamageMana(ui.GetMana() - i/100);
    }
}