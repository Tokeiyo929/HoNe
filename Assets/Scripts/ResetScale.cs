using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScale : MonoBehaviour
{
    [SerializeField] List<GameObject> objSet;
    [SerializeField] GameObject addOnUI;
    // 为每个对象定义特定的缩放值
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
                // 如果字典中有定义该索引的缩放值，则使用它，否则使用默认值0.5f
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