using UnityEngine;
using UnityEngine.SceneManagement; // OBLIGATOIRE pour recharger la scène

public class MenuManager : MonoBehaviour
{
    [Header("Éléments de l'UI")]
    [SerializeField] private GameObject _menuPanel;        // Ton MenuPrincipalPanel
    [SerializeField] private GameObject _gameHUDPanel;      // Optionnel (Peut rester vide)
    [SerializeField] private GameObject _menuRestartPanel;  // Ton MenuRestartPanel (Le Game Over)

    [Header("Le Joueur")]
    [SerializeField] private MonoBehaviour _playerMovementScript; // Le script PlayerController de Johnny

    void Start()
    {
        // On remet obligatoirement le temps à flot au chargement
        Time.timeScale = 1f;

        // 🌟 SÉCURITÉ ABSOLUE : On éteint le Game Over EN PREMIER !
        if (_menuRestartPanel != null) _menuRestartPanel.SetActive(false);

        // 🔊 AUDIO : Lancement de la musique du menu principal
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.JouerMusiqueMenu();
        }

        // Configuration visuelle de départ
        if (_menuPanel != null) _menuPanel.SetActive(true);
        
        // On ne touche au HUD que s'il y a quelque chose dans la case, sinon on s'en fout
        if (_gameHUDPanel != null) _gameHUDPanel.SetActive(false);

        // Johnny ne bouge pas tant qu'on est au menu principal
        if (_playerMovementScript != null) _playerMovementScript.enabled = false;
    }
    
    
    // Appelé par le bouton START
    public void BoutonStartPresse()
    {
        if (_menuPanel != null) _menuPanel.SetActive(false);
        if (_menuRestartPanel != null) _menuRestartPanel.SetActive(false);
        if (_gameHUDPanel != null) _gameHUDPanel.SetActive(true);

        // Johnny est libre de bouger
        if (_playerMovementScript != null) _playerMovementScript.enabled = true;

        // 🔊 AUDIO : On bascule sur les musiques de combat !
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.JouerMusiqueJeu();
        }

        Debug.Log("Le jeu commence !");
    }

    // Appelé par tes boutons RESTART et MAIN MENU du Game Over
    public void RechargerLaPartie()
    {
        Time.timeScale = 1f; // On dégèle le temps avant de charger

        // 🌟 NETTOYAGE FORCÉ : Évite les crashs visuels de l'inspecteur Unity (MissingReferenceException)
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        
        // Recharge la scène actuelle de zéro
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}