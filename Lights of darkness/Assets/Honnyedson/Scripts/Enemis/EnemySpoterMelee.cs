using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float followDistance = 5f; 
    public float attackDistance = 1.5f; 
    public float moveSpeed = 10f;
    private int vida = 2;
    private float lastAttackTime;
    private Transform player;
    private bool isAttacking = false; 
    private bool isdead;
    public int Dano;
    private Animator animator;
    private BossController bossController; // Referência ao Boss

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bossController = FindObjectOfType<BossController>(); // Encontra o BossController na cena
        animator = GetComponent<Animator>();

        // Inicia o timer de auto-destruição
        StartCoroutine(AutoDestruct());
    }


    void Update()
    {
        if (bossController != null && bossController.Lifeboss <= 0) // Se o Boss morreu
        {
            Destroy(gameObject); // Destrói este inimigo
            return; // Sai do método para evitar execução desnecessária
        }

        if (InimigoMovimentoLinear.PlayerVivo == false || InimigoRaycastVisao.PlayerVivo == false)
        {
            Destroy(this.gameObject);
        }

        if (GameManager.Instance.Life <= 0)
        {
            Destroy(gameObject);
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
            playerHealth.TakeDmg(2);
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
    private void OnDestroy()
    {
        if (!isdead && InimigoRaycastVisao.PlayerVivo)
        {
            // Notifica que um inimigo foi destruído
            InimigoRaycastVisao.InimigoDestruido();
        }
        if (!isdead && InimigoMovimentoLinear.PlayerVivo)
        {
            InimigoMovimentoLinear.InimigoDestruido();
        }
        
    }
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followDistance);
    }

    private IEnumerator AutoDestruct()
    {
        yield return new WaitForSeconds(10f); // Aguarda 10 segundos
        if (!isdead) // Apenas destrói se ainda estiver vivo
        {
            Destroy(gameObject);
        }
    }
}
