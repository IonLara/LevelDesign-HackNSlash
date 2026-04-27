using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : IInteractable
{
    public enum Direction
    {
        Right,
        Left,
        Forward,
        Backward,
        Up,
        Down
    }
    public Direction moveDirection;
    public float speed = 2f;
    public float distance = 3f;
    
    public bool moveOnStart = true;
    [HideInInspector]
    public bool moving = false;
    public bool going = true;
    private Vector3 startPos;

    public bool debug = false;

    private float t = 0;

    private Rigidbody rb;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        if (moveOnStart == true)
        {
            moving = true;
        }
    }

    void Update()
    {
        if (moving)
        {
            if (going)
            {
                t += Time.deltaTime * (speed/10);

                if (t >= 1)
                {
                    t = 1;
                    going = false;
                }
            }
            else
            {
                t -= Time.deltaTime * (speed/10);

                if (t <= 0)
                {
                    t = 0;
                    going = true;
                }
            }
            Vector3 direction = Vector3.zero;
            switch (moveDirection)
            {
                case Direction.Right:
                    direction = Vector3.right;
                    break;
                case Direction.Left:
                    direction = Vector3.left;
                    break;
                case Direction.Forward:
                    direction = Vector3.forward;
                    break;
                case Direction.Backward:
                    direction = Vector3.back;
                    break;
                case Direction.Up:
                    direction = Vector3.up;
                    break;
                case Direction.Down:
                    direction = Vector3.down;
                    break;
            }
            rb.MovePosition(Vector3.Lerp(startPos, startPos + direction * distance, t));
        }
    }

    public override void Interact()
    {
        moving = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.platformRb = rb;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.platformRb = null;
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.IsPlaying(gameObject) && debug)
        {
            Gizmos.color = Color.green;
            Vector3 direction = Vector3.zero;
            switch (moveDirection)
            {
                case Direction.Right:
                    direction = Vector3.right;
                    break;
                case Direction.Left:
                    direction = Vector3.left;
                    break;
                case Direction.Forward:
                    direction = Vector3.forward;
                    break;
                case Direction.Backward:
                    direction = Vector3.back;
                    break;
                case Direction.Up:
                    direction = Vector3.up;
                    break;
                case Direction.Down:
                    direction = Vector3.down;
                    break;
            }
            Gizmos.DrawLine(transform.position, transform.position + (direction * distance));
            Gizmos.DrawSphere(transform.position + (direction * distance), 0.2f);
        }        
    }
}
