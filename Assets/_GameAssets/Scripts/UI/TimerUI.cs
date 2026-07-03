using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _timerRotateableTransform;
    [SerializeField] private TMP_Text _timerText;

    [Header("Settings")]
    [SerializeField] private float _rotationDuration = 1f;
    [SerializeField] private Ease _rotationEase = Ease.Linear;

    private Vector3 _rotationVector = new Vector3(0, 0, -360f);
    private float _elapsedTime;
    private bool _isTimerRunning;
    private string _finalTime; // Bitiş süresini burada saklayacağız
    private Tween _rotationTween;

    private void Start()
    {
        PlayRotationAnimation();
        StartTimer();
    }

    private void PlayRotationAnimation()
    {
        if (_timerRotateableTransform != null)
        {
            _rotationTween = _timerRotateableTransform.DORotate(_rotationVector, _rotationDuration, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(_rotationEase);
        }
    }

    public void StartTimer()
    {
        _elapsedTime = 0f;
        _isTimerRunning = true;
        _finalTime = ""; // Resetle
        InvokeRepeating(nameof(UpdateTimerUI), 0f, 1f);
        
        if (_rotationTween != null && !_rotationTween.IsPlaying()) 
            _rotationTween.Play();
    }

    // --- ÖNEMLİ: DIŞARIDAN ERİŞİLEBİLİR DURDURMA ---
    public void StopTimer()
    {
        _isTimerRunning = false;
        CancelInvoke(nameof(UpdateTimerUI));
        
        if (_rotationTween != null) 
            _rotationTween.Pause();

        // Süreyi kaydet
        _finalTime = GetFormattedElapsedTime();
    }
    // -----------------------------------------------

    private string GetFormattedElapsedTime()
    {
        int minutes = Mathf.FloorToInt(_elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(_elapsedTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateTimerUI()
    {
        if (!_isTimerRunning) return;

        _elapsedTime += 1f;
        
        if (_timerText != null)
            _timerText.text = GetFormattedElapsedTime();
    }

    // WinPopup bu fonksiyonu çağırıp süreyi alacak
    public string GetFinalTime()
    {
        // Eğer oyun bitmeden istendiyse anlık süreyi ver
        if (string.IsNullOrEmpty(_finalTime))
        {
            return GetFormattedElapsedTime();
        }
        return _finalTime;
    }
}