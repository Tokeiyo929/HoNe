using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour
{
    [System.Serializable]
    public class DragStep
    {
        public GameObject draggableObject;      // 需要拖拽的物体
        public Collider targetTrigger;          // 目标BaseTrigger
        public Transform targetPosition;        // 指定移动位置

        [HideInInspector] public Vector3 initialPosition;
        [HideInInspector] public Quaternion initialRotation;
        [HideInInspector] public bool triggerInitialEnabled;
        [HideInInspector] public bool isCompleted;
        [HideInInspector] public GameObject occupiedObject; // 当前占用此格子的拼图
    }

    public List<DragStep> level1Steps = new List<DragStep>();
    public List<DragStep> level2Steps = new List<DragStep>();
    public List<DragStep> level3Steps = new List<DragStep>();
    public List<DragStep> level4Steps = new List<DragStep>();
    public List<DragStep> level5Steps = new List<DragStep>();

    private List<DragStep> dragSteps = new List<DragStep>();
    private int currentStep = 0;
    private Transform selectedObject = null;
    private Vector3 offset;
    private float zCoord;
    private float initialZ;
    private bool isMovingToTarget = false;
    private bool isFoorLevel = false;

    private List<GameObject> draggableObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> initialRotations = new Dictionary<GameObject, Quaternion>();
    private Dictionary<GameObject, DragStep> objectToStepMap = new Dictionary<GameObject, DragStep>();

    public Action<bool> OnLevelFinished;
    public bool canDrag = true;

    void Start()
    {
        SetLevel(1);
    }

    public void SetLevel(int level)
    {
        switch (level)
        {
            case 1:
                dragSteps = level1Steps;
                isFoorLevel = false;
                break;
            case 2:
                dragSteps = level2Steps;
                isFoorLevel = false;
                break;
            case 3:
                dragSteps = level3Steps;
                isFoorLevel = false;
                break;
            case 4:
                dragSteps = level4Steps;
                isFoorLevel = true;
                break;
            case 5:
                dragSteps = level5Steps;
                isFoorLevel = false;
                break;
            default:
                dragSteps = new List<DragStep>();
                isFoorLevel = false;
                break;
        }

        objectToStepMap.Clear();

        foreach (var step in dragSteps)
        {
            step.isCompleted = false;
            step.occupiedObject = null;
            if (step.draggableObject != null)
            {
                step.initialPosition = step.draggableObject.transform.position;
                step.initialRotation = step.draggableObject.transform.rotation;
            }
            if (step.targetTrigger != null)
            {
                step.triggerInitialEnabled = step.targetTrigger.enabled;
            }
        }

        currentStep = 0;
        selectedObject = null;
        isMovingToTarget = false;

        draggableObjects.Clear();
        initialPositions.Clear();
        initialRotations.Clear();

        foreach (var step in dragSteps)
        {
            if (step.draggableObject != null && !draggableObjects.Contains(step.draggableObject))
            {
                draggableObjects.Add(step.draggableObject);
                initialPositions[step.draggableObject] = step.draggableObject.transform.position;
                initialRotations[step.draggableObject] = step.draggableObject.transform.rotation;
            }
        }
    }

    public void ResetLevel()
    {
        foreach (var step in dragSteps)
        {
            if (step.draggableObject != null)
            {
                step.draggableObject.transform.position = step.initialPosition;
                step.draggableObject.transform.rotation = step.initialRotation;
            }
            if (step.targetTrigger != null)
            {
                step.targetTrigger.enabled = step.triggerInitialEnabled;
            }
            step.occupiedObject = null;
            step.isCompleted = false;
        }

        objectToStepMap.Clear();

        draggableObjects.Clear();
        initialPositions.Clear();
        initialRotations.Clear();

        foreach (var step in dragSteps)
        {
            if (step.draggableObject != null && !draggableObjects.Contains(step.draggableObject))
            {
                draggableObjects.Add(step.draggableObject);
                initialPositions[step.draggableObject] = step.initialPosition;
                initialRotations[step.draggableObject] = step.initialRotation;
            }
        }

        currentStep = 0;
        selectedObject = null;
        isMovingToTarget = false;

        StopAllCoroutines();
    }

    void Update()
    {
        if (IsLevelComplete()) return;
        if (!canDrag || isMovingToTarget) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (draggableObjects.Contains(hit.transform.gameObject))
                {
                    selectedObject = hit.transform;
                    zCoord = Camera.main.WorldToScreenPoint(selectedObject.position).z;
                    Vector3 mousePoint = Input.mousePosition;
                    mousePoint.z = zCoord;
                    offset = selectedObject.position - Camera.main.ScreenToWorldPoint(mousePoint);
                    initialZ = selectedObject.position.z;
                }
            }
        }
        else if (Input.GetMouseButton(0) && selectedObject != null)
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = zCoord;
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(mousePoint) + offset;
            newPosition.z = initialZ;
            selectedObject.position = newPosition;
        }
        else if (Input.GetMouseButtonUp(0) && selectedObject != null)
        {
            if (isFoorLevel)
            {
                HandleReleaseForFourthLevel();
            }
            else
            {
                HandleReleaseForOtherLevels();
            }
        }
#else
    // 触摸操作（Android等移动端）
    if (Input.touchCount > 0)
    {
        UnityEngine.Touch touch = Input.GetTouch(0);
        Vector3 touchPos = touch.position;
        if (touch.phase == TouchPhase.Began){
        
            Ray ray = Camera.main.ScreenPointToRay(touchPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.layer == 6)
                {                    hit.transform.localScale = Vector3.one;
                }
                if (draggableObjects.Contains(hit.transform.gameObject))
                {
                    selectedObject = hit.transform;
                    zCoord = Camera.main.WorldToScreenPoint(selectedObject.position).z;
                    touchPos.z = zCoord;
                    offset = selectedObject.position - Camera.main.ScreenToWorldPoint(touchPos);
                    // 记录初始 Z 坐标
                    initialZ = selectedObject.position.z;
                }            }
        }
        else if (touch.phase == TouchPhase.Moved && selectedObject != null)
{
    touchPos.z = zCoord;
    Vector3 newPosition = Camera.main.ScreenToWorldPoint(touchPos) + offset;
    newPosition.z = initialZ;  // 明确保持Z坐标不变
    selectedObject.position = newPosition;
}
else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
{
    if (selectedObject != null)
    {
        if (isFoorLevel)
        {
            HandleReleaseForFourthLevel();
        }
        else
        {
            HandleReleaseForOtherLevels();
        }
        selectedObject = null;
    }
}
    }
#endif
    }

    private void HandleReleaseForFourthLevel()
    {
        Collider selectedCol = selectedObject.GetComponent<Collider>();
        if (selectedCol == null) return;

        bool placed = false;

        foreach (var step in dragSteps)
        {
            if (step.targetTrigger != null)
            {
                // 判断是否与格子碰撞盒重叠
                if (selectedCol.bounds.Intersects(step.targetTrigger.bounds))
                {
                    // 如果此格子被其他拼图占用，释放那个拼图回原位
                    if (step.occupiedObject != null && step.occupiedObject != selectedObject.gameObject)
                    {
                        GameObject oldObj = step.occupiedObject;
                        StartCoroutine(MoveToInitialPosition(oldObj.transform));
                        objectToStepMap.Remove(oldObj);
                        draggableObjects.Add(oldObj);
                    }

                    // 清除当前物体旧占用格子信息
                    if (objectToStepMap.TryGetValue(selectedObject.gameObject, out DragStep oldStep))
                    {
                        oldStep.occupiedObject = null;
                        oldStep.targetTrigger.enabled = true;
                        objectToStepMap.Remove(selectedObject.gameObject);
                    }

                    // 占用当前格子
                    step.occupiedObject = selectedObject.gameObject;
                    step.targetTrigger.enabled = false;
                    objectToStepMap[selectedObject.gameObject] = step;

                    StartCoroutine(MoveToTarget(selectedObject, step.targetPosition.position, step.targetPosition.rotation));

                    //if (AIChatManager.Instance != null)
                    //    AIChatManager.Instance.ShowDragRightChat();

                    placed = true;
                    break;
                }
            }
        }

        if (!placed)
        {
            StartCoroutine(MoveToInitialPosition(selectedObject));
        }

        selectedObject = null;
    }

    private void HandleReleaseForOtherLevels()
{
    Collider selectedCol = selectedObject.GetComponent<Collider>();
    if (selectedCol == null) return;

    var step = dragSteps[currentStep];

    if (selectedObject.gameObject == step.draggableObject)
    {
        if (step.targetTrigger != null && selectedCol.bounds.Intersects(step.targetTrigger.bounds))
        {
            step.targetTrigger.enabled = false;
            step.isCompleted = true;

            // 从可拖拽列表中移除该物体，防止再次拖拽
            if (draggableObjects.Contains(selectedObject.gameObject))
            {
                draggableObjects.Remove(selectedObject.gameObject);
            }

            StartCoroutine(MoveToTarget(selectedObject, step.targetPosition.position, step.targetPosition.rotation));

            if (AIChatManager.Instance != null)
                AIChatManager.Instance.ShowDragRightChat();

            currentStep++;
        }
        else
        {
            StartCoroutine(MoveToInitialPosition(selectedObject));

            if (AIChatManager.Instance != null)
                AIChatManager.Instance.ShowDragWrongChat();
        }
    }
    else
    {
        if (step.targetTrigger != null && selectedCol.bounds.Intersects(step.targetTrigger.bounds))
        {
            StartCoroutine(MoveToInitialPosition(selectedObject));

            if (AIChatManager.Instance != null)
                AIChatManager.Instance.ShowDragWrongChat();
        }
        else
        {
            StartCoroutine(MoveToInitialPosition(selectedObject));
        }
    }

    selectedObject = null;
}

    //void FixedUpdate()
    //{
    //    if (IsLevelComplete()) return;
    //    if (selectedObject == null) return;
    //    if (isMovingToTarget) return;

    //    Collider objCol = selectedObject.GetComponent<Collider>();
    //    if (objCol == null) return;

    //    if (!isFoorLevel)
    //    {
    //        var step = dragSteps[currentStep];
    //        if (selectedObject.gameObject == step.draggableObject)
    //        {
    //            if (step.targetTrigger != null && objCol.bounds.Intersects(step.targetTrigger.bounds))
    //            {
    //                step.targetTrigger.enabled = false;
    //                step.isCompleted = true;
    //                StartCoroutine(MoveToTarget(selectedObject, step.targetPosition.position, step.targetPosition.rotation));

    //                if (AIChatManager.Instance != null)
    //                    AIChatManager.Instance.ShowDragRightChat();
    //            }
    //        }
    //        else
    //        {
    //            if (step.targetTrigger != null && objCol.bounds.Intersects(step.targetTrigger.bounds))
    //            {
    //                StartCoroutine(MoveToInitialPosition(selectedObject));
    //                selectedObject = null;

    //                if (AIChatManager.Instance != null)
    //                    AIChatManager.Instance.ShowDragWrongChat();
    //            }
    //        }
    //    }
    //}

    private bool IsLevelComplete()
    {
        if (isFoorLevel)
        {
            foreach (var step in dragSteps)
            {
                if (step.occupiedObject != step.draggableObject)
                    return false;
            }
            return true;
        }
        else
        {
            return currentStep >= dragSteps.Count;
        }
    }

    private IEnumerator MoveToInitialPosition(Transform obj)
    {
        if (obj.gameObject.layer == 6)
            obj.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        isMovingToTarget = true;
        Vector3 startPos = obj.position;
        Quaternion startRot = obj.rotation;
        Vector3 targetPos = initialPositions[obj.gameObject];
        Quaternion targetRot = initialRotations[obj.gameObject];

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            obj.position = Vector3.Lerp(startPos, targetPos, t);
            obj.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        obj.position = targetPos;
        obj.rotation = targetRot;

        isMovingToTarget = false;
    }

    private IEnumerator MoveToTarget(Transform obj, Vector3 targetPos, Quaternion targetRot)
    {
        isMovingToTarget = true;
        Vector3 startPos = obj.position;
        Quaternion startRot = obj.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            obj.position = Vector3.Lerp(startPos, targetPos, t);
            obj.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        obj.position = targetPos;
        obj.rotation = targetRot;

        selectedObject = null;
        isMovingToTarget = false;

        if (IsLevelComplete() && OnLevelFinished != null)
        {
            OnLevelFinished.Invoke(true);
            OnLevelFinished = null;
        }
    }
}
