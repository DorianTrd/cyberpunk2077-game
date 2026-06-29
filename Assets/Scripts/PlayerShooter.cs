using UnityEngine;
using UnityEngine.Serialization;

public class PlayerShooter : MonoBehaviour
{
    [FormerlySerializedAs("_bulletPrefab")]
    [Header("Configuration du Tir")]
    [SerializeField] private GameObject bulletPrefab;
    [FormerlySerializedAs("_firePoint")] [SerializeField] private Transform firePoint;

    public void SpawnBullet()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Applique maintenant la position ET la rotation corrigée (gauche ou droite)
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 🔊 AUDIO : Bruit de tir !
        if (AudioManager.Instance != null) AudioManager.Instance.PlayTir();
    }
}