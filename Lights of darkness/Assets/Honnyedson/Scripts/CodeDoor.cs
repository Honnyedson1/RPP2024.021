using UnityEngine;
using UnityEngine.UI;

public class CodeDoor : MonoBehaviour
{
    public GameObject codePanel;          
    public Text[] codeDigits;             

    private int[] codeEntered = {0, 0, 0}; 
    private int[] correctCode = {1, 6, 8}; 
    private bool isPlayerNear = false;

    public GameObject targetObject; 

    private void Start()
    {
        codePanel.SetActive(false); 
        UpdateCodeDisplay();
    }

    private void Update()
    {
        if (isPlayerNear) 
        {
            codePanel.SetActive(true);
        }
    }

    public void IncrementDigit1()
    {
        codeEntered[0] = (codeEntered[0] + 1) % 10;
        UpdateCodeDisplay();
    }

    public void DecrementDigit1()
    {
        codeEntered[0] = (codeEntered[0] - 1 + 10) % 10;
        UpdateCodeDisplay();
    }

    public void IncrementDigit2()
    {
        codeEntered[1] = (codeEntered[1] + 1) % 10;
        UpdateCodeDisplay();
    }

    public void DecrementDigit2()
    {
        codeEntered[1] = (codeEntered[1] - 1 + 10) % 10;
        UpdateCodeDisplay();
    }

    public void IncrementDigit3()
    {
        codeEntered[2] = (codeEntered[2] + 1) % 10;
        UpdateCodeDisplay();
    }

    public void DecrementDigit3()
    {
        codeEntered[2] = (codeEntered[2] - 1 + 10) % 10;
        UpdateCodeDisplay();
    }

    private void UpdateCodeDisplay()
    {
        if (codeDigits.Length != 3)
        {
            Debug.LogError("Certifique-se de que todos os Texts de dígitos foram atribuídos no inspector.");
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            codeDigits[i].text = codeEntered[i].ToString(); 
        }
        CheckCode();
    }

    private void CheckCode()
    {
        for (int i = 0; i < 3; i++)
        {
            if (codeEntered[i] != correctCode[i])
            {
                return; 
            }
        }
        OpenDoor(); 
    }

    private void OpenDoor()
    {
        Debug.Log("Porta aberta!");
        codePanel.SetActive(false);
        targetObject.GetComponent<Animator>().SetBool("IsClose", false);
        if (targetObject != null)
        {
            Collider2D targetCollider = targetObject.GetComponent<Collider2D>();
            if (targetCollider != null)
            {
                targetCollider.enabled = false; 
                Debug.Log("Collider do objeto alvo desativado.");
            }
            else
            {
                Debug.LogError("O objeto alvo não tem um Collider 2D.");
            }
        }
        
        gameObject.SetActive(false); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            codePanel.SetActive(false); 
        }
    }
}
