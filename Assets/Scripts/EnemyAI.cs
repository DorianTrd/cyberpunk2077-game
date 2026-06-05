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
    [Range(0f, 100f)] public float dodgeChance = 25f; 
    public float dodgeDetectionRadius = 3f;           
    public float dodgeSpeedMultiplier = 2f;          
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

        if (Time.time < dodgeEndTime)
        {
            rb.MovePosition(rb.position + dodgeDirection * (speed * dodgeSpeedMultiplier * Time.fixedDeltaTime));
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
            rb.MovePosition(rb.position + direction * (speed * Time.fixedDeltaTime));

            if (anim != null) anim.SetBool("IsWalking", true);

            // GESTION DU RETOURNEMENT ET DU DEPLACEMENT DE LA HITBOX
            if (direction.x > 0.1f)
            {
                spriteRenderer.flipX = true;       
                
                // Si l'ennemi regarde à DROITE, on force l'AttackPoint à passer à DROITE (X positif)
                if (attackPoint != null && attackPoint.localPosition.x < 0)
                {
                    attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
                }
            }       
            else if (direction.x < -0.1f)
            {
                spriteRenderer.flipX = false; 
                
                // Si l'ennemi regarde à GAUCHE, on force l'AttackPoint à passer à GAUCHE (X négatif)
                if (attackPoint != null && attackPoint.localPosition.x > 0)
                {
                    attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, attackPoint.localPosition.z);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Bullet") && Time.time >= nextDodgeTime)
        {
            if (Random.Range(0f, 100f) <= dodgeChance)
            {
                Rigidbody2D bulletRb = other.GetComponent<Rigidbody2D>();
                Vector2 bulletVelocity = bulletRb != null ? bulletRb.linearVelocity : Vector2.right;
                
                dodgeDirection = new Vector2(-bulletVelocity.y, bulletVelocity.x).normalized;
                if (Random.Range(0, 2) == 0) dodgeDirection = -dodgeDirection; 

                dodgeEndTime = Time.time + 0.2f; 
                nextDodgeTime = Time.time + dodgeCooldown; 
            }
            else
            {
                nextDodgeTime = Time.time + 0.5f; 
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