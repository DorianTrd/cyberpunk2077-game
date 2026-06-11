using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Configuration du Tir")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;

    public void SpawnBullet()
    {
        if (_bulletPrefab == null || _firePoint == null) return;

        // Applique maintenant la position ET la rotation corrigée (gauche ou droite)
        Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);

        // 🔊 AUDIO : Bruit de tir !
        if (AudioManager.Instance != null) AudioManager.Instance.PlayTir();
    }
}