﻿using UnityEngine;

[System.Serializable]
public class Checker
{
    [SerializeField] LayerMask targetLayerMask = new LayerMask();
    [SerializeField] Transform[] raycastOrigins = new Transform[0];
    [SerializeField] float checkDistance = 0.05f;

    private RaycastHit2D[] hits = new RaycastHit2D[10];
    private Vector2 direction;

    internal void SetUp(Vector2 direction)
    {
        this.direction = direction;
    }
    
    internal bool IsInContactWithTarget()
    {
        foreach (var raycastOrigin in raycastOrigins)
        {
            int hitsNumber = CastRaycast(raycastOrigin);
            if (hitsNumber > 0) return true;
        }
        return false;
    }
    internal bool IsInContactWithTarget(string tag)
    {
        foreach (var raycastOrigin in raycastOrigins)
        {
            int hitsNumber = CastRaycast(raycastOrigin);
            if (HittedObjectsContainsTag(hitsNumber, tag)) return true;
        }
        return false;
    }
    
    private int CastRaycast(Transform groundCheck)
    {
        return Physics2D.RaycastNonAlloc(groundCheck.position, direction, hits, checkDistance, targetLayerMask);
    }
    
    private bool HittedObjectsContainsTag(int hitsNumber, string tag)
    {
        for (int i = 0; i < hitsNumber; i++)
        {
            var hitGameObject = hits[i].collider.gameObject;
            if (hitGameObject.CompareTag(tag)) return true;
        }
        return false;
    }

}