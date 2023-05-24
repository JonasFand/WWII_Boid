using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class SwarmManager : MonoBehaviour
{
    public static SwarmManager Instance { get; private set; }
   
    public Vector2Int Amount = new Vector2Int(2,5);
    public GameObject PrefabToSpawn;
    public Transform SpawnPoint;
    public float DistanceBetweenEntitys = 0.2f;
    public List<Movement> EntityList;
    public BoxCollider2D Region;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    
    void Start()
    {
        EntityList = new List<Movement>();
        foreach (var GA in GameObject.FindGameObjectsWithTag("entity"))
        {
            EntityList.Add(GA.GetComponent<Movement>());
        }
        
        //Spawn();
    }
    
    void Update()
    {
        
    }

    void Spawn()
    {
        Vector3 modifier = new Vector3((Amount.x-1)/2*DistanceBetweenEntitys, (Amount.y-1)/2*DistanceBetweenEntitys, 0);
        Vector3 pos = SpawnPoint.position;
        for (int i = 0; i < Amount.y; i++)
        {
            pos.y = SpawnPoint.position.y + i * DistanceBetweenEntitys;
            pos.y -= modifier.y;
            for (int j = 0; j < Amount.x; j++)
            {
                pos.x = SpawnPoint.position.x + j * DistanceBetweenEntitys;
                pos.x -= modifier.x;
                EntityList.Add(Instantiate(PrefabToSpawn, pos, Quaternion.Euler(0, 0, Random.Range(0, 360))).GetComponent<Movement>());
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 modifier = new Vector3((Amount.x-1)/2*DistanceBetweenEntitys, (Amount.y-1)/2*DistanceBetweenEntitys, 0);
        Vector3 pos = SpawnPoint.position-modifier;
        for (int i = 0; i < Amount.y; i++)
        {
            pos.y = SpawnPoint.position.y + i * DistanceBetweenEntitys;
            pos.y -= modifier.y;
            for (int j = 0; j < Amount.x; j++)
            {
                pos.x = SpawnPoint.position.x + j * DistanceBetweenEntitys;
                pos.x -= modifier.x;
                Gizmos.DrawCube(pos,new Vector3(0.5f,0.5f,0.5f));
            }
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Region.transform.position,Region.size);

    }*/
}
