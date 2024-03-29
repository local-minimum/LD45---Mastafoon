﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class LevelStats
{
    public int steps;
    public int turns;
    public int captures;
    public int escapees;

    public LevelStats(int steps, int turns, int captures, int escapes)
    {
        this.steps = steps;
        this.turns = turns;
        this.captures = captures;
        this.escapees = escapes;
    }
}

public class SessionManager : MonoBehaviour
{
    [SerializeField] LevelSummation levelSummation;
    [SerializeField] AudioSource speakers;
    [SerializeField] AudioClip closeSound;
    [SerializeField] AudioClip openSound;

    List<LevelStats> levelStats = new List<LevelStats>();

    [SerializeField] Animator anim;
    [SerializeField]
    string[] levels;

    [SerializeField] int currentLevel;

    [SerializeField] ActionsInventory inventory;

    int levelRestarts;
    int stepsTaken;
    int turnsPassed;

    private void OnEnable()
    {
        DisableSurrendering();
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
                StartCoroutine(DelayOpenScene());
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
                    StartCoroutine(DelayOpenScene());
                    return;
                }
            }
            SceneManager.LoadScene(levels[currentLevel], LoadSceneMode.Additive);
            return;
        }
        Debug.LogWarning(string.Format("Unhandled Scene {0}, forgot to add to SessionManager.levels?", scene.name));
    }

    float delayOpen = 1f;

    IEnumerator<WaitForSeconds> DelayOpenScene()
    {
        WorldClock worldClock = FindObjectOfType<WorldClock>();
        worldClock.GiveTurnTo(Turn.None);
        yield return new WaitForSeconds(delayOpen);
        speakers.PlayOneShot(openSound);
        anim.SetTrigger("Open");
        worldClock.GiveTurnTo(Turn.Player);
        EnableSurrendering();
    }

    private void SceneManager_sceneUnloaded(Scene scene)
    {
        Debug.Log(string.Format("Unloaded level: {0}", scene));
        if (currentLevel < levels.Length)
        {
            SceneManager.LoadScene(levels[currentLevel], LoadSceneMode.Additive);            
        } else {
            anim.SetTrigger("End");
        }   
    }

    void UpdateTurnsAndSteps()
    {
        Character character = FindObjectOfType<Character>();
        stepsTaken += character.stepsTaken;
        WorldClock worldClock = FindObjectOfType<WorldClock>();
        turnsPassed += worldClock.turnsTaken;
    }

    void ResetTurnsAndSteps()
    {
        stepsTaken = 0;
        turnsPassed = 0;
    }

    public void ReportLevelCompleted()
    {
        DisableSurrendering();
        speakers.PlayOneShot(closeSound);
        anim.SetTrigger("Close");
        UpdateTurnsAndSteps();
        int escapes = 1 + inventory.itemCount;
        LevelStats stats = new LevelStats(stepsTaken, turnsPassed, levelRestarts, escapes);
        levelStats.Add(stats);        
        ResetTurnsAndSteps();
        levelSummation.Show(levels[currentLevel], stats, LoadNextLevel);
    }

    public void ImprisonCharacter()
    {
        DisableSurrendering();
        levelRestarts += 1;
        UpdateTurnsAndSteps();
        Debug.Log(string.Format("Death: {0} Restarts, {1} Turns, {2} Steps", levelRestarts, this.turnsPassed, this.stepsTaken));
        
        StartCoroutine(ReloadLevel());
    }    

    void LoadNextLevel()
    {  
        inventory.ForgetCharacter();
        inventory.ClearInventory();
        SceneManager.UnloadSceneAsync(levels[currentLevel]);
        delayOpen = 0.5f;
        currentLevel++;
    }

    IEnumerator<WaitForSeconds> ReloadLevel()
    {
        delayOpen = 2f;        
        yield return new WaitForSeconds(1f);
        speakers.PlayOneShot(closeSound);
        anim.SetTrigger("Jail");
        SceneManager.UnloadSceneAsync(levels[currentLevel]);
        inventory.ForgetCharacter();
        inventory.ClearInventory();
    }


    bool surrendering = false;

    float surrenderStart = 0;
    float surrenderEnd = 0;
    [SerializeField]
    float timeToSurrender = 2f;
    [SerializeField]
    Button surrenderButton;
    [SerializeField]
    Image surrenderProgress;

    public void DisableSurrendering()
    {
        surrendering = false;
        surrenderStart = Time.timeSinceLevelLoad;
        surrenderButton.interactable = false;
    }

    public void EnableSurrendering()
    {
        surrenderStart = Time.timeSinceLevelLoad;
        surrenderButton.interactable = true;
    }

    public void SurrenderStart()
    {
        if (surrenderButton.interactable)
        {
            surrenderStart = Time.timeSinceLevelLoad - Mathf.Max(0f, ((surrenderEnd - surrenderStart) - (Time.timeSinceLevelLoad - surrenderEnd)) / timeToSurrender);
            surrendering = true;
        }
    }

    public void SurrenderStop()
    {
        if (surrenderButton.interactable)
        {
            surrenderEnd = Time.timeSinceLevelLoad;
            surrendering = false;
        }
    }

    public void UpdateSurrendering()
    {
        float progress = 0f;
        if (surrendering)
        {
            progress = Mathf.Min(1f, (Time.timeSinceLevelLoad - surrenderStart) / timeToSurrender);
        } else
        {
            progress = Mathf.Max(0f, ((surrenderEnd - surrenderStart) - (Time.timeSinceLevelLoad - surrenderEnd)) / timeToSurrender);
        }
        surrenderProgress.fillAmount = progress;
        if (progress == 1f)
        {
            ImprisonCharacter();
        }
    }

    private void CheckSurrenderHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SurrenderStart();
        } else if (Input.GetKeyUp(KeyCode.R))
        {
            SurrenderStop();
        }
    }

    private void Update()
    {
        CheckSurrenderHotkeys();
        UpdateSurrendering();
    }

    public void QuitToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
