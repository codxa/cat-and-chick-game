using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image[] _playerHealthImages;

    [Header("Sprites")]
    [SerializeField] private Sprite _playerHealthySprite;
    [SerializeField] private Sprite _playerUnhealthySprite;

    [Header("Settings")]
    [SerializeField] private float _scaleDuration = 0.2f;

    private RectTransform[] _playerHealthTransforms;

    private void Awake() 
    {
        _playerHealthTransforms = new RectTransform[_playerHealthImages.Length];
        for (int i = 0; i < _playerHealthImages.Length; ++i)
        {
            if(_playerHealthImages[i] != null)
                _playerHealthTransforms[i] = _playerHealthImages[i].rectTransform;
        } 
    }

    // Hasar Animasyonu (Kalp Boşaltma)
    public void AnimateDamage()
    {
        // Dolu olan son kalbi bul ve boşalt
        for (int i = _playerHealthImages.Length - 1; i >= 0; --i)
        {
            if (_playerHealthImages[i].sprite == _playerHealthySprite)
            {
                AnimateSprite(_playerHealthImages[i], _playerHealthTransforms[i], _playerUnhealthySprite);
                break; 
            }
        }
    }

    // İyileşme Animasyonu (Kalp Doldurma)
    public void AnimateHeal()
    {
        // Boş olan ilk kalbi bul ve doldur
        for (int i = 0; i < _playerHealthImages.Length; ++i)
        {
            if (_playerHealthImages[i].sprite == _playerUnhealthySprite)
            {
                AnimateSprite(_playerHealthImages[i], _playerHealthTransforms[i], _playerHealthySprite);
                break; 
            }
        }
    }

    private void AnimateSprite(Image img, RectTransform trans, Sprite targetSprite)
    {
        trans.DOScale(0f, _scaleDuration).SetEase(Ease.InBack).OnComplete(() =>
        {
            img.sprite = targetSprite;
            trans.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
        });
    }
}