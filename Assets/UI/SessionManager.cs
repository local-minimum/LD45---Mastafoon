using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : MonoBehaviour
{
    [SerializeField]
    string[] levels;

    [SerializeField] int currentLevel;

    [SerializeField] ActionsInventory inventory;

    int levelRestarts;
    int stepsTaken;
    int turnsPassed;

    private void Awake()
    {
 
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        const string uiSceneName = "GameUI";
        for (int i=0; i<levels.Length; i++)
        {
            string lvl = levels[i];
            if (scene.name == lvl)
            {
                currentLevel = i;
                inventory.NewTurn();
                Debug.Log(string.Format("Loaded level: {0}", lvl));
                return;
            }
        }
        if (scene.name == uiSceneName)
        {
            Debug.Log("Loaded UI");
            for (int i = 0; i < levels.Length; i++)
            {
                string lvl = levels[i];
                Scene lvlScene = SceneManager.GetSceneByName(lvl);
                if (lvlScene.isLoaded)
                {
                    currentLevel = i;
                    Debug.Log(string.Format("Playing on: {0}", lvl));
                    return;
                }
            }
            SceneManager.LoadScene(levels[currentLevel], LoadSceneMode.Additive);
            return;
        }
        Debug.LogWarning(string.Format("Unhandled Scene {0}, forgot to add to SessionManager.levels?", scene.name));
    }

    private void SceneManager_sceneUnloaded(Scene scene)
    {
        Debug.Log(string.Format("Unloaded level: {0}", scene));
        if (currentLevel < levels.Length)
        {
            SceneManager.LoadScene(levels[currentLevel], LoadSceneMode.Additive);            
        }        
    }

    public void ReportLevelCompleted(int stepsTaken, int turnsPassed)
    {
        this.stepsTaken += stepsTaken;
        this.turnsPassed += turnsPassed;
        Debug.Log(string.Format("Completed: {0} Restarts, {1} Turns, {2} Steps", levelRestarts, this.turnsPassed, this.stepsTaken));

        this.stepsTaken = 0;
        this.turnsPassed = 0;        
        LoadNextLevel();
    }

    public void KillCharacter(int stepsTaken, int turnsPassed)
    {
        levelRestarts += 1;
        this.stepsTaken += stepsTaken;
        this.turnsPassed += turnsPassed;
        Debug.Log(string.Format("Death: {0} Restarts, {1} Turns, {2} Steps", levelRestarts, this.turnsPassed, this.stepsTaken));

        ReloadLevel();
    }

    void LoadNextLevel()
    {
        inventory.ForgetCharacter();
        inventory.ClearInventory();
        SceneManager.UnloadSceneAsync(levels[currentLevel]);
        currentLevel++;
    }

    void ReloadLevel(bool unload=true)
    {
        inventory.ForgetCharacter();
        inventory.ClearInventory();
        SceneManager.UnloadSceneAsync(levels[currentLevel]);
    }
}
