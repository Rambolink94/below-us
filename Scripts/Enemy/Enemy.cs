using BelowUs.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy UI")]
    public Image healthbar;
    [SerializeField] AudioClip hitSound;

    public float jumpRange = 1.5f;
    public float normalRange = 0.5f;
    public float rotationSpeed = 1f;
    public float timeBetweenAttacks = 1f;

    public LayerMask playerMask;

    private EnemyStats enemyStats;
    private Transform healthbarParent;
    private Animator animator;
    private AudioSource audioSource;
    private NavMeshAgent agent;

    private float timeSinceLastAttack = 0;
    private Transform closestPlayer;

    private int attackRangeHash = Animator.StringToHash("AttackRange");
    private int onAttackHash = Animator.StringToHash("OnAttack");
    private int playerDetectedHash = Animator.StringToHash("PlayerDetected");

    private void Start()
    {
        healthbarParent = healthbar.gameObject.transform.parent;
        healthbarParent.gameObject.SetActive(false);
        enemyStats = gameObject.GetComponent<EnemyStats>();
        animator = gameObject.GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        closestPlayer = CheckForPlayers();
        if (closestPlayer != null)
        {
            animator.SetBool(playerDetectedHash, true);
            MoveTowardsPlayer();
            //Debug.Log((timeSinceLastAttack + Time.deltaTime) + " >= " + timeBetweenAttacks);
            RotateTowardsPlayer(closestPlayer.position);
            if (Time.time > timeBetweenAttacks + timeSinceLastAttack)
            {
                Debug.Log("HERE");
                Attack();
                timeSinceLastAttack = Time.time;
            }
        }
        else
        {
            animator.SetBool(playerDetectedHash, false);
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, normalRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, jumpRange);
    }

    public void MoveTowardsPlayer()
    {
        agent.SetDestination(closestPlayer.position);
    }

    public void Attack()
    {
        float distance = Vector3.Distance(closestPlayer.position, transform.position);
        animator.SetFloat(attackRangeHash, distance);
        animator.SetTrigger(onAttackHash);

        //Character character = closestPlayer.GetComponentInChildren<Character>();
        //character.Health -= enemyStats.damage;
    }

    //public bool CheckAnimations()
    //{
    //    animator.GetCurrentAnimatorStateInfo(0).IsName("")
    //    if ()
    //}

    public void Strike()
    {
        Debug.Log("Strike");
        audioSource.clip = hitSound;
        audioSource.pitch = Random.Range(0.4f, 1f);
        audioSource.Play();
    }

    public Transform CheckForPlayers()
    {
        // Check if any players are within normal range
        closestPlayer = GetClosestPlayer(normalRange);

        // Check if any players are within jumpRange
        if (closestPlayer == null)
        {
            closestPlayer = GetClosestPlayer(jumpRange);
        }

        return closestPlayer;
    }

    public Transform GetClosestPlayer(float range)
    {
        Collider[] players = Physics.OverlapSphere(transform.position, range, playerMask);
        foreach (Collider player in players)
        {
            // Original player is still close enough to attack
            if (player.transform == closestPlayer) return closestPlayer;

            // Order players by distance
            Collider[] closestPlayers = players.OrderBy(closestPlayer => (transform.position - closestPlayer.transform.position).sqrMagnitude).ToArray();
            return closestPlayers[0].transform;
        }

        return null;
    }

    public void RotateTowardsPlayer(Vector3 playerPosition)
    {
        Vector3 targetDirection = playerPosition - transform.position;
        float singleStep = rotationSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    public void TakeDamage(int damage)
    {
        healthbarParent.gameObject.SetActive(true);
        animator.SetTrigger("OnHit");
        animator.SetInteger("HitDirection", 1);

        enemyStats.health -= damage;

        UpdateHealthbar();

        if(enemyStats.health <= 0)
        {
            HandleDeath();
        }
    }

    void HandleDeath()
    {
        Debug.Log("Enemy died...reseting health.");
        animator.SetBool("IsDead", true);
        enemyStats.ResetHealth();
        UpdateHealthbar();
    }

    void UpdateHealthbar()
    {
        healthbar.fillAmount = Mathf.InverseLerp(0f, enemyStats.maxHealth, enemyStats.health);
    }
}
