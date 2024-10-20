using System.Collections;
using UnityEngine;

public class InimigoMovimentoLinear : MonoBehaviour
{
    public float distanciaMovimento = 5f; // Distância que o inimigo se moverá
    public float velocidade = 2f; // Velocidade do movimento
    public float raioVisao = 10f; // Distância dos raycasts (campo de visão)
    public float anguloBaseVisao = 270f; // Ângulo base para onde o inimigo "olha" (270 graus = para baixo)
    public float campoDeVisao = 90f; // Ângulo de abertura do campo de visão
    public int quantidadeRaycasts = 10; // Número de raios no campo de visão
    public Transform alvo; // Referência ao jogador
    public LayerMask camadaJogador; // Layer do jogador
    public LayerMask camadaObstaculos; // Obstáculos que bloqueiam o campo de visão

    private Vector3 pontoInicial; // Ponto inicial do movimento
    private Vector3 pontoFinal; // Ponto final do movimento
    private bool indoParaFrente = true; // Define se o inimigo está indo ou voltando

    public GameObject inimigoPrefab; // Prefab do inimigo a ser spawnado
    private bool inimigosAtivos = false; // Verifica se inimigos já foram spawnados
    private int inimigosSpawnados = 0; // Contador de inimigos spawnados
    public static bool PlayerVivo = true;

    void Start()
    {
        pontoInicial = transform.position;
        pontoFinal = pontoInicial + transform.right * distanciaMovimento; // Movimento no eixo x
    }

    void Update()
    {
        if (GameManager.Instance.Life <= 0)
        {
            StartCoroutine(PlayerMorreu());
        }
        Mover();
        VerificarVisao();
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
        float anguloInicio = anguloBaseVisao - (campoDeVisao / 2f); // Começa no limite esquerdo do campo de visão
        float incrementoAngulo = campoDeVisao / (quantidadeRaycasts - 1); // Divide o campo de visão entre os raycasts

        for (int i = 0; i < quantidadeRaycasts; i++)
        {
            float anguloRay = anguloInicio + incrementoAngulo * i;
            Vector2 direcaoRay = DirecaoAPartirDeAngulo(anguloRay);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direcaoRay, raioVisao, camadaJogador | camadaObstaculos);

            if (hit)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Jogador Detectado!");
                    SpawnInimigos();
                    break; // Evita múltiplos spawns
                }
            }
        }
    }

    IEnumerator PlayerMorreu()
    {
        yield return new WaitForSeconds(1f);
        PlayerVivo = false;
        yield return new WaitForSeconds(1f);
        PlayerVivo = true;
        inimigosSpawnados = 0;
        inimigosAtivos = false;
    }

    void SpawnInimigos()
    {
        if (!inimigosAtivos)
        {
            Vector3 jogadorPosicao = alvo.position;

            // Distância fixa para spawn dos inimigos em relação ao jogador
            float distanciaSpawn = 3f; // Ajuste essa distância conforme necessário

            // Calcular a posição de spawn à esquerda e à direita do jogador
            Vector3 posicaoEsquerda = new Vector3(jogadorPosicao.x - distanciaSpawn, jogadorPosicao.y, jogadorPosicao.z);
            Vector3 posicaoDireita = new Vector3(jogadorPosicao.x + distanciaSpawn, jogadorPosicao.y, jogadorPosicao.z);

            // Spawnar os inimigos nas posições calculadas
            Instantiate(inimigoPrefab, posicaoEsquerda, Quaternion.identity);
            Instantiate(inimigoPrefab, posicaoDireita, Quaternion.identity);

            inimigosAtivos = true;
            inimigosSpawnados = 2; // Define que 2 inimigos foram spawnados
        }
    }

    Vector2 DirecaoAPartirDeAngulo(float anguloGraus)
    {
        float radianos = anguloGraus * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radianos), Mathf.Sin(radianos));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // Desenha linha de movimento entre ponto inicial e ponto final
        Gizmos.DrawLine(pontoInicial, pontoFinal);

        // Desenha o campo de visão no ponto atual
        float anguloInicio = anguloBaseVisao - (campoDeVisao / 2f);
        float incrementoAngulo = campoDeVisao / (quantidadeRaycasts - 1);

        for (int i = 0; i < quantidadeRaycasts; i++)
        {
            float anguloRay = anguloInicio + incrementoAngulo * i;
            Vector2 direcaoRay = DirecaoAPartirDeAngulo(anguloRay);

            Gizmos.DrawRay(transform.position, direcaoRay * raioVisao); // Aumenta a distância com raioVisao
        }
    }
}
