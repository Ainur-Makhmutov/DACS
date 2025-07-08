using System.Collections;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5f;                 // �������� ������
    [SerializeField] private int _lives = 5;                    // ����� ������

    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 8f;             // ���� ���������� �������� ������
    [SerializeField] private float _normalMass = 0.36f;         // ����� ��������� �� �������           
    [SerializeField] private float _fallMass = 0.6f;            // ����������� ����� ��� ������� ���������, ����� ����� �������, � �� ������ ������ ��� �� ����
    private bool _hasJumped = false;                            // ������ �� ������������� ������� � �������

    [Header("GroundCheck Settings")]
    [SerializeField] private GameObject _groundCheckObj;        // Cc���� �� ��������������� ������ ��� �������� �����������
    [SerializeField] private float _radiusGroundCheck = 1f;     // ������ ���������������� �������
    [SerializeField] LayerMask _groundMask;                     // ������ �� ���� �����������
    private bool _isGrounded = false;                           // True, ���� �������� ����� �� �����������

    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;
    private Animator _anim;
    private Vector2 _moveVector;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = _rb.GetComponentInChildren<SpriteRenderer>();
        _anim = _rb.GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    void Update()
    {
        if (_isGrounded) State = States.idle;

        if (Input.GetButtonDown("Jump") && _isGrounded && !_hasJumped)
        {
            Jump();
            _hasJumped = false;
        }

        if (Input.GetButton("Horizontal"))
            Run();
    }

    private void Run()
    {
        if (_isGrounded) State = States.run;

        _moveVector.x = Input.GetAxis("Horizontal");
        _rb.linearVelocity = new Vector2(_moveVector.x * _speed, _rb.linearVelocity.y);

        if (_moveVector.x != 0) _sprite.flipX = _moveVector.x < 0;      // �������� ����������� ��������
    }

    private void Jump()
    {
        if (!_isGrounded) State = States.jump;

        _hasJumped = true;
        //_rb.mass = _normalMass;

        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);

        //StartCoroutine(ChangeMassWhenFall());                           // ��� ������� ������������� ����������� �����
    }

    private void CheckGround()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheckObj.transform.position, _radiusGroundCheck, _groundMask);

        if (!_isGrounded) State = States.jump;
    }

    private States State
    {
        get { return (States)_anim.GetInteger("state"); }
        set { _anim.SetInteger("state", (int)value); }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groundCheckObj.transform.position, _radiusGroundCheck); // ��������� ����� ��� �������� ����������� 
    }

    private IEnumerator ChangeMassWhenFall()
    {
        yield return new WaitUntil(() => _rb.linearVelocity.y < 0); 
        _rb.mass = _fallMass;
    }
}

public enum States
{
    idle,
    idleWait,
    run,
    jump,
    collect,
    hang,
    climb,
    fire,
    die
}