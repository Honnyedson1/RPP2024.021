using UnityEngine;

public class ArrowItem : MonoBehaviour
{
    public int arrowIncrease = 10; // Quantidade de flechas adicionadas

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.QFlechas += arrowIncrease;
            GameManager.Instance.FlechasText.text = GameManager.Instance.QFlechas.ToString();
            Destroy(gameObject); // Remove o item após ser coletado
        }
    }
}
