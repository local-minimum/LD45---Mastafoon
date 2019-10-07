using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSummation : MonoBehaviour
{
    [SerializeField] GameObject container;
    [SerializeField] TMPro.TextMeshProUGUI title;
    [SerializeField] TMPro.TextMeshProUGUI capturesValue;
    [SerializeField] TMPro.TextMeshProUGUI turnsValue;
    [SerializeField] TMPro.TextMeshProUGUI stepsValue;
    [SerializeField] TMPro.TextMeshProUGUI escapeesValue;
    [SerializeField] TMPro.TextMeshProUGUI totalValue;

    private void Awake()
    {
        container.SetActive(false);
    }

    public void Show(string levelName, LevelStats stats, System.Action<int> reportScore)
    {
        title.text = levelName;
        capturesValue.text = "";
        turnsValue.text = "";
        stepsValue.text = "";
        escapeesValue.text = "";
        totalValue.text = "";
        StartCoroutine(PlayScore(stats, reportScore));
    }

    [SerializeField] float playScoreDelay = 0.2f;
    [SerializeField] float finalDelay = 3f;

    IEnumerator<WaitForSeconds> PlayScore(LevelStats stats, System.Action<int> reportScore)
    {
        container.SetActive(true);
        int totalScore = 0;
        int partScore = 0;
        yield return new WaitForSeconds(playScoreDelay);

        partScore = Mathf.Max(0, 4 - stats.captures) * 100;
        capturesValue.text = partScore.ToString();
        totalScore += partScore;
        yield return new WaitForSeconds(playScoreDelay);

        partScore = Mathf.Max(0, 40 - stats.turns) * 10;
        turnsValue.text = partScore.ToString();
        totalScore += partScore;
        yield return new WaitForSeconds(playScoreDelay);

        partScore = Mathf.Max(0, 80 - stats.steps) * 10;
        stepsValue.text = partScore.ToString();
        totalScore += partScore;
        yield return new WaitForSeconds(playScoreDelay);

        partScore = stats.escapees * 200;
        escapeesValue.text = partScore.ToString();
        totalScore += partScore;
        yield return new WaitForSeconds(playScoreDelay);

        totalValue.text = totalScore.ToString();
        yield return new WaitForSeconds(finalDelay);
        container.SetActive(false);
        reportScore(totalScore);
    }
}
