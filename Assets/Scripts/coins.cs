using UnityEngine;

public class coins : MonoBehaviour
{
    [SerializeField] private int vaule;
    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            Destroy(gameObject);
        }
    }
}
