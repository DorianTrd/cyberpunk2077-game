using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;

    // Appelé via Animation Event
    public void SpawnBullet()
    {
        Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
    }
}