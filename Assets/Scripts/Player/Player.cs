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


    //vars for left and right movement
    [Header("Movement")]
    public float horizontal;
    public float speed = 8f;

    // vars for jumping
    [Header("Jumping")]
    public float jumpingPower = 16f;
    public int numJumps = 2;
    int jumpsRemaining;

    // vars for gravity
    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 10f;
    public float fallSpeedMult = 2f;

    //vars for dashing
    [Header("Dashing")]
    public float dashSpeed = 20.0f;
    public float dashDuration = 0.1f;
    public float dashCooldowon = 0.1f;
    public bool isDashing;
    public bool canDash = true;

    //vars for the coded animation (idle, moving, and flipping)
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
    
    [Header("Parry")]
    public float parryWindow = 0.1f;
    public bool isParrying;

    [Header("Grabbed Components")]
    [SerializeField] public Animator animator;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    bool dead = false;
    public int health;
    SpriteFlasher spriteFlasher;

    private Coroutine walkWiggleCoroutine;
    Vector3 defaultScale;
    //using a static for easy access in the animator
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
        spriteFlasher = GetComponent<SpriteFlasher>();
    }
    // grabbing the normal size so the coded animations dont break the scale
    void Start()
    {
        defaultScale = transform.localScale;
    }
    private void Update()
    {
        // make the dash overwrite the basic movement
        if (isDashing)
        {
            return;
        }
        //rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        Gravity();
        GroundCheck();
        Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing)
            return;

        if (dead) //if dead stop all movement
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
        }
        else if(!spriteFlasher.hit)
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }
        //play Idlw when standing still when moving reset scale
        if (horizontal == 0)
        {
            IdleAnim();
        }
        else
        {
            transform.localScale = new Vector2(1, 1);
        }

    }

    //when touching the ground reset the playe jumps and dash
    private void GroundCheck()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.4f, groundLayer))
        {
            jumpsRemaining = numJumps;
            canDash = true;
        }
    }
    // tells if facing right
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


// listens for movement keys called in player input component
    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        transform.localScale = defaultScale;

//play the walk animation until the player stop
        if (horizontal != 0)
        {
            isWalking = true;
            walkWiggleCoroutine = StartCoroutine(WalkAnim());
        }
        else
        {
//stop the walk animation and reset the player rotation
            isWalking = false;
            if (walkWiggleCoroutine != null)
                StopCoroutine(walkWiggleCoroutine);
            transform.eulerAngles = Vector3.zero;

        }
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
        float dashDirection = facingRight ? 1f : -1f;
        // dash forward based on where theyre facing for however long dash duration lasts
        rb.linearVelocity = new Vector2(dashSpeed * dashDirection, rb.linearVelocity.y);
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isDashing = false;
        trailRenderer.emitting = false;
        //wait for the end of dashCooldown till dash is possible
        yield return new WaitForSeconds(dashCooldowon);

    }
    void IdleAnim()
    {
        time += Time.deltaTime * Mathf.PI * idleSpeed;
        float fluct = defaultScale.y + Mathf.Sin(time) * idleIntensity; // oscillates around original y-scale
        transform.localScale = new Vector3(defaultScale.x, fluct, defaultScale.z);
    }
    //most of this is in the animator under the idle and transition behavior
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            isAttacking = true;
        }

    }
    public void Guard(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            StartCoroutine(Parry());
        }
    }
    IEnumerator Parry()
    {
        isParrying = true;
        yield return new WaitForSeconds(parryWindow);
        isParrying = false;
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
//when key pushed decrement amount of jumps left and on last one do a flip
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                jumpsRemaining--;
                if (jumpsRemaining == 0)
                {
                    StartCoroutine(CoolFlip());
                }
            }
// if the player lets go stop the jump. It gives more control over the jump and can do short quick bunny hops
            else if (context.canceled)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
            }
        }
    }
    IEnumerator CoolFlip()
    {
        isWalking = false;
        // a is the incrementor for the flip and flipDirection is added until a full circle is made
        float a = 0.0f;
        float flipDireciton = facingRight ? -15.0f : 15.0f;
        while (Mathf.Abs(a) <= 345) //Im not wasting memory on a var thats just the degrees of a full circle deal with the magic number
        {
            a += flipDireciton;
            flipping = true;
            transform.eulerAngles = new Vector3(0f, 0f, a);
            yield return new WaitForFixedUpdate();
        }
        flipping = false;
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
