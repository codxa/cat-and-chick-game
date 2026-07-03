using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealthUI _playerHealthUI; 
    [SerializeField] private GameManager _gameManager;       

    [Header("Settings")]
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
        // Otomatik bulma
        if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
        if (_playerHealthUI == null) _playerHealthUI = FindObjectOfType<PlayerHealthUI>();
    }

    public void Damage(int damage)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damage;
            if (_playerHealthUI != null) _playerHealthUI.AnimateDamage();

            if(_currentHealth <= 0)
            {
                if (_gameManager != null) _gameManager.LoseGame();
            }
        }
    }

    // --- BURAYI DEĞİŞTİRDİM ---
    public void Kill()
    {
        _currentHealth = 0; 

        // Hata veren "AnimateDamageForAll" yerine
        // elinde olanı 3 kere çağırıyoruz. Kesin çalışır.
        if (_playerHealthUI != null)
        {
            _playerHealthUI.AnimateDamage();
            _playerHealthUI.AnimateDamage();
            _playerHealthUI.AnimateDamage();
        }

        if (_gameManager != null) _gameManager.LoseGame();
    }
    // ---------------------------

    public void Heal(int healAmount)
    {
        if (_currentHealth < _maxHealth)
        {
            _currentHealth = Mathf.Min(_currentHealth + healAmount, _maxHealth);
            if (_playerHealthUI != null) _playerHealthUI.AnimateHeal();
        }
    }
}