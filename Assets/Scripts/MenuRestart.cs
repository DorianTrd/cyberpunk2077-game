using UnityEngine;
using UnityEngine.SceneManagement; // Obligatoire pour charger des scènes

public class MenuRestart : MonoBehaviour
{
    [SerializeField] private GameObject panelRestart; // <-- On lui donne aussi accès au panel ici
    
    public void RecommencerPartie()
    {
        // On remet le temps à la normale (1 = vitesse normale)
        Time.timeScale = 1f;

        // On recharge la scène actuelle de zéro
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}