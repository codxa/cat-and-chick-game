using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _blackBackgroundObject;
    [SerializeField] private GameObject _winPopup;
    [SerializeField] private GameObject _losePopup;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.5f; // Biraz yavaşlattım, daha şık dursun

    private Image _blackBackgroundImage;
    private RectTransform _winPopupTransform;
    private RectTransform _losePopupTransform;

    private void Awake() 
    {
        if (_blackBackgroundObject) _blackBackgroundImage = _blackBackgroundObject.GetComponent<Image>();
        if (_winPopup) _winPopupTransform = _winPopup.GetComponent<RectTransform>();
        if (_losePopup) _losePopupTransform = _losePopup.GetComponent<RectTransform>();    
    }

    public void OnGameOver()
    {
        if(_blackBackgroundObject) _blackBackgroundObject.SetActive(true);
        if(_losePopup) _losePopup.SetActive(true);

        // Animasyonlar
        if(_blackBackgroundImage) _blackBackgroundImage.DOFade(0.8f, _animationDuration).SetEase(Ease.Linear);
        if(_losePopupTransform) 
        {
            _losePopupTransform.localScale = Vector3.zero; // Sıfırdan başlasın
            _losePopupTransform.DOScale(1f, _animationDuration).SetEase(Ease.OutBack);
        }
    }

    public void OnGameWin()
    {
        if(_blackBackgroundObject) _blackBackgroundObject.SetActive(true);
        if(_winPopup) _winPopup.SetActive(true);

        // Animasyonlar
        if(_blackBackgroundImage) _blackBackgroundImage.DOFade(0.8f, _animationDuration).SetEase(Ease.Linear);
        if(_winPopupTransform) 
        {
            _winPopupTransform.localScale = Vector3.zero; // Sıfırdan başlasın
            _winPopupTransform.DOScale(1f, _animationDuration).SetEase(Ease.OutBack);
        }
    }
}