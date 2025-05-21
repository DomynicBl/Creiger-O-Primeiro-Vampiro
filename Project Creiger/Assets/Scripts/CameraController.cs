using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Room camera
    [SerializeField] private float speed;
    [SerializeField] private float verticalOffset = 2f; // Ajuste a altura acima do jogador

    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    //Follow player
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    private float lookAhead;

    private void Update()
    {
        // Calcula o "lookAhead" horizontal suavemente
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);

        // Define a posição desejada da câmera, com offset no Y
        Vector3 targetPosition = new Vector3(
            player.position.x + lookAhead,
            player.position.y + verticalOffset,
            transform.position.z
        );

        // Move suavemente a câmera até a posição desejada
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, speed * Time.deltaTime);
    }



    public void MoveToNewRoom(Transform _newRoom)
    {
        currentPosX = _newRoom.position.x;
    }
}