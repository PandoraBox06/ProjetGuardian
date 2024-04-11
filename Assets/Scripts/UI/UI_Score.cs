using UnityEngine;
using UnityEngine.UI;

public class UI_Score : MonoBehaviour
{
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        quitButton.onClick.AddListener(Quit);
    }

    private void Quit()
    {
        UIManager.Instance.OpenOnePanel(PanelsNames.MainMenu);
    }
}
