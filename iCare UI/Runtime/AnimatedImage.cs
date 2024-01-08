using Common.Core.TweenAnim;
using Sirenix.OdinInspector;


namespace Common.iCare_UI.Runtime {
    public class AnimatedImage : BaseResetable {
        public bool ResetOnShow;
        
        public TweenAnimController ShowAnimation;
        public TweenAnimController HideAnimation;
        
        
        [Button]
        public void Show() {
            if(ResetOnShow) Reset();
            ShowAnimation.Play();
        }
        [Button]
        public void Hide() {
            HideAnimation.Play();
        }
    }
}