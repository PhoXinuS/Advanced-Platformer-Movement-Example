using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollow : MonoBehaviour
{
    public Transform follower;

    public List<Transform> objectsToFollow = new List<Transform>();

    [SerializeField] private float smooothens = 10f;

    [SerializeField] Vector2 offset = new Vector2(0, 2);
    [SerializeField] float yDistance = -10;

    private void LateUpdate()
    {
        Vector2 targetPosition = new Vector2();
        foreach (var _object in objectsToFollow)
        {
            if (_object != null)
            {
                targetPosition += (Vector2)_object.position;
            }
            else
            {
                Debug.LogWarning("You are trying to follow destroyed object, Sir!");
            }
        }
        targetPosition = targetPosition / objectsToFollow.Count + offset;

        follower.position = Vector2.Lerp(follower.position, targetPosition, smooothens * Time.deltaTime);
        follower.position = new Vector3(follower.position.x, follower.position.y, yDistance);
    }
}
