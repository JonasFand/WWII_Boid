using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [HideInInspector]public Vector3 Position = Vector3.zero;
    [HideInInspector]public Vector3 Velocity = Vector3.zero;
    [HideInInspector]public Vector3 Acceleration = Vector3.zero;
    [HideInInspector]public Vector3 Checkpoint = Vector3.zero;
    public float NeighborhoodRadius = 3;
    public float SeparationRadius = 2;
    public float EvasionMultiplier = 1;
    public float CohesionMultiplier = 1;
    public float SeparationMultiplier = 1;
    
    
    public LayerMask LayerMask;
    
    [SerializeField]private GameObject SelectionObj;

    public EntityListObj flock;
    private Vector3 averageFlockPosition;
    [SerializeField] private CapsuleCollider collider;

    private void Start()
    {
        flock.entities.Add(this);
        Checkpoint = new Vector3(10, 0, 5);
    }

    private void Update()
    {
        Position = transform.position;
        ManageMovement();
        CheckCheckPoint();

    }

    private void CheckCheckPoint()
    {
        if ((Position-Checkpoint).magnitude<SeparationRadius)
        {
            Checkpoint = Vector3.zero;
        }
    }

    private void ManageMovement()
    {
        Velocity = Vector3.zero;
        Vector3 separationVector = CheckNeighbourhood();
        Vector3 averageVector = (FindFlockCenter() - Position).normalized;
        Acceleration = AlignVelocity();
        Vector3 checkPointVector = (Checkpoint - Position).normalized;
        if (separationVector.magnitude>0)
        {
            Velocity += separationVector*SeparationMultiplier;
        }
        if (averageVector.magnitude>0)
        {
            Velocity += averageVector*CohesionMultiplier;
        }
        if (Acceleration.magnitude > 0 && Checkpoint != Vector3.zero) 
        {
            Velocity += Acceleration;
        }
        if (checkPointVector.magnitude > 0 && Checkpoint != Vector3.zero) 
        {
            Velocity += checkPointVector.normalized;
        }
        //Velocity = separationVector + averageVector + Acceleration + checkPointVector;
        Velocity.y = 0;
        
        if (Velocity.magnitude > 0) 
        {
            transform.position += Velocity * Time.deltaTime;
        }
    }



    Vector3 CheckNeighbourhood()
    {
        int i = 0;
        Vector3 vectors = Vector3.zero;
        List<Entity> neighbours = flock.entities.Where(x => (x.Position - Position).magnitude <= NeighborhoodRadius).ToList();
        if (neighbours.Count == 0) 
        {
            return vectors;
        }
        foreach (var entity in neighbours.Where(entity => entity.Velocity.magnitude>0))
        {
            i++;
            Vector3 colliderPosition = entity.Position;
            float vectorMagnitude = (Position - colliderPosition).magnitude;
            vectors += (Position - colliderPosition ).normalized * ((1/vectorMagnitude) * EvasionMultiplier);
        }

        if (i==0)
        {
            return vectors;
        }

        vectors /= i;
        return vectors;
    }

    Vector3 FindFlockCenter()
    {
        Vector3 centerVector = Vector3.zero;
        HashSet<Entity> tempSet = new HashSet<Entity>();
        tempSet = FlockHelper(new HashSet<Entity>());
        foreach (var entity in tempSet)
        {
            centerVector += entity.Position;
        }
        
        centerVector /= tempSet.Count;
        averageFlockPosition = centerVector;
        return centerVector;
    }

    public HashSet<Entity> FlockHelper(HashSet<Entity> CheckedEntities)
    {
        HashSet<Entity> tempSet = CheckedEntities;
        HashSet<Entity> tempSet2 = new HashSet<Entity>();
        foreach (var entity in flock.entities.Where(x => (x.Position - Position).magnitude <= NeighborhoodRadius))
        {
            if (entity && !tempSet.Contains(entity))
            {
                tempSet.Add(entity);
                entity.FlockHelper(tempSet);
            }
        }
        return tempSet;
    }

    Vector3 AlignVelocity()
    {
        
        int i = 0;
        Vector3 vectors = Vector3.zero;
        List<Entity> neighbours = flock.entities.Where(x => (x.Position - Position).magnitude <= NeighborhoodRadius).ToList();
        if (neighbours.Count==0)
        {
            return vectors;
        }
        foreach (var entity in neighbours.Where(entity => entity.Velocity.magnitude>0))
        {
            i++;
            vectors += entity.Velocity;
        }

        if (i==0)
        {
            return vectors;
        }
        vectors /= i;
        return vectors.normalized;
    }
    public void IsSelected(bool selected)
    {
        SelectionObj.SetActive(selected);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Position,NeighborhoodRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Position,SeparationRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Position,Position+Velocity);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(Position,Position+(averageFlockPosition - Position));
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(averageFlockPosition,0.25f);
    }
}
