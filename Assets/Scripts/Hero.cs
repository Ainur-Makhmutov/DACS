using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5f;                     // �������� ������
    [SerializeField] private int _lives = 5;                        // ����� ������
    [SerializeField] private bool _faceRight = true;                // True, ���� �������� ������� ������

    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 8f;                 // ���� ���������� �������� ������

    [Header("GroundCheck Settings")]
    [SerializeField] private GameObject _groundCheckObj;            // Cc���� �� ������ ��� �������� �����������
    [SerializeField] private float _radiusGroundCheck = 1f;         // ������ ���������������� �������
    [SerializeField] LayerMask _groundMask;                         // ������ �� ���� �����������
    [SerializeField] private bool _onGround = false;                // True, ���� �������� ����� �� �����������

    [Header("WallCheck and LedgeCheck Settings")]
    [SerializeField] private GameObject _wallCheckObj;              // Cc���� �� ������ ��� �������� ����� ����� ������
    [SerializeField] private GameObject _ledgeCheckObj;             // ������ �� ������ ��� �������� ������
    [SerializeField] private GameObject _finishClimbPositionObj;    // ������ �� ������ c ������������ ��� ����������� ��������� ����� ������� �� �����
    [SerializeField] private float _lengthLedgeCheck = 1f;          // ������ ���������������� �������
    [SerializeField] private bool _onWall = false;                  // True, ���� ����� ���������� �����
    [SerializeField] private bool _onLedge = false;                 // True, ���� ����� ���������� �����
    [SerializeField] private bool _blockMoveForClimb = false;       // True, ���� �������� ���������� �� �����


    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;
    private Animator _anim;
    private Vector2 _moveVector;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = _rb.GetComponent<SpriteRenderer>();
        _anim = _rb.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    void Update()
    {
        if (_onGround) State = States.idle;

        if (Input.GetButtonDown("Jump") && _onGround && !_blockMoveForClimb)
            Jump();

        if (Input.GetButton("Horizontal") && !_blockMoveForClimb)
            Run();

        if (_onLedge)
        {
            Hang();
        }

        if (_blockMoveForClimb)
            State = States.climb;

        CheckLedge();
    }

    private void Run()
    {
        if (_onGround) State = States.run;

        if (!_onLedge)
            _moveVector.x = Input.GetAxis("Horizontal");

        _rb.linearVelocity = new Vector2(_moveVector.x * _speed, _rb.linearVelocity.y);

        //_sprite.flipX = _moveVector.x < 0;      // �������� ����������� ��������
        if ((_moveVector.x > 0 && !_faceRight) || (_moveVector.x < 0 && _faceRight))
        {
            transform.localScale *= new Vector2(-1, 1);
            _faceRight = !_faceRight;
        }
    }

    private void Jump()
    {
        if (!_onGround && !_onLedge) State = States.jump;

        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    private void Hang()
    {
        if (!_onGround && _onLedge) State = States.hang;

        _rb.constraints = RigidbodyConstraints2D.FreezeAll;

        float moveInput = Input.GetAxisRaw("Horizontal");
        float climbInput = Input.GetAxisRaw("Vertical");
        float facingDirection = transform.localScale.x >= 0 ? 1 : -1;

        // ���� ���� �������������� ����������� ���������
        if (moveInput != 0 && Mathf.Sign(moveInput) != facingDirection && !_blockMoveForClimb)
        {
            _onLedge = false;
            _rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
        }
        else if (climbInput != 0 && Mathf.Sign(climbInput) == 1)
        {
            _blockMoveForClimb = true;
        }
    }

    private void Climb()
    {
        
    }

    void FinishClimb()
    {
        transform.position = new Vector3(_finishClimbPositionObj.transform.position.x, 
                                         _finishClimbPositionObj.transform.position.y, 
                                         _finishClimbPositionObj.transform.position.z);

        _blockMoveForClimb = false;
        _onLedge = false;
        _rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
        State = States.idle;
    }

    private void CheckGround()
    {
        _onGround = Physics2D.OverlapCircle(_groundCheckObj.transform.position, 
                                            _radiusGroundCheck, 
                                            _groundMask);

        if (!_onGround && !_onLedge)
        {
            State = States.jump;

            _rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
        }
    }

    private void CheckLedge()
    {
        _onWall = Physics2D.Raycast(_wallCheckObj.transform.position, 
                                    new Vector2(transform.localScale.x, 0), 
                                    _lengthLedgeCheck, 
                                    _groundMask);

        if (_onWall)
        {
            _onLedge = !Physics2D.Raycast(_ledgeCheckObj.transform.position, 
                                          new Vector2(transform.localScale.x, 0), 
                                          _lengthLedgeCheck, 
                                          _groundMask);
        }
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
        Gizmos.DrawLine(_wallCheckObj.transform.position, 
                        new Vector2(_wallCheckObj.transform.position.x + _lengthLedgeCheck * transform.localScale.x, 
                        _wallCheckObj.transform.position.y));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(_ledgeCheckObj.transform.position, 
                        new Vector2(_ledgeCheckObj.transform.position.x + _lengthLedgeCheck * transform.localScale.x, 
                        _ledgeCheckObj.transform.position.y));
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