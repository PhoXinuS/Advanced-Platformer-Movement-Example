using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public GrapplingRope grappleRope;

    [Header("Layers Settings:")]
    public LayerMask grappableLayers;

    [Header("Main Camera:")]
    public Camera m_camera;

    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    public float minDistnace = 2;
    public float maxDistnace = 10;

    [Header("Launching:")]
    // [SerializeField] private bool launchToPoint = true;
    // [SerializeField] private float launchSpeed = 1;

    //[Header("No Launch To Point")]
    //[SerializeField] private bool autoConfigureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequncy = 1;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    private IGrapplingHookInput grapplingHookInput;

    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        
        grapplingHookInput = new PlayerGrapplingHookInput(gameObject);
    }

    private void Update()
    {
        if (grapplingHookInput.grappleInput > 0 && grapplingHookInput.prevGrappleInput <= 0)
        {
            SetGrapplePoint();
        }
        else if (grapplingHookInput.grappleInput > 0)
        {
            if (grappleRope.enabled)
            {
                RotateGun(grapplePoint, false);
            }
            else
            {
                Vector2 mousePos = m_camera.ScreenToWorldPoint(grapplingHookInput.aimInput);
                RotateGun(mousePos, true);
            }
        }
        else if (grapplingHookInput.grappleInput <= 0 && grapplingHookInput.prevGrappleInput > 0)
        {
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            m_rigidbody.gravityScale = 1;
        }
        else
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(grapplingHookInput.aimInput);
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
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(grapplingHookInput.aimInput) - gunPivot.position;
        if (Physics2D.Raycast(firePoint.position, distanceVector.normalized))
        {
            RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized);
            if (grappableLayers.Contains(_hit.transform.gameObject.layer))
            {
                float distance = Vector2.Distance(_hit.point, firePoint.position);
                if (distance >= minDistnace && distance <= maxDistnace)
                {
                    grapplePoint = _hit.point;
                    grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                    grappleRope.enabled = true;
                }
            }
        }
    }

    public void Grapple()
    {
        m_springJoint2D.autoConfigureDistance = false;
        /*if (!launchToPoint)
        {
            if (autoConfigureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }
            else
            {
                m_springJoint2D.distance = targetDistance;
                m_springJoint2D.frequency = targetFrequncy;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }
        else
        {
            Vector2 distanceVector = firePoint.position - gunHolder.position;
            m_springJoint2D.distance = distanceVector.magnitude;
            
            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.frequency = launchSpeed;
            m_springJoint2D.enabled = true;
        }*/
        m_springJoint2D.distance = targetDistance; 
        m_springJoint2D.frequency = targetFrequncy;
        m_springJoint2D.connectedAnchor = grapplePoint;
        m_springJoint2D.enabled = true;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, minDistnace);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistnace);
        }
    }

}
