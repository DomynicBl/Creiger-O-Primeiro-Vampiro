using UnityEngine;
using System.Collections;

public class Stamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float drainRate = 1f;       // Quantidade por segundo enquanto E está pressionado
    [SerializeField] private float regenRate = 2f;       // Quantidade por segundo ao soltar E

    public float currentStamina { get; private set; }

    private bool isDraining;

    private void Awake()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            DrainStamina();
        }
        else
        {
            RegenerateStamina();
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    private void DrainStamina()
    {
        if (currentStamina > 0)
        {
            currentStamina -= drainRate * Time.deltaTime;
        }
    }

    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
        }
    }

    // Método público se você quiser forçar recuperação por itens ou eventos externos
    public void AddStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
    }

    public void ConsumeStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina - amount, 0, maxStamina);
    }

}
