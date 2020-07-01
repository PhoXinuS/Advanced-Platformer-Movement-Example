using UnityEngine;

[System.Serializable]
public class CanStandCheck
{
    [SerializeField] float checkLength = 0.8f;
    [SerializeField] LayerMask whatIsCeiling = new LayerMask();
    [SerializeField] Transform[] ceilingCheckers = new Transform[0];

    private RaycastHit2D[] hits = new RaycastHit2D[10];
    private GameObject gameObject;

    internal void SetUp(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    
    public bool CanStand()
    {
        foreach (var standingSpaceCheck in ceilingCheckers)
        {
            int hitsNumber = Physics2D.RaycastNonAlloc(standingSpaceCheck.position, Vector2.up, hits, checkLength, whatIsCeiling);
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