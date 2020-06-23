using UnityEngine;

[System.Serializable]
public struct CameraShakeData
{
    [Range(0, 1)] public float stress;
    [Range(0, 1)] public float maxStress;
}


