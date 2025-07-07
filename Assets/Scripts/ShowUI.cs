using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUI : MonoBehaviour
{
    [SerializeField] List<GameObject> objSet;
    [SerializeField] GameObject addOnUI;

    void OnEnable()
    {
        addOnUI.SetActive(true);
        for (int i = 0; i < objSet.Count; i++)
        {
            if (objSet[i] != null)
            {
                ResetScale resetScale = objSet[i].GetComponent<ResetScale>();
                if(resetScale != null)
                {
                    resetScale.SetScale();
                }
            }
        }
    }
    void OnDisable()
    {
        addOnUI.SetActive(false);
    }
}