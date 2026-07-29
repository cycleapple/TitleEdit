using TitleEdit.Data.Lobby;
using TitleEdit.Utility;

namespace TitleEdit.Data.Persistence
{
    public enum TitleScreenLogo
    {
        [EnumTranslation("無")]
        None,
        [EnumTranslation("未指定")]
        Unspecified,
        [EnumExpansion(TitleScreenExpansion.ARealmReborn)]
        [EnumTranslation("新生艾歐澤亞")]
        ARealmReborn,
        [EnumExpansion(TitleScreenExpansion.ARealmReborn)]
        [EnumTranslation("FFXIV 免費試玩版")]
        FreeTrial,
        [EnumExpansion(TitleScreenExpansion.Heavensward)]
        [EnumTranslation("蒼天伊修加德")]
        Heavensward,
        [EnumExpansion(TitleScreenExpansion.Stormblood)]
        [EnumTranslation("紅蓮之狂潮")]
        Stormblood,
        [EnumExpansion(TitleScreenExpansion.Shadowbringers)]
        [EnumTranslation("漆黑的反叛者")]
        Shadowbringers,
        [EnumExpansion(TitleScreenExpansion.Endwalker)]
        [EnumTranslation("曉月之終途")]
        Endwalker,
        [EnumExpansion(TitleScreenExpansion.Dawntrail)]
        [EnumTranslation("黃金的遺產")]
        Dawntrail
    }
}
