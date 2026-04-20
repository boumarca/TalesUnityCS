using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class Textbox : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Component References")]
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private GameObject _nameBox;
        #endregion

        #region Public Methods
        public void ShowMessage(string message)
        {
            _messageText.text = message;
            gameObject.SetActive(true);
        }

        public void ShowSpeakerName(string speakerName)
        {
            _nameText.text = speakerName;
            _nameBox.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            _nameBox.SetActive(false);
        }
        #endregion
    }
}
