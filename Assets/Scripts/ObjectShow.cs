using System.Collections.Generic;
using UnityEngine;

public class ObjectShow : MonoBehaviour
{
    [Header("需要高亮的对象集合（顺序与题目顺序一致）")]
    public List<GameObject> objects = new List<GameObject>();

    // 重置所有对象的Outline为关闭
    public void ResetAllOutline()
    {
        foreach (var obj in objects)
        {
            if (obj == null) continue;
            var outline = obj.GetComponent<Outline>();
            if (outline != null) outline.enabled = false;
        }
    }

    // 只高亮指定index的对象，其余全部关闭高亮
    public void ShowObjectWithOutline(int index)
    {
        // 新增：index为-1时全部高亮
        if (index == -1)
        {
            foreach (var obj in objects)
            {
                if (obj == null) continue;
                var outline = obj.GetComponent<Outline>();
                if (outline != null) outline.enabled = true;
            }
            return;
        }
        for (int i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];
            if (obj == null) continue;
            var outline = obj.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = (i == index);
        }
    }
}
