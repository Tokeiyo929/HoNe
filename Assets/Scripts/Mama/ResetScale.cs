using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScale : MonoBehaviour
{
    [Range(0.2f, 0.5f)]
    public float resetScale;
    public void SetScale()
    {
        transform.localScale = new Vector3(resetScale, resetScale, resetScale);
    }
}