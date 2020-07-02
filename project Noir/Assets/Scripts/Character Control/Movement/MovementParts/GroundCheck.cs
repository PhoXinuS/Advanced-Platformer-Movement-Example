using System.Linq;
using UnityEngine;

[System.Serializable]
public class GroundCheck
{
    [SerializeField] LayerMask whatIsGround = new LayerMask();
    [SerializeField] Transform[] groundCheckers = new Transform[0];

    private RaycastHit2D[] hits = new RaycastHit2D[10];
    private GameObject[] gameObjectsToIgonore;

    internal void SetUp(GameObject[] gameObjectsToIgonore)
    {
        this.gameObjectsToIgonore = gameObjectsToIgonore;
    }
    
    internal bool IsTouchingGround()
    {
        foreach (var groundCheck in groundCheckers)
        {
            int hitsNumber = CastRaycastGround(groundCheck);
            if (HittedGround(hitsNumber)) return true;
        }
        return false;
    }

    private int CastRaycastGround(Transform groundCheck)
    {
        return Physics2D.RaycastNonAlloc(groundCheck.position, Vector2.down, hits, 0.05f, whatIsGround);
    }

    private bool HittedGround(int hitsNumber)
    {
        for (int i = 0; i < hitsNumber; i++)
        {
            GameObject hitGameObject = hits[i].collider.gameObject;
            if (!gameObjectsToIgonore.Contains(hitGameObject)) return true;
        }

        return false;
    }
}