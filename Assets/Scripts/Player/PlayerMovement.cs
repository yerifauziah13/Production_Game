using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // == Variabel dan Komponen ==
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;

    private bool grounded;
    private bool onWall;
    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        // Ambil komponen yang diperlukan
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // == Flip Sprite Berdasarkan Arah Gerakan ==
        if (horizontalInput > 0.01f)
            spriteRenderer.flipX = false;
        else if (horizontalInput < -0.01f)
            spriteRenderer.flipX = true;

        // Cek apakah pemain berada di tanah atau di dinding
        grounded = IsGrounded();
        onWall = OnWall();
        
        // Kurangi cooldown wall jump
        if (wallJumpCooldown > 0)
            wallJumpCooldown -= Time.deltaTime;

        // Set parameter animator
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", grounded);

        // == Gerakan Horizontal ==
        if (wallJumpCooldown <= 0)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            
            // Jika menempel di dinding dan tidak di tanah, nolkan gravitasi
            if (onWall && !grounded)
            {
                body.gravityScale = 0;
                body.velocity = new Vector2(0, 0); // Membuat karakter tetap diam saat menempel di dinding
            }
            else
            {
                body.gravityScale = 3; // Reset gravitasi ke normal
            }
        }

        // Lompat saat tombol Space ditekan
        if (Input.GetKeyDown(KeyCode.Space))
            body.velocity = new Vector2(body.velocity.x, speed);
        {
            
            Jump();
        }
    }

    #region Movement Functions

    private void Jump()
    {
        if (grounded)
        {
            // Lompatan normal dari tanah
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("jump");
            Debug.Log("Jump from ground!");
        }
        else if (onWall && !grounded)
        {
            // Wall jump
            float jumpDir = spriteRenderer.flipX ? 1f : -1f; // Tentukan arah lompatan dari dinding
            body.velocity = new Vector2(jumpDir * speed, jumpPower);
            body.gravityScale = 3;
            wallJumpCooldown = 0.3f; // Tambahkan cooldown agar tidak langsung menempel di dinding lagi
            Debug.Log("Wall Jump!");
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider2D.bounds.center,
            new Vector2(boxCollider2D.bounds.size.x * 0.8f, boxCollider2D.bounds.size.y * 0.1f),
            0,
            Vector2.down,
            0.1f,
            groundLayer
        );

        //Debug.Log("Grounded: " + (raycastHit.collider != null));
        return raycastHit.collider != null;
    }

    private bool OnWall()
    {
        float direction = spriteRenderer.flipX ? -1f : 1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider2D.bounds.center,
            boxCollider2D.bounds.size,
            0,
            new Vector2(direction, 0),
            0.1f,
            wallLayer
        );

        //Debug.Log("On Wall: " + (raycastHit.collider != null));
        return raycastHit.collider != null;
    }

    public bool CanHurt()
    {
        return Mathf.Abs(body.velocity.x) < 0.1f && IsGrounded() && !OnWall();
    }

    #endregion
}
