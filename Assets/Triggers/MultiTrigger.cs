using UnityEngine;

[RequireComponent (typeof(Collider))]
public class MultiTrigger : MonoBehaviour
{
    public GameObject[] targets;

    public TriggerType type;
    public enum TriggerType
    {
        Toggle,
        Spawner
    }

    private void Start()
    {
        gameObject.GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player _))
        {
            switch (type)
            {
                case TriggerType.Toggle:
                    foreach (var item in targets)
                    {
                        item.SetActive(!item.activeInHierarchy);
                    }
                    break;
                case TriggerType.Spawner:
                    foreach (var item in targets)
                    {
                        if(item.TryGetComponent(out EnemySpawner spawner))
                        {
                            spawner.Startwaves();
                        }
                    }
                    break;
                default:
                    break;
            }

            Destroy(gameObject);
        }
    }


}
