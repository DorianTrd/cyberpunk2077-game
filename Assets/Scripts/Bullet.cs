using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField, Range(1f, 50f)] private float _speed = 20f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private GameObject _bloodPrefab; // Glisse ton Prefab de sang ici dans Unity

    private void Update()
    {
        transform.Translate(transform.right * (_speed * Time.deltaTime), Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        if (other.CompareTag("Obstacle") || other.CompareTag("Finish"))
        {
            Destroy(gameObject); 
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            // 1. Infliger les dégâts à l'ennemi
            other.GetComponent<IDamageable>()?.TakeDamage(_damage);

            // 2. Faire apparaître le sang pile à l'endroit du choc
            if (_bloodPrefab != null)
            {
                GameObject bloodEffect = Instantiate(_bloodPrefab, transform.position, Quaternion.identity);
                
                // On détruit le sang après 0.3 seconde pour couper l'anim avant qu'elle ne reboucle !
                Destroy(bloodEffect, 0.3f); 
            }

            // 3. Détruire la balle
            Destroy(gameObject); 
            return;
        }
    }
}