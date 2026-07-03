using UnityEngine;

public class GoldWheatCollectibie : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 100f;

    // Kendi etrafında dönsün
  // Update fonksiyonunun içi boş kalsın veya sil
    void Update()
    {
        // transform.Rotate(...);  <-- BUNU SİL, animasyon zaten yapıyor.
    }
}