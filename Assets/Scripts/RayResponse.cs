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
    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.F) && currentShownObj != null)
        //{
        //    SetCanvasEnabled(currentShownObj, true);
        //}
    }
    void HandleObjectHit(GameObject _obj, RaycastHit _hitInfo)
    {
        //ShowCanvas(_obj);
        //ShowOutline(_obj);
        //CreateOrUpdateCursor(_hitInfo);
    }
    void HandleObjectStayHit(RaycastHit _hitInfo)
    {
        ScaleObject(_hitInfo);
    }
    void HandleNoObjectHit()
    {
        //HideCanvas();
        //HideOutline();
    }
    void ScaleObject(RaycastHit _hitInfo)
    {
        if (_hitInfo.collider.gameObject.layer != 6)
            return;
        
        if (Input.GetMouseButtonDown(0))
        {
            Transform _objTrans = _hitInfo.collider.gameObject.transform;
            _objTrans.localScale = new Vector3(1, 1, 1);
        }
    }

}
