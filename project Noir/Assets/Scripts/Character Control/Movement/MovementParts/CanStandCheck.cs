using UnityEngine;

public class CanStandCheck
{
    private RaycastHit2D[] hits = new RaycastHit2D[10];
    
    public bool CanStand(LayerMask whatPreventsStandingUp, Transform[] standingSpaceChekers, float checkLength, GameObject gameObject)
    {
        foreach (var standingSpaceCheck in standingSpaceChekers)
        {
            int hitsNumber = Physics2D.RaycastNonAlloc(standingSpaceCheck.position, Vector2.up, hits, checkLength, whatPreventsStandingUp);
            for (int i = 0; i < hitsNumber; i++)
            {
                GameObject hitGameObject = hits[i].collider.gameObject;
                if (hitGameObject != gameObject)
                    return false;
            }
        }
        return true;
    }
}