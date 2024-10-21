using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public LayerMask groundlayer;
    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;
    private bool isJumping;

    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        //Movimiento Horizontal

        float moveInputX = Input.GetAxis("Horizontal");
        float moveInputZ = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(moveInputX * moveSpeed, rb.velocity.y, moveInputZ * moveSpeed);
        rb.velocity = move;

        if (Mathf.Abs(moveInputX) > 0.2f || Mathf.Abs(moveInputZ) > 0.2f)
        {
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }

        // Comprobar si esta en el suelo

        isGrounded = Physics.CheckSphere(transform.position, 1f, groundlayer);

        // Salto

        if (isGrounded && !isJumping)
        {
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }

        ApplyJumpPhysics();

        UpdateAnimationStates();

    }

    void Jump()
    {
        isJumping = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animator.SetTrigger("JumpStart");
    }

    void ApplyJumpPhysics()
    {
        if (rb.velocity.y < 0)
        {
            // Cayendo
            rb.velocity += Vector3.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Salto bajo
            rb.velocity += Vector3.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void UpdateAnimationStates()
    {
        if (isJumping)
        {
            if (rb.velocity.y == 0f)
            {
                animator.SetTrigger("JumpEnd");
            }

            if (isGrounded)
            {
                isJumping = false;
                animator.SetTrigger("Landing");
            }
        }

    }

}
