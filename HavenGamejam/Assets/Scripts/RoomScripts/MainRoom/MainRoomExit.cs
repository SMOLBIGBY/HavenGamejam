using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainRoomExit : MonoBehaviour
{
    BlackScreenFade blackScreenScript;
    [SerializeField] private GameObject DoorAnimation;
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
            
            StartCoroutine(LoadNextScene(0.5f, 2f, 2f));
        }
    }

    IEnumerator LoadNextScene(float delay, float seconddelay, float thirddelay)
    {
        blackScreenScript.FadeIn(0.5f);
        yield return new WaitForSeconds(delay);
        blackScreenScript.FadeOut(0.5f);
        DoorAnimation.SetActive(true);
        yield return new WaitForSeconds(seconddelay);
        blackScreenScript.FadeIn(0.5f);
        yield return new WaitForSeconds(thirddelay);
        SceneManager.LoadScene("EndScreen");
        // Load the next scene
    }
}
