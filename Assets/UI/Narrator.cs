using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StoryPiece
{
    public string key;
    public Sprite profilePicture;
    public string characterName;
    public string body;
}


public class Narrator : MonoBehaviour
{
    [SerializeField]
    StoryPiece[] pieces;
    [SerializeField]
    Image profilePicture;
    [SerializeField]
    TMPro.TextMeshProUGUI characterName;
    [SerializeField]
    TMPro.TextMeshProUGUI body;
    string mostRecentKey;
    
    public void ShowPieceByKey(string key)
    {
        for (int i=0; i<pieces.Length; i++)
        {
            StoryPiece piece = pieces[i];
            if (piece.key == key)
            {
                DisplayPiece(piece);
                return;
            }
        }
        Debug.LogError(string.Format("Could not find story piece '{0}'", key));
    }

    void DisplayPiece(StoryPiece piece)
    {
        characterName.text = piece.characterName;
        body.text = piece.body;
        profilePicture.sprite = piece.profilePicture;
        profilePicture.enabled = true;
        mostRecentKey = piece.key;
    }

    public void ClearDisplay(string key)
    {
        if (key == mostRecentKey) ClearDisplay();
    }

    public void ClearDisplay()
    {
        characterName.text = "";
        body.text = "";
        profilePicture.enabled = false;
        mostRecentKey = "";
    }
}
