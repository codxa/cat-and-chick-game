using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<GameState> OnGameStateChanged;
    public event Action<int> OnEggCountChanged;

    [Header("UI References")]
    [SerializeField] private WinLoseUI _winLoseUI; 
    [SerializeField] private TimerUI _timerUI;     

    // --- YENİ EKLENEN REFERANSLAR ---
    [Header("Game Objects")]
    [SerializeField] private CatController _catController; // Kediyi buraya sürükleyeceksin
    [SerializeField] private HealthManager _healthManager; // Can yöneticisi
    // --------------------------------

    private GameState _currentGameState;
    private int _collectedEggCount = 0;
    private const int _targetEggCount = 5;
    private float _lastCollectTime = -1f; 

    private void Start()
    {
        ChangeGameState(GameState.Play);
        
        // Hata vermemesi için try-catch (Senin kodun yapısı bozulmasın diye)
        try { OnEggCountChanged?.Invoke(0); } catch { }

        // Referansları otomatik bul (Sürüklemeyi unutursan diye)
        if (_catController == null) _catController = FindObjectOfType<CatController>();
        if (_healthManager == null) _healthManager = FindObjectOfType<HealthManager>();

        // --- KEDİYİ DİNLEME KISMI ---
        if (_catController != null)
        {
            // Kedi yakaladığında "OnCatCatchedHandler" çalışsın
            _catController.OnCatCatched += OnCatCatchedHandler;
        }
    }

    // Kedi yakalayınca burası çalışacak
    private void OnCatCatchedHandler(Transform playerTransform)
    {
        // Canları sıfırla ve Lose ekranını aç
        if (_healthManager != null)
        {
            _healthManager.Kill();
        }
        else
        {
            // HealthManager yoksa bile oyunu bitir
            LoseGame();
        }
    }

    public void CollectEgg()
    {
        if (Time.time - _lastCollectTime < 0.1f) return;
        _lastCollectTime = Time.time;

        _collectedEggCount++; 
        try { OnEggCountChanged?.Invoke(_collectedEggCount); }
        catch (Exception e) { Debug.LogWarning("UI Hatası: " + e.Message); }

        if (_collectedEggCount >= _targetEggCount)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        if (_timerUI != null) _timerUI.StopTimer();
        if (_winLoseUI != null) _winLoseUI.OnGameWin();
        ChangeGameState(GameState.GameOver); 
    }

    public void LoseGame()
    {
        Debug.Log("LOSE - KAYBETTİN!");

        if (_timerUI != null) _timerUI.StopTimer();

        if (_winLoseUI != null)
        {
            _winLoseUI.OnGameOver();
        }

        ChangeGameState(GameState.GameOver);
    }

    public void ChangeGameState(GameState newGameState)
    {
        if (_currentGameState == newGameState) return;
        _currentGameState = newGameState;
        OnGameStateChanged?.Invoke(_currentGameState);
        if (newGameState == GameState.Resume) ChangeGameState(GameState.Play);
    }
    
    public GameState GetCurrentGameState()
    {
        return _currentGameState;
    }
}