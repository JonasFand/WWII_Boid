using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 1f;
    public float neighborRadius = 2f;

    private BoidManager boidManager;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        boidManager = transform.parent.GetComponent<BoidManager>();
    }

    private void Update()
    {
        Vector3 separationVector = Separation();
        Vector3 alignmentVector = Alignment();
        Vector3 cohesionVector = Cohesion();

        Vector3 combinedVector = (separationVector + alignmentVector + cohesionVector).normalized;
        Debug.Log(combinedVector);
        MoveBoid(combinedVector);
    }

    private Vector3 Separation()
    {
        Vector3 separationVector = Vector3.zero;
        int count = 0;

        foreach (var otherBoid in boidManager.Boids)
        {
            if (otherBoid != this)
            {
                float distance = Vector3.Distance(transform.position, otherBoid.transform.position);
                if (distance < neighborRadius)
                {
                    separationVector += (transform.position - otherBoid.transform.position);
                    count++;
                }
            }
        }

        if (count > 0)
            separationVector /= count;

        return separationVector;
    }

    private Vector3 Alignment()
    {
        Vector3 alignmentVector = Vector3.zero;
        int count = 0;

        foreach (var otherBoid in boidManager.Boids)
        {
            if (otherBoid != this)
            {
                float distance = Vector3.Distance(transform.position, otherBoid.transform.position);
                if (distance < neighborRadius)
                {
                    alignmentVector += otherBoid.transform.up;
                    count++;
                }
            }
        }

        if (count > 0)
            alignmentVector /= count;

        return alignmentVector;
    }

    private Vector3 Cohesion()
    {
        Vector3 cohesionVector = Vector3.zero;
        int count = 0;

        foreach (var otherBoid in boidManager.Boids)
        {
            if (otherBoid != this)
            {
                float distance = Vector3.Distance(transform.position, otherBoid.transform.position);
                if (distance < neighborRadius)
                {
                    cohesionVector += otherBoid.transform.position;
                    count++;
                }
            }
        }

        if (count > 0)
            cohesionVector /= count;

        return (cohesionVector - transform.position).normalized;
    }

    private void MoveBoid(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.position += transform.up * speed;
    }
}
