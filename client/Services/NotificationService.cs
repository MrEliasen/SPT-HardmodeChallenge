using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Vagabond.Client.Services;

public sealed class NotificationService : MonoBehaviour
{
    public sealed class UiSettings
    {
        // Canvas
        public Vector2 ReferenceResolution = new(1920f, 1080f);
        public int SortingOrder = 5000;

        // Overlay
        public Color OverlayColor = new(0f, 0f, 0f, 0.72f);

        // Panel
        public float PanelWidth = 760f;           // Main thing to tweak
        public float PanelYOffset = 120f;
        public Color PanelColor = new(0.12f, 0.12f, 0.12f, 1f);
        public RectOffset PanelPadding = new(24, 24, 18, 18);
        public float PanelSpacing = 12f;

        // Title
        public string Title = "Vagabond";
        public int TitleFontSize = 14;
        public float TitleHeight = 32f;

        // Message
        public int MessageFontSize = 11;
        public float MessageMinHeight = 40f;

        // Buttons
        public float ButtonsRowHeight = 36f;
        public float ButtonWidth = 100f;
        public float ButtonHeight = 32f;
        public float ButtonSpacing = 10f;
        public Color ButtonColor = new(0.22f, 0.22f, 0.22f, 1f);

        public float MessageWidth =>
            PanelWidth - PanelPadding.left - PanelPadding.right;
    }

    public static NotificationService Instance { get; private set; } = null!;

    private static UiSettings _pendingSettings = new();

    private UiSettings _ui = null!;
    private GameObject _root = null!;
    private GameObject _overlay = null!;
    private RectTransform _panel = null!;
    private RectTransform _messageRect = null!;
    private Text _messageText = null!;

    private Coroutine _hideCoroutine;

    public static void Create(Transform parent = null, UiSettings settings = null)
    {
        if (Instance != null)
            return;

        _pendingSettings = settings ?? new UiSettings();

        var go = new GameObject("Vagabond.NotificationService");
        if (parent != null)
            go.transform.SetParent(parent, false);

        DontDestroyOnLoad(go);
        Instance = go.AddComponent<NotificationService>();
    }

    private void Awake()
    {
        _ui = _pendingSettings ?? new UiSettings();

        EnsureEventSystem();
        BuildUi();
        HideMessage();
    }

    public void ShowMessage(string text, float durationSeconds = 0f)
    {
        _messageText.text = text;
        _overlay.SetActive(true);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);

        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
            _hideCoroutine = null;
        }

        if (durationSeconds > 0f)
            _hideCoroutine = StartCoroutine(HideAfterDelay(durationSeconds));
    }

    public void HideMessage()
    {
        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
            _hideCoroutine = null;
        }

        if (_overlay != null)
            _overlay.SetActive(false);
    }

    public void SetPanelWidth(float width)
    {
        _ui.PanelWidth = width;
        ApplySizing();
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideMessage();
    }

    private void BuildUi()
    {
        var font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        CreateCanvasRoot();
        CreateOverlay();
        CreatePanel();
        CreateTitle(font);
        CreateMessage(font);
        CreateButtons(font);

        ApplySizing();
    }

    private void CreateCanvasRoot()
    {
        _root = new GameObject(
            "NotificationCanvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        _root.transform.SetParent(transform, false);
        DontDestroyOnLoad(_root);

        var canvas = _root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = _ui.SortingOrder;

        var scaler = _root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = _ui.ReferenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
    }

    private void CreateOverlay()
    {
        _overlay = new GameObject("Overlay", typeof(RectTransform), typeof(Image));
        _overlay.transform.SetParent(_root.transform, false);

        var overlayRect = (RectTransform)_overlay.transform;
        StretchFull(overlayRect);

        var overlayImage = _overlay.GetComponent<Image>();
        overlayImage.color = _ui.OverlayColor;
        overlayImage.raycastTarget = true;
    }

    private void CreatePanel()
    {
        var panelGo = new GameObject(
            "Panel",
            typeof(RectTransform),
            typeof(Image),
            typeof(VerticalLayoutGroup),
            typeof(ContentSizeFitter));

        panelGo.transform.SetParent(_overlay.transform, false);
        _panel = (RectTransform)panelGo.transform;

        _panel.anchorMin = new Vector2(0.5f, 0.5f);
        _panel.anchorMax = new Vector2(0.5f, 0.5f);
        _panel.pivot = new Vector2(0.5f, 0.5f);
        _panel.anchoredPosition = new Vector2(0f, _ui.PanelYOffset);

        var panelImage = panelGo.GetComponent<Image>();
        panelImage.color = _ui.PanelColor;
        panelImage.raycastTarget = true;

        var layout = panelGo.GetComponent<VerticalLayoutGroup>();
        layout.padding = _ui.PanelPadding;
        layout.spacing = _ui.PanelSpacing;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        var fitter = panelGo.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void CreateTitle(Font font)
    {
        var titleGo = new GameObject("Title", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
        titleGo.transform.SetParent(_panel, false);

        var titleLayout = titleGo.GetComponent<LayoutElement>();
        titleLayout.preferredHeight = _ui.TitleHeight;

        var titleText = titleGo.GetComponent<Text>();
        titleText.font = font;
        titleText.text = _ui.Title;
        titleText.fontSize = _ui.TitleFontSize;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        titleText.horizontalOverflow = HorizontalWrapMode.Wrap;
        titleText.verticalOverflow = VerticalWrapMode.Truncate;
        titleText.raycastTarget = false;
    }

    private void CreateMessage(Font font)
    {
        var messageGo = new GameObject(
            "Message",
            typeof(RectTransform),
            typeof(Text),
            typeof(ContentSizeFitter),
            typeof(LayoutElement));

        messageGo.transform.SetParent(_panel, false);
        _messageRect = (RectTransform)messageGo.transform;

        var messageLayout = messageGo.GetComponent<LayoutElement>();
        messageLayout.minHeight = _ui.MessageMinHeight;
        messageLayout.flexibleWidth = 0f;

        _messageText = messageGo.GetComponent<Text>();
        _messageText.font = font;
        _messageText.fontSize = _ui.MessageFontSize;
        _messageText.alignment = TextAnchor.UpperLeft;
        _messageText.color = Color.white;
        _messageText.horizontalOverflow = HorizontalWrapMode.Wrap;
        _messageText.verticalOverflow = VerticalWrapMode.Overflow;
        _messageText.supportRichText = true;
        _messageText.raycastTarget = false;

        var fitter = messageGo.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void CreateButtons(Font font)
    {
        var buttonsGo = new GameObject(
            "Buttons",
            typeof(RectTransform),
            typeof(HorizontalLayoutGroup),
            typeof(LayoutElement));

        buttonsGo.transform.SetParent(_panel, false);

        var rowLayout = buttonsGo.GetComponent<LayoutElement>();
        rowLayout.preferredHeight = _ui.ButtonsRowHeight;

        var buttonsGroup = buttonsGo.GetComponent<HorizontalLayoutGroup>();
        buttonsGroup.childAlignment = TextAnchor.MiddleRight;
        buttonsGroup.childControlWidth = false;
        buttonsGroup.childControlHeight = true;
        buttonsGroup.childForceExpandWidth = false;
        buttonsGroup.childForceExpandHeight = false;
        buttonsGroup.spacing = _ui.ButtonSpacing;
        buttonsGroup.padding = new RectOffset(0, 0, 0, 0);

        CreateButton(buttonsGo.transform, font, "OK", HideMessage);
    }

    private Button CreateButton(Transform parent, Font font, string text, UnityEngine.Events.UnityAction onClick)
    {
        var buttonGo = new GameObject(
            "Button_" + text,
            typeof(RectTransform),
            typeof(Image),
            typeof(Button),
            typeof(LayoutElement));

        buttonGo.transform.SetParent(parent, false);

        var layout = buttonGo.GetComponent<LayoutElement>();
        layout.preferredWidth = _ui.ButtonWidth;
        layout.preferredHeight = _ui.ButtonHeight;

        var image = buttonGo.GetComponent<Image>();
        image.color = _ui.ButtonColor;

        var button = buttonGo.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        var labelGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
        labelGo.transform.SetParent(buttonGo.transform, false);

        var labelRect = (RectTransform)labelGo.transform;
        StretchFull(labelRect);

        var label = labelGo.GetComponent<Text>();
        label.font = font;
        label.text = text;
        label.fontSize = 12;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.raycastTarget = false;

        return button;
    }

    private void ApplySizing()
    {
        if (_panel == null)
            return;

        _panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _ui.PanelWidth);

        if (_messageRect != null)
            _messageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _ui.MessageWidth);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_panel);
    }

    private static void StretchFull(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        var eventSystemGo = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        DontDestroyOnLoad(eventSystemGo);
    }
}