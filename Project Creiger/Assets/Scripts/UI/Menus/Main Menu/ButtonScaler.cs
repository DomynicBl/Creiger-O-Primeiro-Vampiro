using UnityEngine;
using UnityEngine.EventSystems; // Necessário para interfaces de eventos
// using UnityEngine.UI;

public class ButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler{
    public Vector3 normalScale = new Vector3(1f, 1f, 1f); // Escala padrão do botão
    public Vector3 highlightedScale = new Vector3(1.1f, 1.1f, 1.1f); // Escala quando o mouse/selecionado
    public float scaleSpeed = 10f; // Velocidade da transição de escala (ajustei para um valor inicial mais razoável)

    private RectTransform rectTransform;
    private Vector3 targetScale;

    void Awake(){
        rectTransform = GetComponent<RectTransform>();
        // Garante que a escala inicial seja a normalScale definida no Inspector
        rectTransform.localScale = normalScale; 
        targetScale = normalScale; // Começa na escala normal
    }

    void Update(){
        // Interpolação suave entre a escala atual e a escala alvo
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    // Chamado quando o ponteiro do mouse entra no botão
    public void OnPointerEnter(PointerEventData eventData){
        targetScale = highlightedScale;
    }

    // CORRIGIDO: O tipo correto é PointerEventData
    // Chamado quando o ponteiro do mouse sai do botão
    public void OnPointerExit(PointerEventData eventData) {
        targetScale = normalScale;
    }

    // Chamado quando o botão é selecionado (via teclado/controle)
    public void OnSelect(BaseEventData eventData){
        targetScale = highlightedScale;
    }

    // Chamado quando o botão é deselecionado
    public void OnDeselect(BaseEventData eventData){
        targetScale = normalScale;
    }
}