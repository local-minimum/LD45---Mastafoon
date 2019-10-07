using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalScorer : MonoBehaviour
{

    [SerializeField] TMPro.TextMeshProUGUI scoreText;
    int showingScore = 0;
    int score = 0;

    void Start()
    {
        scoreText.text = "0";
    }

    public void AddScore(int score)
    {
        this.score += score;
    }

    public void ClearScore()
    {
        score = 0;
        showingScore = 0;
        scoreText.text = "0";
    }

    void Update()
    {
        if (showingScore != score)
        {
            int step = Mathf.Abs(showingScore - score) > 100 ? 10 : 1;
            if (score > showingScore)
            {
                showingScore += step;
            } else
            {
                showingScore -= step;
            }
            scoreText.text = showingScore.ToString();
        }
    }
}
