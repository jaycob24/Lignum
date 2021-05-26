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
    public AudioClip hillAudioClips;
    public AudioClip notEnoughManaAudioClips;

    // LSS ¯\_(ツ)_/¯
    IEnumerator LifeSupportSystem()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            
            ui.DamageMana(ui.GetMana() + 0.07f);
        }
    }
    private void Start()
    {
        // Give my mana!!1
        StartCoroutine(LifeSupportSystem());
        
        _audioSource = GetComponent<AudioSource>();
        PointClick = Instantiate(PointClick, Vector3.zero + Vector3.up / 2, Quaternion.identity);
        PointClick.transform.Rotate(90, 0, 0);

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;

        // set default parms
        defaultFireWallScale = fireWallGameObject.transform.localScale;
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
            Debug.Log("Hill!");
            if(!isNowAttack)
                StartCoroutine(UseHill());
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("FireBall");
            if(!isNowAttack)
                StartCoroutine(UseFireWall());
        }
    }

    public GameObject fireWallGameObject;
    public MeshRenderer fireWallMeshRenderer;
    public FireWall fireWall;
    
    public Vector3 defaultFireWallScale;
    IEnumerator UseFireWall()
    {

        // если навык открыт
        if(!ui.spellsList[1].learned)
            yield break;

        if (ui.GetMana() < 0.3)
        {
            _audioSource.PlayOneShot(notEnoughManaAudioClips);
            yield break;
        }

        
        
        var maxScale = new Vector3(4, 4, 4);
        if (ui.spellsList[2].learned)
        {
            fireWallMeshRenderer.material = fireWall.lvl2;
            maxScale *= 2;
        }
        else
        {
            fireWallMeshRenderer.material = fireWall.lvl1;
        }
        
        
        // такова цена огня..
        DamageMana(30f);

        _audioSource.PlayOneShot(fireWallAudioClips);
        
        isNowAttack = true;
        
        fireWallGameObject.transform.localScale = defaultFireWallScale;
        fireWallGameObject.transform.position = transform.position + Vector3.down;
        fireWallGameObject.SetActive(true);
        
        while (true)
        {
            yield return new WaitForFixedUpdate();
            
            if (fireWallGameObject.transform.localScale.magnitude < maxScale.magnitude)
            {
                fireWallGameObject.transform.localScale *= 1.2f;
                fireWallGameObject.transform.Rotate(0, 60 * Time.deltaTime, 0);
                fireWallGameObject.transform.position = transform.position + Vector3.down;
            }
            else
            {
                break;
            }
        }
        
        yield return new WaitForSeconds(.85f);

        fireWallGameObject.SetActive(false);
        
        isNowAttack = false;
        yield return null;
    }
    IEnumerator UseHill()
    {
        // если навык открыт
        if(!ui.spellsList[0].learned)
            yield break;
        
        // если хватает маны и мало здоровья
        if (ui.GetMana() < 0.4f || ui.GetHealth() > 0.9f)
        {
            _audioSource.PlayOneShot(notEnoughManaAudioClips);
            yield break;
        }
        
        // такова цена хила..
        DamageMana(40f);
        
        // получаем 40% хп за хилл
        ui.DamageHealth(ui.GetHealth() + 0.4f);
        
        _audioSource.PlayOneShot(hillAudioClips);
        
        isNowAttack = true;

        yield return new WaitForSeconds(.85f);

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