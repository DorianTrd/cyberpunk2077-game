using System; // OBLIGATOIRE pour utiliser Action (l'événement pour l'UI)
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IDamageable
{
    // ÉVÉNEMENT POUR L'UI
    // L'UI va s'abonner à cette alerte pour savoir quand éteindre les cœurs
    public event Action<int> OnPlayerHealthChanged;

    [SerializeField, Range(1f, 20f)] private float speed = 5f;
    
    [Header("Santé de Johnny")]
    public int maxHealth = 3; // Mis en public pour que le script HealthUI puisse lire le chiffre "3"
    [SerializeField] private GameObject bloodPrefab; 
    private int currentHealth;
    private bool isDead = false;

    [Header("Système de Tir")]
    [SerializeField] private Transform firePoint; 

    [Header("Effets Visuels")]
    [SerializeField] private Transform cigaretteFumee; // <-- NOUVEAU : Glisse l'objet de ta fumée ici !

    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _anim;
    private SpriteRenderer _spriteRenderer; 
    private bool _canMove = true; 
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        currentHealth = maxHealth;
    }

    // FONCTION AJOUTÉE : Permet au script HealthUI de lire la vie de Johnny sans erreur d'accès
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void OnMove(InputValue value)
    {
        if (isDead) return;

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
        if (!_canMove || isDead) return; 

        SetCanMove(false);
        _anim.SetTrigger("Shoot");

        Invoke(nameof(DebloquerApresTir), 0.5f);
    }

    private void DebloquerApresTir()
    {
        if (isDead) return;
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
        if (isDead) return;

        _rb.linearVelocity = _movement * speed;
        
        float vitesseglobale = _rb.linearVelocity.magnitude;
        _anim.SetFloat("Velocity", vitesseglobale);

        // GESTION DU RETOURNEMENT (JOHNNY + FIREPOINT + FUMÉE)
        if (_spriteRenderer != null)
        {
            if (_movement.x > 0.1f)
            {
                _spriteRenderer.flipX = false; // Regarde à DROITE

                // Reset du FirePoint
                if (firePoint != null && firePoint.localEulerAngles.y != 0)
                {
                    firePoint.localEulerAngles = new Vector3(0, 0, 0);
                    firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
                }

                // NOUVEAU : On remet la fumée à DROITE (Position X positive)
                if (cigaretteFumee != null && cigaretteFumee.localPosition.x < 0)
                {
                    cigaretteFumee.localPosition = new Vector3(Mathf.Abs(cigaretteFumee.localPosition.x), cigaretteFumee.localPosition.y, cigaretteFumee.localPosition.z);
                    cigaretteFumee.localEulerAngles = new Vector3(0, 0, 0);
                }
            }
            else if (_movement.x < -0.1f)
            {
                _spriteRenderer.flipX = true;  // Regarde à GAUCHE

                // Pivot du FirePoint
                if (firePoint != null && firePoint.localEulerAngles.y != 180)
                {
                    firePoint.localEulerAngles = new Vector3(0, 180, 0);
                    firePoint.localPosition = new Vector3(-Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, firePoint.localPosition.z);
                }

                // NOUVEAU : On pousse la fumée à GAUCHE (Position X négative)
                if (cigaretteFumee != null && cigaretteFumee.localPosition.x > 0)
                {
                    cigaretteFumee.localPosition = new Vector3(-Mathf.Abs(cigaretteFumee.localPosition.x), cigaretteFumee.localPosition.y, cigaretteFumee.localPosition.z);
                    cigaretteFumee.localEulerAngles = new Vector3(0, 180, 0);
                }
            }
        }
    }

    // GESTION DES DÉGÂTS REÇUS (Appelée par les ennemis)
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log("Johnny a été touché ! PV restants : " + currentHealth);

        // ALERT UI : On envoie la nouvelle vie actuelle au script HealthUI
        OnPlayerHealthChanged?.Invoke(currentHealth);

        if (bloodPrefab != null)
        {
            GameObject bloodEffect = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
            Destroy(bloodEffect, 0.3f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        _movement = Vector2.zero;
        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;

        gameObject.SetActive(false); 
        Debug.Log("Game Over ! Johnny a disparu.");
    }
}