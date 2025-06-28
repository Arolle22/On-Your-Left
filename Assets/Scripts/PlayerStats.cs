using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 7;
    public int currentHealth;

    public int maxLives;
    public int currentLives;

    public int coinCount = 0;

    public bool hasDied = false;
    private bool victoryTriggered = false;
    private bool animationPlayedOnGround = false;
    private bool victorySequenceStarted = false;


    public HealthBar healthBar;
    public TMP_Text lives;
    public TMP_Text coinAmount;
    public TMP_Text lifePanelText;
    public Animator animator;
    public PlayerMovement playerMovement;

    public SpriteRenderer spriteRenderer;
    private Coroutine damageFlashCoroutine;
    public float flashDuration = 0.1f;
    public int flashCount = 5;

    public AudioSource audioSource;
    public AudioSource gameOverAudioSource;
    public AudioSource mainMusicSource;
    public GameObject gameOverPanel;
    public GameObject MusicPlayer;
    public GameObject LivesPanel;

    public AudioClip hurtSFX;
    public AudioClip coinSFX;
    public AudioClip lifeSFX;
    public AudioClip loseSFX;
    public AudioClip gameOverSFX;
    public AudioClip victorySFX;

    private bool canTakeDamage = true;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        currentLives = LevelManager.instance.currentLives;
        coinCount = LevelManager.instance.coinCount;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
            Debug.LogWarning("SpriteRenderer not found on Player or its children.");

        if (healthBar == null)
            healthBar = Object.FindFirstObjectByType<HealthBar>();

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
        else
            Debug.LogWarning("HealthBar not assigned.");

        UpdateLivesText();
        UpdateCoinText();

        playerMovement = GetComponent<PlayerMovement>();
        if (mainMusicSource == null)
        {
            mainMusicSource = MusicPlayer.GetComponent<AudioSource>();
            if (mainMusicSource == null)
                Debug.LogWarning("Main music source not found!");
        }

        if (gameOverAudioSource == null)
        {
            gameOverAudioSource = gameOverPanel.GetComponent<AudioSource>();
            if (gameOverAudioSource == null)
                Debug.LogWarning("GameOver music source not found!");
        }
        if (LivesPanel != null && lifePanelText == null)
        {
            Transform textTransform = LivesPanel.transform.Find("Heart/Life");
            if (textTransform != null)
            {
                lifePanelText = textTransform.GetComponent<TMP_Text>();
                if (lifePanelText == null)
                {
                    Debug.LogWarning("Life text component not found on LivesPanel.Heart.Life");
                }
            }
            else
            {
                Debug.LogWarning("Path 'Heart/Life' not found under LivesPanel");
            }
        }

    }

    void TakeDamage(int damage)
    {
        if (!canTakeDamage) return;
        canTakeDamage = false;

        currentHealth -= damage;
        damageFlashCoroutine = StartCoroutine(FlashOnHit());

        if (currentHealth <= 0)
        {
            LoseLife();
        }
        else
        {
            healthBar.SetHealth(currentHealth);
        }

        StartCoroutine(DamageCooldown());
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(flashDuration * flashCount);
        canTakeDamage = true;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    public void UpdateLivesText()
    {
        if (lives != null)
            lives.text = currentLives + "/" + maxLives;
    }

    public void LoseLife()
    {
        currentLives = Mathf.Max(0, currentLives - 1);
        LevelManager.instance.currentLives = currentLives;

        if (loseSFX != null)
            audioSource.PlayOneShot(loseSFX);

        UpdateLivesText();

        if (currentLives > 0)
        {
            if (LivesPanel != null)
            {
                Debug.Log("Activating Lives panel");
                LivesPanel.SetActive(true);
                lifePanelText.text = $"{currentLives}/{maxLives}";

            }
            StartCoroutine(WaitThenRespawn());
        }
        else
        {
            Debug.Log("Game Over");
            StartCoroutine(PlayGameOverThenLoadScene());
        }
    }


    private IEnumerator WaitThenRespawn()
    {
        LevelManager.instance.Respawn();

        yield return new WaitForSeconds(2f);
        if (LivesPanel != null)
        {
            LivesPanel.SetActive(false);
        }

        Destroy(gameObject);
    }



    public void AddLife()
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            LevelManager.instance.currentLives = currentLives;
            UpdateLivesText();
        }
    }

    public void UpdateCoinText()
    {
        if (coinAmount != null)
            coinAmount.text = coinCount.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDied) return;

        if (other.CompareTag("Pit"))
        {
            hasDied = true;
            LoseLife();
            UpdateLivesText();
        }
        else if (other.CompareTag("Coin"))
        {
            if (!other.gameObject.activeInHierarchy) return;

            coinCount += 1;
            if (coinSFX != null)
                audioSource.PlayOneShot(coinSFX);

            LevelManager.instance.coinCount = coinCount;
            UpdateCoinText();

            if (coinCount >= 100)
            {
                coinCount = 0;
                LevelManager.instance.coinCount = coinCount;

                if (currentLives < LevelManager.instance.maxLives)
                {
                    currentLives++;
                    if (lifeSFX != null)
                        audioSource.PlayOneShot(lifeSFX);

                    LevelManager.instance.currentLives = currentLives;
                    UpdateLivesText();
                }
            }

            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
        else if (other.CompareTag("Bullet"))
        {
            TakeDamage(2);
        }
        else if (other.CompareTag("Collect"))
        {
            if (!other.gameObject.activeInHierarchy) return;

            Debug.Log("Victory!");

            other.gameObject.SetActive(false);
            Destroy(other.gameObject);

            victoryTriggered = true;
            animationPlayedOnGround = false;

            if (playerMovement != null)
            {
                playerMovement.SetControl(false);
                playerMovement.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }
        }
    }

    private IEnumerator FlashOnHit()
    {
        for (int i = 0; i < flashCount; i++)
        {
            if (spriteRenderer != null)
            {
                if (hurtSFX != null)
                    audioSource.PlayOneShot(hurtSFX);

                spriteRenderer.color = new Color(1f, 1f, 1f, 0.2f);
                yield return new WaitForSeconds(flashDuration);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(flashDuration);
            }
        }
    }

    private void Update()
    {
        if (victoryTriggered && !animationPlayedOnGround && playerMovement.grounded)
        {
            if (Mathf.Abs(playerMovement.GetComponent<Rigidbody2D>().linearVelocity.y) < 0.1f)
            {
                playerMovement.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
                animator.SetFloat("xVelocity", 0);
                animator.SetFloat("yVelocity", 0);
                animator.SetTrigger("Victory");
                animationPlayedOnGround = true;

                if (victorySFX != null && !victorySequenceStarted)
                {
                    victorySequenceStarted = true;
                    PlayVictoryAndEndLevel();
                }
            }
        }
    }


    public void EndLevel()
    {
        SceneManager.LoadSceneAsync(0);
    }

    private IEnumerator PlayGameOverThenLoadScene()
    {
        Debug.Log("GameOver sequence started");

        if (mainMusicSource != null && mainMusicSource.isPlaying)
        {
            mainMusicSource.Pause();
        }


        if (gameOverPanel != null)
        {
            Debug.Log("Activating Game Over panel");
            gameOverPanel.SetActive(true);

            if (gameOverAudioSource != null && gameOverSFX != null)
            {
                gameOverAudioSource.clip = gameOverSFX;
                gameOverAudioSource.loop = false;
                gameOverAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("GameOver audio source or clip not assigned!");
            }

            yield return new WaitForSeconds(3f);
        }
        else
        {
            Debug.LogWarning("GameOver panel not assigned!");
        }

        EndLevel();
    }

    private void PlayVictoryAndEndLevel()
    {
        if (mainMusicSource != null && mainMusicSource.isPlaying)
        {
            mainMusicSource.Pause();
        }

        audioSource.PlayOneShot(victorySFX);
        StartCoroutine(VictoryDelayThenEnd());
    }
    private IEnumerator VictoryDelayThenEnd()
    {
        yield return new WaitForSeconds(4f);
        EndLevel();
    }
}
