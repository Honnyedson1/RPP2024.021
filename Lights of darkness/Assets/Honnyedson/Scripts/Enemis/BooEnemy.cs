using Unity.VisualScripting;
using UnityEngine;

public class BooEnemy : MonoBehaviour
{
    public GameObject player;
    public float speed = 2f;
    public float stopDistance = 2f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public Transform areaLimit;

    private SpriteRenderer spriteRenderer;       
    private float lastAttackTime;
    private BossController boss;
    public int Life = 3;
    public int dmg;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boss = FindObjectOfType<BossController>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void Update()
    {
        if (IsPlayerInArea())
        {
            Vector3 directionToPlayer = player.transform.position - transform.position; 
            float distanceToPlayer = directionToPlayer.magnitude;           

            bool isPlayerLooking = IsPlayerLooking(); 

            if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                player.GetComponent<PlayerController>().TakeDmg(dmg);
                lastAttackTime = Time.time; 
            }
            else if (!isPlayerLooking && distanceToPlayer > stopDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            }
            spriteRenderer.flipX = directionToPlayer.x < 0;
        }
    }

    private bool IsPlayerInArea()
    {
        return areaLimit.GetComponent<Collider2D>().bounds.Contains(player.transform.position);
    }

    private bool IsPlayerLooking()
    {
        Vector3 playerLookDirection = player.transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        return Vector3.Dot(playerLookDirection, (transform.position - player.transform.position).normalized) > 0;
    }

    public void TakeDamage(int damage)
    {
        Life -= damage;
        if (Life <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("O inimigo morreu!");
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        if (areaLimit != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(areaLimit.position, areaLimit.localScale);
        }
        
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    private void OnDrawGizmosSelected()
    {
        if (areaLimit != null)
        {
            Gizmos.color = Color.yellow; 
            Gizmos.DrawWireCube(areaLimit.position, areaLimit.localScale);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
