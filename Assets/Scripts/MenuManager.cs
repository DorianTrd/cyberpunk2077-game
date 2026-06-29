using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization; // OBLIGATOIRE pour recharger la scène

public class MenuManager : MonoBehaviour
{
    [FormerlySerializedAs("_menuPanel")]
    [Header("Éléments de l'UI")]
    [SerializeField] private GameObject menuPanel;        // Ton MenuPrincipalPanel
    [FormerlySerializedAs("_gameHUDPanel")] [SerializeField] private GameObject gameHUDPanel;      // Optionnel (Peut rester vide)
    [FormerlySerializedAs("_menuRestartPanel")] [SerializeField] private GameObject menuRestartPanel;  // Ton MenuRestartPanel (Le Game Over)

    [FormerlySerializedAs("_playerMovementScript")]
    [Header("Le Joueur")]
    [SerializeField] private MonoBehaviour playerMovementScript; // Le script PlayerController de Johnny

    void Start()
    {
        // On remet obligatoirement le temps à flot au chargement
        Time.timeScale = 1f;

        // 🌟 SÉCURITÉ ABSOLUE : On éteint le Game Over EN PREMIER !
        if (menuRestartPanel != null) menuRestartPanel.SetActive(false);

        // 🔊 AUDIO : Lancement de la musique du menu principal
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.JouerMusiqueMenu();
        }

        // Configuration visuelle de départ
        if (menuPanel != null) menuPanel.SetActive(true);
        
        // On ne touche au HUD que s'il y a quelque chose dans la case, sinon on s'en fout
        if (gameHUDPanel != null) gameHUDPanel.SetActive(false);

        // Johnny ne bouge pas tant qu'on est au menu principal
        if (playerMovementScript != null) playerMovementScript.enabled = false;
    }
    
    
    // Appelé par le bouton START
    public void BoutonStartPresse()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (menuRestartPanel != null) menuRestartPanel.SetActive(false);
        if (gameHUDPanel != null) gameHUDPanel.SetActive(true);

        // Johnny est libre de bouger
        if (playerMovementScript != null) playerMovementScript.enabled = true;

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