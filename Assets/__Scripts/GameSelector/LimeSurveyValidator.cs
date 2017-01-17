using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LimeSurveyValidator : MonoBehaviour {

    Net connection;
    string host = "localhost";
    string survey_pre;

    public Text token, response;
    
	void Start () {
        connection = new Net(this);

        SimpleJSON.JSONNode hostfile = new SimpleJSON.JSONClass();

#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
        if (!System.IO.File.Exists("host.cfg"))
            hostfile.Add("limesurvey_host", "localhost/limesurvey/");
        else
            hostfile = SimpleJSON.JSON.Parse(System.IO.File.ReadAllText("host.cfg"));
#endif

        host = hostfile["limesurvey_host"];
        survey_pre = hostfile["limesurvey_pre"];

        PlayerPrefs.SetString("LimesurveyHost", host);
        PlayerPrefs.SetString("LimesurveyPre", survey_pre);
        PlayerPrefs.SetString("LimesurveyPost", survey_pre);
        PlayerPrefs.Save();
    }
    
    void Update () {
	
	}

    public void validate()
    {
        string token = "";
        if (this.token != null)
            token = this.token.text;
        else if (PlayerPrefs.HasKey("LimesurveyToken"))
            token = PlayerPrefs.GetString("LimesurveyToken");

        connection.GET(host + "validator.php?survey=" + survey_pre + ((token.Length>0)? "&token=" + token : ""), new ValidateListener(response, token));
    }

    public void completed()
    {
        string token = "";
        if (this.token != null)
            token = this.token.text;
        else if (PlayerPrefs.HasKey("LimesurveyToken"))
            token = PlayerPrefs.GetString("LimesurveyToken");

        connection.GET(host + "completed.php?survey=" + survey_pre + ((token.Length > 0) ? "&token=" + token : ""), new CompleteListener(response, token));
    }

    public class ValidateListener : Net.IRequestListenerErrorData
    {
        Text response;
        string token;

        public ValidateListener(Text response, string token)
        {
            this.response = response;
            this.token = token;
        }

        public void Error(string error, string data)
        {
            SimpleJSON.JSONNode result = SimpleJSON.JSON.Parse(data);
            response.text = result["error"];
        }

        public void Result(string data)
        {
            PlayerPrefs.SetString("LimesurveyToken", token);
            PlayerPrefs.Save();
            if(PlayerPrefs.HasKey("LimesurveyPre"))
                SceneManager.LoadScene("_Survey");
            else
                SceneManager.LoadScene("_Scene1");
        }
    }

    public class CompleteListener : Net.IRequestListenerErrorData
    {
        Text response;
        string token;

        public CompleteListener(Text response, string token)
        {
            this.response = response;
            this.token = token;
        }

        public void Error(string error, string data)
        {
            SimpleJSON.JSONNode result = SimpleJSON.JSON.Parse(data);
            response.text = result["error"];
        }

        public void Result(string data)
        {
            SceneManager.LoadScene("_Scene1");
        }
    }
}
