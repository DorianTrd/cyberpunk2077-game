using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField, Range(1f, 50f)] private float _speed = 20f;
    [SerializeField] private int _damage = 1;

    private void Update()
    {
        transform.Translate(Vector2.right * (_speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<IDamageable>()?.TakeDamage(_damage);
        Destroy(gameObject);
    }
}