using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Movement : MonoBehaviour
{
    [Range(0,5)]
    public float Speed = 30;
    public Vector3 Direction;

    private Vector3 newDirection;
    public Vector3 NewDirection
    {
        get => newDirection;
        set
        {
            newDirection = value;
            ApplyNewDirection();
        }
        
    }

    public Vector3 ForceVector = Vector3.zero;
    public Vector3 UnitEvasionVector = Vector3.zero;
    public Vector3 CheckPointVector = Vector3.zero;
    public Vector3 CheckPoint = Vector3.zero;
    public float EvasionRadius;
    private BoxCollider2D region;
    //[SerializeField]private CircleCollider2D evasionCollider;
    [SerializeField]public CapsuleCollider collider;
    [SerializeField]private float timer = 0;
    Quaternion startRotation;
    Quaternion newRotation;
    private bool colliderExit;
    [SerializeField]private GameObject SelectionObj;

    private void Awake()
    {
        //region = GameObject.Find("Region").GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        newDirection = RandomVector();
        ApplyNewDirection();
        float angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
        //newRotation = Quaternion.AngleAxis(angle, Vector3.right);
    }

    

    void Update()
    {
        //transform.position += transform.forward * (Speed * Time.deltaTime);
        transform.position += newDirection * (Speed * Time.deltaTime);
        ManagerSteering();
    }

    void ManagerSteering()
    {
        if (timer < 1)
        {
            //transform.rotation = Quaternion.Lerp(startRotation, newRotation, timer);
        }
        else
        {
            
        }

        timer += Time.deltaTime;
    }

    void ApplyNewDirection()
    {
        Direction = transform.forward;
        newDirection = (ForceVector+newDirection);
        if (UnitEvasionVector.magnitude>0)
        {
            newDirection = (newDirection-UnitEvasionVector);
        }
        if (CheckPointVector.magnitude>0)
        {
            newDirection = (newDirection+CheckPointVector);
        }


        newDirection.y = 0;
        newDirection.Normalize();
        /*startRotation = transform.rotation;
        float angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
        newRotation = Quaternion.AngleAxis(angle, Vector3.up);*/
        timer = 0;
    }

    Vector3 RandomVector()
    {
        return Vector3.zero;
        return new Vector3(Random.Range(-1f, 1f), 0,Random.Range(-1f, 1f)).normalized;
    }
    public void IsSelected(bool selected)
    {
        SelectionObj.SetActive(selected);
    }
    private void OnDrawGizmos()
    {
        
        //Gizmos.DrawWireCube(region.transform.position,region.size);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position+(Vector3)newDirection.normalized);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position+transform.forward.normalized);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position+(Vector3)ForceVector);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position-(Vector3)UnitEvasionVector);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position+(Vector3)CheckPointVector);
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        Gizmos.DrawWireSphere(transform.position,EvasionRadius);
    }
}
