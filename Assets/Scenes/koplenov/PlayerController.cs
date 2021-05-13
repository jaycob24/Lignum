using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private Camera _camera;
    private NavMeshAgent _navMeshAgent;
    public Animator animator;
    
    public UI ui;
    
    private void Start()
    {
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
                    default:
                        _navMeshAgent.SetDestination(hitInfo.point);
                        break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        animator.SetBool("isRun", _navMeshAgent.velocity.magnitude > 1 );

        #region Demo

        if (Input.GetKey(KeyCode.F)) ui.DamageHealth(ui.GetHealth() - 0.02f);
        if (Input.GetKey(KeyCode.R)) ui.DamageHealth(ui.GetHealth() + 0.02f);
        if (Input.GetKey(KeyCode.G)) ui.DamageMana(ui.GetMana() - 0.02f);
        if (Input.GetKey(KeyCode.T)) ui.DamageMana(ui.GetMana() + 0.02f);

        #endregion
    }
}