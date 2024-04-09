using TMPro;
using UnityEngine;

public class UI_LeaderboardInfos : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pseudo;
    [SerializeField] private TextMeshProUGUI bestScore;
    [SerializeField] private TextMeshProUGUI myBestScore;
    [SerializeField] private TextMeshProUGUI myLastScore;
    
    private void OnEnable()
    {
        pseudo.text = UIManager.Instance.playerPseudo;
        bestScore.text = UIManager.Instance.bestScore;
        myBestScore.text = UIManager.Instance.currentBestScore;
        myLastScore.text = UIManager.Instance.currentLastScore;
    }
}