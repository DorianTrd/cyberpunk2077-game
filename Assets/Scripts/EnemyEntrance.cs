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


        GameObject rightWallObj = GameObject.Find("Border_Right");
        if (rightWallObj != null)
        {
            _rightWallCollider = rightWallObj.GetComponent<Collider2D>();
        }

 
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


        if (transform.position.x < _wallX)
        {
            _hasEntered = true;
            
     
            if (_rightWallCollider != null && _myColliders != null)
            {
                foreach (var col in _myColliders)
                {
                    Physics2D.IgnoreCollision(col, _rightWallCollider, false);
                }
            }
            
            Destroy(this);
        }
    }
}