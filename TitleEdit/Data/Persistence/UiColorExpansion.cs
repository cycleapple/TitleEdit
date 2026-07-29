using TitleEdit.Utility;

namespace TitleEdit.Data.Persistence
{
    public enum UiColorExpansion : int
    {
        [EnumTranslation("未指定")]
        Unspecified = -2,
        [EnumTranslation("自訂")]
        Custom = -1,
        [EnumTranslation("新生艾歐澤亞")]
        ARealmReborn = 0,
        [EnumTranslation("蒼天伊修加德")]
        Heavensward = 1,
        [EnumTranslation("紅蓮之狂潮")]
        Stormblood = 2,
        [EnumTranslation("漆黑的反叛者")]
        Shadowbringers = 3,
        [EnumTranslation("曉月之終途")]
        Endwalker = 4,
        [EnumTranslation("黃金的遺產")]
        Dawntrail = 5
    }
}
