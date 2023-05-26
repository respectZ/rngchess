using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject ButtonVSAI;
    [SerializeField]
    private GameObject ButtonVSLocalPlayer;
    [SerializeField]
    private GameObject ButtonExit;

    // Scenes
    [SerializeField]
    private string VSAISceneName;
    [SerializeField]
    private string VSLocalPlayerSceneName;

    private void Start()
    {
        ButtonVSAI.GetComponent<Button>().onClick.AddListener(ButtonVSAIOnClick);
        ButtonVSLocalPlayer.GetComponent<Button>().onClick.AddListener(ButtonVSLocalPlayerOnClick);
        ButtonExit.GetComponent<Button>().onClick.AddListener(ButtonExitOnClick);
    }

    private void ButtonVSAIOnClick()
    {
        SceneManager.LoadScene(VSAISceneName);
    }

    private void ButtonVSLocalPlayerOnClick()
    {
        SceneManager.LoadScene(VSLocalPlayerSceneName);
    }

    private void ButtonExitOnClick()
    {
        Application.Quit();
    }
}