using System.Collections.Generic;
using UnityEngine;

public static class GroundCheck
{
    public static bool IsTouchingGround(LayerMask whatIsGround, Vector2 groundCheckPosition, GameObject gameObject)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPosition, 0.05f, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }  
    
    public static bool IsTouchingGround(LayerMask whatIsGround, Transform[] groundCheckers, GameObject gameObject)
    {
        foreach (var groundCheck in groundCheckers)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.05f, whatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    return true;
                }
            }
        }
        
        return false;
    }  
    
    public static bool IsTouchingGround(LayerMask whatIsGround, Vector2 groundCheckPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPosition, 0.05f, whatIsGround);
        if (colliders.Length > 0)
            return true;
        
        return false;
    } 
}