using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class CatController : MonoBehaviour
{
    public event Action<Transform> OnCatCatched;

    [Header("References")]
    [SerializeField] public Transform _playerTransform; 
    [SerializeField] private CinemachineCamera _catCinemachineCamera;

    [Header("Settings")]
    [SerializeField] private float _defaultSpeed = 3.5f;
    [SerializeField] private float _chaseSpeed = 8f; // Hızı biraz arttırdım, daha korkunç olsun

    [Header("AI Settings")]
    [Tooltip("Bu değeri BÜYÜK yap (Mesela 20-30), masadayken bile seni fark etsin")]
    [SerializeField] private float _detectionRadius = 20f; 
    [SerializeField] private float _catchDistance = 1.2f;  

    [Header("Patrol Settings")]
    [SerializeField] private float _patrolRadius = 20f;
    [SerializeField] private float _waitTime = 2f;
    
    private NavMeshAgent _catAgent;
    private Vector3 _initialPosition;
    private float _timer;
    private bool _isWaiting;
    private bool _isChasing = false;

    private CatStateController _catStateController;

    private void Awake() 
    {
        _catStateController = GetComponent<CatStateController>();
        _catAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // Otomatik Player Bulma
        if (_playerTransform == null)
        {
            var playerScript = FindObjectOfType<PlayerController>();
            if (playerScript != null) _playerTransform = playerScript.transform;
            else
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) _playerTransform = playerObj.transform;
            }
        }

        if (_catAgent == null)
        {
            enabled = false;
            return;
        }

        _initialPosition = transform.position;
        SetRandomDestination();
    }

    private void Update()
    {
        if (_playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        // --- DEĞİŞİKLİK BURADA: KESİN GÖRÜŞ ---
        // Sadece mesafeye bakıyoruz, seni gördüğü an affetmez.
        if (distanceToPlayer < _detectionRadius)
        {
            SetChaseMovement(distanceToPlayer);
        }
        else
        {
            SetPatrolMovement();
        }
    }

    private void SetChaseMovement(float distanceToPlayer)
    {
        // --- ÖNEMLİ DÜZELTME: BEKLEMEYİ İPTAL ET ---
        // Eğer kedi köşede "saf gibi" bekliyorsa, onu zorla uyandırıyoruz!
        if (_isWaiting)
        {
            _isWaiting = false;
            _timer = 0f;
        }
        // ------------------------------------------

        if (!_isChasing)
        {
            _isChasing = true;
            if (_catStateController != null) _catStateController.ChangeState(CatState.Chasing);
        }

        _catAgent.speed = _chaseSpeed;
        _catAgent.isStopped = false; // Donmuşsa çözülsün
        _catAgent.SetDestination(_playerTransform.position); 

        // Yakalama
        if (distanceToPlayer <= _catchDistance)
        {
            Debug.Log("YAKALANDIN!");
            if (_catStateController != null) _catStateController.ChangeState(CatState.Catched);
            
            OnCatCatched?.Invoke(_playerTransform);
            _catAgent.isStopped = true; 
            
            FindObjectOfType<GameManager>()?.LoseGame();
            
            if (_catCinemachineCamera != null) _catCinemachineCamera.Priority = 20;
        }
    }

    private void SetPatrolMovement()
    {
        if (_isChasing)
        {
            _isChasing = false;
            if (_catCinemachineCamera != null) _catCinemachineCamera.Priority = 0;
        }

        _catAgent.speed = _defaultSpeed;

        if (!_catAgent.pathPending && _catAgent.remainingDistance <= _catAgent.stoppingDistance)
        {
            if (!_isWaiting)
            {
                _isWaiting = true;
                _timer = _waitTime;
                if (_catStateController != null) _catStateController.ChangeState(CatState.Idle);
            }
        }
        else
        {
             // Yürüyorsa animasyonu oynat
             if (_catStateController != null && !_isWaiting) _catStateController.ChangeState(CatState.Running);
        }

        if (_isWaiting)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _isWaiting = false;
                SetRandomDestination();
                if (_catStateController != null) _catStateController.ChangeState(CatState.Running);
            }
        }
    }

    private void SetRandomDestination()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _patrolRadius;
            randomDirection += _initialPosition;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _patrolRadius, NavMesh.AllAreas))
            {
                _catAgent.SetDestination(hit.position);
                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}