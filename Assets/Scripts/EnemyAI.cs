using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamageable // <-- Ajout de l'interface ici !
{
    [Header("Réglages de déplacement")]
    public float speed = 3f; 
    public float attackRange = 1.2f;     // Distance à laquelle l'ennemi s'arrête pour taper
    public float attackCooldown = 1.5f;  // Temps d'attente entre deux coups de poing

    [Header("Santé de l'Ennemi")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private float nextAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Récupère les composants visuels et d'animation dans l'enfant "Visual"
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        currentHealth = maxHealth;

        // Recherche automatique du joueur via son Tag
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (isDead || playerTransform is null) return;

        // 1. Calcul de la distance avec Johnny
        float distanceToPlayer = Vector2.Distance(rb.position, playerTransform.position);

        // 2. Comportement d'Attaque (Proche du joueur)
        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = Vector2.zero; 

            if (anim is not null) anim.SetBool("IsWalking", false);

            if (Time.time >= nextAttackTime)
            {
                TriggerAttack();
            }
        }
        // 3. Comportement de Traque (Loin du joueur)
        else
        {
            Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
            rb.MovePosition(rb.position + direction * (speed * Time.fixedDeltaTime));

            if (anim is not null) anim.SetBool("IsWalking", true);

            if (direction.x > 0.1f) spriteRenderer.flipX = true;       // Regarde à droite
            else if (direction.x < -0.1f) spriteRenderer.flipX = false; // Regarde à gauche
        }
    }

    void TriggerAttack()
    {
        if (anim is not null)
        {
            anim.SetTrigger("Attack"); 
        }
        nextAttackTime = Time.time + attackCooldown;
    }

    // Cette fonction répond maintenant parfaitement à l'appel de la balle
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (anim != null) 
        {
            anim.SetTrigger("Death");
        }

        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true; 

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; 

        Destroy(gameObject, 2f);
    }
}