using System;
using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    [Header("General References:")]
    public GrapplingHook grapplingHook;
    public LineRenderer lineRenderer;

    [Header("General Settings:")]
    [SerializeField] private int ropePolygons = 40;
    [Range(0, 20)] [SerializeField] private float straightenLineSpeed = 5f;

    [Header("Rope Animation Settings:")]
    public AnimationCurve ropeAnimationCurve;
    public AnimationCurve waveSizeWithDistance;
    [Range(0.01f, 6f)] [SerializeField] private float startWaveSize = 2f;
    float waveSize = 0;

    [Header("Rope Progression:")]
    public AnimationCurve ropeProgressionCurve;
    [SerializeField] [Range(1, 50)] private float ropeProgressionSpeed = 1;

    float moveTime = 0;

    [HideInInspector] public bool isGrappling = true;
   
    bool straightLine = true;

    private void OnEnable()
    {
        moveTime = 0;
        lineRenderer.positionCount = ropePolygons;
        waveSize = startWaveSize;
        float distanceToPass = grapplingHook.grappleDistanceVector.magnitude;
        waveSize *= waveSizeWithDistance.Evaluate(distanceToPass / grapplingHook.maxDistnace);
        straightLine = false;

        LinePointsToFirePoint();

        lineRenderer.enabled = true;
    }

    private void LinePointsToFirePoint()
    {
        for (int i = 0; i < ropePolygons; i++)
        {
            lineRenderer.SetPosition(i, grapplingHook.firePoint.position);
        }
    }
    
    private void Update()
    {
        moveTime += Time.deltaTime;
        DrawRope();
    }
    
    private void DrawRope()
    {
        if (!straightLine)
        {
            if (RopeReachedGrapplePoint())
            {
                straightLine = true;
            }
            else
            {
                DrawRopeWaves();
            }
        }
        else
        {
            if (!isGrappling)
            {
                grapplingHook.Grapple();
                isGrappling = true;
            }

            if (waveSize > 0)
            {
                waveSize -= Time.deltaTime * straightenLineSpeed;
                DrawRopeWaves();
            }
            else
            {
                StraightenRope();
            }
        }
    }

    private bool RopeReachedGrapplePoint()
    {
        float lineRendererTolerance = 0.1f;
        return Math.Abs(lineRenderer.GetPosition(ropePolygons - 1).x - grapplingHook.connectedToPoint.x) < lineRendererTolerance;
    }

    private void DrawRopeWaves()
    {
        for (int i = 0; i < ropePolygons; i++)
        {
            float delta = i / (ropePolygons - 1f);
            Vector2 offset = Vector2.Perpendicular(grapplingHook.grappleDistanceVector).normalized * (ropeAnimationCurve.Evaluate(delta) * waveSize);
            Vector2 firePointPosition = grapplingHook.firePoint.position;
            Vector2 targetPosition = Vector2.Lerp(firePointPosition, grapplingHook.connectedToPoint, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(firePointPosition, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);

            lineRenderer.SetPosition(i, currentPosition);
        }
    }

    private void StraightenRope()
    {
        waveSize = 0;

        if (lineRenderer.positionCount != 2)
        {
            lineRenderer.positionCount = 2;
        }

        DrawRopeNoWaves();
    }

    private void DrawRopeNoWaves()
    {
        lineRenderer.SetPosition(0, grapplingHook.firePoint.position);
        lineRenderer.SetPosition(1, grapplingHook.connectedToPoint);
    }
    
    private void OnDisable()
    {
        lineRenderer.enabled = false;
        isGrappling = false;
    }
}