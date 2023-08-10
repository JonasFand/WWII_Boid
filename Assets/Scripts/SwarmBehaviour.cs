using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SwarmBehaviour : MonoBehaviour
{
    private BoxCollider2D Region;
    public Vector3 CheckPoint = Vector3.zero;
    public LayerMask LayerMask;
    public float EvasionMultiplier = 1;
    public float CheckPointMultiplier = 1;
    public float ForceDivider = 10;
    private float timer = 0;
    private void Start()
    {
        
    }

    private void Update()
    {
        if (timer > 0.01f)
        {
            timer = 0;
            foreach (var entity in SwarmManager.Instance.EntityList)
            {
                ChangeDirection(entity);
            }
        }
        
        if (CheckPoint.magnitude>0)
        {
            TowardsCheckPoint();
        }
        TowardsCheckPoint();
        timer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        foreach (var entity in SwarmManager.Instance.EntityList)
        {
            //CheckRegion(entity.collider.Distance(Region).distance,entity);
        }

        foreach (var entity in SwarmManager.Instance.EntityList)
        {
            //CheckEvasion(Physics2D.OverlapCircleAll(entity.transform.position, entity.EvasionRadius, LayerMask).ToList(), entity);
            CheckEvasion(Physics.OverlapSphere(entity.transform.position, entity.EvasionRadius, LayerMask).ToList(), entity);
        }
    }

    private void TowardsCheckPoint()
    {
        foreach (var entity in SwarmManager.Instance.EntityList)
        {
            if (entity.CheckPoint.magnitude>0)
            {
                var tempVector = entity.CheckPoint - entity.transform.position;
                entity.CheckPointVector = tempVector.normalized * CheckPointMultiplier;
            }
        }
    }

    public void SetCheckPoint(Vector3 newCheckPoint)
    {
        CheckPoint = newCheckPoint;
    }

    private void CheckEvasion(List<Collider> colliders, Movement entity)
    {
        Debug.Log(colliders.Count);
        Vector3 tempVector = Vector3.zero;
        int i = 0;
        if (colliders.Count == 0)
        {
            entity.UnitEvasionVector = Vector2.zero;
        }
        else
        {
            foreach (var col in colliders.Where(collider => collider != entity.collider))
            {
                i++;
                //tempVector += (Vector2)collider.transform.position - (Vector2)entity.transform.position;
                tempVector += (col.transform.position - entity.transform.position).normalized * (1/(Vector3.Distance(col.transform.position,entity.transform.position)) * EvasionMultiplier);
            }
            tempVector /= i;
            entity.UnitEvasionVector = tempVector;
        }
    }

    private void CheckRegion(float distance, Movement entity)
    {
        if (distance > 0)
        {
            entity.ForceVector = (Region.transform.position - entity.transform.position.normalized);
            entity.ForceVector *= distance / ForceDivider;
        }
        else
        {
            entity.ForceVector = Vector2.zero;
        }
    }

    static void ChangeDirection(Movement entity)
    {
        Vector3 tempVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized +
                             entity.NewDirection;
        entity.NewDirection = tempVector.normalized;
        entity.NewDirection = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (CheckPoint.magnitude>0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(CheckPoint,2f);
        }
        
    }
}
