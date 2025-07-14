using UnityEngine;
using DG.Tweening;
using UnityEngine.UI; // �����UIԪ��

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private Vector2 targetPosition; // Ŀ��λ��
    [SerializeField] private float scaleDuration = 0.5f; // ���Ŷ�������ʱ��
    [SerializeField] private float moveDuration = 1f; // �ƶ���������ʱ��

    private RectTransform rectTransform; // UI�����RectTransform
    private Vector2 originalPosition; // ԭʼλ��

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition; // ����ԭʼλ��
    }

    public void AnimatedUI()
    {
        // ����״̬
        rectTransform.anchoredPosition = new Vector2(0, -220); // ��Ļ����(����ê��������)
        rectTransform.localScale = Vector3.zero;

        // ������������
        Sequence sequence = DOTween.Sequence();

        // ��һ������0���ŵ�2
        sequence.Append(rectTransform.DOScale(1.5f, scaleDuration).SetEase(Ease.OutBack));

        // �ڶ������ƶ���Ŀ��λ�ò����ŵ�1
        sequence.Append(rectTransform.DOAnchorPos(targetPosition, moveDuration).SetEase(Ease.OutQuint));
        sequence.Join(rectTransform.DOScale(1f, moveDuration).SetEase(Ease.OutBack));

        // ��ѡ��������ɺ�ִ�еĲ���
        sequence.OnComplete(() => {
            // ������������ӻص���״̬����
        });
    }

    private void OnDisable()
    {
        // ���õ�ԭʼλ��
        rectTransform.anchoredPosition = originalPosition;
    }
}