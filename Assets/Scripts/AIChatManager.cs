using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 聊天面板管理器，支持内容配置和全局调用
/// </summary>
public class AIChatManager : MonoBehaviour
{
    public static AIChatManager Instance { get; private set; }

    [Header("聊天面板")]
    public GameObject chatPanel; // 聊天面板父物体
    public TextMeshProUGUI chatText; // 聊天内容文本

    //新增音频配置
    [Header("各界面聊天内容和音频")]
    [TextArea] public string selectPanelText = "欢迎来到选择界面，请选择你要挑战的关卡！";
    [SerializeField] AudioClip selectPanelAudio; // 选择界面音频
    [TextArea]
    public string[] levelStartTexts = new string[5] {
        "第一关：请按照提示顺序搭建模型，加油！",
        "第二关：挑战升级，注意细节！",
        "第三关：终极挑战，展现你的实力！",
        "第四关：全新挑战，发挥你的创造力！",
        "第五关：最终关卡，冲刺巅峰！"
    };
    [SerializeField]
    AudioClip[] levelStartAudios = new AudioClip[5];
    //新增关卡提示文本
    public string[] levelTipsTexts = new string[5] {
        "请将纤维环、髓核、软骨终板按正确的顺序及位置进行组装。",
        "请将腰椎结构的L1-L5,尾椎结构的S1-S5按正确的顺序及位置进行组装。",
        "请拖动骨骼结构将颈椎结构的C1-C8,胸椎结构的T1-T12按正确的顺序及位置进行组装。",
        "请拖动上方拼图碎片拼出整体的腰椎神经分布图。",
        "请拖动肌肉模型进行正确的肌肉组装。"
    };
    public AudioClip[] levelTipsAudios = new AudioClip[5];
    [TextArea] public string dragWrongText = "请按照顺序搭建！";
    [TextArea] public string dragRightText = "做的好，继续搭建剩余模型！";
    [TextArea] public string resultSuccessText = "恭喜你，完美达成了这次挑战！期待下一关你的表现！";
    [TextArea] public string resultFailText = "别灰心，失败只是暂时的，它是你优化策略、提升实力的最佳机会。要再试一次吗？";

    //新增音频组件
    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        // 单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    private void PlayAudio(AudioClip clip)
    {
        Debug.Log($"Attempting to play audio clip: {clip?.name ?? "null"}, currently playing: {audioSource.isPlaying}");
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Stopped current audio.");
        }
        audioSource.clip = clip;
        audioSource.Play();
        Debug.Log($"Started playing: {clip?.name ?? "null"}");
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// 显示聊天面板并设置内容
    /// </summary>
    public void ShowChat(string content)
    {
        if (chatPanel != null) chatPanel.SetActive(true);
        if (chatText != null) chatText.text = content;
    }
    //新增关卡提示
    public void ShowLevelTips(int level)
    {
        if (chatPanel != null) chatPanel.SetActive(true);
        if (chatText != null) chatText.text = levelTipsTexts[level - 1];
        PlayAudio(levelTipsAudios[level - 1]);
    }
    /// <summary>
    /// 隐藏聊天面板
    /// </summary>
    public void HideChat()
    {
        if (chatPanel != null) chatPanel.SetActive(false);
    }

    /// <summary>
    /// 显示选择界面聊天内容
    /// </summary>
    public void ShowSelectPanelChat()
    {
        ShowChat(selectPanelText);
        PlayAudio(selectPanelAudio); 
    }

    /// <summary>
    /// 显示关卡开始聊天内容
    /// </summary>
    public void ShowLevelStartChat(int level)
    {
        int idx = Mathf.Clamp(level - 1, 0, levelStartTexts.Length - 1);
        ShowChat(levelStartTexts[idx]);
        PlayAudio(levelStartAudios[idx]);
    }

    /// <summary>
    /// 拖拽错误提示
    /// </summary>
    public void ShowDragWrongChat()
    {
        ShowChat(dragWrongText);
    }

    /// <summary>
    /// 拖拽正确提示
    /// </summary>
    public void ShowDragRightChat()
    {
        ShowChat(dragRightText);
    }

    /// <summary>
    /// 挑战结果聊天内容
    /// </summary>
    public void ShowResultChat(bool success)
    {
        ShowChat(success ? resultSuccessText : resultFailText);
    }
    public void ShowResultChat(bool success, int level)
    {
        if (level == 5)
        {
            ShowChat("恭喜你，完美达成了这次挑战！本次挑战到此结束，您可以选择进入知识答题环节。");
            return;
        }
        ShowChat(success ? resultSuccessText : resultFailText);
    }

}
