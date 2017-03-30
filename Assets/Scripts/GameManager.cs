using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Component references")]
    [SerializeField]
    private PlayerUI playerUI;

    [Header("Game Variables")]
    // The hour the current day should start at
    // Note: This is always assumed to be AM
    [SerializeField]
    private float startHour; 
    // The hour that the current day should end at
    // Note: This is always assumed to be AM
    [SerializeField]
    private float endHour; 

    void Awake()
    {

    }

    void OnEnable()
    {
        PlayerUI.OnDayEnd += HandleDayEnd;
    }

    void OnDisable()
    {
        PlayerUI.OnDayEnd -= HandleDayEnd;
    }

    private void HandleDayEnd()
    {

    }
}
