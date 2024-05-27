using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [Header("MainMenu")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button scoreButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [Header("PopUp")]
    [SerializeField] private GameObject popUpGO;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField pseudoInput;

    private void Awake()
    {
        playButton.onClick.AddListener(Play);
        scoreButton.onClick.AddListener(Score);
        creditsButton.onClick.AddListener(Credits);
        quitButton.onClick.AddListener(Quit);
        continueButton.onClick.AddListener(Continue);
        backButton.onClick.AddListener(Back);
    }

    private void Continue()
    {
        UIManager.Instance.RegisterNewPseudo(pseudoInput.text);
        Debug.Log($"Pseudo : {pseudoInput.text}");
        GameManager.Instance.StartGame();
    }

    private void Back()
    {
        popUpGO.SetActive(false);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(playButton.gameObject); 
    }

    private void Play()
    {
        popUpGO.SetActive(true);
        pseudoInput.Select();
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

    private void OnEnable()
    {
        popUpGO.SetActive(false);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(playButton.gameObject);
    }
}
