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
    [SerializeField] Animator anim;
    [SerializeField]
    StoryPiece[] pieces;
    [SerializeField]
    Image profilePicture;
    [SerializeField]
    TMPro.TextMeshProUGUI characterName;
    [SerializeField]
    TMPro.TextMeshProUGUI body;
    string mostRecentKey;

    static Narrator _instance;
    public static Narrator instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Narrator>();
            }
            return _instance;
        }

        private set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        } else if (_instance != this)
        {
            Debug.LogError(string.Format("Two narrators active (keeping {0}, destroying {1})", _instance, this));
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }


    public static void ShowPieceByKey(string key)
    {
        instance._ShowPieceByKey(key);
    }

    void _ShowPieceByKey(string key)
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
        if (anim) anim.SetTrigger("Flash");

    }

    public static void ClearDisplay(string key)
    {
        if (key == instance.mostRecentKey) ClearDisplay();
    }

    public static void ClearDisplay()
    {
        instance._ClearDisplay();
    }

    void _ClearDisplay()
    {
        characterName.text = "";
        body.text = "";
        profilePicture.enabled = false;
        mostRecentKey = "";
        if (anim) anim.SetTrigger("NoContent");
    }
}
