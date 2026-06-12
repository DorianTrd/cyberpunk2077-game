using UnityEngine;

public class SpawnersActivator : MonoBehaviour
{
    [Header("Le palier de Kills")]
    [SerializeField] private int _killsRequis = 20;

    [Header("L'objet à activer (EnemyManager ou SpawnPoint)")]
    [SerializeField] private GameObject _cibleAActiver;

    private bool _estActive = false;

    void Start()
    {
        // Sécurité : On s'assure que la cible est bien éteinte au tout début du jeu
        if (_cibleAActiver != null)
        {
            _cibleAActiver.SetActive(false);
        }
    }

    void Update()
    {
        // Si on a déjà activé le spawner bonus, on ne fait plus rien
        if (_estActive) return;

        // On vérifie si le ScoreManager existe et si on a atteint les kills requis
        if (ScoreManager.Instance != null && ScoreManager.Instance.GetKillCount() >= _killsRequis)
        {
            if (_cibleAActiver != null)
            {
                // 1. 🔥 On allume l'objet dans la Hierarchy
                _cibleAActiver.SetActive(true); 
                _estActive = true;
                Debug.Log($"Félicitations ! {_killsRequis} kills atteints. Activation du spawner bonus !");

                // 2. ⚡ FORCE LE DÉMARRAGE : On force TOUS les scripts attachés à l'objet à se réveiller
                // Cela va forcer Unity à exécuter les fonctions de démarrage (comme Start ou OnEnable) du spawner
                MonoBehaviour[] scripts = _cibleAActiver.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    if (script != null)
                    {
                        script.enabled = false;
                        script.enabled = true; // Désactiver puis réactiver force le réveil du script !
                    }
                }
            }
        }
    }
}