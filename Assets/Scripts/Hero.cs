using UnityEngine;
using UnityEngine.Audio;
using static UnityEditor.VersionControl.Asset;

public class Hero : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;                  // �������� ������
    [SerializeField] private int lives = 5;                     // ����� ������

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8f;              // ���� ���������� �������� ������
    [SerializeField] private float jumpTime = 0.3f;             // ����� � ��������, ������� ����� ���������� ������
    [SerializeField] private float jumpMultiplier = 1.5f;       // ����������� ���������� ���� ������ ��� ���������
    [SerializeField] private float fallMultiplier = 2.5f;       // �������� ���������� ��� ������� (������ ������� �������)
    [SerializeField] private float lowJumpMultiplier = 2f;      // ����������� ��� ���������� ������ ������ ��� ������ ����������

    private bool isGrounded = false;                            // True, ���� �������� ����� �� �����������
    private bool isJumping = false;                             // True, ���� ����������� ������ � ����������
    private bool hasJumped = false;                             // ������ �� ������������� ������� � �������
    private float jumpTimer = 0f;                               // ���������� ����� ��������� ������

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private Vector2 moveVector;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = rb.GetComponentInChildren<SpriteRenderer>();
        anim = rb.GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        CheckGround();
        ApplyBetterJumpPhysics();
    }

    void Update()
    {
        if (isGrounded) State = States.idle;

        

        if (Input.GetButtonDown("Jump") && isGrounded && !hasJumped)
            HandleJump();
        hasJumped = false;
        if (Input.GetButton("Horizontal"))
            HandleMovement();
    }

    private void HandleMovement()
    {
        if (isGrounded) State = States.run;

        moveVector.x = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveVector.x * speed, rb.linearVelocity.y);

        if (moveVector.x != 0)
        {
            sprite.flipX = moveVector.x < 0;
        }
    }

    private void HandleJump()
    {
        if (!isGrounded) State = States.jump;

        // ������ ������
        if (Input.GetButtonDown("Jump") && isGrounded && !hasJumped)
        {
            isJumping = true;
            hasJumped = true;
            jumpTimer = jumpTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // ��������� ������
        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimer > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpMultiplier);
                jumpTimer -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        // ���������� ������ ������
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
    }

    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = collider.Length > 1;

        if (!isGrounded) State = States.jump;
    }

    private void ApplyBetterJumpPhysics()
    {
        if (rb.linearVelocity.y < 0) // �������
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump")) // �������� ������
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
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