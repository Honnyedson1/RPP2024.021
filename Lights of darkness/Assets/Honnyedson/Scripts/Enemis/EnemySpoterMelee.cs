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

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (InimigoMovimentoLinear.PlayerVivo == false || InimigoRaycastVisao.PlayerVivo == false)
        {
            Debug.Log("Morreu");
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
                }
                else
                {
                    Debug.Log("Jogador saiu do campo de visão do inimigo.");
                }
            }    
        }
    }

    public void takedmg(int dmg)
    {
        vida -= dmg;
        if (vida <= 0)
        {
            Destroy(gameObject, 2f);
            isdead = true;
        }
    }

    private void FollowPlayerOnXAxis()
    {
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x); 
        float heightDifference = Mathf.Abs(player.position.y - transform.position.y); // Calcula a diferença de altura

        if (distanceToPlayer <= attackDistance && heightDifference < 1f) // Verifica se a altura é similar
        {
            StartCoroutine(PrepareAttack());
        }
    }

    private IEnumerator PrepareAttack()
    {
        isAttacking = true; 
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float heightDifference = Mathf.Abs(player.position.y - transform.position.y);

        if (distanceToPlayer <= attackDistance && heightDifference < 1f) // Confirma novamente a posição antes do ataque
        {
            AttackPlayer();
        }
        else
        {
            Debug.Log("Jogador saiu da área de ataque ou está pulando. Ataque cancelado.");
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
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followDistance);
    }
}
