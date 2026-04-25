using UnityEngine;
using System.Collections.Generic;

public class Barrier : MonoBehaviour
{
    private bool initialized = false;

    public List<Enemy> enemies;

    public List<EnemySpawner> spawners;


    void OnEnable()
    {
        if (!initialized)
        {
            initialized = true;
            gameObject.SetActive(false);
        }
        else
        {
            foreach (var spawner in spawners)
            {
                spawner.Startwaves(this);
            }
            foreach (var enemy in enemies)
            {
                enemy.barriers.Add(this);
            }
        }
    }

    public void EnemyDeath(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.RemoveAt(enemies.IndexOf(enemy));
        }
        if (enemies.Count == 0  && spawners.Count == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void SpawnerDone(EnemySpawner spawner)
    {
        if (spawners.Contains(spawner))
        {
            spawners.RemoveAt(spawners.IndexOf(spawner));
        }

        if (spawners.Count == 0 && enemies.Count == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
