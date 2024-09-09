using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab do inimigo a ser instanciado
    public Transform spawnPoint1; // Ponto de spawn 1
    public Transform spawnPoint2; // Ponto de spawn 2
    public int enemiesToSpawn = 2; // Quantidade de inimigos a serem instanciados inicialmente
    private int enemiesAlive = 0; // Contador de inimigos vivos
    private Transform player; // Referência ao jogador
    public Transform areaLimit; // Referência ao limite de área para inimigos

    void Start()
    {
        // Encontra o jogador na cena
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SpawnEnemies(enemiesToSpawn); // Instancia os inimigos iniciais
    }

    void SpawnEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // Alterna entre os pontos de spawn
            Transform spawnPoint = (i % 2 == 0) ? spawnPoint1 : spawnPoint2;

            // Instancia o inimigo na posição do ponto de spawn selecionado
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            enemy.GetComponent<DodgeEnemy>().OnDeath += HandleEnemyDeath; // Subscrição ao evento de morte do inimigo

            // Define as referências ao jogador e à área de limite para o inimigo instanciado
            DodgeEnemy dodgeEnemy = enemy.GetComponent<DodgeEnemy>();
            dodgeEnemy.player = player;

            enemiesAlive++;
        }
    }

    void HandleEnemyDeath()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            SpawnEnemies(enemiesToSpawn); // Reinstancia mais 2 inimigos quando todos morrerem
        }
    }
}