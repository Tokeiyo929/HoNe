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
    }

    [Header("第一关拖拽步骤")]
    public List<DragStep> level1Steps = new List<DragStep>();
    [Header("第二关拖拽步骤（todo）")]
    public List<DragStep> level2Steps = new List<DragStep>();
    [Header("第三关拖拽步骤（todo）")]
    public List<DragStep> level3Steps = new List<DragStep>();
    private List<DragStep> dragSteps = new List<DragStep>(); // 当前关卡拖拽步骤
    private int currentStep = 0; // 当前步骤索引
    private Transform selectedObject = null;
    private Vector3 offset;
    private float zCoord;
    private bool isMovingToTarget = false;
    // 新增：记录所有可拖拽物体
    private List<GameObject> draggableObjects = new List<GameObject>();
    // 新增：记录每个物体初始位置和旋转（便于回退）
    private Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> initialRotations = new Dictionary<GameObject, Quaternion>();
    // 关卡完成事件（参数：是否成功）
    public Action<bool> OnLevelFinished;

    public bool canDrag = true; // 控制是否可以拖拽

    void Start()
    {
        SetLevel(1); // 默认第一关
    }

    // 切换关卡时调用，设置当前关卡的拖拽步骤
    public void SetLevel(int level)
    {
        switch (level)
        {
            case 1:
                dragSteps = level1Steps;
                break;
            case 2:
                dragSteps = level2Steps;
                break;
            case 3:
                dragSteps = level3Steps;
                break;
            default:
                dragSteps = new List<DragStep>();
                break;
        }
        // 记录初始状态
        foreach (var step in dragSteps)
        {
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
        // 记录所有可拖拽物体
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
        // TODO: 可在此处重置关卡相关状态
    }

    // 重置当前关卡所有物体和Trigger状态
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
        }
        // 恢复所有可拖拽物体
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
        if (currentStep >= dragSteps.Count)
        {
            // 关卡完成
            if (OnLevelFinished != null)
            {
                OnLevelFinished.Invoke(true);
                OnLevelFinished = null; // 防止多次调用
            }
            return;
        }

        if (!canDrag) return; // 新增：判断是否可以拖拽

        // 修改：所有模型都可被拖拽
        if (isMovingToTarget) return; // 正在自动移动，禁止操作

#if UNITY_EDITOR
        // 鼠标操作（编辑器测试用）
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // 判断是否为任意可拖拽物体
                if (draggableObjects.Contains(hit.transform.gameObject))
                {
                    selectedObject = hit.transform;
                    zCoord = Camera.main.WorldToScreenPoint(selectedObject.position).z;
                    Vector3 mousePoint = Input.mousePosition;
                    mousePoint.z = zCoord;
                    offset = selectedObject.position - Camera.main.ScreenToWorldPoint(mousePoint);
                }
            }
        }
        else if (Input.GetMouseButton(0) && selectedObject != null)
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = zCoord;
            selectedObject.position = Camera.main.ScreenToWorldPoint(mousePoint) + offset;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            selectedObject = null;
        }
#else
        // 触摸操作（Android等移动端）
        if (Input.touchCount > 0)
        {
            UnityEngine.Touch touch = Input.GetTouch(0);
            Vector3 touchPos = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touchPos);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (draggableObjects.Contains(hit.transform.gameObject))
                    {
                        selectedObject = hit.transform;
                        zCoord = Camera.main.WorldToScreenPoint(selectedObject.position).z;
                        touchPos.z = zCoord;
                        offset = selectedObject.position - Camera.main.ScreenToWorldPoint(touchPos);
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved && selectedObject != null)
            {
                touchPos.z = zCoord;
                selectedObject.position = Camera.main.ScreenToWorldPoint(touchPos) + offset;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                selectedObject = null;
            }
        }
#endif
    }

    void FixedUpdate()
    {
        if (currentStep >= dragSteps.Count) return;
        if (selectedObject == null) return;
        if (isMovingToTarget) return;

        var step = dragSteps[currentStep];
        Collider objCol = selectedObject.GetComponent<Collider>();
        if (objCol == null) return;

        // 只允许当前步骤模型与目标基础碰撞体吸附
        if (selectedObject.gameObject == step.draggableObject)
        {
            if (step.targetTrigger != null && objCol.bounds.Intersects(step.targetTrigger.bounds))
            {
                // 禁用当前目标Trigger，防止误触
                step.targetTrigger.enabled = false;
                // 自动移动到目标位置
                StartCoroutine(MoveToTarget(step.draggableObject.transform, step.targetPosition.position, step.targetPosition.rotation));
                // 拖拽正确提示
                if (AIChatManager.Instance != null)
                    AIChatManager.Instance.ShowDragRightChat();
            }
        }
        else
        {
            // 非当前步骤模型碰到当前步骤目标基础碰撞体，平滑回到初始位置
            if (step.targetTrigger != null && objCol.bounds.Intersects(step.targetTrigger.bounds))
            {
                StartCoroutine(MoveToInitialPosition(selectedObject));
                selectedObject = null;
                // 拖拽错误提示
                if (AIChatManager.Instance != null)
                    AIChatManager.Instance.ShowDragWrongChat();
            }
        }
    }

    // 新增：平滑将物体回到初始位置
    private IEnumerator MoveToInitialPosition(Transform obj)
    {
        isMovingToTarget = true;
        Vector3 startPos = obj.position;
        Quaternion startRot = obj.rotation;
        Vector3 targetPos = initialPositions.ContainsKey(obj.gameObject) ? initialPositions[obj.gameObject] : startPos;
        Quaternion targetRot = initialRotations.ContainsKey(obj.gameObject) ? initialRotations[obj.gameObject] : startRot;
        float t = 0;
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

    // 自动移动物体到目标位置与旋转
    private IEnumerator MoveToTarget(Transform obj, Vector3 targetPos, Quaternion targetRot)
    {
        isMovingToTarget = true;
        float t = 0;
        Vector3 startPos = obj.position;
        Quaternion startRot = obj.rotation;
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
        // 移除已吸附物体，不可再拖拽
        draggableObjects.Remove(obj.gameObject);
        currentStep++; // 允许下一个物体拖拽
        // 判断关卡是否完成
        if (currentStep >= dragSteps.Count && OnLevelFinished != null)
        {
            OnLevelFinished.Invoke(true);
            OnLevelFinished = null;
        }
        // TODO: 可在此处判断关卡是否完成并触发后续逻辑
    }
}
