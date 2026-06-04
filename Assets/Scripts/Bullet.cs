using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField, Range(1f, 50f)] private float _speed = 20f;
    [SerializeField] private int _damage = 1;

    private void Update()
    {
        // Avance en ligne droite vers la droite
        transform.Translate(Vector2.right * (_speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Sécurité : On ignore le joueur (Johnny) si la balle le frôle en sortant du pistolet
        if (other.CompareTag("Player"))
        {
            return;
        }

        // 2. Si la balle touche un mur ou le décor (Tag "Obstacle" ou "Finish")
        if (other.CompareTag("Obstacle") || other.CompareTag("Finish"))
        {
            Debug.Log("La balle a touché le mur !");
            Destroy(gameObject); // La balle s'autodétruit
            return;
        }

        // 3. Pour plus tard : Si elle touche un ennemi (Tag "Enemy")
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Touché ! Dégâts infligés à l'ennemi.");

            // On cherche si l'ennemi a un système de vie et on lui applique les dégâts
            other.GetComponent<IDamageable>()?.TakeDamage(_damage);

            Destroy(gameObject); // La balle s'autodétruit après l'impact
            return;
        }
    }
}