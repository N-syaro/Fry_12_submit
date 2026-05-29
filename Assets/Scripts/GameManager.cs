using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ScoreText;

    private int currentScore = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdataScoreText();
    }
    public void AddScore(int score)
    {
        currentScore += score;

    }
    private void UpdataScoreText()
    {
        ScoreText.text ="SCORE:" + currentScore.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        UpdataScoreText();
    }
}
