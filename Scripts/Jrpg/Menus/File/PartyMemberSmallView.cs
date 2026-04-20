using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jrpg.Menus.File
{
    public class PartyMemberSmallView : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Image _actorPortrait;
        [SerializeField] private TextMeshProUGUI _levelText;
        #endregion

        #region Public Methods
        public void SetActorInfo(Sprite actorSprite, int level)
        {
            _actorPortrait.sprite = actorSprite;
            _levelText.text = level.ToString(CultureInfo.InvariantCulture);
        }
        #endregion
    }
}
