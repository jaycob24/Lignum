using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    private Camera _camera;
    private NavMeshAgent _navMeshAgent;
    public Animator animator;

    public UI ui;

    public GameObject pointClick;

    private GameObject _focusedEnemy;
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
            ui.DamageHealth(ui.GetHealth() + 0.03f);
        }
    }
    private void Start()
    {
        // Give my mana!!1
        StartCoroutine(LifeSupportSystem());
        
        _audioSource = GetComponent<AudioSource>();
        pointClick = Instantiate(pointClick, Vector3.zero + Vector3.up / 2, Quaternion.identity);
        pointClick.transform.Rotate(90, 0, 0);

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
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000, 1, QueryTriggerInteraction.Ignore))
            {
                switch (hitInfo.collider.tag)
                {
                    case "Enemy":
                        _focusedEnemy = hitInfo.collider.gameObject;
                        Debug.LogWarning("Click to enemy!");
                        if (!isNowAttack)
                            StartCoroutine(Attack());
                        break;
                    default:
                        _navMeshAgent.SetDestination(hitInfo.point);

                        //Instantiate(PointClick, hitInfo.point + Vector3.up/2, Quaternion.identity).transform.Rotate(90,0,0);
                        pointClick.transform.position = hitInfo.point + Vector3.up / 2;
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

    private void LateUpdate()
    {
        if (Vector3.Distance(pointClick.transform.position, transform.position) < .9f)
            pointClick.SetActive(false);
        else
            pointClick.SetActive(true);
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
        animator.SetBool("isRun", _navMeshAgent.remainingDistance > 2);

        #region Demo

        if (Input.GetKey(KeyCode.F)) ui.DamageHealth(ui.GetHealth() - 0.02f);
        if (Input.GetKey(KeyCode.R)) ui.DamageHealth(ui.GetHealth() + 0.02f);
        if (Input.GetKey(KeyCode.G)) ui.DamageMana(ui.GetMana() - 0.02f);
        if (Input.GetKey(KeyCode.T)) ui.DamageMana(ui.GetMana() + 0.02f);

        #endregion
    }
    
    private void Damage(EnemyBehavior enemyBehavior)
    {
        if (enemyBehavior.health <= 0)
            return;
        enemyBehavior.Damage(50);
    }
    
    IEnumerator Attack()
    {
        var enemyBehavior = _focusedEnemy.GetComponent<EnemyBehavior>();

        isNowAttack = true;

        _navMeshAgent.isStopped = true;
        animator.SetBool("isAttack", true);
        
        // fill my yea)
        transform.LookAt(_focusedEnemy.transform);
        yield return new WaitForSeconds(.85f);
        transform.LookAt(_focusedEnemy.transform);
        
        // sound
        if (enemyBehavior.isNowAttack)
        {
            _audioSource.PlayOneShot(attackToSwordAudioClips);
            Damage(enemyBehavior);
            maybeLvlUp(enemyBehavior);
        }
        else
        {
            if (Vector3.Distance(transform.position, _focusedEnemy.transform.position) < 1.65f)
            {
                _audioSource.PlayOneShot(attackToEnemyAudioClips);
                Damage(enemyBehavior);
                maybeLvlUp(enemyBehavior);
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

    void maybeLvlUp(EnemyBehavior enemyBehavior)
    {
        if(enemyBehavior.health <= 0)
            ui.countOfSkillPoints++;
    }

    public void Damage(float i)
    {
        ui.DamageHealth(ui.GetHealth() - i/100);

        if (ui.GetHealth() <= 0)
        {
            animator.SetBool("isDead", true);
            Destroy(this);
        }
    }
    public void DamageMana(float i)
    {
        ui.DamageMana(ui.GetMana() - i/100);
    }
}