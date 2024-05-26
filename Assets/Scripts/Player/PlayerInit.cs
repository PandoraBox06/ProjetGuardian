using UnityEngine;


public class PlayerInit : MonoBehaviour
{
    private CameraBehavior _cameraBehavior;
    private PlayerMouvement _playerMouvement;
    private DashController _dashController;
    public static PlayerInit Instance { get; private set; }
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _dashController = GetComponent<DashController>();
        _playerMouvement = GetComponent<PlayerMouvement>();
        _cameraBehavior = GetComponent<CameraBehavior>();
        DisablePlayer();
    }

    public void DisablePlayer()
    {
        _cameraBehavior.enabled = false;
        _dashController.enabled = false;
        _playerMouvement.enabled = false;
    }

    public void EnablePlayer()
    {       
        _cameraBehavior.enabled = true;
        _dashController.enabled = true;
        _playerMouvement.enabled = true;
    }
}
