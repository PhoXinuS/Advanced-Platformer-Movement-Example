using UnityEngine;

public class GroundCheck
{
    private RaycastHit2D[] hits = new RaycastHit2D[10];
    
    public bool IsTouchingGround(LayerMask whatIsGround, Transform[] groundCheckers, GameObject gameObject)
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