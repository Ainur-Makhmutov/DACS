using System.Collections;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5f;                 // �������� ������
    [SerializeField] private int _lives = 5;                    // ����� ������

    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 8f;             // ���� ���������� �������� ������

    [Header("GroundCheck Settings")]
    [SerializeField] private GameObject _groundCheckObj;        // Cc���� �� ��� ������ ��� �������� �����������
    [SerializeField] private float _radiusGroundCheck = 1f;     // ������ ���������������� �������
    [SerializeField] LayerMask _groundMask;                     // ������ �� ���� �����������
    private bool _onGround = false;                             // True, ���� �������� ����� �� �����������

    [Header("WallCheck and LedgeCheck Settings")]
    [SerializeField] private GameObject _wallCheckObj;          // Cc���� �� ��� ������ ��� �������� ����� ����� ������
    [SerializeField] private GameObject _ledgeCheckObj;         // ������ �� ��� ������ ��� �������� ������
    [SerializeField] private float _lengthLedgeCheck = 1f;      // ������ ���������������� �������
    private bool _onWall = false;                               // True, ���� ����� ���������� �����
    private bool _onLedge = false;                              // True, ���� ����� ���������� �����

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
        CheckLedge();
    }

    void Update()
    {
        if (_onGround) State = States.idle;

        if (Input.GetButtonDown("Jump") && _onGround)
            Jump();

        if (Input.GetButton("Horizontal"))
            Run();
    }

    private void Run()
    {
        if (_onGround) State = States.run;

        _moveVector.x = Input.GetAxis("Horizontal");
        _rb.linearVelocity = new Vector2(_moveVector.x * _speed, _rb.linearVelocity.y);

        if (_moveVector.x != 0) _sprite.flipX = _moveVector.x < 0;      // �������� ����������� ��������
    }

    private void Jump()
    {
        if (!_onGround) State = States.jump;

        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    private void Hang()
    {
        if (!_onGround) State = States.hang;


    }

    private void Climb()
    {
        if (!_onGround) State = States.climb;


    }

    private void CheckGround()
    {
        _onGround = Physics2D.OverlapCircle(_groundCheckObj.transform.position, _radiusGroundCheck, _groundMask);

        if (!_onGround) State = States.jump;
    }

    private void CheckLedge()
    {
        _onWall = Physics2D.Raycast(_wallCheckObj.transform.position, new Vector2(transform.localScale.x, 0), _lengthLedgeCheck, _groundMask);

        if (_onWall)
        {
            _onLedge = !Physics2D.Raycast(_ledgeCheckObj.transform.position, new Vector2(transform.localScale.x, 0), _lengthLedgeCheck, _groundMask);
        }
        else { _onWall = false; }
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

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_wallCheckObj.transform.position, new Vector2(_wallCheckObj.transform.position.x + _lengthLedgeCheck * transform.localScale.x, _wallCheckObj.transform.position.y));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(_ledgeCheckObj.transform.position, new Vector2(_ledgeCheckObj.transform.position.x + _lengthLedgeCheck * transform.localScale.x, _ledgeCheckObj.transform.position.y));
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