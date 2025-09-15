using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Thanabardi.CentipedeGame.Core.Interface;

namespace Thanabardi.CentipedeGame.Core.UI
{
    public class HUDPanel : MonoBehaviour, IUIPanel
    {
        [SerializeField]
        private Image _lifeImage;
        [SerializeField]
        private Transform _lifeContainer;
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        [SerializeField]
        private TextMeshProUGUI _endlessText;

        // pool for storing life image reference
        private List<Image> _lifeImagePool = new();

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void SetEndlessActive(bool isActive)
        {
            _endlessText.gameObject.SetActive(isActive);
        }

        public void SetScore(int score)
        {
            _scoreText.SetText(score.ToString());
        }

        public void SetLife(int lifeCount)
        {
            // expand pool size
            int lifePoolCount = _lifeImagePool.Count;
            if (lifePoolCount < lifeCount)
            {
                for (int i = 0; i < lifeCount - lifePoolCount; i++)
                {
                    Image image = Instantiate(_lifeImage, _lifeContainer);
                    _lifeImagePool.Add(image);
                }
            }

            // set active life image
            for (int i = 0; i < _lifeImagePool.Count; i++)
            {
                _lifeImagePool[i].gameObject.SetActive(i < lifeCount);
            }
        }
    }
}