using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [FormerlySerializedAs("_speed")] [SerializeField, Range(1f, 50f)] private float speed = 20f;
    [FormerlySerializedAs("_damage")] [SerializeField] private int damage = 1;
    [FormerlySerializedAs("_bloodPrefab")] [SerializeField] private GameObject bloodPrefab; // Glisse ton Prefab de sang ici dans Unity

    private void Update()
    {
        transform.Translate(transform.right * (speed * Time.deltaTime), Space.World);
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
            other.GetComponent<IDamageable>()?.TakeDamage(damage);

            // 2. Faire apparaître le sang pile à l'endroit du choc
            if (bloodPrefab != null)
            {
                GameObject bloodEffect = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
                
                // On détruit le sang après 0.3 seconde pour couper l'anim avant qu'elle ne reboucle !
                Destroy(bloodEffect, 0.3f); 
            }

            // 3. Détruire la balle
            Destroy(gameObject); 
            return;
        }
    }
}