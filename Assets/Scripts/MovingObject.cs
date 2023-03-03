using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private float movementTime = 1;
    [SerializeField] private Vector2 translation;
    [SerializeField] private AnimationCurve movementCurve;
    private Vector2 startPosition;
    private float time = 0;

    private void Start()
    {
        startPosition = transform.position;   
    }
    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        if (time > movementTime)
        {
            time = 0;
        }
        transform.position = Vector3.Lerp(startPosition, startPosition + translation, movementCurve.Evaluate(time / movementTime));   
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)translation);
    }
}
