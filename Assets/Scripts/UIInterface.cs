using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UIInterface;

public class UIInterface : MonoBehaviour
{
    [Header("界面面板")]
    public GameObject startPanel;         // 开始界面
    public GameObject levelSelectPanel;   // 选择关卡界面
    public GameObject quizPanel;          // 知识答题界面
    public GameObject quizMainPanel;      // 答题主界面（quizPanel子对象）
    public GameObject quizScorePanel;     // 得分界面（quizPanel子对象）

    [Header("关卡物体集合")]
    public GameObject level1Objects;      // 第一关物体父节点
    public GameObject level2Objects;      // 第二关物体父节点
    public GameObject level3Objects;      // 第三关物体父节点
    public GameObject level4Objects;      // 第四关物体父节点
    public GameObject level5Objects;      // 第五关物体父节点
    public GameObject level4ObjectsAlt;

    [Header("按钮")]
    public Button startGameButton;        // 进入游戏按钮
    public Button exitGameButton;         // 退出游戏按钮
    public Button level1Button;           // 第一关按钮
    public Button level2Button;           // 第二关按钮
    public Button level3Button;           // 第三关按钮
    public Button level4Button;           // 第四关按钮
    public Button level5Button;           // 第五关按钮
    public Button quizButton;             // 答题按钮
    public Button backButton;             // 返回按钮

    // ===== 答题系统相关 =====
    [Header("答题UI组件")]
    public TextMeshProUGUI questionTitleText;           // 题目标题文本
    public Toggle[] optionToggles;                      // 选项Toggle数组
    public Text[] optionTexts;                          // 选项文本数组
    public Button confirmOrNextButton;                  // 确认/下一题按钮
    public TextMeshProUGUI confirmOrNextButtonText;     // 按钮文本
    public TextMeshProUGUI tipText;                     // 提示文本

    [Header("得分界面")]
    public GameObject scorePanel;                  // 得分界面父物体
    public TextMeshProUGUI scoreText;              // 得分文本
     public Button scoreBackButton;                 // 得分界面返回按钮

    [Header("挑战结果界面")]
    public GameObject resultPanel;                // 挑战结果界面
    public TextMeshProUGUI resultTitleText;       // 结果标题文本
    public Button resultBackButton;               // 返回选择界面按钮
    public Button resultNextOrRetryButton;        // 下一关/重新挑战/知识答题按钮
    public TextMeshProUGUI resultNextOrRetryText; // 按钮文本
    public Button viewModelButton;                // 新增：查看搭建完的模型按钮

    [Header("挑战流程界面")]
    public Button startChallengeButton;    // 开始挑战按钮
    public GameObject countdownPanel;      // 倒计时界面
    public TextMeshProUGUI countdownText;  // 倒计时文本
    public GameObject knowledgePanel;      // 知识界面
    public Image knowledgeImage; // 知识界面图片
    public TextMeshProUGUI knowledgeIntroText;  // 知识界面介绍文本

    [System.Serializable]
    public class KnowledgeInfo
    {
        public Sprite image;
        [TextArea]
        public string intro;
    }
    [Header("关卡知识内容")]
    public KnowledgeInfo[] knowledgeInfos = new KnowledgeInfo[3]; // 0:第一关 1:第二关 2:第三关

    [Header("关卡答题界面（第四/五关）")] // 合并为通用答题界面组件
    public GameObject levelQuizPanel;      // 通用答题界面
    public TextMeshProUGUI levelQuizQuestionText; // 通用题目文本
    public Toggle[] levelQuizOptionToggles;    // 通用选项Toggle数组
    public Button levelQuizConfirmButton;      // 通用确认按钮
    public TextMeshProUGUI levelQuizTipText;   // 通用提示文本

    // 题库结构
    [System.Serializable]
    public class Option
    {
        public string text;      // 选项内容
        public bool isAnswer;    // 是否为正确答案
    }

    [System.Serializable]
    public class Question
    {
        public string title;             // 题目标题
        public List<Option> options;     // 选项列表
        public bool isMultiple;          // 是否多选题
    }

    //[Header("题库自定义")]
    //public List<Question> questionBank = new List<Question>();
    [System.Serializable]
    public class LevelQuestionBank
    {
        public List<Question> questions = new List<Question>();
    }

    [Header("分关卡题库")]
    public List<LevelQuestionBank> levelQuestionBanks = new List<LevelQuestionBank>();

    [Header("拖拽关卡控制")]
    public Touch touchScript; // 拖拽脚本引用

    [Header("倒计时设置")]
    [Tooltip("第一关知识面板倒计时（秒）")]
    public int knowledgeCountdownSeconds1 = 10;
    [Tooltip("第二关知识面板倒计时（秒）")]
    public int knowledgeCountdownSeconds2 = 10;
    [Tooltip("第三关知识面板倒计时（秒）")]
    public int knowledgeCountdownSeconds3 = 10;
    [Tooltip("第四关知识面板倒计时（秒）")]
    public int knowledgeCountdownSeconds4 = 10;
    [Tooltip("第五关知识面板倒计时（秒）")]
    public int knowledgeCountdownSeconds5 = 10;
    [Tooltip("第一关倒计时（秒）")]
    public int level1CountdownSeconds = 60;
    [Tooltip("第二关倒计时（秒）")]
    public int level2CountdownSeconds = 180;
    [Tooltip("第三关倒计时（秒）")]
    public int level3CountdownSeconds = 300;
    [Tooltip("第四关倒计时（秒）")]
    public int level4CountdownSeconds = 300;
    [Tooltip("第五关倒计时（秒）")]
    public int level5CountdownSeconds = 300;
    // 移除第四关和第五关倒计时

    private int currentQuestionIndex = 0;
    private bool isAnswering = true; // true: 等待确认，false: 等待下一题
    private int correctCount = 0;                  // 答对题数
    private int totalScore = 0;                    // 总分
    private int currentLevel = 0; // 当前关卡编号
    private int pendingLevel = 0; // 记录待进入的关卡编号
    private Coroutine countdownCoroutine;
    private Coroutine levelCountdownCoroutine; // 关卡倒计时协程
    private bool isViewingModel = false;          // 新增：是否处于模型查看模式
    private Vector2 lastPointerPos;               // 新增：记录上一次指针位置

    [Header("第四关/第五关物体高亮控制")]
    public ObjectShow level4ObjectShow; // 拖到Inspector
    public ObjectShow level5ObjectShow; // 拖到Inspector

    // 新增：记录第四/五关模型初始位置
    private Vector3 level4InitialPos;
    private Vector3 level5InitialPos;
    private Coroutine level4MoveCoroutine;
    private Coroutine level5MoveCoroutine;

    [Header("关卡显示文本")]
    public TextMeshProUGUI currentLevelText; // 新增：当前关卡显示文本

    //新增关卡结束音效
    [Header("soundFeedback")]
    [SerializeField] private AudioClip completedSound1;
    [SerializeField] private AudioClip completedSound2;
    [SerializeField] private AudioClip completedSound3;
    [SerializeField] private AudioClip failureSound1;
    [SerializeField] private AudioClip failureSound2;

    //新增音频组件
    [SerializeField] private AudioSource audioSource;

    //新增成绩单功能
    [Header("成绩单界面")]
    public GameObject reportCardPanel;         // 成绩单面板
    public Transform reportCardContent;        // 成绩单内容父物体
    public GameObject questionResultPrefab;    // 每道题结果的预制体
    public TextMeshProUGUI totalScoreText;     // 总得分文本
    public TextMeshProUGUI accuracyText;       // 正确率文本
    public Button reportCardBackButton;        // 成绩单返回按钮

    [System.Serializable]
    public class QuestionResult
    {
        public int level;            // 关卡数字(1-5)
        public string levelName;     // 关卡名称(第X关)
        public int questionIndex;    // 题目索引(0-9)
        public string questionText;
        public List<string> userAnswers;
        public List<string> correctAnswers;
        public bool isCorrect;
        public int score;            // 固定每题10分
    }

    private List<QuestionResult> allQuestionResults = new List<QuestionResult>();

    public TMP_FontAsset sourceHanSansFont;


    void Start()
    {
        HideAllLevels();
        ShowStartPanel();
        if (knowledgePanel != null) knowledgePanel.SetActive(false);
        if (countdownPanel != null) countdownPanel.SetActive(false);
        if (levelQuizPanel != null) levelQuizPanel.SetActive(false); // 默认隐藏关卡答题界面

        // 按钮事件绑定
        if (startGameButton != null)
            startGameButton.onClick.AddListener(ShowLevelSelectPanel);
        if (exitGameButton != null)
            exitGameButton.onClick.AddListener(ExitGame);
        if (level1Button != null)
            level1Button.onClick.AddListener(() => EnterLevel(1));
        if (level2Button != null)
            level2Button.onClick.AddListener(() => EnterLevel(2));
        if (level3Button != null)
            level3Button.onClick.AddListener(() => EnterLevel(3));
        if (level4Button != null)
            level4Button.onClick.AddListener(() => EnterLevel(4));
        if (level5Button != null)
            level5Button.onClick.AddListener(() => EnterLevel(5));
        //新增：绑定按钮直接到分数界面
        if (reportCardBackButton != null)
            reportCardBackButton.onClick.AddListener(OnReportCardBack);
        if (quizButton != null)
            quizButton.onClick.AddListener(ShowReportCard);
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
        if (confirmOrNextButton != null)
            confirmOrNextButton.onClick.AddListener(OnConfirmOrNext);
        if (scoreBackButton != null)
            scoreBackButton.onClick.AddListener(OnScoreBack);
        if (resultBackButton != null)
            resultBackButton.onClick.AddListener(OnResultBack);
        if (resultNextOrRetryButton != null)
            resultNextOrRetryButton.onClick.AddListener(OnResultNextOrRetry);
        if (startChallengeButton != null)
            startChallengeButton.onClick.AddListener(OnStartChallengeButton);
        if (viewModelButton != null)
            viewModelButton.onClick.AddListener(OnViewModelButton);

        // 新增：绑定通用关卡答题按钮事件
        if (levelQuizConfirmButton != null)
            levelQuizConfirmButton.onClick.AddListener(HandleLevelQuizAnswer);
        // 记录第四/五关模型初始位置
        if (level4Objects != null) level4InitialPos = level4Objects.transform.position;
        if (level5Objects != null) level5InitialPos = level5Objects.transform.position;
    }

    // 显示开始界面，隐藏其他界面和关卡物体，隐藏返回按钮
    void ShowStartPanel()
    {
        SetUIVisible(startPanel, true);
        SetUIVisible(levelSelectPanel, false);
        SetUIVisible(quizPanel, false);
        HideAllLevels();
        if (backButton != null) backButton.gameObject.SetActive(false);
        // 新增：退出到开始界面时清空关卡显示文本
        UpdateCurrentLevelText(0);
    }

    // 显示选择关卡界面，隐藏其他界面和关卡物体，显示返回按钮
    void ShowLevelSelectPanel()
    {
        SetUIVisible(startPanel, false);
        SetUIVisible(levelSelectPanel, true);
        SetUIVisible(quizPanel, false);
        HideAllLevels();
        if (backButton != null) backButton.gameObject.SetActive(true);
        // 显示选择界面聊天内容
        if (AIChatManager.Instance != null)
            AIChatManager.Instance.ShowSelectPanelChat();
    }

    // 显示答题界面，隐藏其他界面和关卡物体，显示返回按钮
    void ShowQuizPanel()
    {
        // 隐藏聊天面板
        if (AIChatManager.Instance != null)
            AIChatManager.Instance.HideChat();

        SetUIVisible(startPanel, false);
        SetUIVisible(levelSelectPanel, false);
        SetUIVisible(quizPanel, true);
        SetUIVisible(quizMainPanel, true);
        SetUIVisible(quizScorePanel, false);
        SetUIVisible(scorePanel, false); // 兼容旧逻辑，可移除
        //新增注释这段代码
        //HideAllLevels();
        if (backButton != null) backButton.gameObject.SetActive(true);
        // 初始化答题系统
        if (currentLevel >= 1 && currentLevel <= levelQuestionBanks.Count && levelQuestionBanks[currentLevel - 1].questions.Count > 0)
        {
            currentQuestionIndex = 0;
            isAnswering = true;
            ShowQuestion(currentQuestionIndex);
        }
        correctCount = 0;
        totalScore = 0;
    }

    // 显示题目
    void ShowQuestion(int index)
    {
        if (currentLevel < 1 || currentLevel > levelQuestionBanks.Count) return;
        var currentBank = levelQuestionBanks[currentLevel - 1]; // 关卡1对应索引0

        if (index < 0 || index >= currentBank.questions.Count) return;
        var q = currentBank.questions[index];
        if (questionTitleText != null) questionTitleText.text = q.title;
        for (int i = 0; i < optionToggles.Length; i++)
        {
            if (i < q.options.Count)
            {
                optionToggles[i].gameObject.SetActive(true);
                optionToggles[i].isOn = false;
                optionToggles[i].interactable = true;
                optionTexts[i].text = q.options[i].text;
                optionTexts[i].color = Color.white; // 初始为白色
            }
            else
            {
                optionToggles[i].gameObject.SetActive(false);
            }
        }
        // 单选题：只允许选择一个
        if (!q.isMultiple)
        {
            for (int i = 0; i < optionToggles.Length; i++)
            {
                int idx = i;
                optionToggles[i].onValueChanged.RemoveAllListeners();
                optionToggles[i].onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        for (int j = 0; j < optionToggles.Length; j++)
                        {
                            if (j != idx) optionToggles[j].isOn = false;
                        }
                    }
                });
            }
        }
        else
        {
            // 多选题：允许多选
            for (int i = 0; i < optionToggles.Length; i++)
            {
                optionToggles[i].onValueChanged.RemoveAllListeners();
            }
        }
        if (confirmOrNextButtonText != null) confirmOrNextButtonText.text = "确认";
        if (tipText != null)
        {
            tipText.text = "";
            tipText.color = Color.black;
        }
        isAnswering = true;
    }

    // 确认/下一题按钮逻辑
    // 修改OnConfirmOrNext方法中的答题记录部分
    void OnConfirmOrNext()
    {
        if (currentLevel < 1 || currentLevel > levelQuestionBanks.Count) return;
        var currentBank = levelQuestionBanks[currentLevel - 1]; // 修正索引

        if (currentBank.questions.Count == 0) return;
        var currentQuestion = currentBank.questions[currentQuestionIndex]; // 使用currentQuestion替代question

        if (isAnswering)
        {
            // 记录用户答案和正确答案
            List<string> userAnswers = new List<string>();
            List<string> correctAnswers = new List<string>();

            for (int i = 0; i < currentQuestion.options.Count; i++)
            {
                if (currentQuestion.options[i].isAnswer)
                    correctAnswers.Add(currentQuestion.options[i].text);

                if (optionToggles[i].isOn)
                    userAnswers.Add(currentQuestion.options[i].text); // 使用currentQuestion
            }

            // 判断是否正确
            bool isCorrect = true;
            for (int i = 0; i < currentQuestion.options.Count; i++)
            {
                if (currentQuestion.options[i].isAnswer != optionToggles[i].isOn)
                {
                    isCorrect = false;
                    break;
                }
            }

            // 记录答题结果 - 每题固定10分
            var result = new QuestionResult
            {
                level = currentLevel,
                levelName = $"第{currentLevel}关",
                questionIndex = currentQuestionIndex,
                questionText = currentQuestion.title, // 使用currentQuestion
                userAnswers = userAnswers,
                correctAnswers = correctAnswers,
                isCorrect = isCorrect,
                score = isCorrect ? 10 : 0  // 每题固定10分
            };

            allQuestionResults.Add(result);

            // 更新UI反馈
            if (isCorrect)
            {
                if (tipText != null)
                {
                    tipText.text = "回答正确！";
                    tipText.color = Color.green;
                }
                correctCount++;
            }
            else
            {
                if (tipText != null)
                {
                    tipText.text = "回答错误，正确答案已经显示";
                    tipText.color = Color.red;
                }

                // 显示正确答案
                for (int i = 0; i < currentQuestion.options.Count; i++)
                {
                    optionToggles[i].isOn = currentQuestion.options[i].isAnswer;
                    if (optionTexts[i] != null)
                    {
                        optionTexts[i].color = currentQuestion.options[i].isAnswer ? Color.green : Color.white;
                    }
                }
            }

            // 禁用所有Toggle
            foreach (var toggle in optionToggles)
            {
                if (toggle != null) toggle.interactable = false;
            }

            // 更新按钮文本
            if (confirmOrNextButtonText != null)
            {
                confirmOrNextButtonText.text = currentQuestionIndex == currentBank.questions.Count - 1 ?
                    "提交试卷" : "下一题";
            }

            isAnswering = false;
        }
        else
        {
            // 下一题或提交试卷
            if (currentQuestionIndex < currentBank.questions.Count - 1)
            {
                currentQuestionIndex++;
                ShowQuestion(currentQuestionIndex);
            }
            else
            {
                SetUIVisible(quizPanel, false);
                ShowResultPanel(true);
            }
        }
    }
    void OnReportCardBack()
    {
        SetUIVisible(reportCardPanel, false);
        ShowLevelSelectPanel();
    }
    // 显示得分界面
    void ShowReportCard()
    {
        // 隐藏不需要的面板
        SetUIVisible(quizPanel, true);
        SetUIVisible(quizMainPanel, false);
        SetUIVisible(reportCardPanel, true);

        // 计算总分和正确率
        int totalScore = 0;
        int correctCount = 0;

        foreach (var result in allQuestionResults)
        {
            totalScore += result.score;
            if (result.isCorrect) correctCount++;
        }

        float accuracy = allQuestionResults.Count > 0 ?
            (float)correctCount / allQuestionResults.Count * 100f : 0f;

        // 更新总分显示
        totalScoreText?.SetText($"总得分: {totalScore}/100");
        accuracyText?.SetText($"正确率: {accuracy:F1}%");

        // 清空旧内容
        foreach (Transform child in reportCardContent)
            Destroy(child.gameObject);

        // 按关卡分组显示
        for (int level = 1; level <= 5; level++)
        {
            var levelResults = allQuestionResults.FindAll(r => r.level == level);
            if (levelResults.Count == 0) continue;

            // 添加关卡标题
            var header = new GameObject($"Level{level}Header", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            header.transform.SetParent(reportCardContent, false);

            // 设置 RectTransform
            RectTransform rect = header.GetComponent<RectTransform>();
            rect.localScale = Vector3.one;

            // 设置 Text 属性
            var headerText = header.GetComponent<TextMeshProUGUI>();
            headerText.font = sourceHanSansFont; // 使用Source Han Sans字体
            headerText.text = $"第{level}关";
            headerText.fontSize = 25;
            headerText.fontStyle = FontStyles.Bold;
            headerText.color = new Color(0.2f, 0.4f, 0.8f);
            headerText.alignment = TextAlignmentOptions.Left;

            // 设置 LayoutElement 高度为 50
            var layout = header.GetComponent<LayoutElement>();
            layout.preferredHeight = 50;
            layout.flexibleHeight = 0;



            // 添加该关卡的所有题目
            foreach (var result in levelResults.OrderBy(r => r.questionIndex))
            {
                var item = Instantiate(questionResultPrefab, reportCardContent);

                // 设置题目文本 (题号从1开始)
                // 设置题目文本 (题号从1开始)
                item.transform.Find("QuestionText").GetComponent<TextMeshProUGUI>().text =
                    $"Q{result.questionIndex + 1}: {result.questionText}";

                // 设置 ABCD 选项文本（前提是 prefab 中有 OptionAText 等）
                var levelQuestions = levelQuestionBanks[result.level - 1].questions;
                if (result.questionIndex < levelQuestions.Count)
                {
                    var options = levelQuestions[result.questionIndex].options;

                    // A
                    var aText = item.transform.Find("OptionAText")?.GetComponent<TextMeshProUGUI>();
                    if (aText != null && options.Count > 0) aText.text = $"A. {options[0].text}";

                    // B
                    var bText = item.transform.Find("OptionBText")?.GetComponent<TextMeshProUGUI>();
                    if (bText != null && options.Count > 1) bText.text = $"B. {options[1].text}";

                    // C
                    var cText = item.transform.Find("OptionCText")?.GetComponent<TextMeshProUGUI>();
                    if (cText != null && options.Count > 2) cText.text = $"C. {options[2].text}";

                    // D
                    var dText = item.transform.Find("OptionDText")?.GetComponent<TextMeshProUGUI>();
                    if (dText != null && options.Count > 3) dText.text = $"D. {options[3].text}";
                }


                // 设置答案对比
                var userAnswerText = item.transform.Find("UserAnswerText").GetComponent<TextMeshProUGUI>();
                var correctAnswerText = item.transform.Find("CorrectAnswerText").GetComponent<TextMeshProUGUI>();

                userAnswerText.text = "你的答案: " + (result.userAnswers.Count > 0 ?
                    string.Join("; ", result.userAnswers) : "未作答");
                correctAnswerText.text = "正确答案: " + string.Join("; ", result.correctAnswers);

                // 设置结果标记
                var resultMark = item.transform.Find("ResultMark").GetComponent<TextMeshProUGUI>();
                if (result.isCorrect)
                {
                    resultMark.text = "✓ 正确";
                    resultMark.color = Color.green;
                }
                else
                {
                    resultMark.text = "× 错误";
                    resultMark.color = Color.red;
                }

                // 设置得分 (每题固定10分)
                item.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text =
                    $"{result.score}分";
            }
        }

        // 滚动到顶部
        Canvas.ForceUpdateCanvases();
        reportCardContent.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
    }

    // 得分界面返回按钮
    void OnScoreBack()
    {
        SetUIVisible(scorePanel, false);
        ShowLevelSelectPanel();
    }

    // 工具方法：设置UI显隐
    void SetUIVisible(GameObject go, bool visible)
    {
        if (go == null) return;
        var cg = go.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = visible ? 1 : 0;
            cg.interactable = visible;
            cg.blocksRaycasts = visible;
        }
        else
        {
            go.SetActive(visible); // 兼容未加CanvasGroup的情况
        }
    }

    // 工具方法：设置3D物体显隐
    void SetObjectVisible(GameObject go, bool visible)
    {
        if (go == null) return;
        var renderers = go.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
            r.enabled = visible;
        // 若有交互组件可按需处理
    }

    // 优化后的隐藏所有关卡物体
    void HideAllLevels()
    {
        SetObjectVisible(level1Objects, false);
        SetObjectVisible(level2Objects, false);
        SetObjectVisible(level3Objects, false);
        // 新增：隐藏第四关和第五关物体
        SetObjectVisible(level4Objects, false);
        SetObjectVisible(level5Objects, false);
    }

    // 进入指定关卡，显示知识界面，倒计时结束后出现开始挑战按钮
    public void EnterLevel(int level)
    {
        allQuestionResults.RemoveAll(r => r.level == level);

        // 隐藏聊天面板
        if (AIChatManager.Instance != null)
            AIChatManager.Instance.HideChat();

        SetUIVisible(startPanel, false);
        SetUIVisible(levelSelectPanel, false);
        SetUIVisible(quizPanel, false);
        SetUIVisible(resultPanel, false);
        HideAllLevels();
        if (backButton != null) backButton.gameObject.SetActive(false);
        currentLevel = level;
        pendingLevel = level;
        // 新增：更新关卡显示文本
        UpdateCurrentLevelText(level);
        ShowKnowledgePanel(level);
        // 隐藏前三关模型
        if (level1Objects != null) level1Objects.SetActive(false);
        if (level2Objects != null) level2Objects.SetActive(false);
        if (level3Objects != null) level3Objects.SetActive(false);
        // 新增：隐藏第四五关模型
        if (level4Objects != null) level4Objects.SetActive(false);
        if (level5Objects != null) level5Objects.SetActive(false);
        // 新增：每次进入关卡时重置关卡状态并启用拖拽
        if (touchScript != null)
        {
            touchScript.ResetLevel();
            touchScript.enabled = true;
        }
        // Debug.Log($"准备进入关卡 {level}，显示知识界面");
        // 显示关卡开始聊天内容
        if (AIChatManager.Instance != null)
            AIChatManager.Instance.ShowLevelStartChat(level);
    }

    // 新增：协程移动z轴到目标
    private IEnumerator MoveZToTarget(Transform obj, float targetZ, float duration)
    {
        Debug.Log($"开始移动 {obj.name} 到 z={targetZ}，持续时间 {duration} 秒");
        if (obj == null) yield break;
        Vector3 start = obj.position;
        Vector3 end = new Vector3(start.x, start.y, targetZ);
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            obj.position = new Vector3(start.x, start.y, Mathf.Lerp(start.z, targetZ, lerp));
            yield return null;
        }
        obj.position = end;
    }

    // 显示知识界面
    private void ShowKnowledgePanel(int level)
    {
        if (knowledgePanel != null) knowledgePanel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
        // 设置图片和文本
        int idx = Mathf.Clamp(level - 1, 0, knowledgeInfos.Length - 1);
        if (knowledgeInfos != null && knowledgeInfos.Length > idx)
        {
            if (knowledgeImage != null) knowledgeImage.sprite = knowledgeInfos[idx].image;
            if (knowledgeIntroText != null) knowledgeIntroText.text = knowledgeInfos[idx].intro;
        }
        else
        {
            if (knowledgeImage != null) knowledgeImage.sprite = null;
            if (knowledgeIntroText != null) knowledgeIntroText.text = "";
        }
        // 隐藏开始挑战按钮，倒计时结束后再显示
        if (startChallengeButton != null) startChallengeButton.gameObject.SetActive(false);
        // 启动倒计时（每关单独设置）
        if (countdownPanel != null) countdownPanel.SetActive(false);
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        //注释倒计时代码
        //int countdown = 10;
        //switch (level)
        //{
        //    case 1: countdown = knowledgeCountdownSeconds1 > 0 ? knowledgeCountdownSeconds1 : 10; break;
        //    case 2: countdown = knowledgeCountdownSeconds2 > 0 ? knowledgeCountdownSeconds2 : 10; break;
        //    case 3: countdown = knowledgeCountdownSeconds3 > 0 ? knowledgeCountdownSeconds3 : 10; break;
        //    case 4: countdown = knowledgeCountdownSeconds4 > 0 ? knowledgeCountdownSeconds4 : 10; break;
        //    case 5: countdown = knowledgeCountdownSeconds5 > 0 ? knowledgeCountdownSeconds5 : 10; break;
        //}
        //countdownCoroutine = StartCoroutine(KnowledgeCountdownAndShowStartButton(countdown));
        countdownCoroutine = StartCoroutine(KnowledgeCountdownAndShowStartButton(0));
    }

    // 新增：知识面板倒计时，结束后显示开始挑战按钮
    private IEnumerator KnowledgeCountdownAndShowStartButton(int seconds)
    {
        if (countdownPanel != null) countdownPanel.SetActive(true);
        int timeLeft = seconds;
        while (timeLeft > 0)
        {
            if (countdownText != null)
                countdownText.text = $"{timeLeft}";
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }
        //yield return new WaitForSeconds(0.5f);
        if (countdownPanel != null) countdownPanel.SetActive(false);
        // 倒计时结束后显示开始挑战按钮
        if (startChallengeButton != null) startChallengeButton.gameObject.SetActive(true);
    }

    // 开始挑战按钮点击
    private void OnStartChallengeButton()
    {
        if (knowledgePanel != null) knowledgePanel.SetActive(false);
        if (startChallengeButton != null) startChallengeButton.gameObject.SetActive(false);

        // 隐藏聊天面板
        if (AIChatManager.Instance != null)
            AIChatManager.Instance.HideChat();

        // 合并：第四/五关显示通用答题界面
        //if (pendingLevel == 4)
        //{
        //    // 新增：显示对应关卡物体集合并确保Renderer启用
        //    if (pendingLevel == 4 && level4Objects != null)
        //    {
        //        level4Objects.SetActive(true);
        //        SetObjectVisible(level4Objects, true); // 确保所有Renderer启用
        //        // 开始关卡挑战后，对应关卡模型位置移动效果
        //        level4Objects.transform.position = level4InitialPos;
        //        if (level4MoveCoroutine != null) StopCoroutine(level4MoveCoroutine);
        //        level4MoveCoroutine = StartCoroutine(MoveZToTarget(level4Objects.transform, -8.18f, 2.0f));
        //    }

        //    if (levelQuizPanel != null)
        //    {
        //        levelQuizPanel.SetActive(true);
        //        currentQuizIndex = 0;
        //        ShowLevelQuizPanel(pendingLevel);
        //    }
        //    return;
        //}
        //if (pendingLevel == 5)
        //{
        //    if (pendingLevel == 5 && level5Objects != null)
        //    {
        //        level5Objects.SetActive(true);
        //        SetObjectVisible(level5Objects, true); // 确保所有Renderer启用
        //        level5Objects.transform.position = level5InitialPos;
        //        if (level5MoveCoroutine != null) StopCoroutine(level5MoveCoroutine);
        //        level5MoveCoroutine = StartCoroutine(MoveZToTarget(level5Objects.transform, -7f, 2.0f));
        //    }
        //    //if (levelQuizPanel != null)
        //    //{
        //    //    levelQuizPanel.SetActive(true);
        //    //    currentQuizIndex = 0;
        //    //    ShowLevelQuizPanel(pendingLevel);
        //    //}
        //    if (backButton != null) backButton.gameObject.SetActive(false);
        //    ShowLevelQuizResultPanel(5);
        //    return;
        //}

        // 原有逻辑处理前三关
        if (pendingLevel == 1 || pendingLevel == 2 || pendingLevel == 3 || pendingLevel == 5 || pendingLevel == 4)
        {
            StartLevelCountdown(pendingLevel);
        }

        // 新增：开始挑战时显示当前关卡文本
        UpdateCurrentLevelText(currentLevel);
    }

    // 启动关卡倒计时
    private void StartLevelCountdown(int level)
    {
        int levelSeconds = level1CountdownSeconds;
        if (level == 2) levelSeconds = level2CountdownSeconds;
        else if (level == 3) levelSeconds = level3CountdownSeconds;
        else if (level == 4) levelSeconds = level4CountdownSeconds;
        else if (level == 5) levelSeconds = level5CountdownSeconds;

        if (levelCountdownCoroutine != null) StopCoroutine(levelCountdownCoroutine);
        levelCountdownCoroutine = StartCoroutine(LevelCountdown(levelSeconds));
        // 显示关卡模型
        ShowLevelModel(level);
    }

    // 关卡倒计时协程
    private IEnumerator LevelCountdown(int seconds)
    {
        if (countdownPanel != null) countdownPanel.SetActive(true);
        //播放倒计时动画
        countdownPanel.GetComponent<UIAnimation>().AnimatedUI();
        int timeLeft = seconds;
        while (timeLeft > 0)
        {
            if (countdownText != null)
                countdownText.text = $"{timeLeft}";
            yield return new WaitForSeconds(1f);
            timeLeft--;
            // 若关卡已完成则提前退出
            if (!IsLevelModelActive()) yield break;
        }
        // 倒计时结束，若还未完成则判定失败
        if (IsLevelModelActive())
        {
            if (countdownPanel != null) countdownPanel.SetActive(false);
            ShowResultPanel(false);
        }
    }

    // 判断当前是否还在关卡拖拽界面
    private bool IsLevelModelActive()
    {
        return (level1Objects != null && level1Objects.activeSelf) ||
               (level2Objects != null && level2Objects.activeSelf) ||
               (level3Objects != null && level3Objects.activeSelf) ||
               (level4Objects != null && level4Objects.activeSelf) ||
               (level5Objects != null && level5Objects.activeSelf);
    }

    // 修改ShowLevelModel，关卡完成时关闭倒计时面板
    private void ShowLevelModel(int level)
    {
        switch (level)
        {
            case 1:
                if (level1Objects != null) level1Objects.SetActive(true);
                SetObjectVisible(level1Objects, true);
                break;
            case 2:
                if (level2Objects != null) level2Objects.SetActive(true);
                SetObjectVisible(level2Objects, true);
                break;
            case 3:
                if (level3Objects != null) level3Objects.SetActive(true);
                SetObjectVisible(level3Objects, true);
                break;
            case 4:
                if (level4Objects != null) level4Objects.SetActive(true);
                SetObjectVisible(level4Objects, true);
                break;
            case 5:
                if (level5Objects != null) level5Objects.SetActive(true);
                SetObjectVisible(level5Objects, true);
                break;
        }
        Debug.Log($"正式进入关卡 {level}");
        //新增关卡提示
        if (AIChatManager.Instance != null)
            AIChatManager.Instance.ShowLevelTips(level);
        // 初始化关卡拖拽逻辑
        if (touchScript != null)
        {
            touchScript.SetLevel(level);
            touchScript.ResetLevel();
            touchScript.OnLevelFinished = (success) =>
            {
                // 关卡完成时关闭倒计时面板和协程
                if (levelCountdownCoroutine != null) StopCoroutine(levelCountdownCoroutine);
                if (countdownPanel != null) countdownPanel.SetActive(false);
                //修改：胜利直接跳答题面板
                //ShowResultPanel(success);
                ShowQuizPanel();
                //暂时注释并等待新的位置
                // 显示挑战结果聊天内容
                //if (AIChatManager.Instance != null)
                //    AIChatManager.Instance.ShowResultChat(success, level);
            };
        }
    }

    // 返回按钮逻辑
    private void OnBackButton()
    {
        // 新增：模型查看模式下，返回直接回到选择界面
        if (isViewingModel)
        {
            isViewingModel = false;
            // 隐藏关卡物体
            if (level1Objects != null) level1Objects.SetActive(false);
            if (level2Objects != null) level2Objects.SetActive(false);
            if (level3Objects != null) level3Objects.SetActive(false);
            if (level4Objects != null) level4Objects.SetActive(false);
            if (level5Objects != null) level5Objects.SetActive(false);
            if(level4ObjectsAlt != null) level4ObjectsAlt.SetActive(false);
            // 复原当前关卡物体集合的旋转
            GameObject targetObj = null;
            if (currentLevel == 1) targetObj = level1Objects;
            else if (currentLevel == 2) targetObj = level2Objects;
            else if (currentLevel == 3) targetObj = level3Objects;
            else if (currentLevel == 4) targetObj = level4ObjectsAlt;
            else if (currentLevel == 5) targetObj = level5Objects;
            if (targetObj != null)
                targetObj.transform.rotation = originalModelRotation;
            // 隐藏倒计时、知识、挑战流程等界面
            if (countdownPanel != null) countdownPanel.SetActive(false);
            if (knowledgePanel != null) knowledgePanel.SetActive(false);
            // 新增：重置关卡状态并启用拖拽
            if (touchScript != null)
            {
                touchScript.ResetLevel();
                touchScript.enabled = true;
            }
            // 返回选择界面
            ShowLevelSelectPanel();
            return;
        }

        // 新增：第四/五关知识面板/倒计时/流程时也能返回
        if ((pendingLevel == 4 || pendingLevel == 5) &&
            (
                (knowledgePanel != null && knowledgePanel.activeSelf) ||
                (countdownPanel != null && countdownPanel.activeSelf)
            )
            )
        {
            // 关闭所有流程相关界面
            if (knowledgePanel != null) knowledgePanel.SetActive(false);
            if (countdownPanel != null) countdownPanel.SetActive(false);

            // 停止所有倒计时协程
            if (countdownCoroutine != null) { StopCoroutine(countdownCoroutine); countdownCoroutine = null; }
            if (levelCountdownCoroutine != null) { StopCoroutine(levelCountdownCoroutine); levelCountdownCoroutine = null; }
            Debug.Log("返回选择界面，关闭知识面板和倒计时面板");
            ShowLevelSelectPanel();
            return;
        }

        // 在知识界面或倒计时界面时也能返回
        if ((knowledgePanel != null && knowledgePanel.activeSelf) ||
            (countdownPanel != null && countdownPanel.activeSelf) ||
            (level1Objects != null && level1Objects.activeSelf) ||
            (level2Objects != null && level2Objects.activeSelf) ||
            (level3Objects != null && level3Objects.activeSelf) ||
            (quizPanel != null && quizPanel.activeSelf))
        {
            // 关闭所有流程相关界面
            if (knowledgePanel != null) knowledgePanel.SetActive(false);
            if (countdownPanel != null) countdownPanel.SetActive(false);
            if (level1Objects != null) level1Objects.SetActive(false);
            if (level2Objects != null) level2Objects.SetActive(false);
            if (level3Objects != null) level3Objects.SetActive(false);

            // 重置拖拽关卡状态
            if (touchScript != null) touchScript.ResetLevel();

            // 停止所有倒计时协程
            if (countdownCoroutine != null) { StopCoroutine(countdownCoroutine); countdownCoroutine = null; }
            if (levelCountdownCoroutine != null) { StopCoroutine(levelCountdownCoroutine); levelCountdownCoroutine = null; }

            ShowLevelSelectPanel();
            return;
        }
        if (levelSelectPanel != null && levelSelectPanel.activeSelf)
        {
            ShowStartPanel();
        }
        else if ((quizPanel != null && quizPanel.activeSelf) ||
                 (level1Objects != null && level1Objects.activeSelf) ||
                 (level2Objects != null && level2Objects.activeSelf) ||
                 (level3Objects != null && level3Objects.activeSelf))
        {
            ShowLevelSelectPanel();
        }
        // 关卡中返回，重置拖拽进度
        if ((level1Objects != null && level1Objects.activeSelf) ||
            (level2Objects != null && level2Objects.activeSelf) ||
            (level3Objects != null && level3Objects.activeSelf))
        {
            if (touchScript != null) touchScript.ResetLevel();
            if (levelCountdownCoroutine != null) { StopCoroutine(levelCountdownCoroutine); levelCountdownCoroutine = null; }
        }

        // 新增：第四/五关答题界面时返回，关闭答题界面、模型、重置状态
        if ((pendingLevel == 4 || pendingLevel == 5) && levelQuizPanel != null && levelQuizPanel.activeSelf)
        {
            SetUIVisible(levelQuizPanel, false);
            // 隐藏模型
            if (pendingLevel == 4 && level4Objects != null) level4Objects.SetActive(false);
            if (pendingLevel == 5 && level5Objects != null) level5Objects.SetActive(false);
            // 重置高亮
            if (pendingLevel == 4 && level4ObjectShow != null) level4ObjectShow.ResetAllOutline();
            if (pendingLevel == 5 && level5ObjectShow != null) level5ObjectShow.ResetAllOutline();
            // 重置答题索引
            currentQuizIndex = 0;
            // 返回选择界面
            ShowLevelSelectPanel();
            return;
        }
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
    // 显示挑战结果界面
    void ShowResultPanel(bool success)
    {
        Debug.Log($"显示挑战结果界面，成功：{success}");
        SetUIVisible(resultPanel, true);
        if (backButton != null) backButton.gameObject.SetActive(false);

        if (resultTitleText != null)
            resultTitleText.text = success ? "挑战成功" : "挑战失败";

        //新增结束关卡音效
        if (success)
        {
            if (currentLevel < 5)
                PlayAudio(completedSound2);
            else
                PlayAudio(completedSound3);
        }
        else
        {
            PlayAudio(failureSound2);
        }

        // 禁用拖拽功能
        if (touchScript != null)
            touchScript.enabled = false;

        // 判断按钮显示
        if (resultNextOrRetryText != null)
        {
            if (success)
            {
                // 修正：第三关完成后应为“进入下一关”，只有第五关才显示“知识答题”
                // 新增：查看总分的功能，将知识答题按钮改为“查看总分”
                if (currentLevel < 5)
                    resultNextOrRetryText.text = "进入下一关";
                else
                    resultNextOrRetryText.text = "查看总分";
            }
            else
            {
                resultNextOrRetryText.text = "重新挑战";
            }
        }

        // 新增：挑战成功时显示“查看搭建完的模型”按钮，否则隐藏
        if (viewModelButton != null)
            viewModelButton.gameObject.SetActive(success);

        // 新增：显示结果界面时，确保倒计时面板隐藏
        if (countdownPanel != null)
            countdownPanel.SetActive(false);
        if (levelCountdownCoroutine != null)
        {
            StopCoroutine(levelCountdownCoroutine);
            levelCountdownCoroutine = null;
        }

        // 退出模型查看模式
        isViewingModel = false;

        // 显示挑战结果聊天内容
        if (AIChatManager.Instance != null)
            AIChatManager.Instance.ShowResultChat(success);
    }

    // 新增：记录查看模型前的原始旋转
    private Quaternion originalModelRotation;

    // 修改：查看搭建完的模型按钮点击事件，记录原始旋转
    private void OnViewModelButton()
    {
        isViewingModel = true;
        // 记录当前关卡物体的原始旋转
        GameObject targetObj = null;
        if (currentLevel == 1) targetObj = level1Objects;
        else if (currentLevel == 2) targetObj = level2Objects;
        else if (currentLevel == 3) targetObj = level3Objects;
        else if (currentLevel == 4) {
            level4Objects.SetActive(false);
            level4ObjectsAlt.SetActive(true);
            targetObj = level4ObjectsAlt; 
        }
        else if (currentLevel == 5) targetObj = level5Objects;
        if (targetObj != null)
            originalModelRotation = targetObj.transform.rotation;
        // 隐藏倒计时面板
        if (countdownPanel != null)
            countdownPanel.SetActive(false);
        // 隐藏结果界面
        if (resultPanel != null)
            resultPanel.SetActive(false);
        // 显示常驻返回按钮
        if (backButton != null)
            backButton.gameObject.SetActive(true);
        Debug.Log("进入模型查看模式，可旋转当前关卡物体");
    }

    void Update()
    {
        // 新增：模型查看模式下，允许旋转当前关卡物体集合（仅Y轴），支持第四/五关
        if (isViewingModel)
        {
            GameObject targetObj = null;
            if (currentLevel == 1) targetObj = level1Objects;
            else if (currentLevel == 2) targetObj = level2Objects;
            else if (currentLevel == 3) targetObj = level3Objects;
            // 新增：支持第四/五关
            else if (currentLevel == 4) targetObj = level4ObjectsAlt;
            else if (currentLevel == 5) targetObj = level5Objects;

            if (targetObj != null)
            {
#if UNITY_EDITOR
                // 鼠标拖动旋转
                if (Input.GetMouseButtonDown(0))
                {
                    lastPointerPos = Input.mousePosition;
                }
                else if (Input.GetMouseButton(0))
                {
                    Vector2 curPos = Input.mousePosition;
                    float deltaX = curPos.x - lastPointerPos.x;
                    targetObj.transform.Rotate(0, -deltaX * 0.5f, 0, Space.World); // 取反
                    lastPointerPos = curPos;
                }
#else
                // 触摸滑动旋转
                if (Input.touchCount == 1)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Moved)
                    {
                        float deltaX = touch.deltaPosition.x;
                        targetObj.transform.Rotate(0, -deltaX * 0.5f, 0, Space.World); // 取反
                    }
                }
#endif
            }
        }
    }

    // 在返回、重新挑战、进入下一关、答题等操作时，退出模型查看模式
    void OnResultBack()
    {
        SetUIVisible(resultPanel, false);
        isViewingModel = false;
        // 复原模型旋转
        GameObject targetObj = null;
        if (currentLevel == 1) targetObj = level1Objects;
        else if (currentLevel == 2) targetObj = level2Objects;
        else if (currentLevel == 3) targetObj = level3Objects;
        else if (currentLevel == 4) targetObj = level4Objects;
        else if (currentLevel == 5) targetObj = level5Objects;
        if (targetObj != null)
            targetObj.transform.rotation = originalModelRotation;
        // 结果界面返回时也要重置当前关卡
        if (touchScript != null)
        {
            touchScript.ResetLevel();
            touchScript.enabled = true; // 重新启用拖拽
        }
        if (countdownCoroutine != null) { StopCoroutine(countdownCoroutine); countdownCoroutine = null; }
        if (levelCountdownCoroutine != null) { StopCoroutine(levelCountdownCoroutine); levelCountdownCoroutine = null; }
        Debug.Log("返回选择界面，重置当前关卡");
        ShowLevelSelectPanel();
    }

    void OnResultNextOrRetry()
    {
        Debug.Log("处理结果界面下一步或重试逻辑");
        isViewingModel = false;
        // 复原模型旋转
        GameObject targetObj = null;
        if (currentLevel == 1) targetObj = level1Objects;
        else if (currentLevel == 2) targetObj = level2Objects;
        else if (currentLevel == 3) targetObj = level3Objects;
        else if (currentLevel == 4) targetObj = level4Objects;
        else if (currentLevel == 5) targetObj = level5Objects;
        if (targetObj != null)
            targetObj.transform.rotation = originalModelRotation;
        if (resultNextOrRetryText == null) return;
        string txt = resultNextOrRetryText.text;
        if (txt == "进入下一关")
        {
            SetUIVisible(resultPanel, false);
            // 隐藏高亮和模型
            if (currentLevel == 1 && level1Objects != null) level1Objects.SetActive(false);
            if (currentLevel == 2 && level2Objects != null) level2Objects.SetActive(false);
            if (currentLevel == 3 && level3Objects != null) level3Objects.SetActive(false);
            if (currentLevel == 4 && level4Objects != null) level4Objects.SetActive(false);
            if (currentLevel == 1 && level4ObjectShow != null) level4ObjectShow.ResetAllOutline();
            if (currentLevel == 2 && level4ObjectShow != null) level4ObjectShow.ResetAllOutline();
            if (currentLevel == 3 && level4ObjectShow != null) level4ObjectShow.ResetAllOutline();
            if (currentLevel == 4 && level4ObjectShow != null) level4ObjectShow.ResetAllOutline();
            // 进入下一关
            int nextLevel = currentLevel + 1;
            if (nextLevel <= 5)
            {
                EnterLevel(nextLevel);
                currentQuizIndex = 0;
                // 新增：更新关卡显示文本
                UpdateCurrentLevelText(nextLevel);
            }
        }
        else if (txt == "查看总分")
        {
            SetUIVisible(resultPanel, false);
            // 显示得分界面
            ShowReportCard();
        }
        else if (txt == "知识答题")
        {
            // 隐藏高亮和模型
            //if (level5Objects != null) level5Objects.SetActive(false);
            //if (level5ObjectShow != null) level5ObjectShow.ResetAllOutline();
            //SetUIVisible(resultPanel, false);
            // 进入知识答题界面
            ShowQuizPanel();
        }
        else if (txt == "重新挑战")
        {
            SetUIVisible(resultPanel, false);
            // 重新挑战当前关卡
            if (currentLevel == 1)
            {
                if (level1Objects != null) level1Objects.SetActive(false);
                if (level4ObjectShow != null) level4ObjectShow.ResetAllOutline();
                EnterLevel(1);
                UpdateCurrentLevelText(1);
            }
            else if (currentLevel == 2)
            {
                if (level2Objects != null) level2Objects.SetActive(false);
                if (level4ObjectShow != null) level4ObjectShow.ResetAllOutline();
                EnterLevel(2);
                UpdateCurrentLevelText(2);
            }
            else if (currentLevel == 3)
            {
                if (level3Objects != null) level3Objects.SetActive(false);
                if (level4ObjectShow != null) level4ObjectShow.ResetAllOutline();
                EnterLevel(3);
                UpdateCurrentLevelText(3);
            }
            else if (currentLevel == 4)
            {
                if (level4Objects != null) level4Objects.SetActive(false);
                if (level4ObjectShow != null) level4ObjectShow.ResetAllOutline();
                EnterLevel(4);
                currentQuizIndex = 0;
                UpdateCurrentLevelText(4);
            }
            else if (currentLevel == 5)
            {
                if (level5Objects != null) level5Objects.SetActive(false);
                if (level5ObjectShow != null) level5ObjectShow.ResetAllOutline();
                EnterLevel(5);
                currentQuizIndex = 0;
                UpdateCurrentLevelText(5);
            }
        }
    }

    // 显示当前关卡文本
    private void UpdateCurrentLevelText(int level)
    {
        if (currentLevelText == null) return;
        switch (level)
        {
            case 1: currentLevelText.text = "第一关"; break;
            case 2: currentLevelText.text = "第二关"; break;
            case 3: currentLevelText.text = "第三关"; break;
            case 4: currentLevelText.text = "第四关"; break;
            case 5: currentLevelText.text = "第五关"; break;
            default: currentLevelText.text = ""; break;
        }
    }

    // 退出游戏（编辑器下停止播放，打包后退出应用）
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    [System.Serializable]
    public class SimpleQuestion // 新增：简化版题目结构
    {
        public string question;
        public string[] options;
        public int correctAnswerIndex; // 单选题正确答案索引
    }

    [Header("关卡题目数据")] // 新增：第四/五关独立题库
    public List<SimpleQuestion> level4Questions = new List<SimpleQuestion>();
    public List<SimpleQuestion> level5Questions = new List<SimpleQuestion>();
    private int currentQuizIndex = 0; // 当前题目索引
    private int currentQuizLevel = 0; // 当前答题关卡（4或5）
    private bool isQuizAnswering = true; // 是否处于答题确认阶段

    private void ShowLevelQuizPanel(int level)
    {
        SetUIVisible(levelQuizPanel, true);
        currentQuizLevel = level;
        isQuizAnswering = true;
        List<SimpleQuestion> questions = (level == 4) ? level4Questions : level5Questions;
        // 新增：高亮当前题目对应的对象
        if (level == 4 && level4ObjectShow != null)
        {
            level4ObjectShow.ResetAllOutline();
            level4ObjectShow.ShowObjectWithOutline(currentQuizIndex);
        }
        else if (level == 5 && level5ObjectShow != null)
        {
            level5ObjectShow.ResetAllOutline();
            level5ObjectShow.ShowObjectWithOutline(currentQuizIndex);
            // 仅处理第五关第一题时只显示第一个模型，其余隐藏
            for (int i = 0; i < level5ObjectShow.objects.Count; i++)
            {
                if (level5ObjectShow.objects[i] != null)
                    level5ObjectShow.objects[i].SetActive(i == currentQuizIndex);
            }
        }
        if (questions.Count > 0 && levelQuizQuestionText != null)
        {
            var q = questions[currentQuizIndex];
            levelQuizQuestionText.text = q.question;
            for (int i = 0; i < levelQuizOptionToggles.Length; i++)
            {
                if (i < q.options.Length)
                {
                    levelQuizOptionToggles[i].gameObject.SetActive(true);
                    levelQuizOptionToggles[i].isOn = false;
                    levelQuizOptionToggles[i].interactable = true;
                    levelQuizOptionToggles[i].onValueChanged.RemoveAllListeners();
                    int optionIndex = i;
                    levelQuizOptionToggles[i].onValueChanged.AddListener((isOn) =>
                    {
                        if (isOn)
                        {
                            for (int j = 0; j < levelQuizOptionToggles.Length; j++)
                            {
                                if (j != optionIndex) levelQuizOptionToggles[j].isOn = false;
                            }
                        }
                    });
                    // 绑定选项文本（Text组件）
                    var textComp = levelQuizOptionToggles[i].GetComponentInChildren<Text>();
                    if (textComp != null)
                        textComp.text = q.options[i];
                }
                else
                {
                    levelQuizOptionToggles[i].gameObject.SetActive(false);
                }
            }
            if (levelQuizTipText != null)
            {
                levelQuizTipText.text = "";
                levelQuizTipText.color = Color.black;
            }
            if (levelQuizConfirmButton != null)
            {
                levelQuizConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "确认";
                levelQuizConfirmButton.interactable = true;
            }
        }
    }

    // 答题按钮逻辑：确认/下一题
    private void HandleLevelQuizAnswer()
    {
        //Debug.Log($"处理关卡{currentQuizLevel}答题，当前索引：{currentQuizIndex}");
        List<SimpleQuestion> questions = (currentQuizLevel == 4) ? level4Questions : level5Questions;
        if (questions == null || questions.Count == 0) return;
        var currentQuestion = questions.Count > currentQuizIndex ? questions[currentQuizIndex] : null;
        if (currentQuestion == null) return;

        if (levelQuizTipText == null || levelQuizConfirmButton == null) return;

        if (isQuizAnswering)
        {
            // 获取用户选择
            int selectedOption = -1;
            for (int i = 0; i < levelQuizOptionToggles.Length; i++)
            {
                if (levelQuizOptionToggles[i].isOn)
                {
                    selectedOption = i;
                    break;
                }
            }
            // 未选择不允许确认
            if (selectedOption == -1)
            {
                levelQuizTipText.text = "请选择一个选项";
                levelQuizTipText.color = Color.red;
                return;
            }
            // 验证答案
            if (selectedOption == currentQuestion.correctAnswerIndex)
            {
                levelQuizTipText.text = "回答正确！";
                levelQuizTipText.color = Color.green;
            }
            else
            {
                levelQuizTipText.text = "回答错误，正确答案已经显示";
                levelQuizTipText.color = Color.red;
                // 显示正确答案
                for (int i = 0; i < levelQuizOptionToggles.Length; i++)
                {
                    levelQuizOptionToggles[i].isOn = (i == currentQuestion.correctAnswerIndex);
                    levelQuizOptionToggles[i].interactable = false;
                }
            }
            // 禁用所有Toggle
            for (int i = 0; i < levelQuizOptionToggles.Length; i++)
            {
                levelQuizOptionToggles[i].interactable = false;
            }
            // 按钮变为“下一题”
            levelQuizConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "下一题";
            isQuizAnswering = false;
        }
        else
        {
            // 下一题或结束
            if (currentQuizIndex < questions.Count - 1)
            {
                // 仅处理第五关第一题答完后隐藏第一个模型
                if (currentQuizLevel == 5 && currentQuizIndex == 0 && level5ObjectShow != null && level5ObjectShow.objects.Count > 0)
                {
                    if (level5ObjectShow.objects[0] != null)
                        level5ObjectShow.objects[0].SetActive(false);
                }
                currentQuizIndex++;
                ShowLevelQuizPanel(currentQuizLevel);
            }
            else
            {
                // 答题完成，进入挑战成功界面
                Debug.Log($"关卡{currentQuizLevel}答题通过");
                SetUIVisible(levelQuizPanel, false);

                // 仅处理第五关答题结束后显示所有模型
                if (currentQuizLevel == 5 && level5ObjectShow != null)
                {
                    foreach (var obj in level5ObjectShow.objects)
                        if (obj != null) obj.SetActive(true);
                    level5ObjectShow.ShowObjectWithOutline(-1);
                }
                // 新增：第四关答题结束后显示所有模型
                if (currentQuizLevel == 4 && level4ObjectShow != null)
                    level4ObjectShow.ShowObjectWithOutline(-1);

                if (backButton != null) backButton.gameObject.SetActive(false);
                ShowLevelQuizResultPanel(currentQuizLevel);
            }
        }
    }

    // 新增：显示第四/五关挑战成功界面
    private void ShowLevelQuizResultPanel(int level)
    {
        //Debug.Log($"显示第四/五关挑战成功界面，当前关卡：{level}");
        SetUIVisible(resultPanel, true);
        if (resultTitleText != null)
            resultTitleText.text = "挑战成功";
        // 禁用拖拽
        if (touchScript != null) touchScript.enabled = false;
        // 设置按钮文本
        if (resultNextOrRetryText != null)
        {
            // 修正：只有第五关才显示“知识答题”，其余都为“进入下一关”
            if (level < 5)
                resultNextOrRetryText.text = "进入下一关";
            else if (level == 5)
                resultNextOrRetryText.text = "知识答题";
        }
        // 查看模型按钮显示
        if (viewModelButton != null)
            viewModelButton.gameObject.SetActive(true);
        // 结果界面返回按钮显示
        if (resultBackButton != null)
            resultBackButton.gameObject.SetActive(true);
        // 退出模型查看模式
        isViewingModel = false;
        // 当前关卡编号同步
        currentLevel = level;
    }
}

