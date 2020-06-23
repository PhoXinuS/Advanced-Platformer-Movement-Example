using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoEffect : MonoBehaviour
{
    [SerializeField] float destroyEchoAfter = 1f;
    [SerializeField] float timeBetweenEchoes = 0.2f;
    private float currentTimeLeft;

    public Transform echoHolder;
    public GameObject echo;

    private void Start()
    {
        currentTimeLeft = timeBetweenEchoes;
    }

    private void Update()
    {
        if (currentTimeLeft <= 0)
        {
            var echoInstance = GameObject.Instantiate(echo, transform.position, Quaternion.identity, echoHolder);
            GameObject.Destroy(echoInstance, destroyEchoAfter);
            currentTimeLeft = timeBetweenEchoes;
        }
        else
        {
            currentTimeLeft -= Time.deltaTime;
        }
    }
}
