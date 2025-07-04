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

    // X����ֵ
    private float[] xPositions = new float[] { -0.75f, -0.25f, 0.25f, 0.75f };
    // Y����ֵ�����ţ�
    private float[] yPositions = new float[] { 3.75f, 3.25f };

    void OnEnable()
    {
        // �����ж�������б�
        List<GameObject> objects = new List<GameObject> { obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8 };

        // ������Ҷ���˳��
        ShuffleList(objects);

        // ����λ��
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

        // ��һ�� (y = 3.75)
        for (int col = 0; col < 4; col++)
        {
            if (index >= objects.Count) return;
            objects[index].transform.localPosition = new Vector3(xPositions[col], yPositions[0], 0);
            objects[index].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            index++;
        }

        // �ڶ��� (y = 3.25)
        for (int col = 0; col < 4; col++)
        {
            if (index >= objects.Count) return;
            objects[index].transform.localPosition = new Vector3(xPositions[col], yPositions[1], 0);
            objects[index].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            index++;
        }
    }
}