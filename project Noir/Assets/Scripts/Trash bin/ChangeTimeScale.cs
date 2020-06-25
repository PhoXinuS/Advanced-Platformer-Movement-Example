using System;
using UnityEngine;


public class ChangeTimeScale : MonoBehaviour
{
    [SerializeField] float timeScale = 0.3f;
    
    private void Start()
    {
        Time.timeScale = timeScale;
    }
}