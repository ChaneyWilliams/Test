using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.InputSystem;
using System.Collections;
using UnityEditor.Callbacks;
using System.Data.Common;
using NUnit;
using System.Threading;
using System.ComponentModel;


public class Player : MonoBehaviour
{
    public Color color;
    public static Player instance;



    [Header("Movement")]
    public float horizontal;
    public float speed = 8f;

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
    public bool isDashing;
    public bool canDash = true;

    [Header("Animations")]
    public bool isWalking;
    float time = 0;
    public float idleSpeed = 1.0f;
    public float idleIntensity = 0.01f;
    public float wiggleSpeed = 2.0f;
    public float wiggleAngle = 5.0f;
    public float attackCooldown = 0.5f;
    public bool facingRight;
    public bool isAttacking;
    public bool flipping;


    [Header("Grabbed Components")]
    [SerializeField] public Animator animator;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    bool dead = false;


    private Coroutine walkWiggleCoroutine;
    Vector3 defaultScale;
    void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
        else
        {
            Debug.Log("there was a clone");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        defaultScale = transform.localScale;
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
        if (horizontal == 0)
        {
            IdleAnim();
        }
        else
        {
            transform.localScale = new Vector2(1, 1);
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
            facingRight = false;
        }
        else if (horizontal > 0f)
        {
            facingRight = true;
        }
    }
    public void Death()
    {
        dead = true;
    }



    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        transform.localScale = defaultScale;

        if (horizontal != 0)
        {
            if (!isWalking && !flipping)
            {
                isWalking = true;
                walkWiggleCoroutine = StartCoroutine(WalkAnim());
            }
        }
        else
        {
            if (isWalking && !flipping)
            {
                isWalking = false;
                if (walkWiggleCoroutine != null)
                    StopCoroutine(walkWiggleCoroutine);
                transform.eulerAngles = Vector3.zero;
            }
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
        float dashDirection = animator.GetBool("FacingRight") ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashSpeed * dashDirection, rb.linearVelocity.y);
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isDashing = false;
        trailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCooldowon);

    }
    IEnumerator CoolFlip()
    {
        Debug.Log("FLippin");
        float a = 0.0f;
        while (a <= 345 && a >= -345)
        {
            flipping = true;
            if (facingRight)
            {
                a -= 15.0f;
            }
            else
            {
                a += 15.0f;
            }
            transform.eulerAngles = new Vector3(0f, 0f, a);
            yield return null;
        }
        a = 0.0f;
        flipping = false;
    }
    IEnumerator WalkAnim()
    {
        float t = 0f;

        while (isWalking)
        {
            // Wiggle back and forth around Z axis
            float angle = Mathf.Sin(t * wiggleSpeed) * wiggleAngle;
            transform.eulerAngles = new Vector3(0f, 0f, angle);
            t += Time.deltaTime;
            yield return null;
        }

        // Reset rotation when done
        transform.eulerAngles = Vector3.zero;
    }
    void IdleAnim()
    {
        time += Time.deltaTime * Mathf.PI * idleSpeed;
        float fluct = defaultScale.y + Mathf.Sin(time) * idleIntensity; // oscillates around original y-scale
        transform.localScale = new Vector3(defaultScale.x, fluct, defaultScale.z);
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            isAttacking = true;
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
                if (jumpsRemaining == 0)
                {
                    StartCoroutine(CoolFlip());
                }
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
