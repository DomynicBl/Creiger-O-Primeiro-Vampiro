using UnityEngine;
using UnityEngine.UI;  // Para usar UI Image e Button

public class HealingItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;  // Imagem do item de cura
    [SerializeField] private Button itemButton;  // Botão de interação com o item

    private Color originalColor;  // Cor original do item

    private void Awake()
    {
        // Armazenando a cor original do item
        if (itemImage != null)
        {
            originalColor = itemImage.color;
        }
    }

    // Atualiza a aparência do item com base se ele ainda tem usos ou não
    public void UpdateVisual(bool isOutOfUses)
    {
        if (isOutOfUses)
        {
            // Coloca o item em cinza e desabilita o botão
            if (itemImage != null)
                itemImage.color = Color.gray; // Mudando a cor para cinza

            if (itemButton != null)
                itemButton.interactable = false; // Desabilita o botão de interação
        }
        else
        {
            // Restaura a cor original e habilita o botão
            if (itemImage != null)
                itemImage.color = originalColor;

            if (itemButton != null)
                itemButton.interactable = true; // Habilita o botão de interação
        }
    }
}
