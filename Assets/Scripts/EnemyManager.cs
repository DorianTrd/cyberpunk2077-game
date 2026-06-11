using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Configuration du Spawn")]
    [SerializeField] private Transform spawnerPoint;       
    [SerializeField] private EnemyAI enemyPrefab;         
    [SerializeField] private Transform enemyContainer;    
    [SerializeField, Range(0.1f, 10f)] private float spawnDelay = 2f;
    [SerializeField] private float spawnYRange = 4f;       

    [Header("Données & Frontière")]
    [SerializeField] private List<EnemyData> enemyDataList; 
    [SerializeField] private Transform rightBorderTransform; // Ton objet "Border_Right"

    // Cet événement s'active dès qu'un ennemi meurt. Pratique pour un futur score !
    public event Action<EnemyAI> OnEnemyKilled;

    private readonly List<EnemyAI> _activeEnemies = new();
    private Coroutine _spawnRoutine;

    private void Start()
    {
        // Lancement automatique au démarrage du jeu
        StartSpawning();
    }

    public void ResetManager()
    {
        StopSpawning();
        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null) Destroy(enemy.gameObject);
        }
        _activeEnemies.Clear();
    }

    public void StartSpawning()
    {
        if (_spawnRoutine == null)
        {
            _spawnRoutine = StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Spawn()
    {
        if (enemyPrefab == null || enemyDataList.Count == 0 || rightBorderTransform == null || spawnerPoint == null) return;

        // Choix d'une data aléatoire (comme pour tes rubis)
        var data = enemyDataList[UnityEngine.Random.Range(0, enemyDataList.Count)];

        // Position de spawn sur l'axe Y
        float randomY = UnityEngine.Random.Range(spawnerPoint.position.y - spawnYRange, spawnerPoint.position.y + spawnYRange);
        Vector3 spawnPosition = new Vector3(spawnerPoint.position.x, randomY, spawnerPoint.position.z);

        // Instanciation
        EnemyAI newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, enemyContainer);
        
        // Initialisation avec le ScriptableObject
        newEnemy.Init(data);

        // Injection dynamique de la gestion du mur de droite
        EnemyEntrance entranceScript = newEnemy.gameObject.AddComponent<EnemyEntrance>();
        entranceScript.Setup(rightBorderTransform.position.x);

        AddEnemy(newEnemy);
    }

    private void AddEnemy(EnemyAI enemy)
    {
        _activeEnemies.Add(enemy);
        enemy.OnDeath += EnemyDeathHandler;
    }

    private void EnemyDeathHandler(EnemyAI enemy)
    {
        _activeEnemies.Remove(enemy);
        enemy.OnDeath -= EnemyDeathHandler;

        // 🔊 NOUVEAU - AUDIO : Dès qu'un ennemi meurt, on joue le son de sa mort !
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMortEnnemi();
        }
        
        // On propage l'information (pour le score si besoin)
        OnEnemyKilled?.Invoke(enemy);
    }

    private void OnDrawGizmos()
    {
        if (spawnerPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(spawnerPoint.position.x, spawnerPoint.position.y - spawnYRange, 0), 
                        new Vector3(spawnerPoint.position.x, spawnerPoint.position.y + spawnYRange, 0));
    }
}