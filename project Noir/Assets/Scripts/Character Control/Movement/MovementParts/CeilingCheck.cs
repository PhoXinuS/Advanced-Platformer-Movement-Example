using System.Linq;
using UnityEngine;

[System.Serializable]
public class CeilingCheck
{
    [SerializeField] float checkLength = 0.8f;
    [SerializeField] LayerMask whatIsCeiling = new LayerMask();
    [SerializeField] Transform[] ceilingCheckers = new Transform[0];

    private RaycastHit2D[] hits = new RaycastHit2D[10];
    private GameObject[] gameObjectsToIgnore;

    internal void SetUp(GameObject[] gameObjectsToIgnore)
    {
        this.gameObjectsToIgnore = gameObjectsToIgnore;
    }
    
    public bool CanStand()
    {
        foreach (var standingSpaceCheck in ceilingCheckers)
        {
            int hitsNumber = CastRaycastCeiling(standingSpaceCheck);
            if (HitCeiling(hitsNumber)) return false;
        }
        return true;
    }
    
    private int CastRaycastCeiling(Transform standingSpaceCheck)
    {
        return Physics2D.RaycastNonAlloc(standingSpaceCheck.position, Vector2.up, hits, checkLength, whatIsCeiling);
    }
    
    private bool HitCeiling(int hitsNumber)
    {
        for (int i = 0; i < hitsNumber; i++)
        {
            GameObject hitGameObject = hits[i].collider.gameObject;
            if (!gameObjectsToIgnore.Contains(hitGameObject))
                return true;
        }

        return false;
    }

}