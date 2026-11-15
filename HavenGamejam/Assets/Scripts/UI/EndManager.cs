using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class EndManager : MonoBehaviour
{
    [SerializeField] private GameObject retryButton;
    BlackScreenFade blackScreenFade;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blackScreenFade = FindAnyObjectByType<BlackScreenFade>();
        retryButton.SetActive(false);
        StartCoroutine(ShowButton(3f));
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void Retry()
    {
        StartCoroutine(StartTheGame(1f));
    }

    IEnumerator StartTheGame(float delay)
    {
        blackScreenFade.FadeIn(0.5f);
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainScreen");
    }

    IEnumerator ShowButton(float delay)
    {
        yield return new WaitForSeconds(delay);
        retryButton.SetActive(true);
    }
}
