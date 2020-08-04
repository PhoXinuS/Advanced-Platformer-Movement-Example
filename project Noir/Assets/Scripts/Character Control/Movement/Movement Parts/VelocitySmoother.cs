using UnityEngine;

internal class VelocitySmoother
{
    internal float xVelocity;
    
    internal float SmoothedVelocity(float targetVelocity, float currentVelocity, float accelerationTime, float decelerationTime)
    {
        float smoothTime;
        if (VelocityIsDecreasing(targetVelocity, currentVelocity))
        {
            smoothTime = decelerationTime;
        }
        else
        {
            smoothTime = accelerationTime;
        }
        
        float smoothedVelocity = Mathf.SmoothDamp(currentVelocity, targetVelocity, ref xVelocity, smoothTime, Mathf.Infinity, Time.fixedDeltaTime);
        return smoothedVelocity; 
    }
    private bool VelocityIsDecreasing(float TargetVelocity, float currentVelocity)
    {
        return Mathf.Abs(TargetVelocity) < Mathf.Abs(currentVelocity);
    }

}