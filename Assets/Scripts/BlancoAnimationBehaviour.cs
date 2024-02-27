using UnityEngine;

public class BlancoAnimationBehaviour : MonoBehaviour
{
    public static BlancoAnimationBehaviour Instance { get; private set; }
    public string comboName = "Attack";
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        BlancoCombatManager.Instance.InputEvent.AddListener(StartAnimation);
    }

    private void StartAnimation()
    {
        Debug.Log("We have an input working here !");
    }
    
    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }
}
