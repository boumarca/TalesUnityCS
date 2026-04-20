using Game.QuestSystem.Data;
using Game.UI;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace Jrpg.Menus.Views
{
    public class QuestObjectiveEntry : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private LocalizeStringEvent _objectiveText;
        [SerializeField] private Checkbox _checkbox;
        #endregion

        #region Public Methods
        public void Initialize(QuestStepData step, bool isCompleted)
        {
            _objectiveText.StringReference = step.Name;
            SetCompletionIcon(isCompleted);
        }
        #endregion

        #region Private Methods
        private void SetCompletionIcon(bool isCompleted)
        {
            _checkbox.SetChecked(isCompleted);
        }
        #endregion
    }
}
