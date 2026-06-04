using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Configuration du Tir")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;

    // Appelé via l'Animation Event de ton animation de tir
    public void SpawnBullet()
    {
        if (_bulletPrefab == null || _firePoint == null)
        {
            Debug.LogError("Attention : Le Prefab de la balle ou le FirePoint est manquant sur " + gameObject.name);
            return;
        }

        // On utilise la rotation du _firePoint au lieu de Quaternion.identity.
        // Comme ça, si un jour Johnny se retourne vers la gauche, la balle partira vers la gauche !
        Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
    }
}