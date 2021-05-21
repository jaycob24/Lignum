using System;
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
    private bool isNowAttack;

    private void Start()
    {
        PointClick = Instantiate(PointClick, Vector3.zero + Vector3.up / 2, Quaternion.identity);
        PointClick.transform.Rotate(90, 0, 0);

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
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

    IEnumerator Attack()
    {
        //todo дамаг по FocusedEnemy

        // fill my yea)
        transform.LookAt(FocusedEnemy.transform);
        
        isNowAttack = true;

        _navMeshAgent.isStopped = true;
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(.5f);
        animator.SetBool("isAttack", false);
        _navMeshAgent.isStopped = false;
        
        // todo атакуем
        // todo включаем анимацию атаки
        // todo если убили, гг
        // todo если убежал, то бежим за ним
        
        isNowAttack = false;
    }

    public void Damage(float i)
    {
        ui.DamageHealth(ui.GetHealth() - i/100);
    }
}