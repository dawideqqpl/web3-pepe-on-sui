using UnityEngine;
using UnityEngine.UI;
using OPS;
using OPS.AntiCheat;
using OPS.AntiCheat.Field;
using OPS.AntiCheat.Prefs;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public Player player;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;

    public ProtectedInt16 score;
    public ProtectedInt16 Score => score;
    public ProtectedInt16 bonusScore;
    public TextMeshProUGUI bonusScoreText;
    public GameObject summaryObj;
    public GameObject bonusPointsObj;
    public GameObject closeBtn;
    public GameObject loadingObj;
    public ProtectedInt16 highscore;
    public TextMeshProUGUI leaderboardSendText;
    public TextMeshProUGUI summaryGameOverText;
    public GameObject playBtn;
    public ProtectedBool isLife;
    public ProtectedInt16 lifes;
    public ProtectedInt16 multiplier;
    public GameObject buyLifesBtn;
    public GameObject buyMultiplierBtn;

    public GameObject startObject;
    public GameObject playObject;
    public ProtectedInt16 currentLifes;


    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            Application.targetFrameRate = 144;
            DontDestroyOnLoad(gameObject);
            Pause();
        }
    }

    public void Update()
    {
        if(lifes == 2)
        {
            buyLifesBtn.GetComponent<Button>().interactable = false;
        }
        if (multiplier == 2)
        {
            buyMultiplierBtn.GetComponent<Button>().interactable = false;
        }
    }


    public void Start()
    {
        GameManager.Instance.multiplier = 1;
        GameManager.Instance.lifes = 1;
        player.gameObject.SetActive(false);
    }

    public void StartSceneClicked()
    {
        player.gameObject.SetActive(true);

    }

    public void Play()
    {
        if (isLife == false)
        {


            currentLifes = lifes;
            score = 0;
            bonusScore = 0;
        }
        //leaderboardSendText.gameObject.SetActive(false);
        isLife = true;
        scoreText.gameObject.SetActive(true);
        bonusPointsObj.gameObject.SetActive(true);
        
        
        bonusScoreText.text = bonusScore.ToString();

        scoreText.text = score.ToString();
        summaryObj.gameObject.SetActive(false);
        playButton.SetActive(false);
        gameOver.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;

        MemesScript[] memes = FindObjectsOfType<MemesScript>();

        for (int i = 0; i < memes.Length; i++)
        {
            Destroy(memes[i].gameObject);
        }

        Pipes[] pipes = FindObjectsOfType<Pipes>();

        for (int i = 0; i < pipes.Length; i++) {
            Destroy(pipes[i].gameObject);
        }
    }

    public void SecondChance()
    {
        Pause();
        summaryGameOverText.text = "+" + bonusScore;
        leaderboardSendText.text = "Score: " + score;
        player.transform.position = new Vector3(0, 0, 0);
        playButton.SetActive(true);


        MemesScript[] memes = FindObjectsOfType<MemesScript>();

        for (int i = 0; i < memes.Length; i++)
        {
            Destroy(memes[i].gameObject);
        }

        Pipes[] pipes = FindObjectsOfType<Pipes>();

        for (int i = 0; i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }
    }

    public void GameOver()
    {
        isLife = false;
        closeBtn.gameObject.SetActive(true);
        bonusPointsObj.gameObject.SetActive(false);
        summaryObj.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        playButton.SetActive(true);
        player.transform.position = new Vector3(0, 0, 0);

        gameOver.SetActive(true);

        summaryGameOverText.text = "+"+ bonusScore;
        leaderboardSendText.text = "Score: " +  score;
       
        if (highscore < score)
        {

            highscore = score;
        }

        if (bonusScore > 0 || highscore < score)
        {


            PlayFabManager._Instance.ExecuteCloudScriptSendPointsCode();
        }

       





        Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }

    public void GameOverClose()
    {
        Pipes[] pipes = FindObjectsOfType<Pipes>();

        for (int i = 0; i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }

        MemesScript[] memes = FindObjectsOfType<MemesScript>();

        for (int i = 0; i < memes.Length; i++)
        {
            Destroy(memes[i].gameObject);
        }
    }

    public void IncreaseScore()
    {

        if (isLife == true)
        {
           // PlayFabManager._Instance.nomnomnomSound.Play();
            if (multiplier == 1)
            {
                score++;


            }
            if(multiplier == 2)
            {
                score += 2;
            }
            scoreText.text = score.ToString();
            int chanceToBonus;

            chanceToBonus = Random.Range(0, 100);

         // if (chanceToBonus <= GameManager2._Instance.currentChance)
         // {
         //     bonusScore++;
         // }
         // bonusScoreText.text = bonusScore.ToString();
        }
    }

}
