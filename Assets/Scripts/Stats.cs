using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public enum Allegience
{
    Ally,
    Enemy
}
public class Stats : MonoBehaviour
{
    [SerializeField]private Entity entity;
    [SerializeField]private GameObject bullet;
    [SerializeField]private Transform shootposition;
    [SerializeField]private float health = 10;
    public float Damage = 1;
    public Allegience Allegiance = Allegience.Ally;
    
    public float AttackRadius = 20;

    private List<Entity> enemiesInRange;


    private float time = 0;

    private void Start()
    {
        enemiesInRange = new List<Entity>();
        //entity.GetComponent<Entity>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        
        if (time>1)
        {
            GetEnemies();
            if (enemiesInRange.Count>0)
            {
                Shoot(enemiesInRange[0]);
            }
            time = 0;
        }
    }

    void GetEnemies()
    {
        //enemiesInRange.Clear();
        enemiesInRange = entity.flock.entities.Where(x => (x.Position - entity.Position).magnitude <= AttackRadius).Where(x=>x.stats.Allegiance != Allegiance).ToList();
    }

    public void TakeDamage(float i)
    {
        health -= i;
        if (health<=0)
        {
            entity.flock.entities.Remove(entity);
            Destroy(gameObject);
        }
    }

    void Shoot(Entity target)
    {
        var obj = Instantiate(bullet, shootposition.position, Quaternion.identity,transform.parent);
        obj.transform.LookAt(target.transform);
        target.stats.TakeDamage(Damage);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, AttackRadius);
    }
}
