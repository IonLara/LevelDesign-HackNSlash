using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    private int deathCount = 0;
    private int spawnCount = 0;

    public int totalenemies = 3;

    public float spawnCoolDown = 5f;

    public GameObject enemy;

    private List<Barrier> barrier = new List<Barrier>();

    public void Startwaves(Barrier barrier)
    {
        Spawn();
        this.barrier.Add(barrier);
    }

    private void Spawn()
    {
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
            foreach (var item in barrier)
            {
                item.SpawnerDone(this);
            }
            
        }
    }
}
