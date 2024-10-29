using System.Collections;
using UnityEngine;

public class InimigoRaycastVisao : MonoBehaviour
{
    public float raioVisao = 5f; // Distância do campo de visão
    public float anguloVisao = 45f; // Ângulo do cone de visão
    public int quantidadeRaycasts = 10; // Número de raios no campo de visão
    public Transform alvo; // Referência ao jogador
    public LayerMask camadaJogador; // Layer do jogador
    public LayerMask camadaObstaculos; // Obstáculos que bloqueiam o campo de visão

    public float velocidadeRotacao = 10f; // Reduzida a velocidade de rotação
    public float anguloMin = -30f; // Ângulo mínimo de rotação ajustado
    public float anguloMax = 30f; // Ângulo máximo de rotação ajustado
    private float direcao = 1f; // Direção da rotação
    private float anguloAtual; // Variável para armazenar o ângulo atual da oscilação

    public GameObject inimigoPrefab; // Prefab do inimigo a ser spawnado
    private bool inimigosAtivos = false; // Verifica se inimigos já foram spawnados
    private int inimigosSpawnados = 0; // Contador de inimigos spawnados
    public static bool PlayerVivo = true;
    void Update()
    {
        if (GameManager.Instance.Life <= 0)
        {
            StartCoroutine(PlayerMorreu());
        }
        OscilarRotacao();
        VerificarVisao();
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
    void OscilarRotacao()
    {
        anguloAtual += direcao * velocidadeRotacao * Time.deltaTime;

        if (anguloAtual >= anguloMax)
        {
            anguloAtual = anguloMax;
            direcao = -1f;
        }
        else if (anguloAtual <= anguloMin)
        {
            anguloAtual = anguloMin;
            direcao = 1f;
        }

        transform.rotation = Quaternion.Euler(0, 0, anguloAtual);
    }

    void VerificarVisao()
    {
        float anguloInicio = -anguloVisao / 2f;
        float incrementoAngulo = anguloVisao / (quantidadeRaycasts - 1);

        for (int i = 0; i < quantidadeRaycasts; i++)
        {
            float anguloRay = anguloInicio + incrementoAngulo * i;
            Vector2 direcaoRay = DirecaoAPartirDeAngulo(transform.eulerAngles.z + anguloRay - 90);
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direcaoRay, raioVisao, camadaJogador | camadaObstaculos);
            
            if (hit)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Jogador Detectado!");
                    SpawnInimigos();
                    break; // Para evitar múltiplos spawns em um único frame
                }
            }
        }
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

        float anguloInicio = -anguloVisao / 2f;
        float incrementoAngulo = anguloVisao / (quantidadeRaycasts - 1);

        Vector3 origem = transform.position;

        for (int i = 0; i < quantidadeRaycasts; i++)
        {
            float anguloRay = anguloInicio + incrementoAngulo * i;
            Vector2 direcaoRay = DirecaoAPartirDeAngulo(transform.eulerAngles.z + anguloRay - 90);

            Vector3 pontoFinal = origem + (Vector3)(direcaoRay * raioVisao);
            Gizmos.DrawLine(origem, pontoFinal); // Desenha os raios

            if (i > 0)
            {
                // Desenha linhas entre os raios para visualizar o cone
                Vector2 direcaoRayAnterior = DirecaoAPartirDeAngulo(transform.eulerAngles.z + (anguloInicio + incrementoAngulo * (i - 1)) - 90);
                Vector3 pontoAnterior = origem + (Vector3)(direcaoRayAnterior * raioVisao);
                Gizmos.DrawLine(pontoFinal, pontoAnterior);
            }
        }
    }
}
