using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamageable
{
    public event Action<EnemyAI> OnDeath;

    [Header("Réglages de déplacement")]
    public float speed = 3f; 
    public float attackRange = 1.2f;     
    public float attackCooldown = 1.5f;  

    [Header("Santé")]
    public int maxHealth = 3;
    private int _currentHealth;
    private bool _isDead = false;

    [Header("Mouvements Imprévisibles (Zigzag)")]
    [SerializeField] private float changeMovementInterval = 0.6f; 
    [SerializeField, Range(0f, 100f)] private float zigzagChance = 60f; 
    private float _nextMovementChangeTime = 0f;
    private float _currentYOffset = 0f; 

    [Header("Zone de Frappe (Hitbox)")]
    [SerializeField] private Transform attackPoint;   
    [SerializeField] private float attackRadius = 0.5f; 

    private Transform _playerTransform;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _anim;
    private float _nextAttackTime = 0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) _playerTransform = player.transform;
    }

    public void Init(EnemyData data)
    {
        this.speed = data.speed;
        this.maxHealth = data.maxHealth;
        this._currentHealth = data.maxHealth;

        // Optionnel : teinter l'ennemi selon la couleur du ScriptableObject
        if (_spriteRenderer != null) _spriteRenderer.color = data.debugColor;
    }

    void FixedUpdate()
    {
        if (_isDead || _playerTransform == null || !_playerTransform.gameObject.activeInHierarchy)
        {
            _rb.linearVelocity = Vector2.zero;
            if (_anim != null) _anim.SetBool("IsWalking", false);
            return; 
        }

        float distanceToPlayer = Vector2.Distance(_rb.position, _playerTransform.position);

        // MODE ATTAQUE
        if (distanceToPlayer <= attackRange)
        {
            _rb.linearVelocity = Vector2.zero; 
            if (_anim != null) _anim.SetBool("IsWalking", false);

            if (Time.time >= _nextAttackTime) TriggerAttack();
        }
        // MODE TRAQUE (ZIGZAG FILTRÉ PAR LA PHYSIQUE)
        else
        {
            if (Time.time >= _nextMovementChangeTime)
            {
                CalculerProchainChoixDeMouvement();
            }

            Vector2 directionToPlayer = ((Vector2)_playerTransform.position - _rb.position).normalized;
            Vector2 finalDirection = directionToPlayer;

            if (_currentYOffset != 0)
            {
                finalDirection += new Vector2(0f, _currentYOffset);
                finalDirection.Normalize(); 
            }

            // CORRECTION RADICALE : On repasse par la velocity physique standard, 
            // mais gérée proprement dans le FixedUpdate pour que les murs Default le bloquent à 100%
            _rb.linearVelocity = finalDirection * speed;

            if (_anim != null) _anim.SetBool("IsWalking", true);

            // Gestion du visuel (Flip)
            if (directionToPlayer.x > 0.1f)
            {
                _spriteRenderer.flipX = true;       
                if (attackPoint != null && attackPoint.localPosition.x < 0)
                {
                    attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
                }
            }       
            else if (directionToPlayer.x < -0.1f)
            {
                _spriteRenderer.flipX = false; 
                if (attackPoint != null && attackPoint.localPosition.x > 0)
                {
                    attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
                }
            }
        }
    }

    void CalculerProchainChoixDeMouvement()
    {
        _nextMovementChangeTime = Time.time + UnityEngine.Random.Range(changeMovementInterval * 0.7f, changeMovementInterval * 1.3f);

        if (UnityEngine.Random.Range(0f, 100f) <= zigzagChance)
        {
            _currentYOffset = (UnityEngine.Random.Range(0, 2) == 0) ? 1f : -1f;
        }
        else
        {
            _currentYOffset = 0f;
        }
    }

    void TriggerAttack()
    {
        if (_playerTransform == null || !_playerTransform.gameObject.activeInHierarchy) return;

        if (_anim != null) _anim.SetTrigger("Attack"); 
        _nextAttackTime = Time.time + attackCooldown;
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;
        if (_currentHealth <= 0) Die();
    }

    void Die()
    {
        if (_isDead) return;
        _isDead = true;
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddKill();
        }

        if (_anim != null) _anim.SetTrigger("Death");

        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic; 

        // 🌟 CORRECTION 1 : Désactiver le collider pour la physique 2D
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; 

        // 🌟 CORRECTION 2 : Changer le Layer pour "Ignore Raycast" (Calque numéro 2 sous Unity)
        // De cette façon, même si tes balles utilisent un système de Raycast laser, le cadavre est invisible pour elles !
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        // On notifie le Manager de notre décès (ton code de spawner)
        OnDeath?.Invoke(this);

        Destroy(gameObject, 2f); // Laisse 2 secondes pour l'anim de mort
    }

    public void HitPlayerEvent()
    {
        if (_isDead) return;

        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position;
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(origin, attackRadius);

        foreach (var col in hitPlayers)
        {
            if (col.CompareTag("Player"))
            {
                col.GetComponent<IDamageable>()?.TakeDamage(1);

                // 🔊 AUDIO NOUVEAU : Le coup de poing se connecte sur Johnny, on joue le bruit de baffe !
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayCoupEnnemi();
                }

                break; 
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position;
        Gizmos.DrawWireSphere(origin, attackRadius);
    }
}