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
        if (heartPrefab == null || heartsContainer == null)
        {
            Debug.LogError("HealthUI : Les cases Heart Prefab ou Hearts Container ne sont pas remplies dans l'Inspecteur !");
            return;
        }


        _player = GameObject.FindAnyObjectByType<PlayerController>();

        if (_player != null)
        {

            SetupHearts(_player.maxHealth);
            

            UpdateHearts(_player.GetCurrentHealth());

    
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


        foreach (Transform child in heartsContainer)
        {
            child.gameObject.SetActive(false);
        }


        for (int i = 0; i < maxHealth; i++)
        {
            Image newHeart = Instantiate(heartPrefab, heartsContainer);
            
   
            newHeart.gameObject.SetActive(true); 
            newHeart.enabled = true; 
            
            _spawnedHearts.Add(newHeart);
        }
    }

    private void UpdateHearts(int currentHealth)
    {
         for (int i = 0; i < _spawnedHearts.Count; i++)
        {
            if (_spawnedHearts[i] != null)
            {
                bool shouldBeVisible = (i < currentHealth);
                
           
                _spawnedHearts[i].gameObject.SetActive(shouldBeVisible);
                _spawnedHearts[i].enabled = shouldBeVisible;
            }
        }
    }

    void OnDestroy()
    {

        if (_player != null)
        {
            _player.OnPlayerHealthChanged -= UpdateHearts;
        }
    }
}