using TitleEdit.Utility;

namespace TitleEdit.Data.Lobby
{
    public enum TitleScreenMovie : int
    {
        [EnumTranslation("未指定")]
        Unspecified = -1,
        [EnumExpansion(TitleScreenExpansion.ARealmReborn)]
        [EnumTranslation("新生艾歐澤亞")]
        ARealmReborn = 1,
        [EnumExpansion(TitleScreenExpansion.Heavensward)]
        [EnumTranslation("蒼天伊修加德")]
        Heavensward = 4,
        [EnumExpansion(TitleScreenExpansion.Stormblood)]
        [EnumTranslation("紅蓮之狂潮")]
        Stormblood = 5,
        [EnumExpansion(TitleScreenExpansion.Shadowbringers)]
        [EnumTranslation("漆黑的反叛者")]
        Shadowbringers = 6,
        [EnumExpansion(TitleScreenExpansion.Endwalker)]
        [EnumTranslation("曉月之終途")]
        Endwalker = 7,
        [EnumExpansion(TitleScreenExpansion.Dawntrail)]
        [EnumTranslation("黃金的遺產")]
        Dawntrail = 8
    }
}
