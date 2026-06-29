using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Cible à suivre")]
    [SerializeField] private Transform target; 

    [Header("Réglages du suivi")]
    [SerializeField, Range(0.01f, 1f)] private float smoothTime = 0.15f; 
    
    [Header("Limites de la Map (Axe X uniquement)")]
    [SerializeField] private float minX = 0f;   //limite gauche
    [SerializeField] private float maxX = 24.5f;  //limite droite
    
    private float _fixedY; 
    private Vector2 _currentVelocity; 

    private void Start()
    {
        if (target != null)
        {
            _fixedY = transform.position.y;
            
     
            float startX = Mathf.Clamp(target.position.x, minX, maxX);
            transform.position = new Vector3(startX, _fixedY, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        if (!target) return;

        // 1. On récupère la position X de Johnny
        float targetX = target.position.x;
        
        // 2. BLOQUAGE : Si Johnny dépasse 24 ou descend sous 0, targetX reste bloqué à 24 ou 0
        targetX = Mathf.Clamp(targetX, minX, maxX);

        // 3. On applique la fluidité vers cette position bloquée
        float smoothedX = Mathf.SmoothDamp(transform.position.x, targetX, ref _currentVelocity.x, smoothTime);

        // 4. On déplace la caméra
        transform.position = new Vector3(smoothedX, _fixedY, transform.position.z);
    }
}