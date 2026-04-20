using Game.DialogueSystem;
using Game.DialogueSystem.Data;
using Game.Maps.Actors;
using UnityEngine;

namespace Jrpg.Maps.LocalMap
{
    public class InformationInteractable : MonoBehaviour, IMapInteractable
    {
        #region Serialized Fields
        [Header("Game Data")]
        [SerializeField] private ConversationSequence _informationMessage;
        #endregion

        #region IMapInteractable Implementation
        public void Interact(MapPlayer player)
        {
            //TODO: Make it cleaner!
            DialogueManager.Instance.StartDialogue(_informationMessage);
        }
        #endregion
    }
}
