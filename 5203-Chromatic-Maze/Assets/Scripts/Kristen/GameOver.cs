using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    static GameObject tap;

    void Start()
    {
        tap = GameObject.Find("tap");
    }

    public void GoToMenu()
    {
        tap.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(0);
    }

    public void GoToInstructions()
    {
        tap.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(1);
    }

    public void LoadLevel()
    {
        tap.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(2);
    }

    public void setUp()
    {
        tap.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(3);
    }

}
