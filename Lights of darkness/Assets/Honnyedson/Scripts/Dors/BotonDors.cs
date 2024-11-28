using System.Collections;
using UnityEngine;
using UnityEngine.UI;  // Para manipular elementos da UI

public class BotonDors : MonoBehaviour
{
    [SerializeField] public static int Botons;
    public Animator Torch;
    public Text mensagemTexto; // Referência ao componente Text da UI
    private bool mensagemExibida = false;
    public AudioSource Brilho;

    private void Start()
    {
        Brilho = GetComponent<AudioSource>();
        Torch = GetComponent<Animator>();
        mensagemTexto.gameObject.SetActive(false); // Inicialmente, a mensagem não será visível
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Flecha")
        {
            Torch.SetTrigger("Abriu");
            Botons++;
            Brilho.Play();
            // Se os 3 botões forem pressionados
            if (Botons >= 3 && !mensagemExibida)
            {
                mensagemExibida = true;
                StartCoroutine(ExibirMensagem());
            }
        }
    }

    // Corrotina para exibir a mensagem por 3 segundos
    private IEnumerator ExibirMensagem()
    {
        // Exibe a mensagem na tela
        mensagemTexto.gameObject.SetActive(true);
        mensagemTexto.text = "Os cristais ergueram a porta. Volte para sair da caverna.";

        // Aguarda 3 segundos
        yield return new WaitForSeconds(3f);

        // Após 3 segundos, a mensagem desaparece
        mensagemTexto.gameObject.SetActive(false);
    }
}