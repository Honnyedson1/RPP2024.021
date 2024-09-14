using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;

public class DodgeEnemy : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float dodgeProbability = 0.7f; // Probabilidade de desviar das flechas
    public float followRange = 10f;
    public Transform areaLimit;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private float lastAttackTime;
    public int Life = 3;
    public int dmg = 1;
    public event Action OnDeath;
    private Coroutine attackCoroutine;
    public EnemySpawner spawner; // Referência ao spawner para spawnar novos inimigos quando o Boss estiver com metade da vida
    private bool bossHalfLifeTriggered = false;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (areaLimit == null)
        {
            Debug.LogError("Área de Limite não configurada para o inimigo.");
        }
    }

    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(player.position, transform.position);

            if (distanceToPlayer <= followRange)
            {
                if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
                {
                    if (attackCoroutine == null)
                    {
                        attackCoroutine = StartCoroutine(AttackCoroutine());
                    }
                }
                else
                {
                    MoveTowardsPlayer();
                }

                spriteRenderer.flipX = (player.position.x < transform.position.x);
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        if (Vector2.Distance(player.position, transform.position) > attackRange)
        {
            if (Random.value > dodgeProbability)
            {
                rb.velocity = new Vector2(directionToPlayer.x * speed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnDestroy()
    {
        EnemySpawnObserver.TriggerEnemySpawn(null, null);
    }

    private IEnumerator AttackCoroutine()
    {
        Debug.Log("Preparando ataque...");
        yield return new WaitForSeconds(0.3f);

        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceToPlayer <= attackRange)
        {
            ApplyDamage();
        }
        else
        {
            Debug.Log("Ataque falhou, jogador fora do alcance.");
        }

        attackCoroutine = null;
        lastAttackTime = Time.time;
    }

    private void ApplyDamage()
    {
        player.GetComponent<PlayerController>().TakeDmg(dmg);
        Debug.Log("DodgeEnemy atacou!");
    }

    public void TakeDamage(int damage)
    {
        Life -= damage;
        if (Life <= 0)
        {
            Die();
        }

        if (!bossHalfLifeTriggered && Life <= Life / 2)
        {
            spawner.SpawnExtraEnemies(2);
            bossHalfLifeTriggered = true; 
        }
    }

    private void Die()
    {
        Debug.Log("O DodgeEnemy morreu!");
        Destroy(gameObject);
    }

    public void OnArrowHit()
    {
        if (Random.value > dodgeProbability)
        {
            TakeDamage(1);
        }
        else
        {
            Debug.Log("DodgeEnemy desviou da flecha!");
        }
    }
}
