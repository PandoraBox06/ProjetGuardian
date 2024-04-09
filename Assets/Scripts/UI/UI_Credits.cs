using UnityEngine;
using UnityEngine.UI;

public class UI_Credits : MonoBehaviour
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
