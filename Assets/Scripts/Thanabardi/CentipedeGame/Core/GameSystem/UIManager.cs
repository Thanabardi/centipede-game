using UnityEngine;
using Thanabardi.Generic.Utility;
using Thanabardi.CentipedeGame.Core.UI;
using Thanabardi.CentipedeGame.Core.Interface;

namespace Thanabardi.CentipedeGame.Core.GameSystem
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField]
        private TitlePanel _titlePanel;
        [SerializeField]
        private HUDPanel _hudPanel;
        [SerializeField]
        private PausePanel _pausePanel;
        [SerializeField]
        private GameOverPanel _gameOverPanel;

        public override void Awake()
        {
            base.Awake();
        }

        public IUIPanel SetPanelActive(UIKey uIKey, bool isActive)
        {
            switch (uIKey)
            {
                case UIKey.TitlePanel:
                    _titlePanel.SetActive(isActive);
                    _titlePanel.transform.SetAsLastSibling();
                    return _titlePanel;
                case UIKey.HUDPanel:
                    _hudPanel.SetActive(isActive);
                    _hudPanel.transform.SetAsLastSibling();
                    return _hudPanel;
                case UIKey.PausePanel:
                    _pausePanel.SetActive(isActive);
                    _pausePanel.transform.SetAsLastSibling();
                    return _pausePanel;
                case UIKey.GameOverPanel:
                    _gameOverPanel.SetActive(isActive);
                    _gameOverPanel.transform.SetAsLastSibling();
                    return _gameOverPanel;
                default:
                    Debug.LogError("UI Panel not found");
                    return null;
            }
        }

        public enum UIKey
        {
            TitlePanel,
            HUDPanel,
            PausePanel,
            GameOverPanel
        }
    }
}