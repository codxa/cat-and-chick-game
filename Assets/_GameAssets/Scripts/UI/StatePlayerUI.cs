using UnityEngine;
using UnityEngine.UI; 
using DG.Tweening;  // <-- ANİMASYON KÜTÜPHANESİ (DOTween)

public class StatePlayerUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Player objesini buraya sürükle")]
    [SerializeField] private PlayerController _playerController; 
    
    // --- SOL TARAF (Mevcut kodun) ---
    [Header("Kart (E) Referansı")]
    [SerializeField] private RectTransform _playerWalkingTransform; // E Kartı
    [Header("Kart (Q) Referansı")]
    [SerializeField] private RectTransform _playerSlidingTransform; // Q Kartı
    [Header("Sprites (Sol Taraf)")]
    [SerializeField] private Sprite _playerWalkingActiveSprite;
    [SerializeField] private Sprite _playerWalkingPassiveSprite;
    [SerializeField] private Sprite _playerSlidingActiveSprite;
    [SerializeField] private Sprite _playerSlidingPassiveSprite;
    [Header("Settings (Sol Taraf Animasyon)")]
    [SerializeField] private float _moveDuration = 0.35f;
    [SerializeField] private Ease _moveEase = Ease.OutBack;
    [SerializeField] private float _activeXPosition = -25f;
    [SerializeField] private float _passiveXPosition = -90f;
    // ----------------------------------

    // --- YENİ EKLENDİ (Sağ Taraf) ---
    [Header("Booster (Sağ Taraf) Referansları")]
    [Tooltip("Hierarchy'den 'PlayerBoosterSpeed' objesini buraya sürükle")]
    [SerializeField] private RectTransform _boosterSpeedTransform;
    [Tooltip("Hierarchy'den 'PlayerBoosterJump' objesini buraya sürükle")]
    [SerializeField] private RectTransform _boosterJumpTransform;

    [Header("Booster (Sağ Taraf) Sprites")]
    [SerializeField] private Sprite _boosterSpeedActiveSprite;
    [SerializeField] private Sprite _boosterSpeedPassiveSprite;
    [SerializeField] private Sprite _boosterJumpActiveSprite;
    [SerializeField] private Sprite _boosterJumpPassiveSprite;
    
    [Header("Booster (Sağ Taraf) Ayarları")]
    [Tooltip("Kart aktifken X pozisyonu (örn: 25)")]
    [SerializeField] private float _boosterActiveXPosition = 25f;
    [Tooltip("Kart pasifken X pozisyonu (örn: 90)")]
    [SerializeField] private float _boosterPassiveXPosition = 90f;
    // ----------------------------------


    // Image component'lerini kod içinde bulacağız
    private Image _playerWalkingImage;
    private Image _playerSlidingImage;
    private Image _boosterSpeedImage; // YENİ
    private Image _boosterJumpImage;  // YENİ

    private void Awake()
    {
        // Objelerin İÇİNDEKİ Image component'lerini bul
        try
        {
            // Sol taraf
            _playerWalkingImage = _playerWalkingTransform.GetComponentInChildren<Image>();
            _playerSlidingImage = _playerSlidingTransform.GetComponentInChildren<Image>();

            // Sağ taraf (YENİ) - (t23.jpg'ye göre GetComponentInChildren doğru komut)
            _boosterSpeedImage = _boosterSpeedTransform.GetComponentInChildren<Image>();
            _boosterJumpImage = _boosterJumpTransform.GetComponentInChildren<Image>();
        }
        catch (System.Exception e)
        {
            Debug.LogError("StatePlayerUI 'Awake' HATA: " + e.Message);
        }
    }

    private void Start()
    {
        // Güvenlik kontrolüne sağ tarafı da ekle
        if (_playerController == null || _playerWalkingImage == null || _playerSlidingImage == null || _boosterSpeedImage == null || _boosterJumpImage == null)
        {
            Debug.LogError("StatePlayerUI script'indeki referanslardan biri boş! Inspector'u kontrol et.", this.gameObject);
            return;
        }

        // --- Anonslara Abone Ol ---
        // Sol taraf
        _playerController.OnUIModeChanged += OnPlayerUIModeChanged;
        
        // Sağ taraf (YENİ)
        _playerController.OnBuffStarted += OnBuffStarted;
        _playerController.OnBuffEnded += OnBuffEnded;
        // --------------------------
        
        
        // --- Başlangıç Pozisyonları ve Sprite'ları ---
        // Sol taraf
        _playerWalkingImage.sprite = _playerWalkingPassiveSprite;
        _playerSlidingImage.sprite = _playerSlidingPassiveSprite;
        _playerWalkingTransform.anchoredPosition = new Vector2(_passiveXPosition, _playerWalkingTransform.anchoredPosition.y);
        _playerSlidingTransform.anchoredPosition = new Vector2(_passiveXPosition, _playerSlidingTransform.anchoredPosition.y);
        
        // Sağ taraf (YENİ)
        _boosterSpeedImage.sprite = _boosterSpeedPassiveSprite;
        _boosterJumpImage.sprite = _boosterJumpPassiveSprite;
        _boosterSpeedTransform.anchoredPosition = new Vector2(_boosterPassiveXPosition, _boosterSpeedTransform.anchoredPosition.y);
        _boosterJumpTransform.anchoredPosition = new Vector2(_boosterPassiveXPosition, _boosterJumpTransform.anchoredPosition.y);
        // -------------------------------------------
    }
    
    // --- Sol Taraf UI Fonksiyonu (Mevcut kodun) ---
    private void OnPlayerUIModeChanged(PlayerUIMode newMode)
    {
        if (_playerWalkingImage == null || _playerSlidingImage == null) return;

        if (newMode == PlayerUIMode.Boosting) // Q Aktif (Sliding)
        {
            _playerSlidingImage.sprite = _playerSlidingActiveSprite;
            _playerSlidingTransform.DOAnchorPosX(_activeXPosition, _moveDuration).SetEase(_moveEase);
            _playerWalkingImage.sprite = _playerWalkingPassiveSprite;
            _playerWalkingTransform.DOAnchorPosX(_passiveXPosition, _moveDuration).SetEase(_moveEase);
        }
        else if (newMode == PlayerUIMode.Braking) // E Aktif (Walking)
        {
            _playerSlidingImage.sprite = _playerSlidingPassiveSprite;
            _playerSlidingTransform.DOAnchorPosX(_passiveXPosition, _moveDuration).SetEase(_moveEase);
            _playerWalkingImage.sprite = _playerWalkingActiveSprite;
            _playerWalkingTransform.DOAnchorPosX(_activeXPosition, _moveDuration).SetEase(_moveEase);
        }
        else // Normal (İkisi de Pasif)
        {
            _playerSlidingImage.sprite = _playerSlidingPassiveSprite;
            _playerSlidingTransform.DOAnchorPosX(_passiveXPosition, _moveDuration).SetEase(_moveEase);
            _playerWalkingImage.sprite = _playerWalkingPassiveSprite;
            _playerWalkingTransform.DOAnchorPosX(_passiveXPosition, _moveDuration).SetEase(_moveEase);
        }
    }
    
    // --- YENİ EKLENDİ: Sağ Taraf (Booster) Fonksiyonları ---
    
    // PlayerController "Buff Başladı" anonsu yapınca bu çalışır
    private void OnBuffStarted(BuffType buffType)
    {
        if (buffType == BuffType.Speed)
        {
            _boosterSpeedImage.sprite = _boosterSpeedActiveSprite;
            _boosterSpeedTransform.DOAnchorPosX(_boosterActiveXPosition, _moveDuration).SetEase(_moveEase);
        }
        else if (buffType == BuffType.Jump)
        {
            _boosterJumpImage.sprite = _boosterJumpActiveSprite;
            _boosterJumpTransform.DOAnchorPosX(_boosterActiveXPosition, _moveDuration).SetEase(_moveEase);
        }
    }
    
    // PlayerController "Buff Bitti" anonsu yapınca bu çalışır
    private void OnBuffEnded(BuffType buffType)
    {
        if (buffType == BuffType.Speed)
        {
            _boosterSpeedImage.sprite = _boosterSpeedPassiveSprite;
            _boosterSpeedTransform.DOAnchorPosX(_boosterPassiveXPosition, _moveDuration).SetEase(_moveEase);
        }
        else if (buffType == BuffType.Jump)
        {
            _boosterJumpImage.sprite = _boosterJumpPassiveSprite;
            _boosterJumpTransform.DOAnchorPosX(_boosterPassiveXPosition, _moveDuration).SetEase(_moveEase);
        }
    }
    // ----------------------------------------------------


    private void OnDestroy()
    {
        // Sol taraf
        if (_playerController != null)
        {
            _playerController.OnUIModeChanged -= OnPlayerUIModeChanged;
            
            // Sağ taraf (YENİ)
            _playerController.OnBuffStarted -= OnBuffStarted;
            _playerController.OnBuffEnded -= OnBuffEnded;
        }
        
        // Sol taraf animasyonları durdur
        _playerWalkingTransform.DOKill();
        _playerSlidingTransform.DOKill();
        
        // Sağ taraf animasyonları durdur (YENİ)
        _boosterSpeedTransform.DOKill();
        _boosterJumpTransform.DOKill();
    }
}