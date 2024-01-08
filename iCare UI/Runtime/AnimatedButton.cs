using Common.Core.TweenAnim;
using UnityEngine;
using UnityEngine.UI;

namespace Common.iCare_UI.Runtime {
    
    [RequireComponent(typeof(Button))]
    public sealed class AnimatedButton : TweenAnimController {
        [SerializeField] private bool canInteractWhileAnimating;
        [SerializeField] private bool resetOnFinish;
        private Button _button;

        private void Start() {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
            TweenAnimHolder.OnLongestTweenFinish.AddListener(OnFinish);
        }
        private void OnDestroy() {
            _button.onClick.RemoveListener(OnClick);
            TweenAnimHolder.OnLongestTweenFinish.RemoveListener(OnFinish);
        }

        private void OnClick() {
            if(TweenAnimHolder.TweenAnimations.Count <= 0) return;
            Play();
            if(!canInteractWhileAnimating)
                DisableInteraction();
        }


        private void OnFinish() {
            if(resetOnFinish) Reset();
            _button.interactable = true;
        }
        private void DisableInteraction() => _button.interactable = false;
    }
}
