using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D body;
    public BoxCollider2D groundCheck;
    [SerializeField] Collider2D standingCollider;
    [SerializeField] Transform overheadCheckCollider;
    public LayerMask groundMask;
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public LayerMask bulletLayers;


    public float acceleration;
    public float MaxXSpeed;
    [Range(0f, 1f)]
    public float groundDecay = 0.85f;
    public float jumpSpeed;

    const float overheadCheckRadius = 0.2f;
    bool isCrouched;
    public bool grounded;
    float xInput;
    float yInput;

    public int attackDamage = 1;
    public float attackRange = 0.5f;
    float slashCooldown = 0.5f;
    float slashTimer = 0f;

    public bool canControl = true;

    public AudioSource audioSource;

    public AudioClip jumpSFX;
    public AudioClip slashSFX;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (canControl)
        {
            CheckInput();
            HandleVertical();
            slashTimer += Time.deltaTime;
            HandleSlash();
        }

        animator.SetFloat("yVelocity", body.linearVelocity.y);
    }


    void FixedUpdate()
    {
        CheckGround(); 

        if (canControl)
        {
            MoveWithInput();
        }

        ApplyFriction();
    }


    void CheckInput()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
    }

    void MoveWithInput()
    {
        if (Mathf.Abs(xInput) > 0)
        {
            if (!isCrouched)
            {
                float increment = xInput * acceleration;
                float newSpeed = Mathf.Clamp(body.linearVelocity.x + increment, -MaxXSpeed, MaxXSpeed);
                body.linearVelocity = new Vector2(newSpeed, body.linearVelocity.y);

                animator.SetFloat("xVelocity", Mathf.Abs(body.linearVelocity.x));
            }
            else
            {
                float increment = xInput * acceleration;
                float newSpeed = Mathf.Clamp(body.linearVelocity.x + increment, -MaxXSpeed, MaxXSpeed);
                body.linearVelocity = new Vector2(newSpeed / 2, body.linearVelocity.y);

                animator.SetFloat("xVelocity", Mathf.Abs(body.linearVelocity.x));
            }
            FaceInput();
        }
    }

    void FaceInput()
    {
        if (xInput != 0)
        {
            float direction = Mathf.Sign(xInput);
            transform.localScale = new Vector3(direction, 1, 1);
        }
    }

    void HandleVertical()
    {
        if (grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpSpeed);
                animator.SetBool("isJumping", true);
                if (jumpSFX != null)
                    audioSource.PlayOneShot(jumpSFX);
            }

            if (Input.GetButtonDown("Crouch"))
            {
                standingCollider.enabled = false;
                isCrouched = true;
                animator.SetBool("Crouch", true);
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                if (!Physics2D.OverlapCircle(overheadCheckCollider.position, overheadCheckRadius, groundMask))
                {
                    standingCollider.enabled = true;
                    isCrouched = false;
                    animator.SetBool("Crouch", false);
                }
                else
                {
                    isCrouched = true;
                    standingCollider.enabled = false;
                    animator.SetBool("Crouch", true);
                }
            }
        }
    }

    void HandleSlash()
    {
        if (Input.GetButtonDown("Slash") && slashTimer >= slashCooldown)
        {
            animator.SetTrigger("Slash");
            if (slashSFX != null)
                audioSource.PlayOneShot(slashSFX);
            slashTimer = 0f;
        }
    }

    public void PerformSlash()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy enemy = enemyCollider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }

        Collider2D[] hitBullets = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, bulletLayers);
        foreach (Collider2D bulletCollider in hitBullets)
        {
            Rigidbody2D rb = bulletCollider.attachedRigidbody;
            if (rb != null)
            {
                Vector2 reflectDir = (bulletCollider.transform.position - transform.position).normalized;
                rb.linearVelocity = reflectDir * rb.linearVelocity.magnitude;

                EnemyBullet bullet = bulletCollider.GetComponent<EnemyBullet>();
                if (bullet != null)
                {
                    bullet.Reflect(reflectDir);
                }

                Debug.Log("Reflected bullet: " + bulletCollider.name);
            }
            else
            {
                Debug.LogWarning("Rigidbody2D not found for bullet: " + bulletCollider.name);
            }
        }
    }



    private void OnDrawGizmos()
    {
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void CheckGround()
    {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
        animator.SetBool("isJumping", !grounded);
    }

    void ApplyFriction()
    {
        if (grounded && Mathf.Abs(xInput) < 0.01f)
        {
            Vector2 velocity = body.linearVelocity;

            velocity.x = Mathf.Lerp(velocity.x, 0, 1 - groundDecay);

            if (Mathf.Abs(velocity.x) < 0.05f)
            {
                velocity.x = 0;
            }

            body.linearVelocity = velocity;

            animator.SetFloat("xVelocity", Mathf.Abs(velocity.x));
        }
    }



    
    public void SetControl(bool enabled)
    {
        canControl = enabled;
    }
}
