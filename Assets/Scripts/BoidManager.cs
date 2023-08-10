using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public GameObject boidPrefab;
    public int numBoids = 50;

    public float spawnRadius = 10f;
    public float boundsRadius = 20f;

    public Boid[] Boids { get; private set; }

    private void Start()
    {
        Boids = new Boid[numBoids];

        for (int i = 0; i < numBoids; i++)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            GameObject boid = Instantiate(boidPrefab, randomPos, Quaternion.identity, transform);
            Boids[i] = boid.GetComponent<Boid>();
        }
    }

    private void Update()
    {
        foreach (var boid in Boids)
        {
            KeepBoidInBounds(boid);
        }
    }

    private void KeepBoidInBounds(Boid boid)
    {
        Vector3 newPos = boid.transform.position;
        newPos.x = Mathf.Clamp(newPos.x, -boundsRadius, boundsRadius);
        newPos.y = Mathf.Clamp(newPos.y, -boundsRadius, boundsRadius);
        boid.transform.position = newPos;
    }
}
