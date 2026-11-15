using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainRoomStart : MonoBehaviour
{
    BlackScreenFade blackScreenScript;
    [SerializeField] private GameObject EyeAnimation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blackScreenScript = FindObjectOfType<BlackScreenFade>();
        StartCoroutine(LoadAnimation(2f, 0.5f, 1.5f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator LoadAnimation(float delay, float seconddelay, float thirddelay)
    {

        yield return new WaitForSeconds(delay);
        blackScreenScript.FadeIn(0.5f);
        yield return new WaitForSeconds(seconddelay);
        EyeAnimation.SetActive(false);
        yield return new WaitForSeconds(thirddelay);
        blackScreenScript.FadeOut(0.5f);

        // Load the next scene
    }
}
