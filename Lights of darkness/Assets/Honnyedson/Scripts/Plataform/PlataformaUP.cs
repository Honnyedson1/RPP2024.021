using UnityEngine;
using System.Collections.Generic;

public class MovablePlatform : MonoBehaviour
{
    public List<Vector3> pontos;
    public float velocidade = 2f;

    private int pontoAtual = 0;
    private bool moverPlataforma = false;
    private Vector3 posicaoInicial;

    private void Start()
    {
        if (pontos.Count < 2)
        {
            return;
        }

        posicaoInicial = pontos[0]; 
        transform.position = posicaoInicial;
        pontoAtual = 1; 
    }

    private void Update()
    {
        if (moverPlataforma)
        {
            transform.position = Vector3.MoveTowards(transform.position, pontos[pontoAtual], velocidade * Time.deltaTime);
            if (Vector3.Distance(transform.position, pontos[pontoAtual]) < 0.1f)
            {
                pontoAtual = (pontoAtual + 1) % pontos.Count;
            }
        }
        if (GameManager.Instance.Life <= 0)
        {
            ResetarPlataforma();
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
            moverPlataforma = true; 
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null); 
        }
    }

    private void ResetarPlataforma()
    {
        transform.position = posicaoInicial; 
        pontoAtual = 1; 
        moverPlataforma = false; 
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < pontos.Count; i++)
        {
            Gizmos.DrawSphere(pontos[i], 0.2f);
            if (i < pontos.Count - 1)
            {
                Gizmos.DrawLine(pontos[i], pontos[i + 1]);
            }
        }
    }
}
