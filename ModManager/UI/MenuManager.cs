using Doozy.Engine.UI;
using ModManager.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ModManager.UI
{
    internal static class MenuManager
    {
        public static bool isReady = false;
        private static GameObject baseView;
        private static GameObject baseButton;
        private static GameObject baseText;
        private static GameObject baseToggle;
        private static GameObject baseInputField;
        private static GameObject baseSlider;

        /// <summary>
        /// Finds and instantiates base GameObjects.
        /// </summary>
        /// <param name="hud">The HUD transform to search for base GameObjects.</param>
        internal static void Initialize(Transform hud)
        {
            if (isReady) return;
            // Steal some gameobjects
            // "But what if the UI elements move?" you ask, unaware of the 30 ton anvil plumetting towards your head
            // TODO: Create an asset bundle with custom base objects instead

            // BaseView <RectTransform> <Canvas> <GraphicRaycaster> <CanvasGroup> <UIView> <CanvasRenderer> <Image>
            baseView = ObjectHelper.InstantiateWithoutChildren(hud.Find("View - Pause").gameObject);
            baseView.name = "View - MenuManagerBase";

            // BaseButton <RectTransform> <CanvasRenderer> <Image> <Button> <UIButton> <CanvasGroup>
            // └─ Label   <RectTransform> <CanvasRenderer> <TextMeshProUGUI> <Localize>
            baseButton = Object.Instantiate(hud.Find("View - Pause/Main/MainButtons/Button - Options").gameObject);
            baseButton.GetComponent<UIButton>().OnClick.OnTrigger.Event = new UnityEvent();
            baseButton.name = "Button - MenuManagerBase";

            ContentSizeFitter buttonSizeFitter = baseButton.gameObject.AddComponent<ContentSizeFitter>();
            buttonSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            buttonSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            VerticalLayoutGroup buttonLayoutGroup = baseButton.AddComponent<VerticalLayoutGroup>();
            buttonLayoutGroup.padding = new RectOffset(5, 5, 7, 7);

            ContentSizeFitter buttonTextSizeFitter = baseButton.GetComponentInChildren<TextMeshProUGUI>().gameObject.AddComponent<ContentSizeFitter>();
            buttonTextSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            buttonTextSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // BaseText <RectTransform> <CanvasRenderer> <TextMeshProUGUI> <Localize>
            baseText = Object.Instantiate(hud.Find("View - Pause/Main/MainButtons/Button - Options/Label").gameObject);
            baseText.name = "Text - MenuManagerBase";

            ContentSizeFitter textSizeFitter = baseText.AddComponent<ContentSizeFitter>();
            textSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            textSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // BaseToggle         <RectTransform> <HorizontalLayoutGroup>
            // └─ Label           <RectTransform> <CanvasRenderer> <TextMeshProUGUI> <Localize>
            // └─ Toggle          <RectTransform> <Toggle> <UIToggle> <CanvasGroup>
            //    └─ Background   <RectTransform> <CanvasRenderer> <Image>
            //       └─ Checkmark <RectTransform> <CanvasRenderer> <Image> <ColorTargetImage>
            baseToggle = Object.Instantiate(hud.Find("View - Options/MainOptions/AIMEnable").gameObject);
            baseToggle.name = "Toggle - MenuManagerBase";

            isReady = true;
        }

        /// <summary>
        /// Creates a labeled button linking to a new empty UIView. The UIView contains a VerticalLayoutGroup for contents, and a back button.
        /// </summary>
        /// <param name="name">The text to display on the button.</param>
        /// <param name="viewButton">The button object that shows this view.</param>
        /// <param name="buttonParent">The parent transform to attach the button to.</param>
        /// <param name="buttonSiblingIndex">The index to insert the transform at.</param>
        /// <returns>
        /// The VerticalLayoutGroup object within the created view object.
        /// </returns>
        public static GameObject CreateView(string name, out GameObject viewButton, Transform buttonParent = null, int buttonSiblingIndex = -1)
        {
            GameObject menu = Object.Instantiate(baseView, GameController.instance.hud.transform);
            menu.name = $"View - {name}";

            UIView menuView = menu.GetComponent<UIView>();
            menuView.ViewName = name;

            VerticalLayoutGroup layoutGroup = menu.GetComponent<VerticalLayoutGroup>() ?? menu.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;

            // Create a contents container that will exclude the back button
            GameObject contents = new GameObject("ContentsContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
            contents.transform.SetParent(menu.transform, false);

            VerticalLayoutGroup contentsLayoutGroup = contents.GetComponent<VerticalLayoutGroup>();
            contentsLayoutGroup.childControlWidth = false;
            contentsLayoutGroup.childControlHeight = false;
            contentsLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

            // TODO: Disable previous view when showing menuView. Re-enable when hiding menuView.
            viewButton = CreateButton(
                text: name,
                onClick: () => { menuView.Show(); },
                parent: buttonParent,
                siblingIndex: buttonSiblingIndex
            );

            CreateButton(
                text: "Back",
                onClick: () => { menuView.Hide(); },
                parent: menuView.transform
            );

            return contents;
        }

        /// <summary>
        /// Creates a labeled UIButton.
        /// </summary>
        /// <param name="text">The text to display on the button.</param>
        /// <param name="onClick">The action to perform when the button is clicked.</param>
        /// <param name="parent">The parent transform to attach the button to.</param>
        /// <param name="siblingIndex">The index to insert the transform at.</param>
        /// <returns>
        /// The created button object.
        /// </returns>
        public static GameObject CreateButton(string text, UnityAction onClick, Transform parent = null, int siblingIndex = -1)
        {
            GameObject button = Object.Instantiate(baseButton, parent);
            button.transform.SetSiblingIndex(siblingIndex);
            button.name = $"Button - {text}";

            UIButton uiButton = button.GetComponent<UIButton>();
            uiButton.OnClick.OnTrigger.Event.AddListener(onClick);
            uiButton.ButtonName = text;

            TextMeshProUGUI uGUI = button.GetComponentInChildren<TextMeshProUGUI>(true);
            uGUI.SetText(text);

            return button;
        }

        /// <summary>
        /// Creates a UGUI text object.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="parent">The parent transform to attach the text to.</param>
        /// <param name="siblingIndex">The index to insert the transform at.</param>
        /// <returns>
        /// The created text object.
        /// </returns>
        public static GameObject CreateText(string text, Transform parent = null, int siblingIndex = -1)
        {
            GameObject textObj = Object.Instantiate(baseText, parent);
            textObj.name = $"Text - {text}";
            textObj.transform.SetSiblingIndex(siblingIndex);

            TextMeshProUGUI uGUI = textObj.GetComponent<TextMeshProUGUI>();
            uGUI.SetText(text);

            return textObj;
        }

        /// <summary>
        /// Creates a Toggle object.
        /// </summary>
        /// <param name="text">The text to display next to the toggle.</param>
        /// <param name="defaultValue">The default on/off state of the toggle.</param>
        /// <param name="parent">The parent transform to attach the toggle to.</param>
        /// <param name="siblingIndex">The index to insert the transform at.</param>
        /// <returns>
        /// The created toggle object.
        /// </returns>
        public static GameObject CreateToggle(string text, bool defaultValue, UnityAction<bool> onValueChanged = null, Transform parent = null, int siblingIndex = -1)
        {
            GameObject toggleObj = Object.Instantiate(baseToggle, parent);
            toggleObj.name = $"Toggle - {text}";
            toggleObj.transform.SetSiblingIndex(siblingIndex);

            toggleObj.GetComponentInChildren<TextMeshProUGUI>(true).SetText(text);

            Toggle toggle = toggleObj.GetComponentInChildren<Toggle>(true);
            toggle.isOn = defaultValue;
            toggle.onValueChanged.AddListener(onValueChanged);

            return toggleObj;
        }
    }
}
