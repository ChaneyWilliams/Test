using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.InputSystem;
using System.Collections;
using UnityEditor.Callbacks;
using System.Data.Common;


public class Player : MonoBehaviour
{
    public Color color;



    [Header("Movement")]
    float horizontal;
    public float speed = 8f;
    bool isFacingRight;
    [Header("Jumping")]
    public float jumpingPower = 16f;
    public int numJumps = 2;
    int jumpsRemaining;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 10f;
    public float fallSpeedMult = 2f;

    [Header("Dashing")]
    public float dashSpeed = 20.0f;
    public float dashDuration = 0.1f;
    public float dashCooldowon = 0.1f;
    bool isDashing;
    bool canDash = true;
    TrailRenderer trailRenderer;
    Animator animator;


    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    bool dead = false;
    bool isWalking;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (isDashing)
        {
            return;
        }
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        Gravity();
        GroundCheck();
        Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing)
            return;

        if (!dead)
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
        }
    }


    private void GroundCheck()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.4f, groundLayer))
        {
            jumpsRemaining = numJumps;
            canDash = true;
        }
    }
    private void Flip()
    {
        if (horizontal < 0f)
        {
            isFacingRight = false;
            spriteRenderer.flipX = true;
        }
        else if (horizontal > 0f)
        {
            isFacingRight = true;
            spriteRenderer.flipX = false;
        }
    }
    public void Death()
    {
        dead = true;
    }



    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        if (horizontal > 0 || horizontal < 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }


    }
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }
    IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        trailRenderer.emitting = true;
        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashSpeed * dashDirection, rb.linearVelocity.y);
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isDashing = false;
        trailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCooldowon);

    }
    IEnumerator WalkAnim()
    {
        
    }
    public void Attack(InputAction.CallbackContext context)
    {
        int attackNum = 0;
        if (context.performed && attackNum == 0)
        {
            animator.SetTrigger("Attack");
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                jumpsRemaining--;
            }
            else if (context.canceled)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
            }
        }
    }
    public void Gravity()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.gravityScale = baseGravity * fallSpeedMult;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }
}
