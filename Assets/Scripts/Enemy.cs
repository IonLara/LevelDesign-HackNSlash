using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;

    public int health = 2;

    public float chaseDistance = 4;

    public LayerMask playerLayer; 

    public int damage = 1;
    public float attackDistance = 0.3f;
    public float attackCooldown = 1f;
    private float tAttack = 0;
    private bool canAttack = true;
    public float attackRadius = 0.3f;
    public float attackDisplace = 0.2f;

    public float tickTime = 0.1f;
    private float t = 0;

    private GameObject player;
    private bool chasing = false;

    private Animator animator;

    public bool drawGizmos = false;

    [HideInInspector]
    public EnemySpawner spawner;
    [HideInInspector]
    public List<Barrier> barriers = new List<Barrier>();

    void OnEnable()
    {
        player = GameObject.FindFirstObjectByType<Player>().gameObject;
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        t += Time.deltaTime;

        if (t >= tickTime)
        {
            t = 0;
            Tick();
        }
        if (!canAttack)
        {
            tAttack += Time.deltaTime;
            if (tAttack >= attackCooldown)
            {
                tAttack = 0;
                canAttack = true;
            }
        }

        animator.SetBool("IsMoving", agent.velocity.magnitude > 0.05f);
    }

    private void Tick()
    {
        Sense();
        if (chasing)
        {
            TryAttack();
        }
    }

    private void Sense()
    {
        if (!canAttack)
        {
            return;
        }

        if (Vector3.Distance(transform.position, player.transform.position) <= chaseDistance)
        {
            Debug.Log("Start Chasing");
            agent.SetDestination(player.transform.position);
            if (agent.path.status != NavMeshPathStatus.PathPartial)
            {
                chasing = true;
            }
            else
            {
                Debug.Log("PathReset");
                agent.ResetPath();
            }
        }
        else
        {
            agent.ResetPath();
            chasing = false;
        }
    }

    private void TryAttack()
    {
        if (canAttack && Vector3.Distance(transform.position, player.transform.position) < attackDistance)
        {
            canAttack = false;
            animator.SetTrigger("Attack");
            agent.ResetPath();
            transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
        }
    }

    public void TryDealDamage()
    {
        var point = transform.position + (transform.forward.normalized * attackDisplace);
        point.y = transform.position.y + 0.5f;

        var hits = Physics.OverlapSphere(point, attackRadius, playerLayer);
        if (hits.Length > 0)
        {
            player.GetComponent<Player>().Damage(damage);
            Debug.Log("Damaged Player");
        }
    }

    public void Hit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (spawner != null)
            {
                spawner.EnemyDeath();
            }
            if (barriers != null)
            {
                foreach (var item in barriers)
                {
                    item.EnemyDeath(this);
                }
                
            }
            Destroy(gameObject);
        }
    }
    
    void OnDrawGizmos()
    {
        if (!drawGizmos)
        {
            return;
        }
        var point = transform.position + (transform.forward.normalized * attackDisplace);
        point.y = transform.position.y + 0.5f;
        Gizmos.DrawSphere(point, attackRadius);
    }
}
