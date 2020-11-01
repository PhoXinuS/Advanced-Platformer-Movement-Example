using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Grappable : MonoBehaviour, IGrappable
{
    public Rigidbody2D rigidBody2D { get; set; }

    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
    }
}

public interface IGrappable
{
    Rigidbody2D rigidBody2D { get; set; }
}
