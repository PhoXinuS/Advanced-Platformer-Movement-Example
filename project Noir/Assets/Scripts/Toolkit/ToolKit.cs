using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class ToolKit
{
    /// <summary>
    /// Extension method to check if a layer is in a layermask
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mixer"></param>
    /// <param name="exposedName">The name of 'The Exposed to Script' variable</param>
    /// <param name="value">value must be between 0 and 1</param>
    public static void SetVolume(this AudioMixer mixer, string exposedName, float value)
    {
        mixer.SetFloat(exposedName, Mathf.Lerp(-80.0f, 0.0f, Mathf.Clamp01(value)));
    }
 
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mixer"></param>
    /// <param name="exposedName">The name of 'The Exposed to Script' variable</param>
    /// <returns></returns>
    public static float GetVolume(this AudioMixer mixer, string exposedName)
    {
        if (mixer.GetFloat(exposedName, out float volume))
        {
            return Mathf.InverseLerp(-80.0f, 0.0f, volume);
        }
 
        return 0f;
    }
        
    
}


