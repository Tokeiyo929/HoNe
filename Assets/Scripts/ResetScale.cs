using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScale : MonoBehaviour
{
    [SerializeField] List<GameObject> objSet;
    // Start is called before the first frame update
    void OnEnable()
    {
        foreach (GameObject obj in objSet)
        {
            if (obj != null)
            {
                obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
    }
}
