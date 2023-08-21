using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Linq;


public class APIController : StaticInstance<APIController>
{


    public string url = "https://localhost:44380/";
    
    GameManager GM { get { return GameManager.Instance; } }
    
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator GetPlayerCount()
    {
        UnityWebRequest webReq = UnityWebRequest.Get($"{url}api/GetPlayerCount");
        yield return webReq.SendWebRequest();

        if (webReq.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webReq.error);
        }
        else
        {
            Debug.Log(webReq.downloadHandler.text);
            int playerCount = int.Parse(webReq.downloadHandler.text);
            GM.PlayerCount = playerCount;
        }
    }
    public IEnumerator GetQuestionCount()
    {
        UnityWebRequest webReq = UnityWebRequest.Get($"{url}api/GetQuestionCount");
        yield return webReq.SendWebRequest();

        if (webReq.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webReq.error);
        }
        else
        {
            Debug.Log(webReq.downloadHandler.text);
            int questionCount = int.Parse(webReq.downloadHandler.text);
            GM.QuestionCount = questionCount;
        }
    }

    public IEnumerator GetQuestion(int id)
    {
        UnityWebRequest webReq = UnityWebRequest.Get($"{url}api/GetQuestion/{id}");
        yield return webReq.SendWebRequest();

        if(webReq.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webReq.error);
        }
        else
        {
            Debug.Log(webReq.downloadHandler.text);
            QuestionJSON question = JsonUtility.FromJson<QuestionJSON>(webReq.downloadHandler.text);
            if(question != null )
            {
                GM.Questions.Add(question);

            }
        }
    }

    public IEnumerator InsertPlayer(string name, int loginTime)
    {
        UnityWebRequest webReq = UnityWebRequest.Get($"{url}api/SetPlayers?name={name}&time={loginTime}");
        yield return webReq.SendWebRequest();

        if (webReq.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webReq.error);
        }
    }

    public IEnumerator GetPlayerID(string name, int loginTime)
    {
        UnityWebRequest webReq = UnityWebRequest.Get($"{url}api/GetPlayerID?name={name}&loginTime={loginTime}");
        yield return webReq.SendWebRequest();

        if (webReq.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webReq.error);
        }
        else
        {
            Debug.Log(webReq.downloadHandler.text);
            int playerID = int.Parse(webReq.downloadHandler.text);
            GM.playerID = playerID;
        }
    }

    public IEnumerator UpdatePlayerScore(int playerId, int score)
    {
        UnityWebRequest webReq = UnityWebRequest.Get($"{url}api/UpdatePlayerScore?playerId={playerId}&score={score}");
        yield return webReq.SendWebRequest();

        if (webReq.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webReq.error);
        }
    }

    public IEnumerator DeletePlayer(int playerID)
    {
        UnityWebRequest webReq = UnityWebRequest.Get($"{url}api/DeletePlayer?playerID={playerID}");
        yield return webReq.SendWebRequest();

        if (webReq.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webReq.error);
        }
    }

    public IEnumerator GetPlayerScore(int playerId)
    {
        UnityWebRequest webReq = UnityWebRequest.Get($"{url}api/GetPlayerScore?playerId={playerId}");
        yield return webReq.SendWebRequest();

        if (webReq.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webReq.error);
        }
        else
        {
            Debug.Log(webReq.downloadHandler.text);
            int score = int.Parse(webReq.downloadHandler.text);
            GM.OpponentScore = score;
        }
    }

    public IEnumerator GetPlayerTime(int playerId)
    {
        UnityWebRequest webReq = UnityWebRequest.Get($"{url}api/GetPlayerTime?playerId={playerId}");
        yield return webReq.SendWebRequest();

        if (webReq.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webReq.error);
        }
        else
        {
            Debug.Log(webReq.downloadHandler.text);
            int score = int.Parse(webReq.downloadHandler.text);
            GM.Seed = score;
        }
    }


}
