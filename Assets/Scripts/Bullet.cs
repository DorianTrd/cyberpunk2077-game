using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField, Range(1f, 50f)] private float _speed = 20f;
    [SerializeField] private int _damage = 1;

    private void Update()
    {
        // CORRECTION : Avance vers le "devant" de la balle (prend en compte la rotation du FirePoint)
        transform.Translate(transform.right * (_speed * Time.deltaTime), Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }

        if (other.CompareTag("Obstacle") || other.CompareTag("Finish"))
        {
            Debug.Log("La balle a touché le mur !");
            Destroy(gameObject); 
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Touché ! Dégâts infligés à l'ennemi.");

            // Maintenant que EnemyAI a ", IDamageable", cette ligne fonctionne parfaitement !
            other.GetComponent<IDamageable>()?.TakeDamage(_damage);

            Destroy(gameObject); 
            return;
        }
    }
}