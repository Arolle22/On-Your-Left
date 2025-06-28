using System.Collections;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    public float force = 5f;
    private float timer = 0f;

    private bool isReflected = false;

    private int originalLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalLayer = gameObject.layer; 
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * force;
        }
        else
        {
            Debug.LogWarning("Player not found!");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 6f)
        {
            Destroy(gameObject);
        }
    }

    public void Reflect(Vector2 newDirection)
    {
        isReflected = true;

        rb.linearVelocity = newDirection.normalized * force;

        int playerBulletLayer = LayerMask.NameToLayer("Player");

        if (playerBulletLayer != -1 && playerBulletLayer <= 31)
        {
            gameObject.layer = playerBulletLayer;
        }
        else
        {
            gameObject.layer = originalLayer;
        }

        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isReflected)
        {
            if (other.CompareTag("Player") || other.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Ground"))
            {
                if (other.CompareTag("Enemy"))
                {
                    Enemy enemy = other.GetComponentInParent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(3);
                    }
                }

                Destroy(gameObject);
            }
        }
    }

    public void ResetBullet()
    {
        rb.linearVelocity = Vector2.zero;
        gameObject.layer = originalLayer;
        transform.position = Vector3.zero;
        gameObject.SetActive(true);
    }
}
