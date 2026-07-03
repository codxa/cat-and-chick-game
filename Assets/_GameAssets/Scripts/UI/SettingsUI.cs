using UnityEngine;
using UnityEngine.UI;
using MaskTransitions; // 1. Sahne geçiş paketini buraya da ekle
using UnityEngine.SceneManagement; // SceneManager için bunu da eklemek iyi olur

public class SettingsUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Hierarchy'den 'GameManager' objeni buraya sürükle")]
    [SerializeField] private GameManager _gameManager;
    
    [Header("Buttons")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton; // 2. Main Menu butonunu buraya ekledik

    [Header("Popups")]
    [Tooltip("Hierarchy'den 'SettingsPopup' panelini buraya sürükle")]
    [SerializeField] private GameObject _settingsPopupObject;

    
    private void Start()
    {
        // 1. Buton dinleyicilerini ayarla
        _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        _resumeButton.onClick.AddListener(OnResumeButtonClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked); // 3. Yeni butonu da ekle

        // 2. Oyun başlarken ayar menüsü kapalı olsun
        _settingsPopupObject.SetActive(false);
    }

    // Ayarlar butonuna basınca
    private void OnSettingsButtonClicked()
    {
        _settingsPopupObject.SetActive(true);
        _gameManager.ChangeGameState(GameState.Pause);
    }

    // "Devam Et" butonuna basınca
    private void OnResumeButtonClicked()
    {
        _settingsPopupObject.SetActive(false);
        _gameManager.ChangeGameState(GameState.Resume);
    }

    // 4. YENİ FONKSİYON: "Ana Menü" butonuna basınca
    private void OnMainMenuButtonClicked()
    {
        // ÖNEMLİ: Menüye dönmeden önce oyunu duraklatmadan çıkarman lazım.
        // Yoksa ana menüye döndüğünde oyun donmuş kalır (Time.timeScale = 0).
        _gameManager.ChangeGameState(GameState.Resume);

        // Şimdi o güzel efektle ana menüye dön
        // "MenuScene" yazan yeri kendi ana menü sahnenin adıyla değiştirmen gerekebilir.
        // Önceki script'te Consts.SceneNames... kullanmıştın, burada da onu kullanabilirsin.
        TransitionManager.Instance.LoadLevel("MenuScene"); 
        // VEYA: TransitionManager.Instance.LoadLevel(Consts.SceneNames.MENU_SCENE);
    }

    private void OnDestroy() 
    {
        // Hafıza sızıntısını önlemek için dinleyicileri kaldır
        _settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
        _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked); // 5. Buraya da ekle
    }
}