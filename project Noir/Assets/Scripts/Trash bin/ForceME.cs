using UnityEngine;

public class ForceME : MonoBehaviour
{
    [SerializeField] private Vector2 force;
    [SerializeField] bool apply;
    private Rigidbody2D rb2D;

    private void Start()
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        
        if (apply)
        {
            rb2D.AddForce(force);
        }
    }

    private void Update()
    {
        if (apply)
        {
            rb2D.AddForce(force);
        }
    }
}
