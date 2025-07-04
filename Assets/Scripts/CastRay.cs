using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastRay : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private GameObject LastObj = null;
    private GameObject currentObj;
    private bool wasHitting = false;
    //[SerializeField] private LayerMask onlyLayer;

    void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            //Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.green);
            currentObj = hitInfo.collider.gameObject;
            //SetOuline(currentObj, true);

            if (wasHitting)
            {
                if(currentObj == LastObj)
                {
                    RayEventManager.TriggerObjectStayHit(hitInfo);
                }
                else
                {
                    RayEventManager.TriggerNoObjectHit();
                    RayEventManager.TriggerObjectHit(currentObj, hitInfo);
                    LastObj = currentObj;
                }
            }
            else
            {
                RayEventManager.TriggerObjectHit(currentObj, hitInfo);
                LastObj = currentObj;
                wasHitting = true;
            }
        }
        else
        {
            //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
            if (wasHitting)
            {
                RayEventManager.TriggerNoObjectHit();
                //SetOuline(LastObj, false);
                LastObj = null;
                wasHitting = false;
            }
        }
    }
}
