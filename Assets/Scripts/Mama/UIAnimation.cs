using UnityEngine;
using DG.Tweening;
using UnityEngine.UI; // 如果是UI元素

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private Vector2 targetPosition; // 目标位置
    [SerializeField] private float scaleDuration = 0.5f; // 缩放动画持续时间
    [SerializeField] private float moveDuration = 1f; // 移动动画持续时间

    private RectTransform rectTransform; // UI对象的RectTransform
    private Vector2 originalPosition; // 原始位置

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition; // 保存原始位置
    }

    public void AnimatedUI()
    {
        // 重置状态
        rectTransform.anchoredPosition = new Vector2(0, -220); // 屏幕中心(假设锚点在中心)
        rectTransform.localScale = Vector3.zero;

        // 创建动画序列
        Sequence sequence = DOTween.Sequence();

        // 第一步：从0缩放到2
        sequence.Append(rectTransform.DOScale(1.5f, scaleDuration).SetEase(Ease.OutBack));

        // 第二步：移动到目标位置并缩放到1
        sequence.Append(rectTransform.DOAnchorPos(targetPosition, moveDuration).SetEase(Ease.OutQuint));
        sequence.Join(rectTransform.DOScale(1f, moveDuration).SetEase(Ease.OutBack));

        // 可选：动画完成后执行的操作
        sequence.OnComplete(() => {
            // 可以在这里添加回调或状态设置
        });
    }

    private void OnDisable()
    {
        // 重置到原始位置
        rectTransform.anchoredPosition = originalPosition;
    }
}