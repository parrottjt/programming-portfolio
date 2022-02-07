using System.Collections;
using System.Collections.Generic;
using Helpers.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using String = System.String;

public static class SceneManagement
{
    static string _currentLevelName = String.Empty;
    static string _lastLevelName = String.Empty;

    public static bool hasSceneChanged;
    
    static List<AsyncOperation> _loadOperations = new List<AsyncOperation>();

    static Dictionary<GameStates, List<string>> gameScenes = new Dictionary<GameStates, List<string>>
    {
        {
            //Undo changes to World 1-3, 2-3 after build is released
            GameStates.Story, new List<string>
            {
                "Frikins Gym",
                "GoldFishBoss",
                "World 1-1",
                "World 1-2",
                "Octoboss",
                "World 2-1",
                "World 2-2",
                "Puff Daddy",
                "World 3-1",
                "World 3-2",
                "She-Reks",
                "World 4-1",
                "World 4-2",
                "Bionic Prawn",
                "World 5-1",
                "World 5-2",
                "Eel-etric Boogaloo",
                "World 6-1",
                "World 6-2",
                "Pirate Ship",
                "World 7-1",
                "World 7-2",
                "Killy",
                "World 8-1",
                "World 8-2",
                "Shark Family",
                "Frakin",
            }
        },
        {
            GameStates.BossRush, new List<string>
            {
                "GoldFishBoss Boss Rush",
                "SenseiStarfish Boss Rush",
                "Octoboss Boss Rush",
                "Glockadile Boss Rush",
                "PuffDaddy Boss Rush",
                // "TankSteg Boss Rush",
                // "She-Reks Boss Rush",
                // "Eel-etric Boogaloo Boss Rush",
                // "HammerHead Shark Boss Rush",
                // "Killy Boss Rush",
                // "Shark Family Boss Rush",
                //"Frakin Boss Rush"
            }
        }
    };
    
    public static string CurrentLevelName => _currentLevelName;
    public static string LastLevelName => _lastLevelName;
    public static string MainMenuName => "Menu";
    
    public static AsyncOperation LoadLevel(string levelName)
    {
        if (_loadOperations.Count == 1)
        {
            DebugScript.LogError(typeof(SceneManagement), "A load operation is already in progress");
            return null;
        }

        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName);
        if (ao == null)
        {
            DebugScript.LogError(typeof(SceneManagement), "Scene to Load doesn't exist");
            return null;
        }

        _loadOperations.Add(ao);
        ao.completed += OnLoadOperationComplete;

        if(_currentLevelName != levelName) _lastLevelName = _currentLevelName;
        SetCurrentLevelName(levelName);

        return ao;
    }

    public static string GetNextLevelToLoad()
    {
        string nextSceneName;
        var stateList = gameScenes[GameManager.gameState];
        var currentIndex = stateList.FindIndex(levelName => levelName == _currentLevelName);
        if (!gameScenes.ContainsKey(GameManager.gameState))
        {
            DebugScript.LogError(typeof(SceneManagement), 
                "Current GameMode isn't declared in Dictionary!\n Returning to Menu");
            nextSceneName = "Menu";
        }
        else if (currentIndex + 1 >= stateList.Count)
        {
            DebugScript.Log(typeof(SceneManagement), 
                "At the end of the declared scene list\n Returning to Menu");
            nextSceneName = "Menu";
        }
        else nextSceneName = stateList[currentIndex + 1];

        return nextSceneName;
    }

    public static string ChooseNextScene_BossRush(bool canLoadMenu, bool[] completedScenes)
    {
        while (true)
        {
            if (canLoadMenu) return "Menu";
            var stateList = gameScenes[GameStates.BossRush];
            var index = Random.Range(1, stateList.Count - 1);
            if(!completedScenes[index]) return stateList[index];
        }
    }

    static void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);

            hasSceneChanged = true;
        }

        DebugScript.Log(typeof(SceneManagement), "Load Complete");
    }
    
    public static void SetCurrentLevelName(string levelName) => _currentLevelName = levelName;
}