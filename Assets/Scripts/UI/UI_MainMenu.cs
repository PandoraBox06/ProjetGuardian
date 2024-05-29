using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [Header("MainMenu")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(Play);
        creditsButton.onClick.AddListener(Credits);
        quitButton.onClick.AddListener(Quit);
    }

    private void Play()
    {
        GameManager.Instance.StartGame();
    }

    private void Credits()
    {
        UIManager.Instance.OpenOnePanel(PanelsNames.Credits);
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(playButton.gameObject);
    }
}
