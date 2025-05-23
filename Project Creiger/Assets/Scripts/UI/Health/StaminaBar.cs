using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Stamina playerStamina;
    [SerializeField] private Image totalStaminaBar;
    [SerializeField] private Image currentStaminaBar;

    private void Start()
    {
        totalStaminaBar.fillAmount = playerStamina.currentStamina / 10;
    }
    private void Update()
    {
        currentStaminaBar.fillAmount = playerStamina.currentStamina / 10;
    }
}