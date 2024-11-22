using System.Collections;
using UnityEngine;

public class InimigoMovimentoLinear : MonoBehaviour
{
    public float distanciaMovimento = 5f;
    public float velocidade = 2f; 
    public float raioVisao = 10f; 
    public float anguloBaseVisao = 270f; 
    public float campoDeVisao = 90f; 
    public int quantidadeRaycasts = 10; 
    public Transform alvo; 
    public LayerMask camadaJogador;
    public LayerMask camadaObstaculos; 

    private Vector3 pontoInicial; 
    private Vector3 pontoFinal; 
    private bool indoParaFrente = true; 
    
    public AudioClip gritoSound; 
    private AudioSource audioSource; 

    public GameObject inimigoPrefab;
    private static bool inimigosAtivos = false; 
    private static int inimigosSpawnados = 0; 
    public static bool PlayerVivo = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Obter o AudioSource

        pontoInicial = transform.position;
        pontoFinal = pontoInicial + transform.right * distanciaMovimento;
    }

    void Update()
    {
        if (GameManager.Instance.Life <= 0)
        {
            StartCoroutine(PlayerMorreu());
        }
        else 
        {
            Mover();
            VerificarVisao();
        }
    }
    
    private void Gritar()
    {
        if (gritoSound != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(gritoSound); // Reproduz o som de grito
        }
    }

    IEnumerator PlayerMorreu()
    {
        yield return new WaitForSeconds(1f);
        PlayerVivo = false;
        inimigosSpawnados = 0;
        inimigosAtivos = false;
        yield return new WaitForSeconds(2f);
        PlayerVivo = true; // Ativa novamente a possibilidade de spawnar inimigos
    }

    void Mover()
    {
        if (indoParaFrente)
        {
            transform.position = Vector3.MoveTowards(transform.position, pontoFinal, velocidade * Time.deltaTime);

            if (Vector3.Distance(transform.position, pontoFinal) < 0.1f)
            {
                indoParaFrente = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pontoInicial, velocidade * Time.deltaTime);

            if (Vector3.Distance(transform.position, pontoInicial) < 0.1f)
            {
                indoParaFrente = true;
            }
        }
    }

    void VerificarVisao()
    {
        if (!inimigosAtivos) // Verifica se ainda não foram spawnados inimigos
        {
            float anguloInicio = anguloBaseVisao - (campoDeVisao / 2f); 
            float incrementoAngulo = campoDeVisao / (quantidadeRaycasts - 1); 

            for (int i = 0; i < quantidadeRaycasts; i++)
            {
                float anguloRay = anguloInicio + incrementoAngulo * i;
                Vector2 direcaoRay = DirecaoAPartirDeAngulo(anguloRay);

                RaycastHit2D hit = Physics2D.Raycast(transform.position, direcaoRay, raioVisao, camadaJogador | camadaObstaculos);

                if (hit)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Gritar(); // Grita ao encontrar o jogador
                        StartCoroutine(SpawnInimigosComDelay()); // Spawn inimigos com delay
                        break; 
                    }
                }
            }
        }
    }
    
    private IEnumerator SpawnInimigosComDelay()
{
    yield return new WaitForSeconds(1f); // Espera 1 segundo

    SpawnInimigos(); // Chama o método de spawn dos inimigos
}
    

    void SpawnInimigos()
    {
        if (!inimigosAtivos && inimigosSpawnados < 2)  // Garante que não vai spawnar mais que 2 inimigos
        {
            Vector3 jogadorPosicao = alvo.position;
            float distanciaSpawn = 3f;
            Vector3 posicaoEsquerda = new Vector3(jogadorPosicao.x - distanciaSpawn, jogadorPosicao.y, jogadorPosicao.z);
            Instantiate(inimigoPrefab, posicaoEsquerda, Quaternion.identity);
            inimigosAtivos = true;
            inimigosSpawnados = 1; // Adiciona um inimigo
        }
    }
    public static void InimigoDestruido()
    {
        GameManager.Instance.StartCoroutine(ResetInimigo());
    }

    private static IEnumerator ResetInimigo()
    {
        yield return new WaitForSeconds(3f); // Tempo de espera após a destruição
        inimigosAtivos = false;
        inimigosSpawnados = 0; // Reseta a contagem de inimigos
    }


    Vector2 DirecaoAPartirDeAngulo(float anguloGraus)
    {
        float radianos = anguloGraus * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radianos), Mathf.Sin(radianos));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pontoInicial, pontoFinal);
        float anguloInicio = anguloBaseVisao - (campoDeVisao / 2f);
        float incrementoAngulo = campoDeVisao / (quantidadeRaycasts - 1);

        for (int i = 0; i < quantidadeRaycasts; i++)
        {
            float anguloRay = anguloInicio + incrementoAngulo * i;
            Vector2 direcaoRay = DirecaoAPartirDeAngulo(anguloRay);
            Gizmos.DrawRay(transform.position, direcaoRay * raioVisao); 
        }
    }
}
