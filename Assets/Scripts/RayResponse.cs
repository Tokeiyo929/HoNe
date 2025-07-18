using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayResponse : MonoBehaviour
{
    private void OnEnable()
    {
        RayEventManager.OnObjectStayHit += HandleObjectStayHit;
        RayEventManager.OnObjectHit += HandleObjectHit;
        RayEventManager.OnNoObjectHit += HandleNoObjectHit;
    }

    private void OnDisable()
    {
        RayEventManager.OnObjectStayHit -= HandleObjectStayHit;
        RayEventManager.OnObjectHit -= HandleObjectHit;
        RayEventManager.OnNoObjectHit -= HandleNoObjectHit;
    }

    void HandleObjectHit(GameObject _obj, RaycastHit _hitInfo)
    {
    }

    void HandleObjectStayHit(RaycastHit _hitInfo)
    {
        ScaleObject(_hitInfo);
    }

    void HandleNoObjectHit()
    {
    }

    void ScaleObject(RaycastHit _hitInfo)
    {
        if (_hitInfo.collider.gameObject.layer != 6)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Transform _objTrans = _hitInfo.collider.gameObject.transform;
            ResetScale resetScaleComponent = _objTrans.GetComponent<ResetScale>();
            Vector3 targetScale = resetScaleComponent != null ?
                new Vector3(resetScaleComponent.normalScale.x, resetScaleComponent.normalScale.y, resetScaleComponent.normalScale.z) :
                Vector3.one;

            _objTrans.localScale = targetScale;
        }
    }
}