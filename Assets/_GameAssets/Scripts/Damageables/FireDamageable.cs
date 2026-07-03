using UnityEngine;

public class FireDamageable : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private GameObject _hitParticlesPrefab;
    
    // Zenject yerine direkt referans alıyoruz
    [SerializeField] private HealthManager _healthManager;
    [SerializeField] private AudioManager _audioManager;

    [Header("Settings")]
    [SerializeField] private float _force = 10f; // Geri fırlatma gücü
    [SerializeField] private float _hitParticlesDestroyDuration = 2f;
    
    // Karakter ateşe sürekli basarsa saniyede 100 kere vurmasın diye zaman kilidi
    private float _lastDamageTime = 0f;
    private float _damageCooldown = 1.0f; // 1 saniyede bir yakabilsin

    private void Start()
    {
        // Otomatik bulma (Eğer sürüklemeyi unutursan)
        if (_healthManager == null) _healthManager = FindObjectOfType<HealthManager>();
        if (_audioManager == null) _audioManager = FindObjectOfType<AudioManager>();
    }

    public void GiveDamage(Rigidbody playerRigidbody, Transform playerVisualTransform)
    {
        // Eğer son yanmanın üzerinden yeterince vakit geçmediyse vurma
        if (Time.time - _lastDamageTime < _damageCooldown) return;
        _lastDamageTime = Time.time;

        if (_healthManager != null)
        {
            _healthManager.Damage(1);
        }

        // Karakteri geriye doğru fırlat (Knockback)
        if (playerRigidbody != null)
        {
            Vector3 pushDir = (playerVisualTransform.position - transform.position).normalized;
            pushDir.y = 0.5f; // Hafif yukarı da zıplasın
            playerRigidbody.AddForce(pushDir * _force, ForceMode.Impulse);
        }

        if (_audioManager != null)
        {
            _audioManager.Play(SoundType.ChickSound); // Can yanma sesi
        }

        // Destroy(gameObject); <-- BUNU SİLDİM, ARTIK ATEŞ YOK OLMAYACAK
    }

    public void PlayHitParticle(Transform playerTransform)
    {
        if (_hitParticlesPrefab != null)
        {
            Vector3 offset = new Vector3(0f, 0.7f, 0f);
            GameObject particleInstance = Instantiate(_hitParticlesPrefab, playerTransform.position + offset, Quaternion.identity);
            particleInstance.transform.parent = playerTransform;
            Destroy(particleInstance, _hitParticlesDestroyDuration);
        }
    }
}