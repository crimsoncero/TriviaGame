using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;



public enum Answer
{
    None,
    Correct,
    Wrong,
}

public class GameManager : StaticInstance<GameManager>
{
    public TextMeshProUGUI CategoryGenre;
    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI AnswerA;
    public TextMeshProUGUI AnswerB;
    public TextMeshProUGUI AnswerC;
    public TextMeshProUGUI AnswerD;
    public TextMeshProUGUI ClockText;
    public TextMeshProUGUI PlayerNameInput;
    public TextMeshProUGUI EndText;

    public GameObject LoginPanel;
    public GameObject QuestionPanel;
    public GameObject WaitPanel;
    public GameObject EndPanel;


    public int GameLength = 4; // Length of game in questions.
    public int QuestionTime = 10; // Time to Answer questions.
    public int MaxPoints = 10; // Maximum points per question.

    public List<QuestionJSON> Questions;
    [HideInInspector] public int Seed = 0;
    [HideInInspector] public int PlayerCount = 0;
    [HideInInspector] public int QuestionCount = 0;
    [HideInInspector] public int playerID = 0;
    [HideInInspector] public int MyScore = 0;
    [HideInInspector]public int OpponentScore = -1;
    public System.Random Rand { get; private set; }

    private float _timeRemaining = 0;
    private bool _timerRunning = false;
    private int _currentAnswer = 0;
    private int _currentAnswerTime = 0;
    private Answer _answer = Answer.None;

    APIController API { get { return APIController.Instance; } } 
    private int OpponentID { get { return playerID == 1 ? 2 : 1; } }

    void Start()
    {
        Questions = new List<QuestionJSON>();
        LoginPanel.SetActive(true);
        QuestionPanel.SetActive(false);
        WaitPanel.SetActive(false);
        EndPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_timerRunning)
        {
            if(_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time out");
                _timeRemaining = 0;
                _timerRunning = false;
            }
            ClockText.text = Mathf.FloorToInt(_timeRemaining % 60).ToString();
        }


    }


    public void SetPlayer()
    {
        if (PlayerNameInput.text == "")
        {
            Debug.Log("empty");
            return;
        }
        Debug.Log(PlayerNameInput.text);
        int time = (int) DateTime.Now.Ticks;
        StartCoroutine(StartGame(PlayerNameInput.text, time));


    }

    public void SetQuestion(int id)
    {
        _answer = Answer.None;
        _currentAnswerTime = 0;
        _currentAnswer = 0;

        QuestionJSON question = Questions[id];
        CategoryGenre.text = $"{question.Category} - {question.Genre}";
        QuestionText.text = $"{question.Question}";
        SetClock(QuestionTime);

        List<int> order = GenerateRandomListOrder(4);
        _currentAnswer = order.IndexOf(0);

        List<string> answers = new List<string>();
        answers.Add(question.Answer);
        answers.Add(question.False1);
        answers.Add(question.False2);
        answers.Add(question.False3);

        AnswerA.text = answers[order[0]];
        AnswerB.text = answers[order[1]];
        AnswerC.text = answers[order[2]];
        AnswerD.text = answers[order[3]];

    }

    public void ChooseAnswer(int answerId)
    {
        if (_answer != Answer.None) return; // already answered
        if (!_timerRunning) return; // Out of time.

        if(answerId == _currentAnswer)
        {
            Debug.Log("Correct");
            _currentAnswerTime = Mathf.CeilToInt(_timeRemaining % 60);
            _answer = Answer.Correct;
        }
        else
        {
            Debug.Log("Wrong");
            _answer = Answer.Wrong;
        }
    }

    IEnumerator StartGame(string name, int time)
    {
        // add player to game.
        LoginPanel.SetActive(false);
        WaitPanel.SetActive(true);

        yield return StartCoroutine(API.InsertPlayer(name, time));
        yield return StartCoroutine(API.GetPlayerID(name, time));
        Debug.Log("Player ID is = " + playerID);

        if(playerID == 1) // Wait for second player
        {
            Seed = time;
            while(PlayerCount != 2)
            {
                yield return StartCoroutine(API.GetPlayerCount());
                yield return new WaitForSecondsRealtime(0.2f);
            }
        }
        else
        {
            yield return StartCoroutine(API.GetPlayerTime(1)); // Get Seed from player 1
        }

        // Setup game:
        Rand = new System.Random(Seed); // Makes Random based on seed;
        yield return StartCoroutine(API.GetQuestionCount());
        List<int> questionOrder = GenerateRandomListOrder(QuestionCount, Rand); // Generate a list of integers from 0 to question count, and randomize their order.


        for(int i = 0; i < GameLength; i++) // Init questions.
        {
            yield return StartCoroutine(API.GetQuestion(questionOrder[i] + 1)); // Get the question which id corresponds to the value in the i index of questionOrder PLUS one. 
        }

        WaitPanel.SetActive(false);
        QuestionPanel.SetActive(true);


        // Game Loop

        for(int i = 0; i < GameLength; i++)
        {
            SetQuestion(i);
            while(_timerRunning && _answer == Answer.None) 
            {
                // Wait until either timer runs out, or the player answered.
                yield return new WaitForSeconds(0.1f); 
            } 

            if(_answer == Answer.Correct)
            {
                MyScore += PointsAdjustment(_currentAnswerTime, QuestionTime, MaxPoints);
            }
            Debug.Log("Score is : " + MyScore);
            yield return new WaitUntil(() => !_timerRunning);
        }

        // Score and output winner

        yield return StartCoroutine(API.UpdatePlayerScore(playerID, MyScore));
        while(OpponentScore == -1)
        {
            yield return StartCoroutine(API.GetPlayerScore(OpponentID));
            yield return new WaitForSeconds(0.2f);
        }
        yield return StartCoroutine(API.DeletePlayer(OpponentID));

        QuestionPanel.SetActive(false);
        EndPanel.SetActive(true);
        if(MyScore > OpponentScore)
        {
            EndText.text = "You Won!";
            Debug.Log("You win");
        }
        else
        {
            EndText.text = "You Lost!";
            Debug.Log("You lost");
        }


    }

    int PointsAdjustment(int timeRemaining, int totalTime, int maxPoints)
    {
        int pointTime = totalTime / maxPoints; // the number of seconds for each point.
         
        return (timeRemaining + (pointTime - timeRemaining % pointTime) / pointTime);
    }

    void SetClock(float time)
    {
        _timerRunning = true;
        _timeRemaining = time;
    }

    // Using unity random
    List<int> GenerateRandomListOrder(int size)
    {
        List<int> list = new List<int>();
        for(int i = 0; i < size; i++)
        {
            list.Add(i);
        }

        for (int i = list.Count - 1; i > 0; i--)
        {
            var k = UnityEngine.Random.Range(0, i + 1);
            var value = list[k];
            list[k] = list[i];
            list[i] = value;
        }
        return list;
    }

    // Using c# seeded random given
    List<int> GenerateRandomListOrder(int size, System.Random random)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < size; i++)
        {
            list.Add(i);
        }

        for (int i = list.Count - 1; i > 0; i--)
        {
            var k = random.Next(i + 1);
            var value = list[k];
            list[k] = list[i];
            list[i] = value;
        }
        return list;
    }
}
