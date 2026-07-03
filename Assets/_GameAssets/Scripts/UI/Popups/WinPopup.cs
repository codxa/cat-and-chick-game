using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Sahne değişimi için gerekli

public class WinPopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Button _oneMoreButton;
    [SerializeField] private Button _mainMenuButton;
    
    [Header("External References (Sürükle!)")]
    [SerializeField] private TimerUI _timerUI; 
    // Ses yöneticisi varsa buraya sürükle, yoksa boş kalsın hata vermez
    [SerializeField] private AudioManager _audioManager; 

    private void OnEnable() 
    {
        // Panel açıldığında süreyi yazdır
        SetTimerText();
        
        // Butonları dinle
        _oneMoreButton.onClick.AddListener(OnOneMoreButtonClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

        // Kazanma sesi çal (Eğer audioManager atandıysa)
        if (_audioManager != null) 
            _audioManager.Play(SoundType.WinSound);
    }

    private void OnDisable() 
    {
        _oneMoreButton.onClick.RemoveListener(OnOneMoreButtonClicked);
        _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);    
    }   

    private void SetTimerText()
    {
        // TimerUI'dan son süreyi al
        if (_timerUI != null)
        {
            _timerText.text = _timerUI.GetFinalTime();
        }
        else
        {
            // Eğer sürüklemeyi unuttuysan otomatik bulmaya çalış
            var foundTimer = FindObjectOfType<TimerUI>();
            if (foundTimer != null) _timerText.text = foundTimer.GetFinalTime();
        }
    }

    private void OnMainMenuButtonClicked()
    {
        // "MenuScene" isminin Consts dosyanla aynı olduğundan emin ol
        if (_audioManager != null) _audioManager.Play(SoundType.TransitionSound);
        SceneManager.LoadScene("MenuScene"); 
    }

    private void OnOneMoreButtonClicked()
    {
        // Bölümü yeniden başlat
        if (_audioManager != null) _audioManager.Play(SoundType.ButtonClickSound);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}