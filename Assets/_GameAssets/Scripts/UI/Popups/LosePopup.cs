using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Sahne değişimi için gerekli

public class LosePopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button _tryAgainButton; // Tekrar Dene Butonu
    [SerializeField] private Button _mainMenuButton; // Ana Menü Butonu
    
    [Header("External References (Sürükle!)")]
    // Ses yöneticisi varsa buraya sürükle
    [SerializeField] private AudioManager _audioManager; 

    private void OnEnable() 
    {
        // Butonları dinle
        _tryAgainButton.onClick.AddListener(OnTryAgainClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        // Kaybetme sesi çal (Eğer SoundType.LoseSound veya FailSound yoksa enum'a ekle)
        if (_audioManager != null) 
        {
            // Eğer enum'unda 'LoseSound' yoksa burayı 'FailSound' falan yapabilirsin
            // Şimdilik WinSound'un tersi mantığıyla yazdım
            _audioManager.Play(SoundType.LoseSound); 
        }
    }

    private void OnDisable() 
    {
        _tryAgainButton.onClick.RemoveListener(OnTryAgainClicked);
        _mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);    
    }   

    private void OnMainMenuClicked()
    {
        if (_audioManager != null) _audioManager.Play(SoundType.TransitionSound);
        SceneManager.LoadScene("MenuScene"); // Menü sahne isminle aynı olsun
    }

    private void OnTryAgainClicked()
    {
        // Bölümü yeniden başlat
        if (_audioManager != null) _audioManager.Play(SoundType.ButtonClickSound);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}