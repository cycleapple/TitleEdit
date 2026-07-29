using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using Dalamud.Bindings.ImGui;
using TitleEdit.Data.Lobby;
using TitleEdit.Data.Persistence;
using TitleEdit.Utility;

namespace TitleEdit.Windows.Tabs
{
    internal class DisplayTypeTab : AbstractTab
    {
        public override string Title => "顯示";

        private ulong currentContentId = 0;

        public override void Draw()
        {
            base.Draw();
            ImGui.TextUnformatted("標題畫面");
            DrawSelectPresetCombo($"標題畫面設定##Title screen setting", Services.ConfigurationService.TitleDisplayTypeOption, (result) =>
            {
                Services.ConfigurationService.TitleDisplayTypeOption = result;
                Services.LobbyService.ReloadTitleScreen();
            });
            if (GuiUtils.Combo($"預設標題畫面標誌##Default title screen logo", ref Services.ConfigurationService.TitleScreenLogo, filter: (entry) => entry != TitleScreenLogo.Unspecified))
            {
                Services.ConfigurationService.Save();
                Services.LobbyService.ReloadTitleScreenUi();
            }

            if (ImGui.Checkbox($"覆寫預設中的標題畫面標誌設定##Override preset title screen logo", ref Services.ConfigurationService.OverridePresetTitleScreenLogo))
            {
                Services.ConfigurationService.Save();
                Services.LobbyService.ReloadTitleScreenUi();
            }

            if (GuiUtils.DrawUiColorPicker("預設標題畫面色彩##Default title screen colors", ref Services.ConfigurationService.TitleScreenColor))
            {
                Services.ConfigurationService.Save();
                Services.LobbyService.RecolorTitleScreenUi();
            }

            if (ImGui.Checkbox($"覆寫預設中的標題畫面色彩##Override preset title screen colors", ref Services.ConfigurationService.OverridePresetTitleScreenColor))
            {
                Services.ConfigurationService.Save();
                Services.LobbyService.RecolorTitleScreenUi();
            }

            if (GuiUtils.Combo($"預設標題畫面影片##Default title screen movie", ref Services.ConfigurationService.TitleScreenMovie, filter: (entry) => entry != TitleScreenMovie.Unspecified))
            {
                Services.ConfigurationService.Save();
            }

            ImGuiComponents.HelpMarker("設定在標題畫面閒置 60 秒後播放的開場影片。");
            if (ImGui.Checkbox($"覆寫預設中的標題畫面影片設定##Override preset title screen movie", ref Services.ConfigurationService.OverridePresetTitleScreenMovie))
            {
                Services.ConfigurationService.Save();
            }

            ImGui.Separator();
            ImGui.TextUnformatted("角色選擇畫面");
            DrawSelectPresetCombo($"未選擇角色時的設定##Nothing selected setting", Services.ConfigurationService.NoCharacterDisplayType, (result) =>
            {
                Services.ConfigurationService.NoCharacterDisplayType = result;
                unsafe
                {
                    if (Services.LobbyService.CurrentLobbyMap == GameLobbyType.CharaSelect &&
                        Services.LobbyService.CurrentContentId == 0)
                    {
                        Services.LobbyService.ReloadCharacterSelect();
                    }
                }
            }, false);
            ImGuiComponents.HelpMarker("未選擇任何角色時使用的預設。");
            DrawSelectPresetCombo($"所有角色的共用設定##Global character setting", Services.ConfigurationService.GlobalDisplayType, (result) =>
            {
                Services.ConfigurationService.GlobalDisplayType = result;

                unsafe
                {
                    if (Services.LobbyService.CurrentLobbyMap == GameLobbyType.CharaSelect &&
                        Services.LobbyService.CurrentContentId != 0 &&
                        !Services.ConfigurationService.DisplayTypeOverrides.Exists(displayOverride => displayOverride.Key == Services.LobbyService.CurrentContentId))
                    {
                        Services.LobbyService.ReloadCharacterSelect();
                    }
                }
            });


            if (GuiUtils.Combo($"鏡頭跟隨模式##Camera follow mode", ref Services.ConfigurationService.CameraFollowMode, filter: (mode) => mode != CameraFollowMode.Inherit))
            {
                Services.ConfigurationService.Save();
            }

            ImGui.Separator();
            ImGui.TextUnformatted("個別角色覆寫");
            var usedIds = new HashSet<ulong>(Services.ConfigurationService.DisplayTypeOverrides.Count);
            var tableHeight = MathF.Max(1f, MathF.Min(8f, Services.ConfigurationService.DisplayTypeOverrides.Count) * (ImGui.CalcTextSize(" ").Y + ImGui.GetStyle().CellPadding.Y * 2 + ImGui.GetStyle().FramePadding.Y * 2));

            using (var table = ImRaii.Table($"Display Type Overrides", 4, ImGuiTableFlags.ScrollY, new(0, tableHeight)))
            {
                if (table)
                {
                    ImGui.TableSetupColumn("1", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("2", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("3", ImGuiTableColumnFlags.WidthFixed);
                    //Empty column for setting scroll bar spacing
                    ImGui.TableSetupColumn("scrollbar_spacing", ImGuiTableColumnFlags.WidthFixed, ImGui.GetStyle().ScrollbarSize);
                    for (int i = 0; i < Services.ConfigurationService.DisplayTypeOverrides.Count; i++)
                    {
                        using var id = ImRaii.PushId(i);
                        var entry = Services.ConfigurationService.DisplayTypeOverrides[i];
                        usedIds.Add(entry.Key);
                        ImGui.TableNextColumn();
                        var label = Services.CharactersService.Characters.TryGetValue(entry.Key, out var charName) ? charName : entry.Key.ToString("X");
                        ImGui.TextUnformatted(label);
                        ImGui.TableNextColumn();

                        ImGui.SetNextItemWidth(16f * ImGui.GetFontSize());
                        DrawSelectPresetCombo($"##Character override", entry.Value, (result) =>
                        {
                            Services.ConfigurationService.DisplayTypeOverrides[i] = new(entry.Key, result);
                            unsafe
                            {
                                if (Services.LobbyService.CurrentLobbyMap == GameLobbyType.CharaSelect &&
                                    Services.LobbyService.CurrentContentId != 0 &&
                                    entry.Key == Services.LobbyService.CurrentContentId)
                                {
                                    Services.LobbyService.ReloadCharacterSelect();
                                }
                            }
                        });
                        ImGui.TableNextColumn();
                        if (ImGuiComponents.IconButton($"##Delete", FontAwesomeIcon.Trash))
                        {
                            Services.ConfigurationService.DisplayTypeOverrides.RemoveRange(i, 1);
                            Services.ConfigurationService.Save();
                            i--;
                            unsafe
                            {
                                if (Services.LobbyService.CurrentLobbyMap == GameLobbyType.CharaSelect &&
                                    Services.LobbyService.CurrentContentId != 0 &&
                                    !Services.ConfigurationService.DisplayTypeOverrides.Exists(displayOverride => displayOverride.Key == Services.LobbyService.CurrentContentId))
                                {
                                    Services.LobbyService.ReloadCharacterSelect();
                                }
                            }
                        }

                        ImGui.TableNextColumn();
                    }
                }
            }

            var characterLabel = Services.CharactersService.Characters.GetValueOrDefault(currentContentId, "");
            GuiUtils.FilterCombo($"##Character override", characterLabel, () =>
            {
                bool empty = true;
                foreach (var entry in Services.CharactersService.Characters)
                {
                    if (usedIds.Contains(entry.Key)) continue;
                    empty = false;
                    using var id = ImRaii.PushId(entry.Key.ToString());
                    if (GuiUtils.FilterSelectable(entry.Value, entry.Key == currentContentId))
                    {
                        currentContentId = entry.Key;
                        return true;
                    }
                }

                if (empty)
                {
                    ImGui.TextUnformatted("所有角色均已設定");
                }

                return false;
            });
            ImGuiComponents.HelpMarker("這些角色資料是從角色選擇畫面收集而來。\n若有角色遺漏，請依序切換至所有建立過角色的世界。");
            ImGui.SameLine();
            using var disabled = ImRaii.Disabled(currentContentId == 0);
            if (ImGuiComponents.IconButton($"##New", FontAwesomeIcon.Plus))
            {
                Services.ConfigurationService.DisplayTypeOverrides.Add(new(currentContentId, new() { Type = CharacterDisplayType.LastLocation }));
                Services.ConfigurationService.Save();

                unsafe
                {
                    if (Services.LobbyService.CurrentLobbyMap == GameLobbyType.CharaSelect &&
                        Services.LobbyService.CurrentContentId != 0 &&
                        currentContentId == Services.LobbyService.CurrentContentId)
                    {
                        Services.LobbyService.ReloadCharacterSelect();
                    }
                }

                currentContentId = 0;
            }
        }


        private void DrawSelectPresetCombo(string label, CharacterDisplayTypeOption value, Action<CharacterDisplayTypeOption> selectAction, bool showLastLocationOption = true)
        {
            string display;
            bool invalid = false;
            if (value.Type == CharacterDisplayType.LastLocation)
            {
                display = "上次所在位置";
            }
            else if (value.Type == CharacterDisplayType.Preset)
            {
                if (value.PresetPath != null && Services.PresetService.TryGetPreset(value.PresetPath, out var preset, LocationType.CharacterSelect))
                {
                    display = preset.Name;
                }
                else
                {
                    display = "無效的預設";
                    invalid = true;
                }
            }
            else if (value.Type == CharacterDisplayType.Random)
            {
                if (value.PresetPath != null && Services.GroupService.TryGetGroup(value.PresetPath, out var group, LocationType.CharacterSelect))
                {
                    display = $"[G] {group.Name}";
                }
                else
                {
                    {
                        display = "無效的群組";
                        invalid = true;
                    }
                }
            }
            else
            {
                display = "發生未知錯誤";
                invalid = true;
            }

            if (invalid)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, GuiUtils.WarningColor);
            }

            GuiUtils.FilterCombo(label, display, () =>
            {
                if (showLastLocationOption)
                {
                    if (GuiUtils.FilterSelectable("上次所在位置##Last location", value.Type == CharacterDisplayType.LastLocation))
                    {
                        selectAction.Invoke(new()
                        {
                            Type = CharacterDisplayType.LastLocation
                        });
                        Services.ConfigurationService.Save();
                        return true;
                    }

                    GuiUtils.DrawDisplayTypeTooltip(CharacterDisplayType.LastLocation);
                }

                GuiUtils.FilterSeperator();


                foreach (var entry in Services.PresetService.CharacterSelectPresetEnumerator)
                {
                    if (entry.Value.Vanilla && Services.ConfigurationService.HideVanillaPresets) continue;
                    if (entry.Value.BuiltIn && Services.ConfigurationService.HideBuiltInPresets) continue;
                    using var id = ImRaii.PushId(entry.Key);
                    if (GuiUtils.FilterSelectable(entry.Value.Name, value.Type == CharacterDisplayType.Preset && entry.Key == value.PresetPath))
                    {
                        selectAction.Invoke(new()
                        {
                            Type = CharacterDisplayType.Preset,
                            PresetPath = entry.Key
                        });
                        Services.ConfigurationService.Save();
                        return true;
                    }

                    GuiUtils.DrawDisplayTypeTooltip(CharacterDisplayType.Preset, entry.Key);
                }

                foreach (var entry in Services.GroupService.CharacterSelectGroupEnumerator)
                {
                    using var id = ImRaii.PushId(entry.Key);
                    if (GuiUtils.FilterSelectable($"[G] {entry.Value.Name}", value.Type == CharacterDisplayType.Random && entry.Key == value.PresetPath))
                    {
                        selectAction.Invoke(new()
                        {
                            Type = CharacterDisplayType.Random,
                            PresetPath = entry.Key
                        });
                        Services.ConfigurationService.Save();
                        return true;
                    }

                    GuiUtils.DrawDisplayTypeTooltip(CharacterDisplayType.Random, entry.Key);
                }

                return false;
            }, popStyleColor: invalid);
            GuiUtils.DrawDisplayTypeTooltip(value);
        }

        private void DrawSelectPresetCombo(string label, TitleDisplayTypeOption value, Action<TitleDisplayTypeOption> selectAction)
        {
            string display;
            bool invalid = false;
            if (value.Type == TitleDisplayType.Preset)
            {
                if (value.PresetPath != null && Services.PresetService.TryGetPreset(value.PresetPath, out var preset, LocationType.TitleScreen))
                {
                    display = preset.Name;
                }
                else
                {
                    display = "無效的預設";
                    invalid = true;
                }
            }
            else if (value.Type == TitleDisplayType.Random)
            {
                if (value.PresetPath != null && Services.GroupService.TryGetGroup(value.PresetPath, out var group, LocationType.TitleScreen))
                {
                    display = $"[G] {group.Name}";
                }
                else
                {
                    {
                        display = "無效的群組";
                        invalid = true;
                    }
                }
            }
            else
            {
                display = "發生未知錯誤";
                invalid = true;
            }

            if (invalid)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, GuiUtils.WarningColor);
            }

            GuiUtils.FilterCombo(label, display, () =>
            {
                foreach (var entry in Services.PresetService.TitleScreenPresetEnumerator)
                {
                    if (entry.Value.Vanilla && Services.ConfigurationService.HideVanillaPresets) continue;
                    if (entry.Value.BuiltIn && Services.ConfigurationService.HideBuiltInPresets) continue;
                    using var id = ImRaii.PushId(entry.Key);
                    if (GuiUtils.FilterSelectable(entry.Value.Name, value.Type == TitleDisplayType.Preset && entry.Key == value.PresetPath))
                    {
                        selectAction.Invoke(new()
                        {
                            Type = TitleDisplayType.Preset,
                            PresetPath = entry.Key
                        });
                        Services.ConfigurationService.Save();
                        return true;
                    }

                    GuiUtils.DrawDisplayTypeTooltip(TitleDisplayType.Preset, entry.Key);
                }

                foreach (var entry in Services.GroupService.TitleScreenGroupEnumerator)
                {
                    using var id = ImRaii.PushId(entry.Key);
                    if (GuiUtils.FilterSelectable($"[G] {entry.Value.Name}", value.Type == TitleDisplayType.Random && entry.Key == value.PresetPath))
                    {
                        selectAction.Invoke(new()
                        {
                            Type = TitleDisplayType.Random,
                            PresetPath = entry.Key
                        });
                        Services.ConfigurationService.Save();
                        return true;
                    }

                    GuiUtils.DrawDisplayTypeTooltip(TitleDisplayType.Random, entry.Key);
                }

                return false;
            }, popStyleColor: invalid);
            GuiUtils.DrawDisplayTypeTooltip(value);
        }
    }
}
