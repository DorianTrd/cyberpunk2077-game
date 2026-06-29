using System; // OBLIGATOIRE pour utiliser l'Action UI
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IDamageable
{
    // ÉVÉNEMENT POUR L'UI
    public event Action<int> OnPlayerHealthChanged;

    [SerializeField, Range(1f, 20f)] private float speed = 5f;
    
    [Header("Santé de Johnny")]
    public int maxHealth = 3; 
    [SerializeField] private GameObject bloodPrefab; 
    [SerializeField] private GameObject panelRestart; // <-- Relie ton "MenuRestartPanel" ici
    private int _currentHealth;
    private bool _isDead = false;

    [Header("Système de Tir")]
    [SerializeField] private Transform firePoint; 

    [Header("Effets Visuels")]
    [SerializeField] private Transform cigaretteFumee; 

    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _anim;
    private SpriteRenderer _spriteRenderer; 
    private Collider2D _collider; 
    private bool _canMove = true; 
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Réinitialisation complète des états physiques et visuels
        Time.timeScale = 1f; 
        _isDead = false;
        _canMove = true;
        _movement = Vector2.zero;

        if (_rb != null)
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.linearVelocity = Vector2.zero;
        }

        if (_collider != null) _collider.enabled = true;
        if (_spriteRenderer != null) _spriteRenderer.enabled = true; 

        _currentHealth = maxHealth;
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }

    private void OnMove(InputValue value)
    {
        if (_isDead) return;
        Vector2 inputVector = value.Get<Vector2>();
        
        if (!_canMove) 
        {
            if (inputVector == Vector2.zero) _movement = Vector2.zero;
            return; 
        }
        _movement = inputVector;
    }

    private void OnAttack()
    {
        if (!_canMove || _isDead) return; 

        SetCanMove(false);
        _anim.SetTrigger("Shoot");
        Invoke(nameof(DebloquerApresTir), 0.5f);
    }

    private void DebloquerApresTir()
    {
        if (_isDead) return;
        SetCanMove(true);
    }

    public void SetCanMove(bool value)
    {
        _canMove = value;
        if (!value) 
        {
            _movement = Vector2.zero;
            _rb.linearVelocity = Vector2.zero; 
        }
    }

    private void FixedUpdate()
    {
        if (_isDead) return;

        _rb.linearVelocity = _movement * speed;
        _anim.SetFloat("Velocity", _rb.linearVelocity.magnitude);

        if (_spriteRenderer != null)
        {
            if (_movement.x > 0.1f)
            {
                _spriteRenderer.flipX = false;
                
                // --- CONFIGURATION DROITE ---
                if (firePoint != null)
                {
                    firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
                    firePoint.localRotation = Quaternion.Euler(0, 0, 0);
                }

                // 🌟 NOUVEAU : On oriente la fumée vers la droite
                if (cigaretteFumee != null)
                {
                    cigaretteFumee.localPosition = new Vector3(Mathf.Abs(cigaretteFumee.localPosition.x), cigaretteFumee.localPosition.y, cigaretteFumee.localPosition.z);
                    cigaretteFumee.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else if (_movement.x < -0.1f)
            {
                _spriteRenderer.flipX = true;
                
                // --- CONFIGURATION GAUCHE ---
                if (firePoint != null)
                {
                    firePoint.localPosition = new Vector3(-Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
                    firePoint.localRotation = Quaternion.Euler(0, 180, 0);
                }

                // 🌟 NOUVEAU : On déplace la fumée à gauche et on la fait pivoter sur l'axe Y à 180°
                if (cigaretteFumee != null)
                {
                    cigaretteFumee.localPosition = new Vector3(-Mathf.Abs(cigaretteFumee.localPosition.x), cigaretteFumee.localPosition.y, cigaretteFumee.localPosition.z);
                    cigaretteFumee.localRotation = Quaternion.Euler(0, 180, 0);
                }
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        _currentHealth -= amount;
        if (_currentHealth < 0) _currentHealth = 0;
        
        OnPlayerHealthChanged?.Invoke(_currentHealth);

        if (bloodPrefab != null)
        {
            GameObject bloodEffect = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
            Destroy(bloodEffect, 0.3f);
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (_isDead) return;
        _isDead = true;

        _movement = Vector2.zero;
        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 🔊 AUDIO : On coupe les sons du jeu
        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.StopToutLesSons();
        }

        // On affiche le menu Game Over
        if (panelRestart != null) 
        {
            panelRestart.SetActive(true);
        }
    
        // On gèle le temps du jeu
        Time.timeScale = 0f;

        // Disparition propre du joueur
        if (_spriteRenderer != null) _spriteRenderer.enabled = false;
        if (_collider != null) _collider.enabled = false;

        Debug.Log("Game Over !");
    }
}