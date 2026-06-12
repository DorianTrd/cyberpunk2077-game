using UnityEngine;
using TMPro; // OBLIGATOIRE pour utiliser TextMeshPro

public class ScoreManager : MonoBehaviour
{
    // Permet d'accéder au ScoreManager depuis n'importe quel autre script facilement
    public static ScoreManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _killText; // Glisse ton texte "Kills" ici
    [SerializeField] private TextMeshProUGUI _bestText; // <-- NOUVEAU : Glisse ton texte "Best" ici

    private int _killCount = 0;
    private int _bestCount = 0; // Stocke le record en cours de partie

    private void Awake()
    {
        // Système de "Singleton" pour s'assurer qu'il n'y a qu'un seul ScoreManager dans le jeu
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 💾 CHARGEMENT DU RECORD : On va chercher la note "BestKills" enregistrée. 
        // Si elle n'existe pas encore (première partie), Unity renverra 0 automatiquement.
        _bestCount = PlayerPrefs.GetInt("BestKills", 0);

        // Au début du jeu, on s'assure que les deux textes affichent les bonnes valeurs
        UpdateKillUI();
        UpdateBestUI();
    }

    // Cette fonction sera appelée par l'ennemi au moment de sa mort
    public void AddKill()
    {
        _killCount++;
        UpdateKillUI();

        // 🏆 VÉRIFICATION DU RECORD : Si le score actuel dépasse le record
        if (_killCount > _bestCount)
        {
            _bestCount = _killCount; // Le record devient le score actuel
            UpdateBestUI();

            // Enregistre immédiatement le nouveau record sur le disque dur
            PlayerPrefs.SetInt("BestKills", _bestCount);
            PlayerPrefs.Save(); 
        }
    }

    // Met à jour l'affichage des Kills actuels
    private void UpdateKillUI()
    {
        if (_killText != null)
        {
            _killText.text = "Kills: " + _killCount;
        }
    }

    // NOUVEAU : Met à jour l'affichage du Record
    private void UpdateBestUI()
    {
        if (_bestText != null)
        {
            _bestText.text = "Best: " + _bestCount;
        }
    }

    // 🌟 EN JEU : Permet aux autres scripts (comme l'activateur de spawner) de lire le score
    public int GetKillCount()
    {
        return _killCount;
    }
}