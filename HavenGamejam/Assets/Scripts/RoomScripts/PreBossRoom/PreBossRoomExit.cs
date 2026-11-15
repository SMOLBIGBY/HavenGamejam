using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PreBossRoomExit : MonoBehaviour
{
    BlackScreenFade blackScreenScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blackScreenScript = FindObjectOfType<BlackScreenFade>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            StartCoroutine(LoadNextScene(0.5f));
        }
    }

    IEnumerator LoadNextScene(float delay)
    {
        blackScreenScript.FadeIn(0.5f);
        yield return new WaitForSeconds(delay);


        SceneManager.LoadScene("BossFight");
        // Load the next scene
    }
}
