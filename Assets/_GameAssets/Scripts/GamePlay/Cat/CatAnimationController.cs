using UnityEngine;
using UnityEngine.AI;

public class CatAnimationController : MonoBehaviour
{
    private Animator _catAnimator;
    private NavMeshAgent _catAgent;
    private CatStateController _catStateController;

    private void Awake() 
    {
        _catAnimator = GetComponentInChildren<Animator>();
        _catStateController = GetComponent<CatStateController>(); 
        _catAgent = GetComponent<NavMeshAgent>();

        if (_catAnimator == null) Debug.LogError("HATA: Kedi üzerinde Animator yok!");
    }

    private void Update() 
    {
        if (_catStateController == null || _catAnimator == null) return;
        SetCatAnimations();    
    }

    private void SetCatAnimations()
    {
        var currentState = _catStateController.GetCurrentState();
        
        // --- GELİŞMİŞ HAREKET KONTROLÜ ---
        // Kedi gerçekten hareket ediyor mu? (Hızı 0.1'den büyükse)
        bool isMoving = _catAgent.velocity.magnitude > 0.1f;
        
        bool isChasing = (currentState == CatState.Chasing);
        bool isAttacking = (currentState == CatState.Catched);

        // Eğer saldırıyorsa diğerlerini boşver
        if (isAttacking)
        {
            _catAnimator.SetBool("IsAttacking", true);
            _catAnimator.SetBool("IsRunning", false);
            _catAnimator.SetBool("IsChasing", false);
            _catAnimator.SetBool("IsIdling", false);
            return;
        }

        // Hareket ediyorsa koş, etmiyorsa dur
        _catAnimator.SetBool("IsRunning", isMoving); 
        _catAnimator.SetBool("IsIdling", !isMoving);
        
        // Kovalama animasyonu varsa
        _catAnimator.SetBool("IsChasing", isChasing && isMoving);
    }
}