using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform subject;

    Vector2 startPosition;
    float startZ;

    [Range(0f, 1f)]
    public float parallaxEffectMultiplier = 0.5f;

    Vector2 travel => (Vector2)cam.transform.position - startPosition;

    void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    void Update()
    {
        Vector2 newPos = startPosition + travel * parallaxEffectMultiplier;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);
    }
}
