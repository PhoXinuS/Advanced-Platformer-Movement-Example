using UnityEngine;

[System.Serializable]
public class GroundCheck
{
    [SerializeField] LayerMask whatIsGround = new LayerMask();
    [SerializeField] Transform[] groundCheckers = new Transform[0];

    private RaycastHit2D[] hits = new RaycastHit2D[10];
    private GameObject gameObject;

    internal void SetUp(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    
    internal bool IsTouchingGround()
    {
        foreach (var groundCheck in groundCheckers)
        {
            int hitsNumber = Physics2D.RaycastNonAlloc(groundCheck.position, Vector2.down, hits, 0.05f, whatIsGround);
            for (int i = 0; i < hitsNumber; i++)
            {
                GameObject hitGameObject = hits[i].collider.gameObject;
                if (hitGameObject != gameObject)
                    return true;
            }
        }
        return false;
    }
}