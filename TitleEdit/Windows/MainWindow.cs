using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;
using TitleEdit.Utility;

namespace TitleEdit.Windows;

public class MainWindow : Window, IDisposable
{
    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow()
        : base("Title Edit 偵錯視窗##TitleScreen",
               ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    private float test1 = 0;
    private float test2 = 0;

    public void Dispose() { }

    public override void Draw()
    {
        unsafe
        {
            ImGui.TextUnformatted($"目前角色 {Services.ClientState.LocalContentId:X16}");
            ImGui.TextUnformatted(
                $"所選角色 {(IntPtr)Services.LobbyService.CurrentCharacter:X16} - {Services.LobbyService.CurrentContentId:X16}");
            ImGui.TextUnformatted($"目前天氣 {Services.WeatherService.WeatherId}");
            ImGui.TextUnformatted($"目前大廳地圖 {Services.LobbyService.CurrentLobbyMap}");
            var location = Services.LocationService.GetLocationModel(Services.ClientState.LocalContentId);
            ImGui.TextUnformatted(
                $"目前場景配置 {location.Active.Count} {location.Inactive.Count} {location.VfxTriggerIndexes.Count}");
            //ImGui.TextUnformatted($"Current layout2 {Services.LocationService.Active.Count} {Services.LocationService.Inactive.Count} {Services.LocationService.VfxTriggerIndexes.Count}");
            //ImGui.TextUnformatted($"Current layout3 {location.Active.All(Services.LocationService.Active.Contains)} {location.Inactive.All(Services.LocationService.Inactive.Contains)} {location.VfxTriggerIndexes.Keys.Count == Services.LocationService.VfxTriggerIndexes.Keys.Count && location.VfxTriggerIndexes.Keys.All(k => Services.LocationService.VfxTriggerIndexes.ContainsKey(k) && object.Equals(location.VfxTriggerIndexes[k], Services.LocationService.VfxTriggerIndexes[k]))}");
            ImGui.TextUnformatted($"目前曲目 {Services.BgmService.CurrentSongId}");
            ImGui.TextUnformatted($"目前大廳音樂索引 {Services.LobbyService.LobbyInfo->CurrentLobbyMusicIndex}");
            ImGui.TextUnformatted($"目前坐騎 ID {location.Mount.MountId}");
            ImGui.TextUnformatted($"更新時間 {Services.LayoutService.UpdateTime}");
            ImGui.SliderAngle($"測試##test", ref test1, -360, 360);
            ImGui.TextUnformatted($"正規化結果 {Utils.NormalizeAngle(test1) * Utils.RadToDegreeRatio}");
            ImGui.SliderFloat($"測試正規化##Testing normalizer", ref test2, -720, 720);
            ImGui.TextUnformatted(
                $"正規化結果 {Utils.NormalizeAngle((float)(test2 * Utils.DegreeToRadRatio)) * Utils.RadToDegreeRatio}");
            ImGui.TextUnformatted($"時間：{Services.LocationService.TimeOffset}");
            if (ImGui.Button("重新遷移標題畫面"))
            {
                Services.MigrationService.MigrateTitleScreenV2Presets();
            }

            ImGui.TextUnformatted($"LobbyUiStage {Services.LobbyService.LobbyUiStage}");
            ImGui.TextUnformatted($"TitleCutsceneIsLoaded {Services.LobbyService.TitleCutsceneIsLoaded}");
            if (ImGui.Button("重新載入標題畫面介面"))
            {
                Services.LobbyService.ReloadTitleScreenUi();
            }

            if (ImGui.Button("重新載入標題畫面色彩"))
            {
                Services.LobbyService.RecolorTitleScreenUi();
            }

            if (ImGui.Button("重新載入標題畫面"))
            {
                Services.LobbyService.ReloadTitleScreen();
            }

            ImGui.TextUnformatted($"可用資料片 {UIState.Instance()->PlayerState.MaxExpansion}");
            if (ImGui.CollapsingHeader("版本"))
            {
                var framework = Framework.Instance();
                ImGui.TextUnformatted(
                    $"遊戲版本：{framework->GameVersionString} {framework->GameVersionString.Length}");
                ImGui.TextUnformatted(
                    $"Heavensward: {framework->ExVersions.GetValue(0)?.VersionString} {framework->ExVersions.GetValue(0)?.VersionString.Length}");
                ImGui.TextUnformatted(
                    $"Stormblood: {framework->ExVersions.GetValue(1)?.VersionString} {framework->ExVersions.GetValue(1)?.VersionString.Length}");
                ImGui.TextUnformatted(
                    $"Shadowbringers: {framework->ExVersions.GetValue(2)?.VersionString} {framework->ExVersions.GetValue(2)?.VersionString.Length}");
                ImGui.TextUnformatted(
                    $"Endwalker: {framework->ExVersions.GetValue(3)?.VersionString} {framework->ExVersions.GetValue(3)?.VersionString.Length}");
                ImGui.TextUnformatted(
                    $"Dawntrail: {framework->ExVersions.GetValue(4)?.VersionString} {framework->ExVersions.GetValue(4)?.VersionString.Length}");
            }

            if (ImGui.Button("房屋測試"))
            {
                Services.LobbyService.HousingTest();
            }

            ImGui.Checkbox("忽略季節活動日期檢查", ref Services.ConfigurationService.IgnoreSeasonalDateCheck);
        }
    }
}
