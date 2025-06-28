using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public GameObject PointA;
    public GameObject PointB;
    private Rigidbody2D rb;

    private Transform currentPoint;

    public float speed = 2f;
    public float reachDistance = 0.1f;

    private bool facingLeft = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = PointB.transform;
    }

    void Update()
    {
        Vector2 direction = (currentPoint.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        if ((direction.x > 0 && facingLeft) || (direction.x < 0 && !facingLeft))
        {
            Flip();
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < reachDistance)
        {
            currentPoint = (currentPoint == PointB.transform) ? PointA.transform : PointB.transform;
        }
    }

    void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        if (PointA != null)
            Gizmos.DrawWireSphere(PointA.transform.position, 0.5f);
        if (PointB != null)
            Gizmos.DrawWireSphere(PointB.transform.position, 0.5f);
    }
}
