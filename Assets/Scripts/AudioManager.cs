using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    // Permet d'accéder au AudioManager depuis n'importe quel autre script facilement
    public static AudioManager Instance { get; private set; }

    [FormerlySerializedAs("_musicSource")]
    [Header("--- HAUT-PARLEURS ---")]
    [SerializeField] private AudioSource musicSource; 
    [FormerlySerializedAs("_sfxSource")] [SerializeField] private AudioSource sfxSource;   
    [FormerlySerializedAs("_voiceSource")] [SerializeField] private AudioSource voiceSource; 

    [FormerlySerializedAs("_introVocaleMenu")]
    [Header("--- MUSIQUES ---")]
    [SerializeField] private AudioClip introVocaleMenu;  // L'audio de démarrage (laisser VIDE si inutilisé)
    
    // ⏱️ Règle ici le temps d'attente (en secondes) avant que la musique ne commence !
    [FormerlySerializedAs("_tempsAvantMusiqueMenu")] [SerializeField] private float tempsAvantMusiqueMenu = 1.0f; 
    
    [FormerlySerializedAs("_musiqueMenu")] [SerializeField] private AudioClip musiqueMenu;       // Musique du menu principal
    [FormerlySerializedAs("_musiquesJeu")] [SerializeField] private AudioClip[] musiquesJeu;
    
    [FormerlySerializedAs("_sonTirPistolet")]
    [Header("--- EFFETS SONORES (SFX) ---")]
    [SerializeField] private AudioClip sonTirPistolet;
    [FormerlySerializedAs("_sonCoupEnnemi")] [SerializeField] private AudioClip sonCoupEnnemi;
    [FormerlySerializedAs("_sonsMortEnnemi")] [SerializeField] private AudioClip[] sonsMortEnnemi;

    [FormerlySerializedAs("_repliquesJohnny")]
    [Header("--- VOIX DE JOHNNY ---")]
    [SerializeField] private AudioClip[] repliquesJohnny; 
    [FormerlySerializedAs("_tempsEntreRepliques")] [SerializeField] private float tempsEntreRepliques = 1.5f; 
    
    private float _timerVoix;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        _timerVoix = tempsEntreRepliques;
    }

    private void Start()
    {
        // Si la case Intro est remplie, on la joue avant la musique de fond
        if (introVocaleMenu != null)
        {
            musicSource.clip = introVocaleMenu;
            musicSource.loop = false;
            musicSource.Play();
            
            // 🌟 CORRECTION : Utilise maintenant le délai personnalisé au lieu d'attendre la fin du fichier
            Invoke(nameof(JouerMusiqueMenu), tempsAvantMusiqueMenu);
        }
        else // Sinon, la musique de menu démarre immédiatement
        {
            JouerMusiqueMenu();
        }
    }

    private void Update()
    {
        if (Time.timeScale > 0f && repliquesJohnny.Length > 0 && EstEnTrainDeJouerUneMusiqueDeJeu())
        {
            _timerVoix -= Time.deltaTime;
            if (_timerVoix <= 0f)
            {
                JouerVoixAleatoireJohnny();
                _timerVoix = tempsEntreRepliques + Random.Range(-3f, 4f); 
            }
        }
    }

    private bool EstEnTrainDeJouerUneMusiqueDeJeu()
    {
        if (musicSource.clip == null) return false;
        foreach (var musique in musiquesJeu)
        {
            if (musicSource.clip == musique) return true;
        }
        return false;
    }

    public void JouerMusiqueMenu()
    {
        CancelInvoke(nameof(JouerMusiqueMenu));
        if (musiqueMenu == null || musicSource.clip == musiqueMenu) return;
        musicSource.clip = musiqueMenu;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void JouerMusiqueJeu()
    {
        CancelInvoke(nameof(JouerMusiqueMenu)); 
        if (musiquesJeu == null || musiquesJeu.Length == 0) return;

        int indexAleatoire = Random.Range(0, musiquesJeu.Length);
        AudioClip musiqueChoisie = musiquesJeu[indexAleatoire];

        if (musicSource.clip == musiqueChoisie) return;

        musicSource.clip = musiqueChoisie;
        musicSource.loop = true; 
        musicSource.Play();
    }

    public void PlayTir() => sfxSource.PlayOneShot(sonTirPistolet, 1.0f);
    
    public void PlayCoupEnnemi() => sfxSource.PlayOneShot(sonCoupEnnemi, 5f);
   
    public void PlayMortEnnemi()
    {
        if (sonsMortEnnemi == null || sonsMortEnnemi.Length == 0) return;
        int indexAleatoire = Random.Range(0, sonsMortEnnemi.Length);
        sfxSource.PlayOneShot(sonsMortEnnemi[indexAleatoire], 1.0f);
    }

    private void JouerVoixAleatoireJohnny()
    {
        if (voiceSource.isPlaying) return; 
        int index = Random.Range(0, repliquesJohnny.Length);
        voiceSource.PlayOneShot(repliquesJohnny[index]);
    }

    public void StopToutLesSons()
    {
        musicSource.Stop();
        sfxSource.Stop();
        voiceSource.Stop();
    }
}