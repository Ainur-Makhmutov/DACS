using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5f;                     // Скорость игрока
    [SerializeField] private int _maxHealth = 6;                        // Жизни игрока
    [SerializeField] private bool _faceRight = true;                // True, если персонаж смотрит вправо

    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 8f;                 // Сила начального импульса прыжка

    [Header("GroundCheck Settings")]
    [SerializeField] private GameObject _groundCheckObj;            // Ccылка на объект для проверки поверхности
    [SerializeField] private float _radiusGroundCheck = 1f;         // Радиус вспомогательного объекта
    [SerializeField] LayerMask _groundMask;                         // Ссылка на слой поверхности
    [SerializeField] private bool _onGround = false;                // True, если персонаж стоит на поверхности

    [Header("WallCheck and LedgeCheck Settings")]
    [SerializeField] private GameObject _wallCheckObj;              // Ccылка на объект для проверки стены перед персом
    [SerializeField] private GameObject _ledgeCheckObj;             // Ссылка на объект для проверки уступа
    [SerializeField] private GameObject _finishClimbPositionObj;    // Ссылка на объект c координатами для перемещения персонажа после подъема на уступ
    [SerializeField] private float _lengthLedgeCheck = 1f;          // Длинна вспомогательного объекта
    [SerializeField] private bool _onWall = false;                  // True, если перед персонажем стена
    [SerializeField] private bool _onLedge = false;                 // True, если перед персонажем уступ
    [SerializeField] private bool _blockMoveForClimb = false;       // True, если персонаж взбирается на уступ


    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private Vector2 moveVector;

    private int currentHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = rb.GetComponent<SpriteRenderer>();
        anim = rb.GetComponent<Animator>();

        currentHealth = _maxHealth;
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

        if (Input.GetKey(KeyCode.LeftControl) && !_blockMoveForClimb)
            Attack();

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
            moveVector.x = Input.GetAxis("Horizontal");

        rb.linearVelocity = new Vector2(moveVector.x * _speed, rb.linearVelocity.y);

        //sprite.flipX = moveVector.x < 0;      // Проверка направления движения
        if ((moveVector.x > 0 && !_faceRight) || (moveVector.x < 0 && _faceRight))
        {
            transform.localScale *= new Vector2(-1, 1);
            _faceRight = !_faceRight;
        }
    }

    private void Jump()
    {
        if (!_onGround && !_onLedge) State = States.jump;

        rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    private void Hang()
    {
        if (!_onGround && _onLedge) State = States.hang;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        float moveInput = Input.GetAxisRaw("Horizontal");
        float climbInput = Input.GetAxisRaw("Vertical");
        float facingDirection = transform.localScale.x >= 0 ? 1 : -1;

        // Если ввод противоположен направлению персонажа
        if (moveInput != 0 && Mathf.Sign(moveInput) != facingDirection && !_blockMoveForClimb)
        {
            _onLedge = false;
            rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
        }
        else if (climbInput != 0 && Mathf.Sign(climbInput) == 1)
        {
            _blockMoveForClimb = true;
        }
    }

    void FinishClimb()
    {
        transform.position = new Vector3(_finishClimbPositionObj.transform.position.x, 
                                         _finishClimbPositionObj.transform.position.y, 
                                         _finishClimbPositionObj.transform.position.z);

        _blockMoveForClimb = false;
        _onLedge = false;
        rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
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

            rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
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

    private void Attack()
    {
        State = States.fire;
    }

    public void GetDamage()
    {
        currentHealth -= 1;
        Debug.Log("Player took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        // тут можно отключить управление, включить анимацию смерти и т.д.
    }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groundCheckObj.transform.position, _radiusGroundCheck); // Отрисовка сферы для проверки поверхности 

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