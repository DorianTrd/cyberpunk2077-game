using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization; 

public class MenuManager : MonoBehaviour
{
    [FormerlySerializedAs("_menuPanel")]
    [Header("Éléments de l'UI")]
    [SerializeField] private GameObject menuPanel;   
    [FormerlySerializedAs("_gameHUDPanel")] [SerializeField] private GameObject gameHUDPanel;     
    [FormerlySerializedAs("_menuRestartPanel")] [SerializeField] private GameObject menuRestartPanel;  

    [FormerlySerializedAs("_playerMovementScript")]
    [Header("Le Joueur")]
    [SerializeField] private MonoBehaviour playerMovementScript; 

    void Start()
    {
 
        Time.timeScale = 1f;


        if (menuRestartPanel != null) menuRestartPanel.SetActive(false);

       
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.JouerMusiqueMenu();
        }


        if (menuPanel != null) menuPanel.SetActive(true);
        
    
        if (gameHUDPanel != null) gameHUDPanel.SetActive(false);


        if (playerMovementScript != null) playerMovementScript.enabled = false;
    }
    
    

    public void BoutonStartPresse()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (menuRestartPanel != null) menuRestartPanel.SetActive(false);
        if (gameHUDPanel != null) gameHUDPanel.SetActive(true);


        if (playerMovementScript != null) playerMovementScript.enabled = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.JouerMusiqueJeu();
        }

        Debug.Log("Le jeu commence !");
    }


    public void RechargerLaPartie()
    {
        Time.timeScale = 1f; 

        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}