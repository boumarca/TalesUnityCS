using UnityEngine.Localization;

namespace Game.RpgSystem.Data
{
    public enum ItemCategory
    {
        //TODO: Adjust Categories for None = 0.
        None = -1,
        Medecine,
        Liquid,
        Wood,
        Sword,
        Weapon,
        KeyItem,
        Knuckles,
        ArmorBody,
        ArmorHead,
        ArmorArm,
        Accessory,
        Armor,
        Vest,
        Helmet,
        Ribbon,
        Shield,
        Bracer,
        Charm,
        Ring,
        Plant,
        Fuel,
        Metal,
    }

    public static class ItemCategoryExtensions
    {
        public static LocalizedString ToLocalizedString(this ItemCategory itemCategory)
        {
            return new LocalizedString("Items", $"$item.category_{(int)itemCategory}");
        }
    }
}
