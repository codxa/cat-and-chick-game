using UnityEngine;
using TMPro; // TextMeshPro kütüphanesi şart

public class EggCounterUI : MonoBehaviour
{
    [Header("Sürükle Bırak Yapılacaklar")]
    [Tooltip("Hierarchy'den Text (TMP) objesini buraya at")]
    [SerializeField] private TextMeshProUGUI _counterText; 
    
    [Tooltip("GameManager objesini buraya at")]
    [SerializeField] private GameManager _gameManager; 

    private void Start()
    {
        // GameManager boşsa otomatik bulsun (Hata önleyici)
        if (_gameManager == null) 
            _gameManager = FindObjectOfType<GameManager>();

        if (_gameManager != null)
        {
            // Olayı dinlemeye başla
            _gameManager.OnEggCountChanged += UpdateText;
            
            // Başlangıçta 0/5 yaz
            UpdateText(0);
        }
    }

    private void OnDestroy()
    {
        // Obje silinirse dinlemeyi bırak
        if (_gameManager != null)
            _gameManager.OnEggCountChanged -= UpdateText;
    }

    // Yazıyı güncelleyen fonksiyon
    private void UpdateText(int count)
    {
        if (_counterText != null)
        {
            _counterText.text = count.ToString() + "/5";
        }
    }
}