using UnityEngine;

internal class VelocitySmoother
{
    internal float xVelocity;
    
    internal float SmoothedVelocity(float targetVelocity, float currentVelocity, float accelerationTime, float decelerationTime)
    {
        float smoothTime = 0f;
        if (VelocityIsIncreasing(targetVelocity, currentVelocity))
        {
            smoothTime = accelerationTime;
        }
        else if (VelocityIsDecreasing(targetVelocity, currentVelocity))
        {
            smoothTime = decelerationTime;
        }
        
        float smoothedVelocity = Mathf.SmoothDamp(currentVelocity, targetVelocity, ref xVelocity, smoothTime, Mathf.Infinity, Time.fixedDeltaTime);
        return smoothedVelocity; 
    }   
    
    private bool VelocityIsIncreasing(float targetVelocity, float currentVelocity)
    {
        return Mathf.Abs(targetVelocity) > Mathf.Abs(currentVelocity);
    }

    private bool VelocityIsDecreasing(float horizontalTargetVelocity, float currentVelocity)
    {
        return Mathf.Abs(horizontalTargetVelocity) < Mathf.Abs(currentVelocity);
    }

}