using UnityEngine;
using System; // Necessário para o evento Action

public class KillStreakManager : MonoBehaviour
{
    [Header("Kill Streak Settings")]
    [SerializeField] private int killsForFireEffect = 5; // Número de kills para ativar o fogo
    [SerializeField] private float resetStreakTime = 45f; // Tempo para resetar o combo se não houver kill (45 segundos)

    private int _currentKillCount = 0;
    private float _streakTimer;

    // Eventos para o HUD se inscrever
    public event Action<int> OnKillCountChanged;
    public event Action<bool> OnFireEffectStateChanged;

    private Health _playerHealth; // Referência ao componente Health do Player

    private void Awake()
    {
        _playerHealth = GetComponent<Health>(); // Obtém o componente Health do mesmo GameObject (o Player)
        if (_playerHealth == null)
        {
            Debug.LogError("[KillStreakManager] Health component not found on Player! Make sure KillStreakManager is attached to the Player and has a Health component.", this);
        }

        OnFireEffectStateChanged?.Invoke(false); // Inicializa o fogo como desativado
        OnKillCountChanged?.Invoke(_currentKillCount); // Inicializa o contador de kills no HUD com 0
        _streakTimer = 0f; // Garante que o timer comece zerado
        Debug.Log("[KillStreakManager] Awake - Inicializado. Combo: " + _currentKillCount);
    }

    private void OnEnable()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnPlayerDamaged += ResetKillStreakOnDamage; // Se inscreve para resetar o combo ao tomar dano
            Debug.Log("[KillStreakManager] OnEnable - Inscrito no OnPlayerDamaged.");
        }
    }

    private void OnDisable()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnPlayerDamaged -= ResetKillStreakOnDamage; // Desinscreve-se para evitar erros
            Debug.Log("[KillStreakManager] OnDisable - Desinscrito do OnPlayerDamaged.");
        }
    }

    private void Update()
    {
        if (_currentKillCount > 0) // Só atualiza o timer se houver kills no combo
        {
            _streakTimer += Time.deltaTime;
            if (_streakTimer >= resetStreakTime)
            {
                Debug.Log("[KillStreakManager] Combo resetado por TEMPO (" + _streakTimer.ToString("F2") + "s)!");
                ResetKillStreak(); // Reseta o combo por tempo
            }
        }
        else
        {
            _streakTimer = 0f; // Mantém o timer em 0 quando não há combo ativo
        }
    }

    // Chamado pelo PlayerMeleeAttack quando um inimigo morre
    public void AddKill()
    {
        _currentKillCount++;
        OnKillCountChanged?.Invoke(_currentKillCount); // Notifica o HUD

        Debug.Log($"[KillStreakManager] Kill adicionada! Kills atuais: {_currentKillCount}");

        _streakTimer = 0f; // Reseta o timer do combo para dar mais tempo para a próxima kill
        Debug.Log("[KillStreakManager] Timer de combo resetado para 0 (nova kill).");

        if (_currentKillCount >= killsForFireEffect)
        {
            OnFireEffectStateChanged?.Invoke(true); // Ativa o fogo no HUD
            Debug.Log("[KillStreakManager] Efeito de fogo ativado!");
        }
    }

    // Chamado pelo evento OnPlayerDamaged do script Health do Player
    private void ResetKillStreakOnDamage()
    {
        Debug.Log("[KillStreakManager] Resetando combo por DANO EFETIVO no jogador!");
        ResetKillStreak(); // Reseta o combo imediatamente ao tomar dano efetivo
    }

    // Reseta o contador de kills e o estado do fogo
    private void ResetKillStreak()
    {
        _currentKillCount = 0;
        OnKillCountChanged?.Invoke(_currentKillCount); // Notifica o HUD para zerar o texto
        OnFireEffectStateChanged?.Invoke(false); // Desativa o fogo no HUD
        _streakTimer = 0f; // Garante que o timer zere após o reset
        Debug.Log("[KillStreakManager] Combo TOTALMENTE zerado.");
    }

    // Método público se você precisar forçar um reset do combo de fora (ex: fim de fase, game over)
    public void ForceResetKillStreak()
    {
        Debug.Log("[KillStreakManager] Forçando reset do combo.");
        ResetKillStreak();
    }
}