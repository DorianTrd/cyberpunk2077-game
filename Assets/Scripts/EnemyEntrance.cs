using UnityEngine;

public class EnemyEntrance : MonoBehaviour
{
    private float _wallX;
    private bool _hasEntered = false;
    private Collider2D[] _myColliders;
    private Collider2D _rightWallCollider;

    public void Setup(float wallXPosition)
    {
        _wallX = wallXPosition;
        _myColliders = GetComponents<Collider2D>();

        // On va chercher le collider physique du mur de droite directement
        GameObject rightWallObj = GameObject.Find("Border_Right");
        if (rightWallObj != null)
        {
            _rightWallCollider = rightWallObj.GetComponent<Collider2D>();
        }

        // Si on a trouvé le mur de droite, on dit à Unity d'ignorer la collision 
        // UNIQUEMENT entre cet ennemi et CE mur spécifique.
        if (_rightWallCollider != null && _myColliders != null)
        {
            foreach (var col in _myColliders)
            {
                Physics2D.IgnoreCollision(col, _rightWallCollider, true);
            }
        }
    }

    void Update()
    {
        if (_hasEntered) return;

        // Dès que l'ennemi passe à gauche du mur de droite
        if (transform.position.x < _wallX)
        {
            _hasEntered = true;
            
            // On réactive la collision avec le mur de droite (au cas où il veut reculer)
            if (_rightWallCollider != null && _myColliders != null)
            {
                foreach (var col in _myColliders)
                {
                    Physics2D.IgnoreCollision(col, _rightWallCollider, false);
                }
            }

            // On nettoie le script
            Destroy(this);
        }
    }
}