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
    
    private void OnEnable()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(quitButton.gameObject); 
    }
}
