using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button scoreButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(Play);
        scoreButton.onClick.AddListener(Score);
        creditsButton.onClick.AddListener(Credits);
        quitButton.onClick.AddListener(Quit);
    }

    private void Play()
    {
        GameManager.Instance.RestartGame();
    }

    private void Score()
    {
        UIManager.Instance.OpenPopUpPanel(PanelsNames.Score);
    }

    private void Credits()
    {
        UIManager.Instance.OpenOnePanel(PanelsNames.Credits);
    }

    private void Quit()
    {
        Application.Quit();
    }
}
