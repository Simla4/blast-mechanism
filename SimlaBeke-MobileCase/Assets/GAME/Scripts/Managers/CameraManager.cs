using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Refferances")]
    [SerializeField] private SpriteRenderer borderSpriteRenderer;

    private void Awake()
    {
        transform.position = new Vector3(borderSpriteRenderer.size.x / 2, borderSpriteRenderer.size.y / 2, transform.position.z);
    }
}
