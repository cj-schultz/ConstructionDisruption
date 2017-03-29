using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;

    [Header("UI Components")]
    [SerializeField]
    private TextMeshProUGUI yellCooldownText;
    [SerializeField]
    private Image yellCooldownImage;

    void Start()
    {
        UpdateYellCooldown();

        // Don't use text for now, I'm not sure if I want to keep it.
        yellCooldownText.text = ""; 
    }

    void Update()
    {
        UpdateYellCooldown();
    }

    private void UpdateYellCooldown()
    {
        yellCooldownImage.fillAmount = (playerController.yellCooldown - playerController.yellCooldownTimeLeft) / playerController.yellCooldown;
        //yellCooldownText.text = (int)Mathf.Clamp(playerController.yellCooldownTimeLeft, 0, playerController.yellCooldownTimeLeft) + "";
    }
}
