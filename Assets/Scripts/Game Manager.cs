using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; 
public enum GameState
{
    Intro,
    Playing,
    Dead
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State = GameState.Intro;

    public float PlayStartTime;
    public int Lives = 3;

    [Header("References")]
    public GameObject IntroUI;
    public GameObject DeadUI;
    public GameObject EnemySpawner;
    public GameObject FoodSpawner;
    public GameObject GoldenSpawner;

    private int startIssue = 0; 
    public Player PlayerScript;
    public TMP_Text ScoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    IEnumerator Start()
    {
        Lives = 3; // ✅ 첫 실행에서 Lives 초기화
        IntroUI.SetActive(true); 
        yield return new WaitForSeconds(0.1f);
        InitializeGame();
    }
    void InitializeGame()
    {
        Debug.Log("게임 초기화 완료!");
        // 초기 설정: 캐릭터 위치, UI, 게임 변수 초기화
        // ✅ 첫 실행일 때만 씬을 변경
        if (!PlayerPrefs.HasKey("firstRun"))
        {
            PlayerPrefs.SetInt("firstRun", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("main");
        } 
    }
 
    float CalculateScore()
    {
        return Time.time - PlayStartTime;
    }

    void SaveHighScore()
    {
        int score = Mathf.FloorToInt(CalculateScore());
        int currentHighScore = PlayerPrefs.GetInt("highScore", 0); //// : 기본값 설정 추가*** ////
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt("highScore", score);
            PlayerPrefs.Save();
        }
    }

    int GetHighScore()
    {
        return PlayerPrefs.GetInt("highScore", 0); //// : 기본값 설정 추가*** ////
    }

    public float CalculateGameSpeed()
    {
        if (State != GameState.Playing)
        {
            return 5f;
        }
        float speed = 8f + (0.5f * Mathf.Floor(CalculateScore() / 10f));
        return Mathf.Min(speed, 30f);
    }

    void Update()
    {
        if (State == GameState.Playing)
        {
            ScoreText.text = "score: " + Mathf.FloorToInt(CalculateScore());
        }
        else if (State == GameState.Dead)
        {
            ScoreText.text = "High Score: " + GetHighScore(); //// : High Score 제대로 표시*** ////
        }


        if (State == GameState.Intro && Input.GetKeyDown(KeyCode.Space))
        {

            State = GameState.Playing;
            IntroUI.SetActive(false);
            EnemySpawner.SetActive(true);
            FoodSpawner.SetActive(true);
            GoldenSpawner.SetActive(true);
            PlayStartTime = Time.time;
        }
        if (State == GameState.Playing && Lives == 0)
        {
            PlayerScript.KillPlayer();
            EnemySpawner.SetActive(false);
            FoodSpawner.SetActive(false);
            GoldenSpawner.SetActive(false);
            DeadUI.SetActive(true);
            SaveHighScore(); //// : 게임이 끝날 때 High Score 저장*** ////
            State = GameState.Dead;
        }
        if (State == GameState.Dead && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("main");
        }
    }
}
 