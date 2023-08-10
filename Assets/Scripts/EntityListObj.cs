using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu][InitializeOnLoad]
public class EntityListObj : ScriptableObject
{
    public List<Entity> entities = new List<Entity>();
    
    public Entity Leader = null;

    private void Awake()
    {
        entities.Clear();
    }

    private void OnEnable()
    {
        entities.Clear();
    }

    public void Add(Entity entity)
    {
        entities.Add(entity);
        if (entity.isLeader)
        {
            Leader = entity;
        }
    }
}
