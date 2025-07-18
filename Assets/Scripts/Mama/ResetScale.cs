using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScale : MonoBehaviour
{
    [Range(0.1f, 20.0f)]
    public float resetScale = 0.3f;

    [Tooltip("The normal scale to reset to when clicked")]
    public Vector3 normalScale = Vector3.one;

    public void SetScale()
    {
        transform.localScale = new Vector3(resetScale, resetScale, resetScale);
    }
}