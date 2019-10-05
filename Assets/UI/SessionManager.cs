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
        LoadLevel(false);
    }

    public void ReportLevelCompleted(int stepsTaken, int turnsPassed)
    {
        this.stepsTaken += stepsTaken;
        this.turnsPassed += turnsPassed;
        Debug.Log(string.Format("Completed: {0} Restarts, {1} Turns, {2} Steps", levelRestarts, this.turnsPassed, this.stepsTaken));

        this.stepsTaken = 0;
        this.turnsPassed = 0;

        //TODO: increase index
        LoadLevel();
    }

    public void KillCharacter(int stepsTaken, int turnsPassed)
    {
        levelRestarts += 1;
        this.stepsTaken += stepsTaken;
        this.turnsPassed += turnsPassed;
        Debug.Log(string.Format("Death: {0} Restarts, {1} Turns, {2} Steps", levelRestarts, this.turnsPassed, this.stepsTaken));

        LoadLevel();
    }

    void LoadLevel(bool unload=true)
    {
        inventory.ForgetCharacter();
        inventory.ClearInventory();
        if (unload) SceneManager.UnloadSceneAsync(levels[currentLevel]);
        SceneManager.LoadScene(levels[currentLevel], LoadSceneMode.Additive);
    }
}
