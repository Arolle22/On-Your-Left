using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;

    public float shootInterval = 2f;
    public float detectionRange = 6f;

    private float timer;
    private GameObject player;

    public AudioSource audioSource;

    public AudioClip bulletSFX;
    
    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        FindPlayer();
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < detectionRange)
        {
            timer += Time.deltaTime;
            if (timer >= shootInterval)
            {
                timer = 0f;
                Shoot();
            }
        }
    }

    void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Shoot()
    {
        Instantiate(bullet, bulletPos.position, Quaternion.identity);
        if (bulletSFX != null)
                audioSource.PlayOneShot(bulletSFX);

    }
}
