using System;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    private PlayerController _playerController;
    private StateController _stateController;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _stateController = GetComponent<StateController>();

        if (_playerController == null)
        {
            Debug.LogError("PlayerController bulunamadı! Animasyonlar çalışmayacak.");
            enabled = false; 
        }
    }
    
    private void Update() 
    {
        if (_playerController == null) return;
        SetPlayerAnimations();
    }

    private void Start()
    {
        if (_playerController != null)
        {
            // Mevcut zıplama olayını dinle
            _playerController.OnPlayerJumped += PlayerController_OnPlayerJumped;
            
            // YENİ EKLENDİ: Duvara çarpma olayını dinle
            _playerController.OnPlayerHitWall += PlayerController_OnPlayerHitWall;
        }
    }
    
    // --- YENİ EKLENEN ANİMASYON FONKSİYONU ---
    private void PlayerController_OnPlayerHitWall()
    {
        // Animator'deki "HitWall" trigger'ını ateşle
        _playerAnimator.SetTrigger("HitWall"); 
    }
    // ------------------------------------------

    private void PlayerController_OnPlayerJumped()
    {
        _playerAnimator.SetBool("IsJumping", true); 
        Invoke(nameof(ResetJumping), 0.5f);
    }
    
    private void ResetJumping()
    {
         _playerAnimator.SetBool("IsJumping", false);
    }

    private void SetPlayerAnimations()
    {
        Rigidbody playerRb = _playerController.GetPlayerRigidbody(); 
        
        if (playerRb == null)
        {
            Debug.LogWarning("Rigidbody henüz hazır değil veya GetPlayerRigidbody() metodu null döndürdü.");
            return; 
        }

        Vector3 flatVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
        float horizontalSpeed = flatVelocity.magnitude;
        bool isMoving = horizontalSpeed > 0.1f; 
        _playerAnimator.SetBool("IsMoving", isMoving);
        
        bool isFalling = playerRb.linearVelocity.y < -0.5f;
        _playerAnimator.SetBool("IsFalling", isFalling); 
    }
}