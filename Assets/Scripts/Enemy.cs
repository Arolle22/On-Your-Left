using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 1;
    private int currentHealth;
    public float flashDuration = 0.1f;
    public int flashCount = 3;

    private bool isDying = false;

    public Animator animator;
    public SpriteRenderer spriteRenderer;

    public AudioSource audioSource;

    public AudioClip explodeSFX;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDying) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashOnHit());
        }
    }



    void Die()
    {
        if (isDying) return;
        isDying = true;

        StopAllCoroutines();

        animator.SetTrigger("Explode");
        
        if (explodeSFX != null)
            audioSource.PlayOneShot(explodeSFX);

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        StartCoroutine(WaitForAnimationAndDestroy());
    }


    private IEnumerator WaitForAnimationAndDestroy()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("Explode"))
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        yield return new WaitForSeconds(stateInfo.length);
        Destroy(gameObject);
    }

    private IEnumerator FlashOnHit()
    {
        if (spriteRenderer == null || isDying) yield break;

        for (int i = 0; i < flashCount; i++)
        {
            if (isDying) yield break;

            spriteRenderer.color = new Color(1f, 1f, 1f, 0.2f);
            yield return new WaitForSeconds(flashDuration);

            if (isDying) yield break;

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
        }
    }

}
