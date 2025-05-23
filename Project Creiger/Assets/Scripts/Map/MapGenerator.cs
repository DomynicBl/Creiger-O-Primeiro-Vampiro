using UnityEngine;
using System.Collections.Generic; // Para usar List
using UnityEngine.SceneManagement; // Para reiniciar a cena


// Definindo uma classe para representar um Nó do grafo (plataforma)
public class PlatformNode
{
    public Vector2 position; // Posição central da plataforma
    public GameObject platformObject; // Referência ao objeto GameObject da plataforma
    public List<PlatformNode> neighbors; // Plataformas "vizinhas" (alcançáveis)

    public PlatformNode(Vector2 pos, GameObject obj)
    {
        position = pos;
        platformObject = obj;
        neighbors = new List<PlatformNode>();
    }

    public void AddNeighbor(PlatformNode neighbor)
    {
        if (!neighbors.Contains(neighbor))
        {
            neighbors.Add(neighbor);
        }
    }
}

public class MapGenerator : MonoBehaviour
{
    // Prefabs que serão usados para construir o mapa
    public GameObject platformPrefab;
    public GameObject BlueEnemy;
    public GameObject PinkEnemy;

    // Tamanho de cada bloco/plataforma (assumindo que são blocos quadrados de tamanho unitário)
    public float platformWidth = 5;
    public float platformHeight = 0.71f;

    private Vector2 currentSpawnPosition; // Posição atual para instanciar o próximo bloco

    // Lista para armazenar todos os nós das plataformas geradas (o grafo)
    private List<PlatformNode> platformGraph;

    /*
        Funcao principal: Gerencia criacao do mapa
    */
    void Start()
    {
        bool conectividade = false;
        int antLoop = 0;
        do
        {
            platformGraph = new List<PlatformNode>();
            // Inicializa a posição de spawn (pode ser ajustada para onde você quer que o mapa comece)
            currentSpawnPosition = new Vector2(-5f, -2f); // começa em X = -5, Y = -2

            float randonSize = Random.Range(12, 20);
            for (int i = 0; i < randonSize; i++)
            {
                GeneratePlatformSegment();
            }

            // Log de verificação
            Debug.Log("Grafo de Plataformas Gerado. Total de nós: " + platformGraph.Count);

            // Verificação de Conectividade do Grafo usando BFS
            if (platformGraph.Count > 0)
            {
                conectividade = CheckMapConnectivity();
            }
            antLoop++;
        } while ((conectividade == true) && (antLoop < 10) );
    }

    /*
        Método para verificar a conectividade do mapa
        Algoritmo base: BFS
    */
    bool CheckMapConnectivity()
    {
        bool resposta = false;

        if (platformGraph.Count != 0)
        {

            Queue<PlatformNode> queue = new Queue<PlatformNode>();
            HashSet<PlatformNode> visitedNodes = new HashSet<PlatformNode>();

            // Começa a busca a partir da primeira plataforma gerada
            PlatformNode startNode = platformGraph[0];
            queue.Enqueue(startNode);
            visitedNodes.Add(startNode);

            while (queue.Count > 0)
            {
                PlatformNode currentNode = queue.Dequeue();

                foreach (PlatformNode neighbor in currentNode.neighbors)
                {
                    if (!visitedNodes.Contains(neighbor))
                    {
                        visitedNodes.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            if (visitedNodes.Count == platformGraph.Count)
            {
                Debug.Log("Todas as plataformas estão conectadas!");
                resposta = true;
            }
            else
            {
                Debug.LogWarning("AVISO: Existem plataformas desconectadas no mapa! (" + (platformGraph.Count - visitedNodes.Count) + " ilhas).");
            }
        }

        return resposta;
    }

    void GeneratePlatformSegment()
    {
        float randomDistance = Random.Range(10, 15); // Distância horizontal aleatória entre plataformas (agora faz mais sentido com o nome)

        float randomY = Random.Range(-5, 5); // Variação Y aleatória entre plataformas
        if (currentSpawnPosition.y >= 5) randomY = Random.Range(-5, 0);
        if (currentSpawnPosition.y <= -5) randomY = Random.Range(0, 5);

        // Armazena a posição da plataforma ANTES de atualizar currentSpawnPosition
        Vector2 platformPosition = currentSpawnPosition;
        GameObject newPlatform = Instantiate(platformPrefab, platformPosition, Quaternion.identity, this.transform);

        // Cria um novo nó para a plataforma e adiciona ao grafo
        PlatformNode newPlatformNode = new PlatformNode(platformPosition, newPlatform);

        // Adiciona arestas ao grafo (conectividade)
        if (platformGraph.Count > 0)
        {
            PlatformNode lastPlatformNode = platformGraph[platformGraph.Count - 1];
            float jumpReachX = 5f; // alcance horizontal máximo do pulo
            float jumpReachY = 5f; // alcance vertical máximo do pulo (para cima ou para baixo)

            float distToLastX = Mathf.Abs(newPlatformNode.position.x - lastPlatformNode.position.x);
            float distToLastY = Mathf.Abs(newPlatformNode.position.y - lastPlatformNode.position.y);

            if (distToLastX <= jumpReachX && distToLastY <= jumpReachY)
            {
                lastPlatformNode.AddNeighbor(newPlatformNode);
                newPlatformNode.AddNeighbor(lastPlatformNode); // Se a conexão é bidirecional
            }
        }
        platformGraph.Add(newPlatformNode); // Adiciona o nó da nova plataforma ao grafo

        // Atualiza a posição para a PRÓXIMA plataforma
        currentSpawnPosition.x += platformWidth + randomDistance;
        currentSpawnPosition.y += randomY;

        // Gera entre 0 e 2 inimigos em cima da plataforma
        int numEnemiesToSpawn = Random.Range(0, 3);
        for (int i = 0; i < numEnemiesToSpawn; i++)
        {
            GenerateEnemy(platformPosition.x, platformPosition.y);
        }
    }

    void GenerateEnemy(float platPX, float platPY)
    {
        int EnemyType = Random.Range(0, 2);
        float enemySpawnY = platPY + platformHeight / 2f + 0.6f;
        float randomEnemyXOffset = Random.Range(-15, platformWidth - 10); // Offset a partir do ponto de spawn da plataforma
        Vector2 enemyPosition = new Vector2(platPX + randomEnemyXOffset, enemySpawnY);

        if (EnemyType == 0)
        {
            Instantiate(BlueEnemy, enemyPosition, Quaternion.identity, this.transform);
        }
        else
        {
            Instantiate(PinkEnemy, enemyPosition, Quaternion.identity, this.transform);
        }
    }
}