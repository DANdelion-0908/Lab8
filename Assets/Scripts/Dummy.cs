using UnityEngine;

public class Dummy : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    [Header("Configuración de Estados")]
    [SerializeField] private bool isKnockedDown = false;

    [Header("Configuración de Caída")]
    [SerializeField] private float pushBackForce = 3f;
    [SerializeField] private float knockUpForce = 4f;

    [Header("Configuración de Sonidos")]
    [SerializeField] private AudioClip weakHitSound;
    [SerializeField] private AudioClip strongHitSound;
    [SerializeField] private AudioClip fallSound;

    public enum HitType { Weak, Strong, Knockdown }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(HitType hitType)
    {
        if (isKnockedDown) return;

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        switch (hitType)
        {
            case HitType.Weak:
                animator.Play("Ryu_Hit", 0, 0f);
                PlaySound(weakHitSound);
                break;

            case HitType.Strong:
                animator.Play("Ryu_HardHit", 0, 0f);
                PlaySound(strongHitSound);
                break;

            case HitType.Knockdown:
                isKnockedDown = true;
                animator.Play("Ryu_KnockDown", 0, 0f);
                PlaySound(fallSound);
                ApplyKnockback();
                break;
        }
    }

    private void ApplyKnockback()
    {
        float facingDirection = Mathf.Sign(transform.localScale.x);
        Vector2 knockbackVector = new Vector2(-facingDirection * pushBackForce, knockUpForce);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackVector, ForceMode2D.Impulse);
    }

    public void StartGetUp()
    {
        animator.Play("Ryu_GetUp", 0, 0f);
    }

    public void ResetStance()
    {
        isKnockedDown = false;
        animator.Play("Ryu_Idle", 0, 0f);
    }

    public void RecoverFromHit()
    {
        animator.Play("Ryu_Idle", 0, 0f);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
