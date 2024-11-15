using System;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public Text itemText; // UI Text para mostrar a quantidade de itens
    public Text healthCostText; // UI Text para mostrar o custo de +1 vida
    public Text damageCostText; // UI Text para mostrar o custo de +1 dano
    public Text shootTimeCostText; // UI Text para mostrar o custo de diminuir o tempo de disparo

    public int healthCost = 2; // Custo para comprar +1 vida
    public int damageCost = 5; // Custo para comprar +1 dano
    public int shootTimeCost = 4; // Custo para diminuir o tempo de disparo
    public int dashCost = 8; // Custo para comprar o dash
    public int doubleJumpCost = 10; // Custo para comprar o double jump

    public Text dashCostText; // UI Text para mostrar o custo do dash
    public Text doubleJumpCostText; // UI Text para mostrar o custo do double jump
    public Text playerHealthText; // UI Text para mostrar a saúde do jogador
    public Text playerDamageText; // UI Text para mostrar o dano do jogador
    public Text shootTimeText; // UI Text para mostrar o tempo de disparo
    public Text DashEstaBlock;
    public Text DoubleJumpEstablock;

    private int playerItems;

    void Start()
    {
        UpdateUI();
        dashCostText.text = "Dash Está bloqueado";
        DoubleJumpEstablock.text = "Double Jump Está bloqueado";
    }

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            playerItems = GameManager.Instance.score;
            UpdateUI();
        }
    }

    public void BuyHealth()
    {
        if (GameManager.Instance != null && playerItems >= healthCost)
        {
            playerItems -= healthCost;
            GameManager.Instance.VidaMaxima += 2; // Aumenta a saúde do jogador
            GameManager.Instance.score = playerItems; // Atualiza o score no GameManager
            UpdateUI();
            Debug.Log("Bought Health: Remaining items = " + playerItems);
        }
        else
        {
            Debug.Log("Not enough items to buy Health.");
        }
    }

    public void BuyDamage()
    {
        if (GameManager.Instance != null && playerItems >= damageCost)
        {
            playerItems -= damageCost;
            GameManager.Instance.PlayerDmage += 1; // Aumenta o dano do jogador
            GameManager.Instance.score = playerItems; // Atualiza o score no GameManager
            UpdateUI();
            Debug.Log("Bought Damage: Remaining items = " + playerItems);
        }
        else
        {
            Debug.Log("Not enough items to buy Damage.");
        }
    }

    public void BuyShootTimeReduction()
    {
        if (GameManager.Instance != null && playerItems >= shootTimeCost)
        {
            playerItems -= shootTimeCost;
            GameManager.Instance.attackInterval -= 0.1f; // Reduz o tempo de disparo
            if (GameManager.Instance.attackInterval < 0.1f) 
                GameManager.Instance.attackInterval = 0.1f; // Limita o tempo mínimo de disparo
            GameManager.Instance.score = playerItems; // Atualiza o score no GameManager
            UpdateUI();
            Debug.Log("Bought Shoot Time Reduction: Remaining items = " + playerItems);
        }
        else
        {
            Debug.Log("Not enough items to buy Shoot Time Reduction.");
        }
    }
    
    public void BuyDash()
    {
        if (GameManager.Instance != null && playerItems >= dashCost && GameManager.Instance.hasDash == false)
        {
            playerItems -= dashCost;
            GameManager.Instance.hasDash = true; // Ativa a habilidade de dash
            GameManager.Instance.score = playerItems; // Atualiza o score no GameManager
            DashEstaBlock.text = "Dash Desbloqueado";
            UpdateUI();
            
        }
        else
        {
            Debug.Log("Not enough items to buy Dash.");
        }
    }

    public void BuyDoubleJump()
    {
        if (GameManager.Instance != null && playerItems >= doubleJumpCost && GameManager.Instance.hasDoubleJump == false)
        {
            playerItems -= doubleJumpCost;
            GameManager.Instance.hasDoubleJump = true; // Ativa a habilidade de double jump
            GameManager.Instance.score = playerItems; // Atualiza o score no GameManager
            DoubleJumpEstablock.text = "Double Jump Desbloqueado";
            UpdateUI();
            Debug.Log("Bought Double Jump: Remaining items = " + playerItems);
        }
    }

    void UpdateUI()
    {
        if (GameManager.Instance != null)
        {
            itemText.text = "Você Tem: " + playerItems + " Saquinhos";
            healthCostText.text = "Comprar +1 Vida: " + healthCost + " Saquinhos";
            damageCostText.text = "Comprar +1 Dano: " + damageCost + " Saquinho";
            shootTimeCostText.text = "Velocidade de Ataque: " + shootTimeCost + " Saquinho";
            dashCostText.text = "Comprar Dash: " + dashCost + " Saquinhos";
            doubleJumpCostText.text = "Comprar Double Jump: " + doubleJumpCost + " Saquinhos";

            playerHealthText.text = "Você Tem: " + GameManager.Instance.VidaMaxima + "Vida Maxima";
            playerDamageText.text = "Você Tem: " + GameManager.Instance.PlayerDmage + "Dano";
            shootTimeText.text = "Você Tem: " + GameManager.Instance.attackInterval.ToString("F1") + "s" + "Velocidade de Ataque";
        }
    }
}
