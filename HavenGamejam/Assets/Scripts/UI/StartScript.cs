using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class StartScript : MonoBehaviour
{
    BlackScreenFade blackScreenFade;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blackScreenFade = FindAnyObjectByType<BlackScreenFade>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        StartCoroutine(StartTheGame(1f));
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    IEnumerator StartTheGame(float delay)
    {
        blackScreenFade.FadeIn(0.5f);
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainRoom");
    }
}
