using System.Collections;
using UnityEngine;

public class InimigoRaycastVisao : MonoBehaviour
{
    public float raioVisao = 5f; 
    public float anguloVisao = 45f; 
    public int quantidadeRaycasts = 10; 
    public Transform alvo;
    public LayerMask camadaJogador; 
    public LayerMask camadaObstaculos; 

    public float velocidadeRotacao = 10f; 
    public float anguloMin = -30f; 
    public float anguloMax = 30f;
    private float direcao = 1f;
    private float anguloAtual; 

    public GameObject inimigoPrefab; 
    private static bool inimigosAtivos = false; 
    private static int inimigosSpawnados = 0; 
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
        inimigosSpawnados = 0; 
        inimigosAtivos = false;

        yield return new WaitForSeconds(2f);
        PlayerVivo = true; // Ativa novamente o sistema de visão
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
        if (!inimigosAtivos)
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
                        SpawnInimigos();
                        break;
                    }
                }
            }
        }
    }
    public static void InimigoDestruido()
    {
        GameManager.Instance.StartCoroutine(ResetInimigo());
    }

    private static IEnumerator ResetInimigo()
    {
        yield return new WaitForSeconds(3f); // Tempo para reativar o spawn
        inimigosAtivos = false;
        inimigosSpawnados = 0;
    }

    void SpawnInimigos()
    {
        if (!inimigosAtivos && inimigosSpawnados < 2)
        {
            Vector3 jogadorPosicao = alvo.position;
            float distanciaSpawn = 3f;

            // Gera inimigos em duas posições
            Vector3 posicaoEsquerda = jogadorPosicao + Vector3.left * distanciaSpawn;
            Vector3 posicaoDireita = jogadorPosicao + Vector3.right * distanciaSpawn;

            Instantiate(inimigoPrefab, posicaoEsquerda, Quaternion.identity);
            Instantiate(inimigoPrefab, posicaoDireita, Quaternion.identity);

            inimigosAtivos = true;
            inimigosSpawnados += 2; // Incrementa o contador
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
            Gizmos.DrawLine(origem, pontoFinal);

            if (i > 0)
            {
                Vector2 direcaoRayAnterior = DirecaoAPartirDeAngulo(transform.eulerAngles.z + (anguloInicio + incrementoAngulo * (i - 1)) - 90);
                Vector3 pontoAnterior = origem + (Vector3)(direcaoRayAnterior * raioVisao);
                Gizmos.DrawLine(pontoFinal, pontoAnterior);
            }
        }
    }
}
