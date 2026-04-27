using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 0.5f;

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 7)
        {
            Invoke("Crumble", fallDelay);
        }
    }

    private void Crumble()
    {
        Destroy(gameObject);
    }
}
