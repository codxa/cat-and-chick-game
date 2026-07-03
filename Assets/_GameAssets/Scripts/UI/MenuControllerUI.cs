using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MaskTransitions; // Sahne geçiş paketin

public class MenuContentUI : MonoBehaviour
{
    // --- 1. BURAYI EKLE ---
    [Header("System References")]
    [Tooltip("Hierarchy'den 'AudioManager' objeni buraya sürükle")]
    [SerializeField] private AudioManager _audioManager;
    // --------------------

    [Header("Main Buttons")]
    [SerializeField] private Button _playButton; // Play Butonu
    [SerializeField] private Button _quitButton; // Quit Butonu
    
    private void Awake() 
    {
        // Buton dinleyicilerin (Bunlar doğru, elleme)
        _playButton.onClick.AddListener(OnPlayButtonClick);
        _quitButton.onClick.AddListener(OnQuitButtonClick); 
    }

    private void OnPlayButtonClick()
    {
        // --- 2. SESİ BURAYA EKLE (Play butonuna basınca) ---
        if (_audioManager != null)
        {
            // Resimde istediğin ses buydu:
            _audioManager.Play(SoundType.TransitionSound); 
        }
        // --------------------------------------------------

        // Senin mevcut sahne geçiş kodun
        TransitionManager.Instance.LoadLevel(Consts.SceneNames.GAME_SCENE); 
    }

    private void OnQuitButtonClick()
    {
        // --- 3. SESİ BURAYA EKLE (Quit butonuna basınca) ---
        if (_audioManager != null)
        {
            // Resimde istediğin ses buydu:
            _audioManager.Play(SoundType.ButtonClickSound);
        }
        // -------------------------------------------------

        // Senin mevcut çıkış kodun
        Debug.Log("Quitting the Game..");
        Application.Quit();
    }
}