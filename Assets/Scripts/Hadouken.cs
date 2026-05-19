using UnityEngine;

public class Hadouken : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private Dummy.HitType hitType;
    private float direction = 1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Setup(float facingDirection)
    {
        direction = facingDirection;
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * (direction * speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dummy"))
        {
            Dummy dummy = collision.GetComponent<Dummy>();

            if (dummy != null)
            {
                dummy.TakeDamage(hitType);
                Debug.Log($"Tipo: {hitType}");
            }

            Destroy(gameObject);
        }
    }
}
