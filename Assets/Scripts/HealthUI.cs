using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Réglages des Cœurs")]
    [SerializeField] private Image heartPrefab;       
    [SerializeField] private Transform heartsContainer; 

    private List<Image> _spawnedHearts = new List<Image>();
    private PlayerController _player;

    void Start()
    {
        // Sécurité de base : On verifies que les cases sont bien remplies dans l'inspecteur
        if (heartPrefab == null || heartsContainer == null)
        {
            Debug.LogError("HealthUI : Les cases Heart Prefab ou Hearts Container ne sont pas remplies dans l'Inspecteur !");
            return;
        }

        // On cherche Johnny (le joueur) dans la scène
        _player = GameObject.FindAnyObjectByType<PlayerController>();

        if (_player != null)
        {
            // 1. On génère les cœurs de départ
            SetupHearts(_player.maxHealth);
            
            // 2. MODIFICATION : On utilise GetCurrentHealth() pour obtenir la vie sans erreur de sécurité
            UpdateHearts(_player.GetCurrentHealth());

            // 3. On s'abonne à l'événement de changement de vie
            _player.OnPlayerHealthChanged += UpdateHearts;
        }
        else
        {
            Debug.LogWarning("HealthUI : Johnny (PlayerController) est introuvable dans la scène.");
        }
    }

    private void SetupHearts(int maxHealth)
    {
        _spawnedHearts.Clear();

        // On masque le contenu préexistant dans le container pour repartir sur du propre
        foreach (Transform child in heartsContainer)
        {
            child.gameObject.SetActive(false);
        }

        // On génère le nombre maximum de cœurs requis
        for (int i = 0; i < maxHealth; i++)
        {
            Image newHeart = Instantiate(heartPrefab, heartsContainer);
            
            // PROTECTION : On force l'activation de l'objet et du visuel, même si le modèle était masqué
            newHeart.gameObject.SetActive(true); 
            newHeart.enabled = true; 
            
            _spawnedHearts.Add(newHeart);
        }
    }

    private void UpdateHearts(int currentHealth)
    {
        // On boucle sur tous les cœurs générés pour les allumer ou les éteindre
        for (int i = 0; i < _spawnedHearts.Count; i++)
        {
            if (_spawnedHearts[i] != null)
            {
                bool shouldBeVisible = (i < currentHealth);
                
                // Force l'état d'activation pour basculer du "gris foncé" au mode visible
                _spawnedHearts[i].gameObject.SetActive(shouldBeVisible);
                _spawnedHearts[i].enabled = shouldBeVisible;
            }
        }
    }

    void OnDestroy()
    {
        // Sécurité : On se désabonne de l'événement si l'UI est détruite pour éviter les fuites de mémoire
        if (_player != null)
        {
            _player.OnPlayerHealthChanged -= UpdateHearts;
        }
    }
}