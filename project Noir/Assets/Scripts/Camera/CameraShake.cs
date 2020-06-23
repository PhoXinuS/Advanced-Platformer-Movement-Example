using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    /// <summary>
    /// Takes flot with range (0f - 1f) and applies shake to the camera
    /// (ensure that the script is attached to the Camera!)
    /// </summary>
    public static Action<CameraShakeData> ShakeCamera;

    public Transform mainCameraPoint;
    public Transform mainCamera;

    [SerializeField] private float shakeSpeed = 15f;
    [SerializeField] private float stressDecrease_perSec = 0.5f;

    [SerializeField] private float maxAngle = 6f;
    [SerializeField] private float maxoffset = 2.5f;

    private float stress;
    private float maxStress;

    private float seed;

    private float angle;
    private float offsetX;
    private float offsetY;

    private void Start()
    {
        seed = UnityEngine.Random.Range(0, 100000f);
        ShakeCamera += AddStress;
    }
    private void AddStress(CameraShakeData shakeData)
    {
        SetMaxStress(shakeData);
        SetStress(shakeData);
    }

    private void SetMaxStress(CameraShakeData shakeData)
    {
        maxStress = shakeData.maxStress;
        if (stress > maxStress)
        {
            maxStress = stress;
        }
    }
    private void SetStress(CameraShakeData shakeData)
    {
        stress += shakeData.stress;
        if (stress > maxStress)
        {
            stress = maxStress;
        }
    }

    private void Update()
    {      
        CalculateShake();
        ApplyShake();
        DecreaseStress();
    }

    private void CalculateShake()
    {
        var shake = stress * stress;
        angle = maxAngle * shake * ((Mathf.PerlinNoise(seed, 0f + Time.time * shakeSpeed) * 2) - 1);
        offsetX = maxoffset * shake * ((Mathf.PerlinNoise(seed + 10 + Time.time * shakeSpeed, 0f) * 2) - 1);
        offsetY = maxoffset * shake * ((Mathf.PerlinNoise(seed + 20 + Time.time * shakeSpeed, 0f) * 2) - 1);
    }

    private void ApplyShake()
    {
        mainCamera.eulerAngles = new Vector3(mainCameraPoint.eulerAngles.x, mainCameraPoint.eulerAngles.y, mainCameraPoint.eulerAngles.z + angle);
        mainCamera.position = new Vector3(mainCameraPoint.position.x + offsetX, mainCameraPoint.position.y + offsetY, mainCameraPoint.position.z);
    }

    private void DecreaseStress()
    {
        if (stress > 0)
        {
            stress -= (stressDecrease_perSec * Time.deltaTime);
        }
        else
        {
            stress = 0;
            maxStress = 1;
        }
    }


    private void OnDestroy()
    {
        ShakeCamera -= AddStress;
    }
}
