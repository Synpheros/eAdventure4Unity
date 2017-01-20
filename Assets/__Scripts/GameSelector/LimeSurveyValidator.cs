﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LimeSurveyValidator : MonoBehaviour {

    Net connection;
    string host = "localhost";
	string survey_pre = "", survey_post = "";

    public Text token, response;
    
	void Start () {
        connection = new Net(this);

        SimpleJSON.JSONNode hostfile = new SimpleJSON.JSONClass();

#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
        if (!System.IO.File.Exists("host.cfg"))
            hostfile.Add("limesurvey_host", "localhost:4000");
        else
            hostfile = SimpleJSON.JSON.Parse(System.IO.File.ReadAllText("host.cfg"));
#endif
		try{
	        host = hostfile["limesurvey_host"];
	        survey_pre = hostfile["limesurvey_pre"];
			survey_post = hostfile["limesurvey_post"];
		}catch(Exception ex){}

        PlayerPrefs.SetString("LimesurveyHost", host);
		if(survey_pre != "")
			PlayerPrefs.SetString("LimesurveyPre", survey_pre);
		if(survey_post != "")
        	PlayerPrefs.SetString("LimesurveyPost", survey_post);
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

        connection.GET(host + "validate?survey=" + survey_pre + ((token.Length>0)? "&token=" + token : ""), new ValidateListener(response, token));
    }

    public void completed()
    {
        string token = "";
        if (this.token != null)
            token = this.token.text;
        else if (PlayerPrefs.HasKey("LimesurveyToken"))
            token = PlayerPrefs.GetString("LimesurveyToken");

		string survey = PlayerPrefs.GetString ("LimesurveyPre");
		string type = "pre";

		if (PlayerPrefs.HasKey ("CurrentSurvey"))
			type = PlayerPrefs.GetString ("CurrentSurvey");

		if(type == "pre")
			survey = PlayerPrefs.GetString ("LimesurveyPre");
		else if(type == "post")
			survey = PlayerPrefs.GetString ("LimesurveyPost");

		connection.GET(host + "completed?survey=" + survey + ((token.Length > 0) ? "&token=" + token : ""), new CompleteListener(response, token));
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
            response.text = result["message"];
        }

        public void Result(string data)
        {
			PlayerPrefs.SetString("name", token);
            PlayerPrefs.SetString("LimesurveyToken", token);
            PlayerPrefs.Save();
			if (PlayerPrefs.HasKey ("LimesurveyPre")) {
				PlayerPrefs.SetString ("CurrentSurvey", "pre");
				SceneManager.LoadScene ("_Survey");
			}else
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
			response.text = result["message"];
        }

        public void Result(string data)
        {
			string type = "pre";

			if (PlayerPrefs.HasKey ("CurrentSurvey"))
				type = PlayerPrefs.GetString ("CurrentSurvey");

			if (type == "pre")
				SceneManager.LoadScene ("_Scene1");
			else if (type == "post")
				Application.Quit ();
        }
    }
}
