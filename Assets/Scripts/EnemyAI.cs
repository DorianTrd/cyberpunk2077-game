using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamageable
{
    [Header("Réglages de déplacement")]
    public float speed = 3f; 
    public float attackRange = 1.2f;     
    public float attackCooldown = 1.5f;  

    [Header("Santé")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    [Header("Système d'Esquive (Ninja)")]
    [Range(0f, 100f)] public float dodgeChance = 40f; // Augmenté un peu pour qu'il esquive plus souvent
    public float dodgeDetectionRadius = 3f;           
    public float dodgeSpeedMultiplier = 3f;          // Un peu plus rapide pour un effet "vif"
    public float dodgeCooldown = 2f;                 

    [Header("Zone de Frappe (Hitbox)")]
    [SerializeField] private Transform attackPoint;   
    [SerializeField] private float attackRadius = 0.5f; 

    private Transform playerTransform;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    
    private float nextAttackTime = 0f;
    private float nextDodgeTime = 0f;
    private Vector2 dodgeDirection = Vector2.zero;
    private float dodgeEndTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void FixedUpdate()
    {
        if (isDead || playerTransform == null || !playerTransform.gameObject.activeInHierarchy)
        {
            rb.linearVelocity = Vector2.zero;
            if (anim != null) anim.SetBool("IsWalking", false);
            return; 
        }

        // Si l'ennemi est en train d'esquiver (bond en haut ou en bas)
        if (Time.time < dodgeEndTime)
        {
            rb.linearVelocity = dodgeDirection * (speed * dodgeSpeedMultiplier);
            return; 
        }

        float distanceToPlayer = Vector2.Distance(rb.position, playerTransform.position);

        // Mode Attaque
        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = Vector2.zero; 
            if (anim != null) anim.SetBool("IsWalking", false);

            if (Time.time >= nextAttackTime) TriggerAttack();
        }
        // Mode Traque
        else
        {
            Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
            rb.linearVelocity = direction * speed; // Utilisation plus propre de linearVelocity

            if (anim != null) anim.SetBool("IsWalking", true);

            // Gestion du retournement et déplacement de la hitbox
            if (direction.x > 0.1f)
            {
                spriteRenderer.flipX = true;       
                if (attackPoint != null && attackPoint.localPosition.x < 0)
                {
                    attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
                }
            }       
            else if (direction.x < -0.1f)
            {
                spriteRenderer.flipX = false; 
                if (attackPoint != null && attackPoint.localPosition.x > 0)
                {
                    attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
                }
            }
        }
    }

    // Détection de la balle par collision (Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Bullet") && Time.time >= nextDodgeTime)
        {
            // Jet de dés : est-ce que l'ennemi a les réflexes d'esquiver cette balle ?
            if (Random.Range(0f, 100f) <= dodgeChance)
            {
                // On regarde si la balle est plus haute ou plus basse que l'ennemi
                float yDiff = other.transform.position.y - transform.position.y;

                // Si la balle arrive plutôt par le haut, l'ennemi esquive vers le bas. Sinon, vers le haut.
                float chooseY = (yDiff > 0) ? -1f : 1f;

                // On applique un mouvement purement vertical (haut ou bas) pour laisser passer le projectile
                dodgeDirection = new Vector2(0f, chooseY).normalized;

                dodgeEndTime = Time.time + 0.15f; // Durée très courte du bond rapide (0.15 seconde)
                nextDodgeTime = Time.time + dodgeCooldown; 
            }
            else
            {
                // S'il rate ses réflexes, il ne peut pas retenter l'esquive sur une autre balle immédiatement
                nextDodgeTime = Time.time + 0.4f; 
            }
        }
    }

    void TriggerAttack()
    {
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
        isDead = true;
        if (anim != null) anim.SetTrigger("Death");

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; 

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; 

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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dodgeDetectionRadius);

        Gizmos.color = Color.red;
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position;
        Gizmos.DrawWireSphere(origin, attackRadius);
    }
}