using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;

    public Vector3 minValues, maxValues;

    public Transform target;

    private Vector3 vel = Vector3.zero;

    private void LateUpdate()
    {
        Follow();
    }

    void Follow()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        targetPosition.z = transform.position.z;

        Vector3 boundPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, minValues.x, maxValues.x),
            Mathf.Clamp(targetPosition.y, minValues.y, maxValues.y),
            Mathf.Clamp(targetPosition.z, minValues.z, maxValues.z));

        transform.position = Vector3.SmoothDamp(transform.position, boundPosition, ref vel, damping);
    }
}
