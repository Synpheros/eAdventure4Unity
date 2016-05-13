using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;


public class LoadFilesController : MonoBehaviour {

    /*string url = "file:///";
    string fileName = "video.mp4";
    string[] files;

    RawImage player;
    AudioSource sound;
    MovieTexture movie;


    void Start () {
        player = GetComponent<Renderer> ();
        //sound = GetComponent<AudioSource> ();

        files = Directory.GetFiles (Game.Instance.selected_path);
        foreach (string file in files){
            //Debug.Log(file);
        }

        StartCoroutine (loadMovie ());
    }


    public void setMovie(string movieName){
        fileName = movieName;
        StartCoroutine (loadMovie ());
    }


    IEnumerator loadMovie(){
        WWW www = new WWW (url + Game.Instance.selected_path + fileName);
        yield return www;
        Debug.Log (www.movie.duration + " <--- wwww");
        if (www.error != null) {
            Debug.Log ("Error: Can't laod movie! - " + www.error);
            yield break;

        } else {
            MovieTexture video = www.movie as MovieTexture;
            Debug.Log("Movie loaded");
            Debug.Log(www.movie);
            movie = video;
            setMovie();
            playMovie();
        }
    }


    public void setMovie(){
        Debug.Log (movie.name + " ------");
        player.texture = movie;
        sound.clip = movie.audioClip;
    }

    public void playMovie(){
        movie.Play ();
        sound.Play ();
    }

    public void pauseMovie(){
        movie.Pause ();
        sound.Pause ();
    }
    */


}