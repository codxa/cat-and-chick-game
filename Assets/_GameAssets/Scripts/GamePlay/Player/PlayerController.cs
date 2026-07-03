using System;
using System.Collections;
using UnityEngine;

// Gerekli Enumlar
public enum PlayerUIMode { Normal, Boosting, Braking }
public enum BuffType { None, Speed, Jump }

public class PlayerController : MonoBehaviour
{
    // --- BAĞLANTILAR (Inspector'dan Sürükle) ---
    [Header("System References")]
    [SerializeField] private GameManager _gameManager;

    [Header("References")]
    [SerializeField] private Transform _orientationTransform;
    [SerializeField] private AudioManager _audioManager;

    // --- EVENTLER ---
    public event Action<PlayerUIMode> OnUIModeChanged; 
    public event Action<BuffType> OnBuffStarted;
    public event Action<BuffType> OnBuffEnded;
    public event Action OnPlayerJumped;
    public event Action OnPlayerHitWall; 
    
    // --- HAREKET AYARLARI ---
    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 16f;
    [SerializeField] private float _maxMovementSpeed = 20f;
    
    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 8f;

    [Header("Boost & Brake Settings")]
    [SerializeField] private float _qTapForce = 25f;
    [SerializeField] private float _maxBoostSpeed = 35f;
    [SerializeField] private float _boostDrag = 5f;
    [SerializeField] private float _brakeDrag = 15f;
    
    [Header("Buff Settings")]
    [SerializeField] private float _effectDuration = 5.0f;
    [SerializeField] private float _goldSpeedBoost = 30f;
    [SerializeField] private float _holyJumpBoost = 15f;

    [Header("Physics Settings")]
    [SerializeField] private float _wallPushBackForce = 15f;
    [SerializeField] private float _airDrag = 0.5f;
    [SerializeField] private float _playerHeight = 2f;
    [SerializeField] private LayerMask _groundLayer;

    // --- UI AYARLARI ---
    [Header("UI References")]
    [SerializeField] private float _boostUICooldown = 0.5f;
    
    // --- GİZLİ DEĞİŞKENLER ---
    private Rigidbody _playerRigidbody;
    private float _horizontalInput, _verticalInput;
    private Vector3 _movementDirection;
    private bool _isGrounded;
    private bool _isBoosting = false;
    private bool _isBraking = false;
    
    private float _originalMovementSpeed;
    private float _originalMaxMovementSpeed;
    private float _originalJumpForce;
    
    private Coroutine _activeEffectCoroutine;
    private Coroutine _boostUICoroutine;
    private PlayerUIMode _currentUIMode = PlayerUIMode.Normal;
    private BuffType _activeBuffType = BuffType.None; 

    private void Awake() 
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;
        _playerRigidbody.mass = 1f;
        
        // Orijinal değerleri sakla
        _originalMovementSpeed = _movementSpeed;
        _originalMaxMovementSpeed = _maxMovementSpeed;
        _originalJumpForce = _jumpForce;

        // GameManager boşsa bulmaya çalış
        if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (_gameManager == null || _gameManager.GetCurrentGameState() != GameState.Play) return;

        SetInputs();
        CheckAndBroadcastUIMode();
    }

    private void FixedUpdate()
    {
        if (_gameManager == null || _gameManager.GetCurrentGameState() != GameState.Play) return;

        SetPlayerMovement();
        ApplySpeedLimit();
        ApplyCustomDrag();
        _isGrounded = IsGroundedRaycast();
    }

    // --- ÇARPIŞMA (DUVAR) ---
    private void OnCollisionEnter(Collision collision)
    {
        if (_gameManager == null || _gameManager.GetCurrentGameState() != GameState.Play) return;
        
        if (collision.gameObject.CompareTag(Consts.Tags.WALL))
        {
            Vector3 pushDirection = collision.contacts[0].normal;
            _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
            _playerRigidbody.AddForce(pushDirection * _wallPushBackForce, ForceMode.Impulse);
            OnPlayerHitWall?.Invoke();
            _isBoosting = false;
        }
    }

    // ================================================================
    // --- TOPLAMA VE HASAR ALMA MERKEZİ (EN ÖNEMLİ KISIM) ---
    // ================================================================

    private void OnTriggerEnter(Collider other) { ProcessCollection(other); }
    private void OnTriggerStay(Collider other) { ProcessCollection(other); }

    private void ProcessCollection(Collider other)
    {
        if (other == null || other.gameObject == null) return;

        // 1. HASAR ALMA (ATEŞ VS.)
        // Çarptığımız şeyde IDamageable scripti var mı?
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.GiveDamage(_playerRigidbody, transform);
            damageable.PlayHitParticle(transform);
            return; // Hasar aldıysak başka işlem yapma
        }

        // 2. YUMURTA TOPLAMA
        if (other.CompareTag("Egg"))
        {
            other.enabled = false; // Çift toplamayı önle
            
            EggCollectible eggScript = other.GetComponent<EggCollectible>();
            if (eggScript != null) eggScript.OnCollected();

            if (_gameManager != null) _gameManager.CollectEgg();

            Destroy(other.gameObject);
        }
        // 3. ALTIN BUĞDAY (HIZ)
        else if (other.CompareTag(Consts.Tags.GOLD_WHEAT))
        {
            other.enabled = false;
            ApplyGoldEffect();
            Destroy(other.gameObject);
        }
        // 4. KUTSAL BUĞDAY (ZIPLAMA)
        else if (other.CompareTag(Consts.Tags.HOLY_WHEAT))
        {
            other.enabled = false;
            ApplyHolyEffect();
            Destroy(other.gameObject);
        }
    }
    // ================================================================


    // --- HAREKET KODLARI ---
    private void SetInputs()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded) SetPlayerJumping();
        if (Input.GetKeyDown(KeyCode.Q) && _isGrounded) ApplyBoostForce(_movementDirection.normalized);
        _isBraking = Input.GetKey(KeyCode.E) && _isGrounded;
    }
    
    public void SetPlayerMovement()
    {
        _movementDirection = _orientationTransform.forward * _verticalInput + _orientationTransform.right * _horizontalInput;
        if (!_isBoosting) _playerRigidbody.AddForce(_movementDirection.normalized * _movementSpeed, ForceMode.Force);
    }
    
    private void ApplyBoostForce(Vector3 direction)
    {
        if (direction.magnitude < 0.1f) direction = _orientationTransform.forward;
        _isBoosting = true;
        _playerRigidbody.AddForce(direction * _qTapForce, ForceMode.Impulse);
    }
    
    private void ApplySpeedLimit()
    {
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        float currentMaxSpeed = _isBoosting ? _maxBoostSpeed : _maxMovementSpeed;

        if (flatVelocity.magnitude > currentMaxSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * currentMaxSpeed;
            _playerRigidbody.linearVelocity = new Vector3(limitedVelocity.x, _playerRigidbody.linearVelocity.y, limitedVelocity.z);
        }
        
        if (!_isBoosting && flatVelocity.magnitude > _movementSpeed) _isBoosting = true;
        else if (flatVelocity.magnitude <= _movementSpeed + 0.1f) _isBoosting = false;
    }

    private void ApplyCustomDrag()
    {
        if (_isBraking) _playerRigidbody.linearDamping = _brakeDrag;
        else if (_isBoosting) _playerRigidbody.linearDamping = _boostDrag;
        else if (_isGrounded) _playerRigidbody.linearDamping = 0f;
        else _playerRigidbody.linearDamping = _airDrag;
        
        if (Input.GetKeyDown(KeyCode.Space) || _isBraking) _isBoosting = false;
    }

    private void SetPlayerJumping()
    {
        if (_audioManager != null) _audioManager.Play(SoundType.JumpSound);
        OnPlayerJumped?.Invoke();
        _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        _playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        _isGrounded = false;
        _isBoosting = false;
    }

    // --- BUFF SİSTEMİ ---
    public void ApplyGoldEffect() { TryStopActiveEffect(); _activeEffectCoroutine = StartCoroutine(GoldEffectCoroutine()); }
    public void ApplyHolyEffect() { TryStopActiveEffect(); _activeEffectCoroutine = StartCoroutine(HolyEffectCoroutine()); }

    private void TryStopActiveEffect()
    {
        if (_activeEffectCoroutine != null)
        {
            StopCoroutine(_activeEffectCoroutine);
            if (_activeBuffType != BuffType.None) OnBuffEnded?.Invoke(_activeBuffType);
            ResetStats();
        }
    }

    private void ResetStats()
    {
        _movementSpeed = _originalMovementSpeed;
        _maxMovementSpeed = _originalMaxMovementSpeed;
        _jumpForce = _originalJumpForce;
        _activeEffectCoroutine = null;
        _activeBuffType = BuffType.None; 
    }

    private IEnumerator GoldEffectCoroutine()
    {
        OnBuffStarted?.Invoke(BuffType.Speed);
        _activeBuffType = BuffType.Speed; 
        _movementSpeed = _goldSpeedBoost;
        _maxMovementSpeed = _goldSpeedBoost;
        yield return new WaitForSeconds(_effectDuration);
        OnBuffEnded?.Invoke(BuffType.Speed);
        ResetStats();
    }
    
    private IEnumerator HolyEffectCoroutine()
    {
        OnBuffStarted?.Invoke(BuffType.Jump);
        _activeBuffType = BuffType.Jump; 
        _jumpForce = _holyJumpBoost;
        yield return new WaitForSeconds(_effectDuration);
        OnBuffEnded?.Invoke(BuffType.Jump);
        ResetStats();
    }

    // --- UI ve DİĞERLERİ ---
    private void CheckAndBroadcastUIMode()
    {
        PlayerUIMode newMode;
        if (_isBoosting)
        {
            if (_boostUICoroutine != null) { StopCoroutine(_boostUICoroutine); _boostUICoroutine = null; }
            newMode = PlayerUIMode.Boosting;
        }
        else if (_isBraking) newMode = PlayerUIMode.Braking;
        else
        {
            if (_currentUIMode == PlayerUIMode.Boosting)
            {
                if (_boostUICoroutine == null) _boostUICoroutine = StartCoroutine(BoostUICooldown());
                newMode = PlayerUIMode.Boosting; 
            }
            else newMode = PlayerUIMode.Normal;
        }
        if (newMode != _currentUIMode)
        {
            _currentUIMode = newMode;
            OnUIModeChanged?.Invoke(_currentUIMode);
        }
    }
    
    private IEnumerator BoostUICooldown()
    {
        yield return new WaitForSeconds(_boostUICooldown);
        if (!_isBoosting && !_isBraking)
        {
            _currentUIMode = PlayerUIMode.Normal;
            OnUIModeChanged?.Invoke(PlayerUIMode.Normal);
        }
        _boostUICoroutine = null;
    }

    private bool IsGroundedRaycast() { return Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundLayer); }
    private void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (Vector3.Angle(collision.contacts[i].normal, Vector3.up) < 45f) { _isGrounded = true; return; }
        }
    }
    private void OnCollisionExit(Collision collision) { _isGrounded = IsGroundedRaycast(); }
    public Rigidbody GetPlayerRigidbody() { return _playerRigidbody; }
}