using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerDataManager
{
    public const string ADDRESS = "www.moistpretzel.com/KineticGlide/";
    private const string POST_SERVER_PATH = ADDRESS + "postserver.php";
    private const string REMOVE_SERVER_PATH = ADDRESS + "removeserver.php";
    private const string GET_SERVERS_PATH = ADDRESS + "getserver.php";
    private const string GET_LEVELS_PATH = ADDRESS + "getlevels.php";
    private const string GET_USER_DATA_PATH = ADDRESS + "getuserdata.php";
    private const string SET_USER_DATA_PATH = ADDRESS + "setuserdata.php";
    private const string LOGIN_PATH = ADDRESS + "login.php";
    private const string REGISTER_PATH = ADDRESS + "register.php";
    private const string POST_RECORD_PATH = ADDRESS + "postrecord.php";
    private const string GET_RECORD_PATH = ADDRESS + "getrecord.php";
    private const string GET_RECORD_LIST_PATH = ADDRESS + "getrecordlist.php";
    private const string GET_VERSION_PATH = ADDRESS + "version";

    public static IEnumerator GetVersion()
    {
        UnityWebRequest www = new UnityWebRequest(GET_VERSION_PATH);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (string.IsNullOrEmpty(www.error))
        {
            DataManager.GetInstance().SetVersion(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    public static IEnumerator GetLevels()
    {
        UnityWebRequest www = new UnityWebRequest(GET_LEVELS_PATH);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (string.IsNullOrEmpty(www.error))
        {
            DataManager.GetInstance().WriteLevels(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    public static IEnumerator GetUserData(int userID)
    {
        UnityWebRequest www = new UnityWebRequest(GET_USER_DATA_PATH + "?id=" + userID);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.downloadHandler.text);
            DataManager.GetInstance().WriteStats(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    public static IEnumerator SetUserData(string query)
    {
        UnityWebRequest www = new UnityWebRequest(SET_USER_DATA_PATH + query);
        Debug.Log(SET_USER_DATA_PATH + query);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
        }
    }

    public static IEnumerator Login(string email, string password)
    {
        UnityWebRequest www = new UnityWebRequest(LOGIN_PATH + "?email=" + email + "&password=" + password);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (string.IsNullOrEmpty(www.error))
        {
            if (www.downloadHandler.text != "0")
            {
                DataManager.userID = int.Parse(www.downloadHandler.text);
                DataManager.GetInstance().Login(email, password);
            }
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    public static IEnumerator Login(string email, string password, Login sender)
    {
        UnityWebRequest www = new UnityWebRequest(LOGIN_PATH + "?email=" + email + "&password=" + password);
        Debug.Log(LOGIN_PATH + "?email=" + email + "&password=" + password);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;

        if (string.IsNullOrEmpty(www.error))
        {
            if (www.downloadHandler.text != "0")
            {
                DataManager.userID = int.Parse(www.downloadHandler.text);
                DataManager.GetInstance().Login(email, password);
            }
            else
            {
                sender.Error();
            }
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    public static IEnumerator Register(string email, string username, string password, Register sender)
    {
        UnityWebRequest www = new UnityWebRequest(REGISTER_PATH + "?email=" + email + "&password=" + password + "&username=" + username);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (string.IsNullOrEmpty(www.error))
        {
            if (www.downloadHandler.text == "0")
            {
                DataManager.username = username;
                DataManager.GetInstance().Login(email, password);
            }
            else
            {
                Debug.Log("Error");
                sender.ShowError(int.Parse(www.downloadHandler.text));
            }
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    public static IEnumerator PublishRecord(int userid, string levelname, string time)
    {
        UnityWebRequest www = new UnityWebRequest(POST_RECORD_PATH + "?id=" + userid + "&level=" + levelname + "&time=" + time);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();
    }

    public static IEnumerator GetRecord(int userid, string levelname)
    {
        UnityWebRequest www = new UnityWebRequest(GET_RECORD_PATH + "?id=" + userid + "&level=" + levelname);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (string.IsNullOrEmpty(www.error))
        {
            float min = float.Parse(www.downloadHandler.text.Split(':')[0]);
            float sec = float.Parse(www.downloadHandler.text.Split(':')[1]) + (min * 60);
            Timer.instance.SetRecordTime(www.downloadHandler.text, sec);
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    public static IEnumerator GetRecordList(string levelname)
    {
        UnityWebRequest www = new UnityWebRequest(GET_RECORD_LIST_PATH + "?level=" + levelname);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (string.IsNullOrEmpty(www.error))
        {
            HighscoresManager.instance.LoadScores(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    public static IEnumerator PostServers(int userID, List<string> addresses)
    {
        string request = POST_SERVER_PATH + $"?id={userID}&name={(DataManager.serverName == string.Empty ? DataManager.username + "\'s Server" : DataManager.serverName)}";

        for (int i = 1; i <= addresses.Count; i++)
        {
            request += $"&ip{i}={addresses[i - 1]}";
        }
        Debug.Log(request);
        UnityWebRequest www = new UnityWebRequest(request);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
        }
    }

    public static IEnumerator UpdateServer(int userID, int playerCount)
    {
        string request = POST_SERVER_PATH + $"?id={userID}&players={playerCount}";

        UnityWebRequest www = new UnityWebRequest(request);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
        }
    }

    public static IEnumerator RemoveServers(int userID)
    {
        UnityWebRequest www = new UnityWebRequest(REMOVE_SERVER_PATH + $"?id={userID}");

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
        }
    }

    public static IEnumerator RemoveServers(int userID, bool close)
    {
        UnityWebRequest www = new UnityWebRequest(REMOVE_SERVER_PATH + $"?id={userID}");

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
        }
        if (close)
        {
            Application.Quit();
        }
    }

    public static IEnumerator GetServers()
    {
        UnityWebRequest www = new UnityWebRequest(GET_SERVERS_PATH);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (string.IsNullOrEmpty(www.error))
        {
            GameObject.FindObjectOfType<ServerListManager>().GetResponses(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError(www.error);
        }
    }
}
