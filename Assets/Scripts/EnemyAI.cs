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
    private int currentHealth;
    private bool isDead = false;

    [Header("Mouvements Imprévisibles (Zigzag)")]
    [SerializeField] private float changeMovementInterval = 0.6f; 
    [SerializeField, Range(0f, 100f)] private float zigzagChance = 60f; 
    private float nextMovementChangeTime = 0f;
    private float currentYOffset = 0f; 

    [Header("Zone de Frappe (Hitbox)")]
    [SerializeField] private Transform attackPoint;   
    [SerializeField] private float attackRadius = 0.5f; 

    private Transform playerTransform;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private float nextAttackTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    public void Init(EnemyData data)
    {
        this.speed = data.speed;
        this.maxHealth = data.maxHealth;
        this.currentHealth = data.maxHealth;

        // Optionnel : teinter l'ennemi selon la couleur du ScriptableObject
        if (spriteRenderer != null) spriteRenderer.color = data.debugColor;
    }

   void FixedUpdate()
    {
        if (isDead || playerTransform == null || !playerTransform.gameObject.activeInHierarchy)
        {
            rb.linearVelocity = Vector2.zero;
            if (anim != null) anim.SetBool("IsWalking", false);
            return; 
        }

        float distanceToPlayer = Vector2.Distance(rb.position, playerTransform.position);

        // MODE ATTAQUE
        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = Vector2.zero; 
            if (anim != null) anim.SetBool("IsWalking", false);

            if (Time.time >= nextAttackTime) TriggerAttack();
        }
        // MODE TRAQUE (ZIGZAG FILTRÉ PAR LA PHYSIQUE)
        else
        {
            if (Time.time >= nextMovementChangeTime)
            {
                CalculerProchainChoixDeMouvement();
            }

            Vector2 directionToPlayer = ((Vector2)playerTransform.position - rb.position).normalized;
            Vector2 finalDirection = directionToPlayer;

            if (currentYOffset != 0)
            {
                finalDirection += new Vector2(0f, currentYOffset);
                finalDirection.Normalize(); 
            }

            // CORRECTION RADICALE : On repasse par la velocity physique standard, 
            // mais gérée proprement dans le FixedUpdate pour que les murs Default le bloquent à 100%
            rb.linearVelocity = finalDirection * speed;

            if (anim != null) anim.SetBool("IsWalking", true);

            // Gestion du visuel (Flip)
            if (directionToPlayer.x > 0.1f)
            {
                spriteRenderer.flipX = true;       
                if (attackPoint != null && attackPoint.localPosition.x < 0)
                {
                    attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
                }
            }       
            else if (directionToPlayer.x < -0.1f)
            {
                spriteRenderer.flipX = false; 
                if (attackPoint != null && attackPoint.localPosition.x > 0)
                {
                    attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
                }
            }
        }
    }

    void CalculerProchainChoixDeMouvement()
    {
        nextMovementChangeTime = Time.time + UnityEngine.Random.Range(changeMovementInterval * 0.7f, changeMovementInterval * 1.3f);

        if (UnityEngine.Random.Range(0f, 100f) <= zigzagChance)
        {
            currentYOffset = (UnityEngine.Random.Range(0, 2) == 0) ? 1f : -1f;
        }
        else
        {
            currentYOffset = 0f;
        }
    }

    void TriggerAttack()
    {
        if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy) return;

        if (anim != null) anim.SetTrigger("Attack"); 
        nextAttackTime = Time.time + attackCooldown;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null) anim.SetTrigger("Death");

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; 

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; 

        // On notifie le Manager de notre décès
        OnDeath?.Invoke(this);

        Destroy(gameObject, 2f);
    }

    public void HitPlayerEvent()
    {
        if (isDead) return;

        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position;
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(origin, attackRadius);

        foreach (var col in hitPlayers)
        {
            if (col.CompareTag("Player"))
            {
                col.GetComponent<IDamageable>()?.TakeDamage(1);
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