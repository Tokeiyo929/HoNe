using UnityEngine;
using System.Collections.Generic;

public class RandomObjectArranger : MonoBehaviour
{
    [Header("需要打乱的对象")]
    [SerializeField] List<GameObject> objects;

    [Header("X坐标位置")]
    [SerializeField] float[] xPositions = new float[] { -1.75f, -1.25f, -0.75f, -0.25f, 0.25f, 0.75f, 1.25f, 1.75f };

    [Header("Y坐标位置")]
    [SerializeField] float[] yPositions = new float[] { 1.75f, 2.25f };

    [Header("统一缩放")]
    [SerializeField] Vector3 objectScale = new Vector3(0.4f, 0.4f, 1f);

    [SerializeField] bool scaleRight = false;

    void OnEnable()
    {
        if (objects == null || objects.Count == 0)
        {
            Debug.LogWarning("请在 Inspector 中分配 objects");
            return;
        }

        ShuffleList(objects);
        ArrangeObjects(objects);
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void ArrangeObjects(List<GameObject> objects)
    {
        int index = 0;
        // 从最小的 Y（最下面）开始
        for (int row = 0; row < yPositions.Length; row++)
        {
            // 从最大的 X（最右边）开始
            for (int col = xPositions.Length - 1; col >= 0; col--)
            {
                if (index >= objects.Count) return;
                objects[index].transform.localPosition = new Vector3(xPositions[col], yPositions[row], 0);
                if (scaleRight)
                    objects[index].transform.localScale = objectScale;

                index++;
            }
        }
    }
}