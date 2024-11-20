using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CrystalTransparencyWithPriority : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    // Transparência quando não há colisão
    public float defaultAlpha = 0.1f;

    // Transparência quando há colisão
    public float glowingAlpha = 1.0f;

    // Contadores de colisão
    private int playerCollisions = 0;
    private int arrowCollisions = 0;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetTransparency(defaultAlpha); // Inicializa com a transparência padrão
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Incrementa contadores de colisão
        if (other.CompareTag("Player"))
        {
            playerCollisions++;
            SetTransparency(glowingAlpha); // Torna visível
        }
        else if (other.CompareTag("Flecha"))
        {
            arrowCollisions++;
            SetTransparency(glowingAlpha); // Torna visível
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Decrementa contadores de colisão
        if (other.CompareTag("Player"))
        {
            playerCollisions--;
        }
        else if (other.CompareTag("Flecha"))
        {
            arrowCollisions--;
        }

        // Atualiza transparência apenas se nenhum dos dois estiver colidindo
        if (playerCollisions <= 0 && arrowCollisions <= 0)
        {
            SetTransparency(defaultAlpha); // Retorna à transparência padrão
        }
    }

    private void SetTransparency(float alpha)
    {
        Color currentColor = spriteRenderer.color;
        spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
    }
}