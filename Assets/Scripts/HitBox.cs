using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private Dummy.HitType hitType;

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
        }
    }
}
