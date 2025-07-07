using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScale : MonoBehaviour
{
    [SerializeField] List<GameObject> objSet;
    [SerializeField] GameObject addOnUI;
    // Ϊÿ���������ض�������ֵ
    private readonly Dictionary<int, float> scaleValues = new Dictionary<int, float>()
    {
        {0, 0.48f},  // object1
        {1, 0.32f},  // object2
        {2, 0.15f},  // object3
        {3, 0.15f},  // object4
        {4, 0.25f},  // object5
        {5, 0.2f}    // object6
    };

    void OnEnable()
    {
        addOnUI.SetActive(true);
        for (int i = 0; i < objSet.Count; i++)
        {
            if (objSet[i] != null)
            {
                // ����ֵ����ж��������������ֵ����ʹ����������ʹ��Ĭ��ֵ0.5f
                float scale = scaleValues.ContainsKey(i) ? scaleValues[i] : 0.5f;
                objSet[i].transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
    void OnDisable()
    {
        addOnUI.SetActive(false);
    }
}