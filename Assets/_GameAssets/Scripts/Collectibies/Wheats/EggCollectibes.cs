using UnityEngine;

public class EggCollectible : MonoBehaviour
{
    [Header("References")]
    // Buraya Unity'den bir patlama efekti (Particle System) sürükle
    [SerializeField] private GameObject _hitParticlesPrefab;

    [Header("Settings")]
    [SerializeField] private float _hitParticlesDestroyDuration = 2f;

    // Bu fonksiyonu PlayerInteractionController çağıracak
    public void OnCollected()
    {
        // Eğer efekt atandıysa, yumurtanın olduğu yerde oluştur
        if (_hitParticlesPrefab != null)
        {
            GameObject particles = Instantiate(_hitParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(particles, _hitParticlesDestroyDuration); // Efekti temizle
        }
    }
}