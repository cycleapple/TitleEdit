using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using TitleEdit.Utility;

namespace TitleEdit.Windows.Tabs
{
    internal class SettingsTab : AbstractTab
    {
        public override string Title => "設定";

        public override void Draw()
        {
            base.Draw();
            if (SettingCheckbox($"追蹤玩家位置##Track player location {Title}", ref Services.ConfigurationService.TrackPlayerLocation))
            {
                Services.LayoutService.SettingsUpdated();
            }

            ImGuiComponents.HelpMarker("持續追蹤玩家位置，讓角色顯示在上次登出的位置。");
            using (ImRaii.Disabled(!Services.ConfigurationService.TrackPlayerLocation))
            {
                SettingCheckbox($"定期儲存##Save periodically", ref Services.ConfigurationService.PeriodicSaving);
                using (ImRaii.Enabled()) ImGuiComponents.HelpMarker("定期儲存資料，以避免遊戲崩潰或程序意外結束時遺失進度。");

                ImGui.SetNextItemWidth(6f * ImGui.GetFontSize());
                var savePeriod = Services.ConfigurationService.SavePeriod;
                if (ImGui.InputInt($"儲存間隔（秒）##Save period in seconds", ref savePeriod))
                {
                    Services.ConfigurationService.SavePeriod = Math.Max(1, savePeriod);
                    Services.ConfigurationService.Save();
                }

                using var color = ImRaii.PushColor(ImGuiCol.Text, GuiUtils.WarningColor);
                if (SettingCheckbox($"實驗性功能：儲存場景配置##Experimental Save layout", ref Services.ConfigurationService.SaveLayout))
                {
                    Services.LayoutService.SettingsUpdated();
                    Services.LocationService.LayoutSettingsUpdated();
                }

                color.Pop();
                using (ImRaii.Enabled()) ImGuiComponents.HelpMarker("儲存世界場景配置的實驗性功能（例如推進主線任務後產生的變化）。\n在野外通常可以正常運作，但在副本內較容易發生問題。");
                color.Push(ImGuiCol.Text, GuiUtils.WarningColor);
                if (SettingCheckbox($"實驗性功能：在副本內儲存場景配置##Experimental Save layout in instance", ref Services.ConfigurationService.SaveLayoutInInstance))
                {
                    Services.LayoutService.SettingsUpdated();
                    Services.LocationService.LayoutSettingsUpdated();
                }

                color.Pop();
                using (ImRaii.Enabled()) ImGuiComponents.HelpMarker("場景發生變化後重新掃描配置，可能使效能較低的系統短暫卡頓。\n你可以只關閉副本內的這項功能。");
                color.Push(ImGuiCol.Text, GuiUtils.WarningColor);
                if (SettingCheckbox($"實驗性功能：儲存房屋配置##Experimental Save housing layout", ref Services.ConfigurationService.SaveHousing))
                {
                    Services.LayoutService.SettingsUpdated();
                    Services.LocationService.LayoutSettingsUpdated();
                }

                color.Pop();

                SettingCheckbox($"儲存坐騎##Save mount", ref Services.ConfigurationService.SaveMount);
                SettingCheckbox($"儲存背景音樂##Save song", ref Services.ConfigurationService.SaveBgm);
                SettingCheckbox($"儲存艾歐澤亞時間##Save Eorzea time", ref Services.ConfigurationService.SaveTime);
                ImGuiComponents.HelpMarker("若未勾選，將使用目前遊戲內時間，而不是登出時的時間。");
            }

            ImGui.Separator();
            SettingCheckbox($"切換至使用相同背景音樂的場景時不要重新播放##Don't restart BGM", ref Services.ConfigurationService.DontInterruptMusicOnSceneSwitch);
            SettingCheckbox($"啟用季節彩蛋##Enable seasonal easter eggs", ref Services.ConfigurationService.SeasonalEasterEggs);
            ImGui.Separator();
            SettingCheckbox($"在角色選擇畫面加入開啟此視窗的按鈕##Character select button", ref Services.ConfigurationService.DrawCharacterSelectButton);
            SettingCheckbox($"載入時顯示目前畫面的名稱##Display current screen name", ref Services.ConfigurationService.DisplayTitleToast);
            if (SettingCheckbox($"在角色選擇畫面隱藏角色名稱##Hide character names", ref Services.ConfigurationService.HideCharacterSelectNames))
            {
                Services.LobbyService.HideCharacterSelectNamesSettingUpdated();
            }

            ImGuiComponents.HelpMarker("分享角色選擇畫面預設時可用來隱藏角色名稱。");
            ImGui.Separator();
            SettingCheckbox($"使用目前登入角色的名稱作為預設作者##Use character name as author", ref Services.ConfigurationService.UseCharacterNameAsAuthor);
            using (ImRaii.Disabled(Services.ConfigurationService.UseCharacterNameAsAuthor))
                if (ImGui.InputText($"預設作者名稱##Default author name", ref Services.ConfigurationService.DefaultAuthorName, 256))
                    Services.ConfigurationService.Save();
            SettingCheckbox($"隱藏內建預設##Hide built in presets", ref Services.ConfigurationService.HideBuiltInPresets);
            ImGuiComponents.HelpMarker("這也會將它們從隨機選取範圍中移除。");
            SettingCheckbox($"隱藏原版預設##Hide vanilla presets", ref Services.ConfigurationService.HideVanillaPresets);
            ImGuiComponents.HelpMarker("這也會將它們從隨機選取範圍中移除。");
        }

        private bool SettingCheckbox(string label, ref bool value, bool save = true)
        {
            if (ImGui.Checkbox(label, ref value))
            {
                if (save) Services.ConfigurationService.Save();
                return true;
            }

            return false;
        }
    }
}
