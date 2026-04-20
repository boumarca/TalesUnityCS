using Framework.Data;
using Game.QuestSystem.Data;
using UnityEngine;

namespace Game.QuestSystem
{
    [CreateAssetMenu(fileName = "NewQuestDatabase", menuName = "Databases/QuestDatabase")]
    public class QuestDatabase : Database<QuestData>
    {
    }
}
