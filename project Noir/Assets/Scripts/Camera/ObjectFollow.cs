using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollow : MonoBehaviour
{
    [SerializeField] List<Transform> pointsOfFocus = new List<Transform>();
    [SerializeField] private float smoothness = 10f;
    [SerializeField] Vector3 offset = new Vector3(0, 2, -10);

    private void LateUpdate()
    {
        var targetPosition = CalculateMeanPositionFromPointsOfFocus();
        var lerpedPosition = LerpedPosition(targetPosition);
        transform.position = new Vector3(lerpedPosition.x, lerpedPosition.y, offset.z);
    }
    
    private Vector2 CalculateMeanPositionFromPointsOfFocus()
    {
        Vector2 targetPosition = new Vector2();
        int focusCount = 0;
        foreach (var _object in pointsOfFocus)
        {
            if (_object != null)
            {
                targetPosition += (Vector2) _object.position;
                focusCount++;
            }
            else
            {
                Debug.LogWarning("You are trying to follow destroyed object, Sir!");
            }
        }

        if (focusCount != 0)
        {
            targetPosition = targetPosition / focusCount + (Vector2) offset;
        }
        else
        {
            Debug.LogWarning("You are trying to 0 objects, Sir!");
            return transform.position;
        }

        return targetPosition;
    }
    
    private Vector2 LerpedPosition(Vector2 targetPosition)
    {
        Vector2 lerpedPosition = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime / smoothness);
        return lerpedPosition;
    }
}
