using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class Player : MonoBehaviour
{
    public int health = 5;
    private int maxHealth;
    private bool isAlive = true;

    public float speed = 4f;
    [Space(10), Header("Dash"), Space(7)]
    public float dashLength = 15f;
    public float dashCoolDown = 0.5f;
    private bool canDash = true;
    private float tDash;
    public LayerMask wallMask;

    private Rigidbody rb;
    private Animator animator;

    private Vector2 movement = new Vector2();
    private Vector3 lastDirection = Vector3.forward;

    private PlayerActions actions;

    [Space(10), Header("Attack"), Space(7)]
    public int damage = 2;
    public float attackRadius = 0.3f;
    public float attackCoolDown = 0.3f;
    [Range(0, 1)]
    public float attackDistance = 0.3f;
    private float tAttack;
    private bool canAttack = true;
    public LayerMask enemyLayer;

    public GameObject playerCam;

    public bool drawGizmos = false;

    private Vector3 checkpoint;

    private bool grounded = true;
    private bool canJump = true;
    public float jumpCoolDown = 0.5f;
    public float tJump;
    private float JumpForce = 5f;
    public LayerMask groundMask;

    private Transform myCamera;

    [HideInInspector]
    public Rigidbody platformRb;
    [HideInInspector]
    public Interactable interactable;


    public float fallDeathDistance = 5;
    private float jumpApex;
    private bool apexed = false;

    void Awake()
    {
        actions = new PlayerActions();
        actions.Game.Enable();
        
        rb = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();

        var cam = Instantiate(playerCam);
        CameraTarget target = new CameraTarget
        {
            TrackingTarget = transform,
            LookAtTarget = transform,
            CustomLookAtTarget = true
        };
        cam.GetComponent<CinemachineCamera>().Target = target;
        myCamera = cam.transform;

        checkpoint = transform.position;
        maxHealth = health;
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }
        movement = actions.Game.Move.ReadValue<Vector2>();

        if (movement.magnitude > 0)
        {
            lastDirection.x = movement.x;
            lastDirection.z = movement.y;

        }
        animator.SetBool("IsMoving", movement.magnitude > 0);

        var rotation = Quaternion.Euler(new Vector3(0, myCamera.eulerAngles.y, 0));
        transform.rotation = Quaternion.LookRotation(rotation * lastDirection.normalized, Vector3.up);

        if (canAttack)
        {
            if (actions.Game.Attack.WasPressedThisFrame())
            {
                Attack();
            }
        } 
        else
        {
            tAttack -= Time.deltaTime;
            if (tAttack <= 0)
            {
                tAttack = 0;
                canAttack = true;
            }
        }

        if (canDash)
        {
            if (actions.Game.Dash.WasPressedThisFrame())
            {
                Dash();
            }
        } 
        else
        {
            tDash -= Time.deltaTime;
            if (tDash <= 0)
            {
                tDash = 0;
                canDash = true;
            }
        }

        if (interactable != null && actions.Game.Interact.WasPressedThisFrame())
        {
            interactable.Activate();
        }

        if (canJump)
        {
            if (grounded && actions.Game.Jump.WasPressedThisFrame())
            {
                Jump();
            }
        }
        else
        {
            tJump -= Time.deltaTime;
            if (tJump <= 0)
            {
                tJump = 0;
                canJump = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isAlive)
        {
            var rotation = Quaternion.Euler(new Vector3(0, myCamera.eulerAngles.y, 0));
            if (platformRb != null)
            {
                var velocity = new Vector3(platformRb.linearVelocity.x, rb.linearVelocity.y, platformRb.linearVelocity.z);
                rb.linearVelocity = velocity;
            }
            rb.AddForce(rotation *new Vector3(movement.x, 0, movement.y).normalized * speed, ForceMode.Impulse);
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }

        var tempGround = Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, 0.2f, groundMask);
        if (grounded == false && tempGround == true)
        {
            animator.SetTrigger("Land");
            
            if (Mathf.Abs(transform.position.y - jumpApex) > fallDeathDistance)
            {
                Damage(10);
            }
            apexed = false;
        }
        grounded = tempGround;

        if (!grounded && rb.linearVelocity.y < 0 && apexed == false)
        {
            apexed = true;
            jumpApex = transform.position.y;
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        canJump = false;
        tJump = jumpCoolDown;
        animator.SetTrigger("Jump");
    }

    private void Dash()
    {
        Ray ray = new Ray(transform.position + (Vector3.up * 0.5f), lastDirection.normalized);
        if (Physics.Raycast(ray, out RaycastHit  hit , dashLength, wallMask))
        {
            var point = hit.point;
            var dir = (transform.position - point).normalized;
            point = point + (dir * 0.5f);
            transform.position = new Vector3(point.x, transform.position.y, point.z);
        } 
        else
        {
            transform.position = transform.position + (lastDirection.normalized * dashLength);
        }
        canDash = false;
        tDash = dashCoolDown;
    }
    private void Attack()
    {
        animator.SetTrigger("Attack");
        canAttack = false;
        tAttack = attackCoolDown;

        var point = transform.position + (transform.forward.normalized * attackDistance);
        point.y = transform.position.y + 0.5f; 
        var enemies = Physics.OverlapSphere(point, attackRadius, enemyLayer);
        if (enemies.Length > 0)
        {
            Debug.Log("Attacking");
            foreach (var enemy in enemies)
            {
                enemy.gameObject.GetComponent<Enemy>().Hit(damage);
            }
        }
    }
    public void Damage(int damage)
    {
        if (!isAlive)
        {
            return;
        }
        health -= damage;
        if (health <= 0)
        {
            isAlive = false;
            animator.SetTrigger("Die");
            StartCoroutine(nameof(EndGame));
        }
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1.5f);
        transform.position = checkpoint;
        isAlive = true;
        health = maxHealth;

    }

    public void SetCheckpoint(Vector3 pos)
    {
        checkpoint = pos;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos)
        {
            return;
        }
        var point = transform.position + (transform.forward.normalized * attackDistance);
        point.y = transform.position.y + 0.5f; 
        Gizmos.DrawSphere(point, attackRadius);
    }
}
