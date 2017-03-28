using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour 
{
	[SerializeField]
	private PlayerController playerController;

	[Header("UI components")]
	[SerializeField]
	private Image yellCooldownImage;
	[SerializeField]
	private TextMeshPro yellCooldownText;

	void Update()
	{
		yellCooldownText.text = playerController.GetYellCooldownTimeLeft () + "";
	}
}
