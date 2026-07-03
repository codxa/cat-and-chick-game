using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private PlayerController _playerController;

    private void Awake()
    {
        // PlayerController script'ine referans al
        _playerController = GetComponent<PlayerController>();

        if (_playerController == null)
        {
            Debug.LogError("PlayerController bu objede bulunamadı! Toplama sistemi çalışmayacak.");
            enabled = false;
        }
    }

    // İlk giriş anı (Mevcut olan)
    private void OnTriggerEnter(Collider other)
    {
        CollectItem(other);
    }

    // --- YENİ EKLENEN KISIM ---
    // Eğer karakter içine girmiş ama 'Enter' çalışmamışsa, içinde durduğu sürece de toplasın
    private void OnTriggerStay(Collider other)
    {
        CollectItem(other);
    }
    // ---------------------------

    // Kod tekrarı olmasın diye toplama işini tek fonksiyona aldık
    private void CollectItem(Collider other)
    {
        // Nesne zaten yok edilmişse işlem yapma (Hata almamak için)
        if (other == null || other.gameObject == null) return;

        // Gold buğdaya çarptık mı?
        if (other.CompareTag(Consts.Tags.GOLD_WHEAT))
        {
            _playerController.ApplyGoldEffect(); // PlayerController'a haber ver
            Destroy(other.gameObject); // Buğdayı yok et
        }
        // Holy buğdaya çarptık mı?
        else if (other.CompareTag(Consts.Tags.HOLY_WHEAT))
        {
            _playerController.ApplyHolyEffect();
            Destroy(other.gameObject);
        }
    }
}