using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayEventManager : MonoBehaviour
{
    public static event Action<GameObject, RaycastHit> OnObjectHit;
    public static event Action OnNoObjectHit;
    public static event Action<RaycastHit> OnObjectStayHit;

    public static void TriggerObjectHit(GameObject _obj, RaycastHit _hitInfo)
    {
        OnObjectHit?.Invoke(_obj, _hitInfo);
    }

    public static void TriggerObjectStayHit(RaycastHit _hitInfo)
    {
        OnObjectStayHit?.Invoke(_hitInfo);
    }

    public static void TriggerNoObjectHit()
    {
        OnNoObjectHit?.Invoke();
    }
}
