using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private int deathCount = 0;
    private int spawnCount = 0;

    public int totalenemies = 3;

    public float spawnCoolDown = 5f;

    public GameObject enemy;

    private List<Barrier> barriers = new List<Barrier>();
    private bool started = false;

    public void Startwaves(Barrier barrier)
    {
        barriers.Add(barrier);
        if (started)
        {
            return;
        }
        Spawn();
    }

    private void Spawn()
    {
        started = true;
        var foo = Instantiate(enemy, transform.position, Quaternion.identity);
        foo.GetComponent<Enemy>().spawner = this;
        spawnCount++;
        if (spawnCount < totalenemies)
        {
            Invoke(nameof(Spawn), spawnCoolDown);
        }
    }

    public void EnemyDeath()
    {
        deathCount++;

        if (deathCount == totalenemies)
        {
            foreach (var barrier in barriers)
            {
                barrier.SpawnerDone(this);
            }
        }
    }
}
