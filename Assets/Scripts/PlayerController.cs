using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(1f, 20f)] 
    private float speed = 5f;
    
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Animator _anim;
    private bool _canMove = true; 
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        
    }

    private void OnMove(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();
        
        if (!_canMove) 
        {
            if (inputVector == Vector2.zero)
            {
                _movement = Vector2.zero;
            }
            return; 
        }

        _movement = inputVector;
    }

    private void OnAttack()
    {
        if (!_canMove) return; 

        SetCanMove(false);
        _anim.SetTrigger("Shoot");

        Invoke(nameof(DebloquerApresTir), 0.5f);
    }

    private void DebloquerApresTir()
    {
        SetCanMove(true);
    }

    public void SetCanMove(bool value)
    {
        _canMove = value;
        if (!value) 
        {
            _movement = Vector2.zero;
            _rb.linearVelocity = Vector2.zero; 
        }
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _movement * speed;
        
        float vitesseglobale = _rb.linearVelocity.magnitude;
        _anim.SetFloat("Velocity", vitesseglobale);
    }
}