using System.Collections.Generic;
using Common.Core.TweenAnim;
using Common.Core.TweenAnim.Scriptable_Object;
using Common.iCare_UI.Runtime;
using iCareGames.Common.Core.AudioSystem;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace iCareGames.Common.iCare_UI.Editor {
    public class UIEditor : EditorWindow {
        private const string BUTTON_OBJ_NAME = "Animated Button";
        private const string PANEL_OBJ_NAME = "Animated Panel";
        private const string IMAGE_OBJ_NAME = "Animated Image";


        private const string BUTTON_SOUND_ASSET_NAME = "ButtonSound";
        private const string BUTTON_SHAKE_ASSET_NAME = "ButtonShake";
        private const string TEXT_OBJ_NAME = "Text";
        private const string BUTTON_LABEL_TEXT = "iCareGames";

        private static readonly Vector2 _buttonSize = new Vector2(400, 150);
        private static readonly Vector2 _imageSize = new Vector2(100, 100);

        private const string FADE_IN_ANIM = "FadeIn";
        private const string FADE_OUT_ANIM = "FadeOut";
        private const string IMAGE_SHOW_SOUND = "PanelShowSound";
        private const string IMAGE_HIDE_SOUND = "PanelHideSound";

        [MenuItem("GameObject/iCare AnimatedUI/Animated Panel", false, 6)]
        public static void SpawnPanel(MenuCommand menuCommand) {
            var panelObj = CreateObject(PANEL_OBJ_NAME);
            CustomizePanel(panelObj);
        }

        [MenuItem("GameObject/iCare AnimatedUI/Animated Image", false, 6)]
        public static void SpawnImage(MenuCommand menuCommand) {
            var imageObj = CreateObject(IMAGE_OBJ_NAME);
            CustomizeImage(imageObj);
        }

        [MenuItem("GameObject/iCare AnimatedUI/Animated Button", false, 6)]
        public static void SpawnButton(MenuCommand menuCommand) {
            var buttonObj = CreateObject(BUTTON_OBJ_NAME);
            CustomizeButton(buttonObj);
        }

        [MenuItem("GameObject/iCare AnimatedUI/Mobile Canvas", false, 6)]
        public static void SpawnMobileCanvas() => SpawnCanvas(new Vector2(1080, 1920), "Mobile Canvas");

        [MenuItem("GameObject/iCare AnimatedUI/PC Canvas", false, 6)]
        public static void SpawnPcCanvas() => SpawnCanvas(new Vector2(1920, 1080), "Pc Canvas");

        private static void SpawnCanvas(Vector2 referenceResolution, string canvasName) {
            var canvasObject = new GameObject(canvasName);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.vertexColorAlwaysGammaSpace = true;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = referenceResolution;
        }

        private static Canvas GetOrSpawnCanvas() {
            CheckEventSystem();
            var canvas = FindObjectOfType<Canvas>();
            if (canvas != null) return canvas;
            SpawnMobileCanvas();
            canvas = FindObjectOfType<Canvas>();
            return canvas;
        }

        private static GameObject CreateObject(string objectName) {
            var canvas = GetOrSpawnCanvas();
            var panelObject = new GameObject(objectName, typeof(RectTransform));
            if (Selection.activeTransform != null && Selection.activeTransform.GetComponentInParent<Canvas>() != null) {
                panelObject.transform.SetParent(Selection.activeTransform, false);
            }
            else {
                panelObject.transform.SetParent(canvas.transform, false);
            }

            return panelObject;
        }

        private static GameObject CreateObject(GameObject parentObject, string objectName) {
            var newObj = new GameObject(objectName, typeof(RectTransform));
            newObj.transform.SetParent(parentObject.transform, false);
            return newObj;
        }


        private static void CheckEventSystem() {
            if (FindObjectOfType<EventSystem>() != null) return;
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>().gameObject.AddComponent<InputSystemUIInputModule>();
        }

        private static void CustomizePanel(GameObject panelObject) {
            var panelImage = panelObject.AddComponent<Image>();
            panelImage.type = Image.Type.Sliced;
            panelImage.color = Color.black;
            StretchAnchor(panelObject.GetComponent<RectTransform>());

            var animatedPanel = panelObject.AddComponent<AnimatedImage>();
            SetImageAnimations(animatedPanel);
        }

        private static void CustomizeImage(GameObject panelObject) {
            var image = panelObject.AddComponent<Image>();
            image.rectTransform.sizeDelta = _imageSize;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            var animatedImage = panelObject.AddComponent<AnimatedImage>();
            SetImageAnimations(animatedImage);
        }

        private static void SetImageAnimations(AnimatedImage animatedImage) {
            animatedImage.ShowAnimation = animatedImage.gameObject.AddComponent<TweenAnimController>();
            animatedImage.HideAnimation = animatedImage.gameObject.AddComponent<TweenAnimController>();

            var fadeIn = AssetFinder.Find<TweenAnimSO>(FADE_IN_ANIM);
            var fadeOut = AssetFinder.Find<TweenAnimSO>(FADE_OUT_ANIM);
            var panelShowSound = AssetFinder.Find<AudioSO>(IMAGE_SHOW_SOUND);
            var panelHideSound = AssetFinder.Find<AudioSO>(IMAGE_HIDE_SOUND);


            animatedImage.ShowAnimation.TweenAnimHolder ??= new TweenAnimHolder();
            animatedImage.HideAnimation.TweenAnimHolder ??= new TweenAnimHolder();
            animatedImage.ShowAnimation.TweenAnimHolder.TweenAnimations ??= new List<TweenTypeSettings>();
            animatedImage.HideAnimation.TweenAnimHolder.TweenAnimations ??= new List<TweenTypeSettings>();

            animatedImage.ShowAnimation.TweenAnimHolder.AudioToPlay = panelShowSound;
            animatedImage.ShowAnimation.TweenAnimHolder.TweenAnimations.Add(new TweenTypeSettings {
                SettingsType = TweenSettingsType.Preset,
                Preset = fadeIn
            });
            animatedImage.HideAnimation.TweenAnimHolder.AudioToPlay = panelHideSound;
            animatedImage.HideAnimation.TweenAnimHolder.TweenAnimations.Add(new TweenTypeSettings {
                SettingsType = TweenSettingsType.Preset,
                Preset = fadeOut
            });
        }

        private static void CustomizeButton(GameObject buttonObject) {
            var buttonComponent = buttonObject.AddComponent<Button>();
            buttonComponent.transition = Selectable.Transition.None;
            var buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.type = Image.Type.Sliced;
            buttonImage.color = Color.grey;

            var textObject = CreateObject(buttonObject, TEXT_OBJ_NAME);
            CustomizeTextMeshPro(textObject);
            StretchAnchor(textObject.GetComponent<RectTransform>());

            var buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.sizeDelta = _buttonSize;
            CenterAnchor(buttonRect);
            SetButtonAnimations(buttonObject);
        }

        private static void SetButtonAnimations(GameObject buttonObject) {
            var animatedImage = buttonObject.AddComponent<AnimatedButton>();

            var buttonShake = AssetFinder.Find<TweenAnimSO>(BUTTON_SHAKE_ASSET_NAME);
            var buttonSound = AssetFinder.Find<AudioSO>(BUTTON_SOUND_ASSET_NAME);


            animatedImage.TweenAnimHolder ??= new TweenAnimHolder();
            animatedImage.TweenAnimHolder.TweenAnimations ??= new List<TweenTypeSettings>();

            animatedImage.TweenAnimHolder.AudioToPlay = buttonSound;
            animatedImage.TweenAnimHolder.TweenAnimations.Add(new TweenTypeSettings {
                SettingsType = TweenSettingsType.Preset,
                Preset = buttonShake
            });
        }

        private static void CustomizeTextMeshPro(GameObject textObject) {
            var buttonText = textObject.AddComponent<TextMeshProUGUI>();
            buttonText.text = BUTTON_LABEL_TEXT;
            buttonText.enableAutoSizing = true;
            buttonText.fontSizeMin = 10;
            buttonText.fontSizeMax = 120;
            buttonText.alignment = TextAlignmentOptions.Center;
        }

        private static void StretchAnchor(RectTransform rectTransform) {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            CenterAnchor(rectTransform);
        }

        private static void CenterAnchor(RectTransform rectTransform) {
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
        }
    }
}

public static class AssetFinder {
    public static T Find<T>(string assetName) where T : Object {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)} {assetName}");

        if (guids.Length > 0) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
        else {
            Debug.LogWarning($"Asset of type {typeof(T)} with name {assetName} not found.");
            return null;
        }
    }
}