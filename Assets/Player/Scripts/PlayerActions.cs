using Gaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public Animator animator;
    public ObjectPool objectPool;
    public Transform firepoint;

    private PlayerLocomotionInput _playerLocomotionInput;
    private PlayerSoundController _soundController;

    private float footstepDelay = 0.5f;
    private float footstepTimer;
    [HideInInspector] public bool isMoving;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        _soundController = GetComponent<PlayerSoundController>();
        footstepTimer = footstepDelay;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Eat();
        }

        if(!isMoving)
        {
            animator.SetBool("Move", false);
        }
        
        /*if (_playerLocomotionInput.JumpPressed)
        {
            
        }*/

    }

    public void Jumping()
    {
        animator.SetTrigger("JumpStart");
        _soundController.PlayJumpSound();
    }
    
    public void Walk()
    {
        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            _soundController.PlayFootstepSound();
            footstepTimer = footstepDelay;
        }

        animator.SetBool("Move", true);
    }

    public void Falling()
    {
        animator.SetTrigger("IsFalling");
    }
    
    public void Shoot()
    {
        animator.SetTrigger("Attack");
        _soundController.PlayShootSound();

        GameObject bullet = objectPool.GetPooledObject();
        bullet.transform.position = firepoint.position;
        bullet.transform.rotation = firepoint.rotation;
        bullet.SetActive(true);
    }

    public void Eat()
    {
        animator.SetTrigger("Eat");
        _soundController.PlayEatSound();
    }

}
