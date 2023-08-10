using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [HideInInspector] public Vector3 Position = Vector3.zero;
    [HideInInspector] public Vector3 Velocity = Vector3.zero;
    [HideInInspector] public Vector3 Acceleration = Vector3.zero;
    [HideInInspector] public Vector3 Checkpoint = Vector3.zero;
    public float NeighborhoodRadius = 3;
    public float SeparationRadius = 2;
    public float EvasionMultiplier = 1;
    public float CohesionMultiplier = 1;
    public float SeparationMultiplier = 1;
    public float LeaderInfluenceMultiplier = 1;
    public float SpeedMultiplier = 3;
    public bool isLeader = false;
    public Animator Animator;

    [SerializeField]public Stats stats;


    public LayerMask LayerMask;

    [SerializeField] private GameObject SelectionObj;

    public EntityListObj flock;
    public bool ShowGizmos;
    private Vector3 averageFlockPosition;
    [SerializeField] private CapsuleCollider collider;

    private void Start()
    {
        stats = GetComponent<Stats>();
        flock.Add(this);
        Checkpoint = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        Position = transform.position;
        ManageMovement();
        CheckCheckPoint();
    }

    private void CheckCheckPoint()
    {
        if (GetNeighbours().Count(x => x.isLeader) > 0)
        {
            var entity = GetNeighbours().Where(x => x.isLeader).ToList();
            if (entity[0].Checkpoint.magnitude > 0)
            {
                Checkpoint = entity[0].Checkpoint;
            }
        }

        if ((Position - Checkpoint).magnitude < SeparationRadius)
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
        if (separationVector.magnitude > 0)
        {
            Velocity += separationVector * SeparationMultiplier;
        }

        if (averageVector.magnitude > 0)
        {
            /*if (!isLeader)
            {
                averageVector = (flock.Leader.Position - Position);
            }*/

            Velocity += averageVector * CohesionMultiplier;
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

        if (Velocity.magnitude > 0.01f)
        {
            Animator.SetBool("Walking", true);
            transform.position += Velocity * (Time.deltaTime * SpeedMultiplier);
            transform.rotation = Quaternion.FromToRotation(transform.forward, transform.forward + Velocity);
        }
        else
        {
            Animator.SetBool("Walking", false);
        }
    }

    List<Entity> GetNeighbours()
    {
        return flock.entities.Where(x => (x.Position - Position).magnitude <= NeighborhoodRadius).ToList();
    }

    Vector3 CheckNeighbourhood()
    {
        int i = 0;
        Vector3 vectors = Vector3.zero;
        List<Entity> neighbours = flock.entities.Where(x => (x.Position - Position).magnitude <= NeighborhoodRadius)
            .ToList();
        if (neighbours.Count == 0)
        {
            return vectors;
        }

        foreach (var entity in neighbours.Where(entity => entity.Velocity.magnitude > 0))
        {
            i++;
            Vector3 colliderPosition = entity.Position;
            float vectorMagnitude = (Position - colliderPosition).magnitude;
            vectors += (Position - colliderPosition).normalized * ((1 / vectorMagnitude) * EvasionMultiplier);
        }

        if (i == 0)
        {
            return vectors;
        }

        vectors /= i;
        return vectors;
    }

    private Vector3 LeaderFollowing(Vector3 cohesionVector)
    {
        // You can adjust the weight of cohesion vs. leader's position
        float cohesionWeight = 0.7f; // Adjust as needed
        Vector3 leaderDirection = (flock.Leader.transform.position - transform.position).normalized;
        Vector3 leaderInfluence = cohesionWeight * cohesionVector + (1 - cohesionWeight) * leaderDirection;

        return leaderInfluence;
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

        return averageFlockPosition;

        /*/*#1#// You can adjust the weight of cohesion vs. leader's position
        
        float cohesionWeight = 0.5f; // Adjust as needed
        Vector3 leaderDirection = Vector3.zero;
        foreach (var entity in flock.entities.Where(x => x.isLeader == true))
        {
            leaderDirection = (entity.transform.position - transform.position).normalized;
        }
        Vector3 leaderInfluence = cohesionWeight * centerVector + (1 - cohesionWeight) * leaderDirection;
        
        return leaderInfluence;*/
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
        bool containsLeader = false;
        int i = 0;
        Vector3 vectors = Vector3.zero;
        List<Entity> neighbours = flock.entities.Where(x => (x.Position - Position).magnitude <= NeighborhoodRadius)
            .ToList();
        if (neighbours.Count == 0)
        {
            return vectors;
        }

        foreach (var entity in neighbours.Where(entity => entity.Velocity.magnitude > 0))
        {
            if (entity.isLeader)
            {
                containsLeader = true;
            }

            i++;
            vectors += entity.Velocity;
        }

        if (i == 0)
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
        if (ShowGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Position, NeighborhoodRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Position, SeparationRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Position, Position + Velocity);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(Position, Position + (averageFlockPosition - Position));
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(averageFlockPosition, 0.25f);
        }
    }
}