using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float followDistance = 5f; 
    public float attackDistance = 1.5f; 
    public float moveSpeed = 10f;
    private int vida = 4;
    private float lastAttackTime;
    private Transform player;
    private bool isAttacking = false; 
    private bool isdead;

    private Animator animator;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        if (InimigoMovimentoLinear.PlayerVivo == false || InimigoRaycastVisao.PlayerVivo == false)
        {
            Destroy(this.gameObject);
        }

        if (!isdead)
        {
            if (player != null)
            {
                float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
                if (!isAttacking && distanceToPlayer <= followDistance)
                {
                    FollowPlayerOnXAxis();
                    animator.SetBool("isWalking", true); 
                }
                else
                {
                    animator.SetBool("isWalking", false); 
                }

                FlipTowardsPlayer(); 
            }    
        }
    }

    public void takedmg(int dmg)
    {
        vida -= dmg;
        animator.SetTrigger("TakeDamage");
        if (vida <= 0)
        {
            isdead = true;
            animator.SetTrigger("Die"); 
            Destroy(gameObject, 2f);
        }
    }

    private void FollowPlayerOnXAxis()
    {
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x); 
        float heightDifference = Mathf.Abs(player.position.y - transform.position.y);

        if (distanceToPlayer <= attackDistance && heightDifference < 1f)
        {
            StartCoroutine(PrepareAttack());
        }
    }

    private IEnumerator PrepareAttack()
    {
        isAttacking = true;
        animator.SetBool("isWalking", false); 
        animator.SetTrigger("Attack"); 
        yield return new WaitForSeconds(0.15f);
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float heightDifference = Mathf.Abs(player.position.y - transform.position.y);

        if (distanceToPlayer <= 2 && heightDifference < 1f)
        {
            AttackPlayer();
        }
        yield return new WaitForSeconds(1);
        isAttacking = false;
    }

    private void AttackPlayer()
    {
        PlayerController playerHealth = player.GetComponent<PlayerController>();
        if (playerHealth != null)
        {
            playerHealth.TakeDmg(100);
        }
    }

    private void FlipTowardsPlayer()
    {
        if (player.position.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 0, 0); 
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followDistance);
    }
}
