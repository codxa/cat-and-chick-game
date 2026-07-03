using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    // --- YENİ EKLENDİ ---
    [Header("References")]
    [Tooltip("Hierarchy'den 'GameManager' objeni buraya sürükle")]
    [SerializeField] private GameManager _gameManager;
    // --------------------

    [SerializeField] private Transform _orientationTransform;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _playerVisualTransform;
    [SerializeField] private float _rotationSpeed;

    private void Start() 
    {
        // ... (Cursor kodların) ...   
    }

    private void Update() 
    {
        // --- YENİ EKLENDİ: OYUN DURUM KONTROLÜ ---
        // Eğer GameManager yoksa veya oyun durumu Play değilse, HİÇBİR ŞEY YAPMA.
        if (_gameManager == null || _gameManager.GetCurrentGameState() != GameState.Play)
        {
            return; // Update fonksiyonunu burada bitir.
        }
        // ------------------------------------------

        // Oyun "Play" durumundaysa, kamera çalışmaya devam eder:
        Vector3 viewDirection = 
            _playerTransform.position - new Vector3(transform.position.x, _playerTransform.position.y, transform.position.z);

        _orientationTransform.forward = viewDirection.normalized;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = _orientationTransform.forward * verticalInput + _orientationTransform.right * horizontalInput;

        if(inputDirection != Vector3.zero)
        {
            _playerVisualTransform.forward = 
                Vector3.Slerp(_playerVisualTransform.forward, inputDirection.normalized, Time.deltaTime * _rotationSpeed);
        }
    }
}