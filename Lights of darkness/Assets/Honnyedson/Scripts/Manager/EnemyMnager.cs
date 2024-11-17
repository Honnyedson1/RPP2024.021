using UnityEngine;

public class InimigoController : MonoBehaviour
{
    // Responsável por destruir todos os inimigos na cena quando o jogador morrer
    public static void DestruirTodosInimigos()
    {
        InimigoController[] inimigos = FindObjectsOfType<InimigoController>();
        
        // Destruir todos os inimigos encontrados
        foreach (InimigoController inimigo in inimigos)
        {
            Destroy(inimigo.gameObject);
        }

        Debug.Log("Todos os inimigos foram destruídos!");
    }
}