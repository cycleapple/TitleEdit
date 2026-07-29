using TitleEdit.Utility;

namespace TitleEdit.Data.Persistence
{

    public enum CameraFollowMode
    {
        [EnumTranslation("使用玩家設定")]
        Inherit = 0, // Only used in presets
        [EnumTranslation("角色模型位置", tooltip: "騎乘坐騎而使模型產生偏移時，讓鏡頭持續朝向實際的角色模型。")]
        ModelPosition = 1,
        [EnumTranslation("固定角色物件位置", tooltip: "忽略騎乘模型的偏移，讓鏡頭持續朝向角色在遊戲中的位置。")]
        ObjectPosition = 2
    }
}
