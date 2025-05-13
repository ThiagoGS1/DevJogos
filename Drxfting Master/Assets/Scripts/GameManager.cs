using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public enum GameStates { countDown, running, raceOver };
public enum SceneType { Race, Dialogue };

[System.Serializable]
public class GameScene
{
    public string sceneName;
    public SceneType sceneType;
}

public class GameManager : MonoBehaviour
{
    //Static instance of GameManager so other scripts can access it
    public static GameManager instance = null;

    //States
    GameStates gameState = GameStates.countDown;

    //Time
    float raceStartedTime = 0;
    float raceCompletedTime = 0;

    //Driver information
    List<DriverInfo> driverInfoList = new List<DriverInfo>();

    //Events
    public event Action<GameManager> OnGameStateChanged;

    // Sequência de cenas do jogo (diálogos e fases)
    [SerializeField]
    private List<GameScene> gameSequence = null;

    private int currentSequenceIndex = 0;
    
    // Manter compatibilidade com código existente
    public string[] raceScenes { 
        get {
            List<string> races = new List<string>();
            foreach (var scene in gameSequence) {
                if (scene.sceneType == SceneType.Race)
                    races.Add(scene.sceneName);
            }
            return races.ToArray();
        }
    }

    // Nome da cena de seleção de carro
    public string carSelectionScene = "Menu";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Initialize scene sequence programmatically
        if (gameSequence == null)
            gameSequence = new List<GameScene>();

        // Clear list to remove any unwanted serialized scenes
        gameSequence.Clear();

        // Add only the scenes we want to use
        gameSequence.Add(new GameScene { sceneName = "Dialogue1", sceneType = SceneType.Dialogue });
        gameSequence.Add(new GameScene { sceneName = "Dialogue2", sceneType = SceneType.Dialogue });
        gameSequence.Add(new GameScene { sceneName = "fase", sceneType = SceneType.Race });
        gameSequence.Add(new GameScene { sceneName = "Dialogue3", sceneType = SceneType.Dialogue });
        gameSequence.Add(new GameScene { sceneName = "Dialogue4", sceneType = SceneType.Dialogue });
        gameSequence.Add(new GameScene { sceneName = "Dialogue5", sceneType = SceneType.Dialogue });
        gameSequence.Add(new GameScene { sceneName = "fase1", sceneType = SceneType.Race });
        gameSequence.Add(new GameScene { sceneName = "fase2", sceneType = SceneType.Race });
        gameSequence.Add(new GameScene { sceneName = "DialogueWin", sceneType = SceneType.Dialogue });

        CheckSceneAvailability();
    }

    private void CheckSceneAvailability()
    {
        List<string> availableScenes = new List<string>();
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            availableScenes.Add(sceneName.ToLower()); // Converte para minúsculo
            Debug.Log("Cena disponível no Build Settings: " + sceneName);
        }
        
        // Verificar se as cenas da sequência existem
        foreach (var scene in gameSequence)
        {
            if (!availableScenes.Contains(scene.sceneName.ToLower())) // Converte para minúsculo
            {
                Debug.LogError("ATENÇÃO: A cena '" + scene.sceneName + "' não está no Build Settings!");
            }
        }
        
        // Verificar se a cena de seleção de carro existe
        if (!availableScenes.Contains(carSelectionScene.ToLower())) // Converte para minúsculo
        {
            Debug.LogError("ATENÇÃO: A cena de seleção de carro '" + carSelectionScene + "' não está no Build Settings!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Supply dummy driver information for testing purposes
        if (driverInfoList.Count == 0)
        {
            driverInfoList.Add(new DriverInfo(1, "Takumi Fujiwara (You)", 0, false));
        }
    }

    void LevelStart()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("Level started: " + currentScene);
        
        // Se estamos em uma cena de corrida, configura o estado
        if (IsRaceScene(currentScene))
        {
            gameState = GameStates.countDown;
        }
        
        // Encontra o índice atual na sequência
        for (int i = 0; i < gameSequence.Count; i++)
        {
            if (gameSequence[i].sceneName == currentScene)
            {
                currentSequenceIndex = i;
                break;
            }
        }
        
        Debug.Log("Índice atual na sequência: " + currentSequenceIndex);
    }

    // Verifica se a cena é de corrida
    private bool IsRaceScene(string sceneName)
    {
        foreach (var scene in gameSequence)
        {
            if (scene.sceneName == sceneName && scene.sceneType == SceneType.Race)
                return true;
        }
        return false;
    }

    // Obtém o índice da próxima cena de corrida
    private int GetNextRaceIndex()
    {
        for (int i = currentSequenceIndex + 1; i < gameSequence.Count; i++)
        {
            if (gameSequence[i].sceneType == SceneType.Race)
                return i;
        }
        return -1;
    }

    public GameStates GetGameState()
    {
        return gameState;
    }

    void ChangeGameState(GameStates newGameState)
    {
        if (gameState != newGameState)
        {
            gameState = newGameState;
            Debug.Log("Game state changed to: " + newGameState);

            //Invoke game state change event
            OnGameStateChanged?.Invoke(this);
        }
    }

    public float GetRaceTime()
    {
        if (gameState == GameStates.countDown)
            return 0;
        else if (gameState == GameStates.raceOver)
            return raceCompletedTime - raceStartedTime;
        else 
            return Time.time - raceStartedTime;
    }

    //Driver information handling
    public void ClearDriversList()
    {
        driverInfoList.Clear();
    }

    public void AddDriverToList(int playerNumber, string name, int carUniqueID, bool isAI)
    {
        driverInfoList.Add(new DriverInfo(playerNumber, name, carUniqueID, isAI));
    }

    public void SetDriversLastRacePosition(int playerNumber, int position)
    {
        DriverInfo driverInfo = FindDriverInfo(playerNumber);
        if (driverInfo != null)
        {
            driverInfo.lastRacePosition = position;
        }
    }

    public void AddPointsToChampionship(int playerNumber, int points)
    {
        DriverInfo driverInfo = FindDriverInfo(playerNumber);
        if (driverInfo != null)
        {
            driverInfo.championshipPoints += points;
        }
    }

    DriverInfo FindDriverInfo(int playerNumber)
    {
        foreach (DriverInfo driverInfo in driverInfoList)
        {
            if (playerNumber == driverInfo.playerNumber)
                return driverInfo;
        }

        Debug.LogError($"FindDriverInfoBasedOnDriverNumber failed to find driver for player number {playerNumber}");
        return null;
    }

    public List<DriverInfo> GetDriverList()
    {
        return driverInfoList;
    }

    public void OnRaceStart()
    {
        Debug.Log("OnRaceStart");
        raceStartedTime = Time.time;
        ChangeGameState(GameStates.running);
    }

    public void OnRaceCompleted()
    {
        Debug.Log("OnRaceCompleted");
        raceCompletedTime = Time.time;
        ChangeGameState(GameStates.raceOver);
        
        // Inicia a rotina para carregar a próxima fase após um delay
        StartCoroutine(LoadNextScene());
    }

    // Método para iniciar a sequência do jogo
    public void StartGameSequence()
    {
        currentSequenceIndex = 0;
        LoadSceneByIndex(currentSequenceIndex);
    }

    // Carrega a próxima cena na sequência
    public IEnumerator LoadNextScene()
    {
        // Aguarda alguns segundos para mostrar resultados, etc.
        yield return new WaitForSeconds(5f);
        
        // Avança para a próxima cena na sequência
        currentSequenceIndex++;
        Debug.Log("Tentando carregar próxima cena: índice " + currentSequenceIndex);
        
        // Verifica se ainda há cenas disponíveis
        if (currentSequenceIndex < gameSequence.Count)
        {
            LoadSceneByIndex(currentSequenceIndex);
        }
        else
        {
            // Todas as cenas foram completadas, volta para o menu de seleção de carro
            Debug.Log("Sequência de jogo completada. Voltando para a seleção de carro.");
            SceneManager.LoadScene(carSelectionScene);
        }
    }

    // Carrega uma cena pelo seu índice na sequência
    private void LoadSceneByIndex(int index)
    {
        if (index >= 0 && index < gameSequence.Count)
        {
            string nextScene = gameSequence[index].sceneName;
            Debug.Log("Nome original da cena: " + nextScene);
            // Se houver alguma transformação do nome da cena, você verá aqui
            Debug.Log("Nome final da cena a ser carregada: " + nextScene);
            
            if (SceneExists(nextScene))
            {
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                Debug.LogError("Cena não encontrada: " + nextScene + ". Voltando para a seleção de carro.");
                SceneManager.LoadScene(carSelectionScene);
            }
        }
    }

    private bool SceneExists(string sceneName)
    {
        Debug.Log("Verificando se a cena existe: " + sceneName);
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            // Compara ignorando maiúsculas/minúsculas
            if (string.Equals(sceneNameFromBuild, sceneName, StringComparison.OrdinalIgnoreCase) ||
                scenePath.IndexOf(sceneName, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Debug.Log("Cena encontrada: " + sceneName);
                return true;
            }
        }
        Debug.LogError("Cena não encontrada no Build Settings: " + sceneName);
        return false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LevelStart();
    }
}