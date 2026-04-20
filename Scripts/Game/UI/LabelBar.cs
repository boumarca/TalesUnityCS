using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LabelBar : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private TextMeshProUGUI _barText;
        [SerializeField] private Slider _barSlider;
        [SerializeField] private Image _projectedIncrease;
        #endregion

        #region Public Methods
        public void SetBarValue(string text, float percent)
        {
            _barText.text = text;
            _barSlider.value = percent;
        }

        public void SetBarValue(int current, int max)
        {
            SetBarValue($"{current}/{max}", (float)current / max);
        }

        public void SetProjectedIncrease(int current, int max, float percent)
        {
            float remainingPercent = 1 - (float)current / max;
            _projectedIncrease.fillAmount = Mathf.Min(remainingPercent, percent);
        }

        public void ShowProjectedIncrease()
        {
            _projectedIncrease.gameObject.SetActive(true);
        }

        public void HideProjectedIncrease()
        {
            _projectedIncrease.gameObject.SetActive(false);
        }
        #endregion
    }
}
