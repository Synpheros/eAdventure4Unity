using UnityEngine;
using System.Collections;

public class WebOpener : MonoBehaviour {
	void Start () {
	}
	void Update () {
	}

    public void OpenWeb(string web)
    {
        Application.OpenURL(web);
    }

    public void OpenSurvey(string type)
    {
        if(type == "pre")
        {
            string url = PlayerPrefs.GetString("LimesurveyHost") + PlayerPrefs.GetString("LimesurveyPre") + "?token=" + PlayerPrefs.GetString("LimesurveyToken");
            if (!url.Contains("http://") && !url.Contains("https://"))
                url = "http://" + url;

            Application.OpenURL(url);
        }
    }
}
