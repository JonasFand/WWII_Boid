using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu][InitializeOnLoad]
public class EntityListObj : ScriptableObject
{
    public List<Entity> entities = new List<Entity>();

    private void Awake()
    {
        entities.Clear();
    }

    private void OnEnable()
    {
        entities.Clear();
    }
}
