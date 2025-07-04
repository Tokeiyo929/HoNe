using UnityEngine;
using System.Collections.Generic;

public class RandomObjectArranger : MonoBehaviour
{
    [SerializeField] GameObject obj1;
    [SerializeField] GameObject obj2;
    [SerializeField] GameObject obj3;
    [SerializeField] GameObject obj4;
    [SerializeField] GameObject obj5;
    [SerializeField] GameObject obj6;
    [SerializeField] GameObject obj7;
    [SerializeField] GameObject obj8;
    [SerializeField] GameObject obj9;
    [SerializeField] GameObject obj10;
    [SerializeField] GameObject obj11;
    [SerializeField] GameObject obj12;
    [SerializeField] GameObject obj13;
    [SerializeField] GameObject obj14;
    [SerializeField] GameObject obj15;
    [SerializeField] GameObject obj16;

    // X坐标值
    private float[] xPositions = new float[] { -1.75f, -1.25f, -0.75f, -0.25f, 0.25f, 0.75f, 1.25f, 1.75f };
    // Y坐标值（两排）
    private float[] yPositions = new float[] { 7f, 7.5f };

    void OnEnable()
    {
        // 将所有对象放入列表
        List<GameObject> objects = new List<GameObject> { obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9, obj10, obj11, obj12, obj13, obj14, obj15, obj16 };

        // 随机打乱对象顺序
        ShuffleList(objects);

        // 分配位置
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

        // 第一排 (y = 3.75)
        for (int col = 0; col < 8; col++)
        {
            if (index >= objects.Count) return;
            objects[index].transform.localPosition = new Vector3(xPositions[col], yPositions[0], 0);
            objects[index].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            index++;
        }

        // 第二排 (y = 3.25)
        for (int col = 0; col < 8; col++)
        {
            if (index >= objects.Count) return;
            objects[index].transform.localPosition = new Vector3(xPositions[col], yPositions[1], 0);
            objects[index].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            index++;
        }
    }
}