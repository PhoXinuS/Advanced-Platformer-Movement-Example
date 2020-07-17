using UnityEngine;

[System.Serializable]
public class OverlapCircleChecker
{
    [SerializeField] LayerMask targetLayerMask = new LayerMask();
    [SerializeField] Transform[] raycastOrigins = new Transform[0];
    [SerializeField] float checkDistance = 0.4f;

    private Collider2D[] hits = new Collider2D[10];

    internal bool IsInContactWithTarget()
    {
        foreach (var raycastOrigin in raycastOrigins)
        {
            int hitsNumber = CastOverlapCircle(raycastOrigin);
            if (hitsNumber > 0) return true;
        }
        return false;
    }
    internal bool IsInContactWithTarget(string tag)
    {
        foreach (var raycastOrigin in raycastOrigins)
        {
            int hitsNumber = CastOverlapCircle(raycastOrigin);
            if (HitObjectsContainsTag(hitsNumber, tag)) return true;
        }
        return false;
    }
    
    private int CastOverlapCircle(Transform groundCheck)
    {
        return Physics2D.OverlapCircleNonAlloc(groundCheck.position, checkDistance, hits, targetLayerMask);
    }
    
    private bool HitObjectsContainsTag(int hitsNumber, string tag)
    {
        for (int i = 0; i < hitsNumber; i++)
        {
            var hitGameObject = hits[i].gameObject;
            if (hitGameObject.CompareTag(tag)) return true;
        }
        return false;
    }

}