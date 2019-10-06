using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoader : MonoBehaviour
{
    private void Awake()
    {
        const string uiSceneName = "GameUI";
        Scene ui = SceneManager.GetSceneByName(uiSceneName);
        
        if (!ui.isLoaded) SceneManager.LoadScene(uiSceneName);
    }
}
