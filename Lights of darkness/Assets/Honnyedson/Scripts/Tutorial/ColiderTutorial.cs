using UnityEngine;

public class ControlTrigger : MonoBehaviour
{
    private ControlTutorial controlTutorial;

    private void Start()
    {
        controlTutorial = FindObjectOfType<ControlTutorial>(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            switch (gameObject.name) 
            {
                case "MovimentarColisor":
                    controlTutorial.DisplayMessage("Pressione A ou D para movimentar.");
                    break;
                case "InteragirColisor":
                    controlTutorial.DisplayMessage("Pressione E para interagir com personagens.");
                    break;
                case "PularColisor":
                    controlTutorial.DisplayMessage("Pressione Espaço para pular.");
                    break;
                case "AlternarArcoColisor":
                    controlTutorial.DisplayMessage("Pressione K para alternar entre o arco e a flecha.");
                    break;
                case "PularNaParede":
                    controlTutorial.DisplayMessage("Pule em Direção a Parede");
                    break;
                case "AtacarColisor":
                    controlTutorial.DisplayMessage("Pressione J para atacar.");
                    break;
                default:
                    controlTutorial.DisplayMessage("Colisor desconhecido.");
                    break;
            }
            Destroy(gameObject); 
        }
    }
}