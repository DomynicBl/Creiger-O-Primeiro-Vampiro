using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour{
    [Header("Referências de Áudio e UI")]
    public AudioSource musicAudioSource;
    public Slider volumeSlider;

    void Awake(){
        DontDestroyOnLoad(gameObject);

        if (FindObjectsOfType<MusicManager>().Length > 1){
            Destroy(gameObject);
            return;
        }
    }

    void Start(){
        if (musicAudioSource == null){
            //Debug.LogError("MusicManager: O 'Music Audio Source' não foi atribuído no Inspector!");
            return;
        }if (volumeSlider == null){
            //Debug.LogError("MusicManager: O 'Volume Slider' não foi atribuído no Inspector!");
            return;
        }

        float fixedInitialVolume = 0.35f; // Valor fixo de volume inicial
        
        musicAudioSource.volume = fixedInitialVolume;
        volumeSlider.value = fixedInitialVolume;

        Debug.Log($"[MusicManager] Volume FORÇADO aplicado ao AudioSource: {musicAudioSource.volume}");
        Debug.Log($"[MusicManager] Valor FORÇADO aplicado ao Slider: {volumeSlider.value}");

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume){
        if (musicAudioSource != null){
            musicAudioSource.volume = volume;
            Debug.Log($"[MusicManager] Volume definido para: {volume}");
            PlayerPrefs.SetFloat("MasterVolume", volume);
            PlayerPrefs.Save();
            Debug.Log("[MusicManager] Volume salvo em PlayerPrefs.");
        }
    }
}