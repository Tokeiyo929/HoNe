using UnityEngine;

public class SetChildrenTransparent : MonoBehaviour
{
    [Range(0, 1)]
    public float transparency = 0.5f; // 可调节的透明度值

    void Start()
    {
        MakeChildrenTransparent();
    }

    public void MakeChildrenTransparent()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                // 设置渲染模式为透明
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                // 设置颜色透明度
                Color color = material.color;
                color.a = transparency;
                material.color = color;
            }
        }
    }
}