using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("--- HAUT-PARLEURS ---")]
    [SerializeField] private AudioSource _musicSource; 
    [SerializeField] private AudioSource _sfxSource;   
    [SerializeField] private AudioSource _voiceSource; 

    [Header("--- MUSIQUES ---")]
    [SerializeField] private AudioClip _introVocaleMenu;  // L'audio de démarrage (laisser VIDE si inutilisé)
    [SerializeField] private AudioClip _musiqueMenu;       // Musique du menu principal
    [SerializeField] private AudioClip[] _musiquesJeu;
    
    [Header("--- EFFETS SONORES (SFX) ---")]
    [SerializeField] private AudioClip _sonTirPistolet;
    [SerializeField] private AudioClip _sonCoupEnnemi;
    [SerializeField] private AudioClip[] _sonsMortEnnemi;

    [Header("--- VOIX DE JOHNNY ---")]
    [SerializeField] private AudioClip[] _repliquesJohnny; 
    [SerializeField] private float _tempsEntreRepliques = 1.5f; 
    
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
        _timerVoix = _tempsEntreRepliques;
    }

    private void Start()
    {
        // Si la case Intro est remplie, on la joue avant la musique de fond
        if (_introVocaleMenu != null)
        {
            _musicSource.clip = _introVocaleMenu;
            _musicSource.loop = false;
            _musicSource.Play();
            Invoke(nameof(JouerMusiqueMenu), _introVocaleMenu.length);
        }
        else // Sinon, la musique de menu démarre immédiatement
        {
            JouerMusiqueMenu();
        }
    }

    private void Update()
    {
        if (Time.timeScale > 0f && _repliquesJohnny.Length > 0 && EstEnTrainDeJouerUneMusiqueDeJeu())
        {
            _timerVoix -= Time.deltaTime;
            if (_timerVoix <= 0f)
            {
                JouerVoixAleatoireJohnny();
                _timerVoix = _tempsEntreRepliques + Random.Range(-3f, 4f); 
            }
        }
    }

    private bool EstEnTrainDeJouerUneMusiqueDeJeu()
    {
        if (_musicSource.clip == null) return false;
        foreach (var musique in _musiquesJeu)
        {
            if (_musicSource.clip == musique) return true;
        }
        return false;
    }

    public void JouerMusiqueMenu()
    {
        CancelInvoke(nameof(JouerMusiqueMenu));
        if (_musiqueMenu == null || _musicSource.clip == _musiqueMenu) return;
        _musicSource.clip = _musiqueMenu;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void JouerMusiqueJeu()
    {
        CancelInvoke(nameof(JouerMusiqueMenu)); 
        if (_musiquesJeu == null || _musiquesJeu.Length == 0) return;

        int indexAleatoire = Random.Range(0, _musiquesJeu.Length);
        AudioClip musiqueChoisie = _musiquesJeu[indexAleatoire];

        if (_musicSource.clip == musiqueChoisie) return;

        _musicSource.clip = musiqueChoisie;
        _musicSource.loop = true; 
        _musicSource.Play();
    }

    public void PlayTir() => _sfxSource.PlayOneShot(_sonTirPistolet, 1.0f);
    
    public void PlayCoupEnnemi() => _sfxSource.PlayOneShot(_sonCoupEnnemi, 5f);
   
    public void PlayMortEnnemi()
    {
        if (_sonsMortEnnemi == null || _sonsMortEnnemi.Length == 0) return;
        int indexAleatoire = Random.Range(0, _sonsMortEnnemi.Length);
        _sfxSource.PlayOneShot(_sonsMortEnnemi[indexAleatoire], 1.0f);
    }

    private void JouerVoixAleatoireJohnny()
    {
        if (_voiceSource.isPlaying) return; 
        int index = Random.Range(0, _repliquesJohnny.Length);
        _voiceSource.PlayOneShot(_repliquesJohnny[index]);
    }

    public void StopToutLesSons()
    {
        _musicSource.Stop();
        _sfxSource.Stop();
        _voiceSource.Stop();
    }
}