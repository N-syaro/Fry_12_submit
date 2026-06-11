using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI TimeText;
    [SerializeField] Player player;
    private int currentScore = 0;
    private float remainingTime = 30f;
    private bool isTimeOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdataScoreText();
    }
    void Update()
    {
        if (isTimeOver) return;
        UpdataScoreText();
        // タイマーを減らす
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            isTimeOver = true;
            player.IsEnd(true);
            Debug.Log("ゲーム終了！");
        }

        UpdateTimeText();
    }
    public void AddScore(int score)
    {
        currentScore += score;
    }
    private void UpdataScoreText()
    {
        ScoreText.text ="SCORE:" + currentScore.ToString();
    }
    private void UpdateTimeText()
    {
        TimeText.text = "TIME : " + Mathf.CeilToInt(remainingTime);
    }
}
