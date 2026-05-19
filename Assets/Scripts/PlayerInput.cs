using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private MyInputSystem controls;

    [Header("Configuración de Combos")]
    [SerializeField] private float comboTimeout = 0.5f;
    private string currentSequence = "";
    private float lastInputTime;

    [Header("Configuración de Rango")] // Parte de otro juego que solo estaba testeando aquí
    [SerializeField] private Rank rank = Rank.F;
    [SerializeField] private float lastActionTimeout = 5f;
    private float lastActionTime = 0f;
    [SerializeField] private TMP_Text rankText;
    public enum Rank { F, E, D, C, B, A, S }

    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private GameObject hadoukenPrefab;
    [SerializeField] private Transform hadoukenSpawnPoint;
    private Vector2 movementInput;

    [Header("Configuración de Sonidos")]
    [SerializeField] private AudioClip hadoukenVoice;
    [SerializeField] private AudioClip shoryukenVoice;
    [SerializeField] private AudioClip tauntVoice;
    [SerializeField] private AudioClip kickVoice;
    [SerializeField] private AudioClip punchVoice;

    private void Awake()
    {
        controls = new MyInputSystem();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        controls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movementInput = Vector2.zero;

        controls.Player.Attack.performed += ctx => RegisterInput("Attack");
        controls.Player.Kick.performed += ctx => RegisterInput("Kick");
        controls.Player.Special.performed += ctx => RegisterInput("Special");

        controls.Player.Forward.performed += ctx => RegisterInput("Right");
        controls.Player.Backward.performed += ctx => RegisterInput("Left");
        controls.Player.Down.performed += ctx => RegisterInput("Down");
        controls.Player.Up.performed += ctx => RegisterInput("Up");

        controls.Player.Jump.performed += ctx => {
            RegisterInput("Jump");
            if (isGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGrounded = false;
                animator.SetBool("IsJumping", true);
            }
        };
    }

    private void Update()
    {
        float HorizontalSpeed = movementInput.x;
        animator.SetFloat("HorizontalSpeed", HorizontalSpeed);
        animator.SetBool("IsStopped", HorizontalSpeed == 0);
    }

    private void FixedUpdate()
    {
        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        else
        {
            rb.linearVelocity = new Vector2(movementInput.x * moveSpeed, rb.linearVelocity.y);
        }

        HandleTimeouts();
    }

    private void HandleTimeouts()
    {
        if (currentSequence != "" && Time.time - lastInputTime > comboTimeout)
        {
            currentSequence = "";
        }

        if (rank != Rank.F && Time.time - lastActionTime > lastActionTimeout)
        {
            rank = Rank.F;
            UpdateRankUI();
        }
    }

    private void RegisterInput(string inputName)
    {
        currentSequence += inputName;
        lastInputTime = Time.time;
        lastActionTime = Time.time;

        CheckCombos();
    }

    private void CheckCombos()
    {

        if (currentSequence.EndsWith("RightDownRightSpecial"))
        {
            PerformSkill("Shoryuken");
            currentSequence = "";
        }

        else if (currentSequence.EndsWith("DownRightSpecial"))
        {
            PerformSkill("Hadouken");
            currentSequence = "";
        }

        else if (currentSequence.EndsWith("UpSpecial"))
        {
            PerformSkill("Taunt");
            currentSequence = "";

        }

        else if (currentSequence.EndsWith("Attack"))
        {
            PerformSkill("Attack");
            currentSequence = "";
        }

        else if (currentSequence.EndsWith("Kick"))
        {
            PerformSkill("Kick");
            currentSequence = "";
        }
    }

    private void PerformSkill(string skill)
    {
        isAttacking = true;
        switch (skill)
        {
            case "Shoryuken":
                animator.Play("Ken_Shoryuken", 0, 0f);
                break;
            case "Hadouken":
                animator.Play("Ken_Hadouken", 0, 0f);
                break;
            case "Taunt":
                animator.Play("Ken_Taunt", 0, 0f);
                break;
            case "Attack":
                animator.Play("Ken_Attack", 0, 0f);
                break;
            case "Kick":
                animator.Play("Ken_Kick", 0, 0f);
                break;
        }
        IncreaseRank();
    }

    private void IncreaseRank()
    {

        if ((int)rank < (int)Rank.S)
        {
            rank = (Rank)((int)rank + 1);
            UpdateRankUI();
        }
    }

    private void UpdateRankUI()
    {
        if (rankText != null)
        {
            rankText.text = rank.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("IsJumping", false);
        }
    }

    public void ResetAttack()
    {
        isAttacking = false;
        animator.Play("Ken_Idle", 0, 0f);
    }

    public void ApplyShoryukenForce()
    {
        float facingDirection = Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = Vector2.zero;

        Vector2 shoryukenVector = new Vector2(4f * facingDirection, 5f);

        rb.AddForce(shoryukenVector, ForceMode2D.Impulse);

        isGrounded = false;
        animator.SetBool("IsJumping", true);
    }

    public void PlayHadoukenSound()
    {
        if (audioSource != null && hadoukenVoice != null)
        {
            audioSource.PlayOneShot(hadoukenVoice);
        }
    }

    public void PlayShoryukenSound()
    {
        if (audioSource != null && shoryukenVoice != null)
        {
            audioSource.PlayOneShot(shoryukenVoice);
        }
    }

    public void PlayTauntSound()
    {
        if (audioSource != null && tauntVoice != null)
        {
            audioSource.PlayOneShot(tauntVoice);
        }
    }

    public void PlayKickSound()
    {
        if (audioSource != null && kickVoice != null)
        {
            audioSource.PlayOneShot(kickVoice);
        }
    }

    public void PlayPunchSound()
    {
        if (audioSource != null && punchVoice != null)
        {
            audioSource.PlayOneShot(punchVoice);
        }
    }

    public void SpawnHadouken()
    {
        if (hadoukenPrefab != null && hadoukenSpawnPoint != null)
        {
            GameObject clone = Instantiate(hadoukenPrefab, hadoukenSpawnPoint.position, hadoukenSpawnPoint.rotation);

            float facingDirection = Mathf.Sign(transform.localScale.x);

            Hadouken projectileScript = clone.GetComponent<Hadouken>();
            if (projectileScript != null)
            {
                projectileScript.Setup(facingDirection);
            }
        }
    }

    private void OnEnable() => controls?.Player.Enable();
    private void OnDisable() => controls?.Player.Disable();
}
