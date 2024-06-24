//COINCOLLECTION
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinCollection : MonoBehaviour
{
    private int Coin = 0;
    public TextMeshProUGUI coinText;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Coin")
        {
            Coin++;
            coinText.text = "Coin: " + Coin.ToString();
            Debug.Log(Coin);
            Destroy(other.gameObject);
        }
    }
}
//--------------------------------COLLISION HANDLER-------------------------------------
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Video;

public class CollisionHandler : MonoBehaviour
{
    public GameObject panel;
    public GameObject questionPanel;
    public TMP_Text title;
    public VideoPlayer videoPlayer;
    public List<TagPanelPair> tagPanelPairs = new List<TagPanelPair>();
    public QuizManager quizManager;
    public AudioSource audioSource;
    public List<AudioClip> cubeAudios;



    [System.Serializable]
    public class TagPanelPair
    {
        public string tag;
        public string titleString;
        public string url;
    }

    private void Start()
    {
        foreach (var pair in tagPanelPairs)
        {
            panel.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var pair in tagPanelPairs)
        {
            if (collision.gameObject.CompareTag(pair.tag))
            {
                audioSource.clip = GetRandomClip();
                panel.SetActive(true);
                if (title != null)
                {
                    title.text = pair.titleString;
                    videoPlayer.url = pair.url;
                    videoPlayer.Play();
                }
            }
            else if (collision.gameObject.CompareTag("Question"))
            {
                questionPanel.SetActive(true);
                quizManager.DisplayQuestion(0);
            }
        }

    }

    private AudioClip GetRandomClip()
    {
        if (cubeAudios.Count == 0)
        {
            Debug.LogError("Cube Audios list is empty!");
            return null;
        }

        int randomIndex = Random.Range(0, cubeAudios.Count);
        AudioClip randomClip = cubeAudios[randomIndex];
        return randomClip;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            foreach (var pair in tagPanelPairs)
            {
                panel.SetActive(false);
            }
        }
    }
}
//--------------------------------QUIZ MANAGER--------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_Text result;

    public string nextLevel;
    public Button[] answerButtons;

    public Question[] questions; // Array of user-defined questions

    private int currentQuestionIndex = 0;

    private int correctAnswers;
    private int wrongAnswers;
    private int totalQuestion;
    // newly added
    [SerializeField] GameObject resultCanvas;
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text statusText;
    int answeredquestion;

    [SerializeField] GameObject quizcanvas;

    [SerializeField] Button nextLevelbutton, retry;

    private void Start()
    {
        nextLevelbutton.gameObject.SetActive(false);
        retry.gameObject.SetActive(false);
        resultCanvas.SetActive(false);
        nextLevelbutton.onClick.AddListener(moveNextlevel);
        retry.onClick.AddListener(() => resultCanvas.SetActive(false));
    }

    private void moveNextlevel()
    {
        SceneManager.LoadScene(nextLevel);
    }
    // newly added


    public void DisplayQuestion(int questionIndex)
    {
        totalQuestion = questions.Length;
        if (questionIndex < questions.Length)
        {
            questionText.text = questions[questionIndex].questionText;

            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerButtons[i].GetComponentInChildren<TMP_Text>().text = questions[questionIndex].answerOptions[i];
                int answerIndex = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => CheckAnswer(answerIndex));
            }
        }
        else
        {
            ValidateScore();
            Debug.Log("Quiz completed!");

        }
    }

    private void CheckAnswer(int selectedAnswerIndex)
    {
        answeredquestion++;
        if (selectedAnswerIndex == questions[currentQuestionIndex].correctAnswerIndex)
        {
            result.gameObject.SetActive(true);
            result.text = "Correct answer!";
            Debug.Log("Correct answer!");
            correctAnswers++;
        }
        else
        {
            int correctanswer = questions[currentQuestionIndex].correctAnswerIndex;
            result.gameObject.SetActive(true);
            result.text = "Wrong Answer," + "Correct Answer is option : " + (correctanswer + 1);
            Debug.Log("Wrong answer!");
            wrongAnswers++;
        }
        Invoke("DisplayNextQuestion", 2);
    }

    private void DisplayNextQuestion()
    {
         if (answeredquestion == questions.Length)
        {
            quizcanvas.SetActive(false);
            resultCanvas.SetActive(true);
            score.text = correctAnswers + "/" + totalQuestion;
        }
        result.gameObject.SetActive(false);
        currentQuestionIndex++;
        DisplayQuestion(currentQuestionIndex);
    }

    private void ValidateScore()
    {
        int avergequestion = totalQuestion / 2;
        if (correctAnswers > avergequestion)
        {
            if (nextLevel != null)
            {
                statusText.text = "You passed the level";
                nextLevelbutton.gameObject.SetActive(true);
                retry.gameObject.SetActive(false);
            }
        }
        else
        {
            nextLevelbutton.gameObject.SetActive(false);
            retry.gameObject.SetActive(true);
            statusText.text = "you not have enough scoe,Retry";
        }
    }
}

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] answerOptions;
    public int correctAnswerIndex; // Index of the correct answer in the answerOptions array
}
//------------------------------------VIDEO MANAGER------------------------------------------------------------
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoplayerParent;

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer not assigned.");
            return;
        }

        videoPlayer.loopPointReached += EndReached;
        videoPlayer.Play();
    }

    void EndReached(VideoPlayer vp)
    {
        videoplayerParent.SetActive(false);
        Debug.Log("Video Over");
    }
}
