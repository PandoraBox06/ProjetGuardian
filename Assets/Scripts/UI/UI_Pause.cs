using UnityEngine.UI;
using UnityEngine;

public class UI_Pause : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(Play);
        quitButton.onClick.AddListener(Quit);
    }

    private void Play()
    {
        //replay
    }

    private void Quit()
    {
        UIManager.Instance.OpenOnePanel(PanelsNames.MainMenu);
    }
}
