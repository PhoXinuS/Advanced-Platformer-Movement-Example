using UnityEngine;

// Spring Joint 2D uses a Box 2D spring-joint.
// Distance Joint 2D also uses the same Box 2D spring-joint but it sets the frequency to zero!
// Technically, a Spring Joint 2D with a frequency of zero and damping of 1 is identical to a Distance Joint 2D

public class GrapplingHook : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public GrapplingRope grappleRope;

    [Header("Layers Settings:")]
    public LayerMask grappableLayers;

    [Header("Main Camera:")]
    public Camera mainCamera;

    [Header("Transform Ref:")]
    public Transform gunPivot;
    public Transform firePoint;
    
    [Header("Spring Joint:")]
    [SerializeField] private bool useSpringJoint = true;
    public SpringJoint2D springJoint2D;
    
    [Header("RigidBody2D:")]
    public Rigidbody2D rigidBody2D;
    [SerializeField] private bool canBeConnectedToOtherRigidBodies2D = true;
    [HideInInspector] public Rigidbody2D connectedRigidBody2D;
    private bool connectedToRigidBody2D = false;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    public float minDistnace = 2;
    public float maxDistnace = 10;

    [Header("Static Object Settings:")]
    [SerializeField] private float targetDistanceStatic = 3;

    [Header("Grappable Object Settings:")]
    [SerializeField] private float targetDistanceGrappable = 2f;

    [Header("Shared Object Settings:")]
    [SerializeField] private float targetDampingRatio = 1f;
    [SerializeField] private float targetFrequency = 0.75f;

    [HideInInspector] public Vector2 grappleDistanceVector;
    [HideInInspector] public Vector2 connectedToPoint;
    private Vector2 connectedAnchor;

    private IGrapplingHookInput grapplingHookInput;

    private void Start()
    {
        grappleRope.enabled = false;
        springJoint2D.enabled = false;

        grapplingHookInput = new PlayerGrapplingHookInput(gameObject);
    }

    private void Update()
    {
        if (connectedToRigidBody2D)
        {
            connectedToPoint = connectedRigidBody2D.transform.TransformPoint(connectedAnchor);
        }
        
        if (grapplingHookInput.grappleInput > 0 && grapplingHookInput.prevGrappleInput <= 0)
        {
            SetGrapplePoint();
        }
        else if (grapplingHookInput.grappleInput > 0)
        {
            if (grappleRope.enabled)
            {
                RotateGun(connectedToPoint, false);
            }
            else
            {
                Vector2 mousePos = mainCamera.ScreenToWorldPoint(grapplingHookInput.aimInput);
                RotateGun(mousePos, true);
            }
        }
        else if (grapplingHookInput.grappleInput <= 0 && grapplingHookInput.prevGrappleInput > 0)
        {
            grappleRope.enabled = false;
            springJoint2D.enabled = false;
            connectedToRigidBody2D = false;

            rigidBody2D.gravityScale = 1;
        }
        else
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(grapplingHookInput.aimInput);
            RotateGun(mousePos, true);
        }
    }

    private void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        AdjustScale();
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    private void AdjustScale()
    {
        if (gunPivot.lossyScale.x < 0)
        {
            Vector3 localScale = gunPivot.localScale;
            localScale = new Vector3(-localScale.x, localScale.y);
            gunPivot.localScale = localScale;
        }

        if (gunPivot.lossyScale.y < 0)
        {
            Vector3 localScale = gunPivot.localScale;
            localScale = new Vector3(localScale.x, -localScale.y);
            gunPivot.localScale = localScale;
        }
    }

    private void SetGrapplePoint()
    {
        Vector2 distanceVector = mainCamera.ScreenToWorldPoint(grapplingHookInput.aimInput) - gunPivot.position;
        if (Physics2D.Raycast(firePoint.position, distanceVector.normalized))
        {
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized);
            if (grappableLayers.Contains(hit.transform.gameObject.layer))
            {
                float distance = Vector2.Distance(hit.point, firePoint.position);
                if (distance >= minDistnace && distance <= maxDistnace)
                {
                    grappleDistanceVector = hit.point - (Vector2)gunPivot.position;
                    grappleRope.enabled = true;
                    
                    if (canBeConnectedToOtherRigidBodies2D &&
                        hit.transform.TryGetComponent(out IGrappable ableToGrapple))
                    {
                        connectedToRigidBody2D = true;
                        connectedRigidBody2D = ableToGrapple.rigidBody2D;
                        connectedAnchor = connectedRigidBody2D.transform.InverseTransformPoint(hit.point);
                        connectedToPoint = hit.point;
                    }
                    else
                    {
                        connectedToRigidBody2D = false;
                        connectedRigidBody2D = null;
                        connectedAnchor = hit.point;
                        connectedToPoint = connectedAnchor;
                    }
                }
            }
        }
    }

    public void Grapple()
    {
        if (useSpringJoint)
        {
            springJoint2D.autoConfigureDistance = false;
            springJoint2D.frequency = targetFrequency;
            springJoint2D.dampingRatio = targetDampingRatio;
            springJoint2D.connectedBody = connectedRigidBody2D;

            if (connectedToRigidBody2D)
            {
                springJoint2D.distance = targetDistanceGrappable;
                springJoint2D.connectedAnchor = connectedAnchor;
            }
            else
            {
                springJoint2D.distance = targetDistanceStatic;
                springJoint2D.connectedAnchor = connectedAnchor;
            }
            
            springJoint2D.enabled = true;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            var position = firePoint.position;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, minDistnace);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(position, maxDistnace);
        }
    }
}