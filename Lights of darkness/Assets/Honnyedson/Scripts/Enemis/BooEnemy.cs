using Unity.VisualScripting;
using UnityEngine;

public class BooEnemy : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float stopDistance = 2f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public Transform areaLimit;

    private SpriteRenderer spriteRenderer;       
    private float lastAttackTime;
    private BossController boss;
    public int Life = 3;

    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform; 
        spriteRenderer = GetComponent<SpriteRenderer>();
        boss = FindObjectOfType<BossController>();
    }

    void Update()
    {
        if (IsPlayerInArea())
        {
            Vector3 directionToPlayer = player.position - transform.position; 
            float distanceToPlayer = directionToPlayer.magnitude;           

            bool isPlayerLooking = IsPlayerLooking(); 

            if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                Attack(); 
                lastAttackTime = Time.time; 
            }
            else if (!isPlayerLooking && distanceToPlayer > stopDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
            spriteRenderer.flipX = directionToPlayer.x < 0;
        }
    }

    private bool IsPlayerInArea()
    {
        return areaLimit.GetComponent<Collider2D>().bounds.Contains(player.position);
    }

    private bool IsPlayerLooking()
    {
        Vector3 playerLookDirection = player.localScale.x > 0 ? Vector3.right : Vector3.left;
        return Vector3.Dot(playerLookDirection, (transform.position - player.position).normalized) > 0;
    }

    private void Attack()
    {
        GameManager.Instance.Life--;
        Debug.Log("Boo atacou!");
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
