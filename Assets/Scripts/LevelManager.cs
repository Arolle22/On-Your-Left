using System.Collections;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Transform respawnPoint;
    public GameObject playerPrefab;
    public float respawnDelay = 0.5f;

    public int currentLives;
    public int maxLives;
    public int coinCount = 0;

    private bool isRespawning = false;

    public GameObject gameOverPanel;
    public GameObject MusicPlayer;
    public GameObject LivesPanel;

    private void Awake()
    {
        instance = this;
    }

    public void Respawn()
    {
        if (isRespawning) return;

        isRespawning = true;
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);

        PlayerMovement movement = newPlayer.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.SetControl(false);
            StartCoroutine(EnableMovementAfterDelay(movement, 2f));
        }


        SmoothCamera camera = Camera.main.GetComponent<SmoothCamera>();
        if (camera != null)
        {
            camera.target = newPlayer.transform;
        }

        PlayerStats stats = newPlayer.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.gameOverPanel = gameOverPanel;
            stats.MusicPlayer = MusicPlayer;
            stats.LivesPanel = LivesPanel;

            stats.healthBar = FindAnyObjectByType<HealthBar>();
            stats.lives = GameObject.FindWithTag("LivesText")?.GetComponent<TextMeshProUGUI>();
            stats.coinAmount = GameObject.FindWithTag("CoinText")?.GetComponent<TextMeshProUGUI>();
            stats.lifePanelText = GameObject.FindWithTag("LivesScreenText")?.GetComponent<TextMeshProUGUI>();

            stats.currentLives = currentLives;
            stats.coinCount = coinCount;

            stats.mainMusicSource = GameObject.FindWithTag("Music")?.GetComponent<AudioSource>();
            stats.gameOverAudioSource = GameObject.FindWithTag("GameOverMusic")?.GetComponent<AudioSource>();
            stats.ResetHealth();
            stats.UpdateLivesText();
            stats.UpdateCoinText();
            stats.hasDied = false;
        }

        isRespawning = false;
    }
    private IEnumerator EnableMovementAfterDelay(PlayerMovement movement, float delay)
    {
        yield return new WaitForSeconds(delay);
        movement.SetControl(true);
    }

}