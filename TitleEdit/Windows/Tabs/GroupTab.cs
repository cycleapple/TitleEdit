using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using TitleEdit.Data.Persistence;
using TitleEdit.Utility;

namespace TitleEdit.Windows.Tabs
{
    internal class GroupTab : AbstractTab
    {
        public override string Title => "群組";

        bool makingNewGroup = false;
        string currentGroup = "";
        GroupModel group = new();

        public override void Draw()
        {
            base.Draw();
            DrawGroupListControls();
            ImGui.Separator();
            DrawGroupControls();
            DrawGroupActions();
        }

        private void DrawGroupListControls()
        {
            string currentGroupName;
            if (makingNewGroup)
            {
                currentGroupName = group.LocationType switch
                {
                    LocationType.TitleScreen => "尚未儲存的標題畫面群組",
                    LocationType.CharacterSelect => "尚未儲存的角色選擇畫面群組",
                    _ => throw new Exception()
                };
                ImGui.PushStyleColor(ImGuiCol.Text, GuiUtils.WarningColor);
            }
            else if (currentGroup == "")
            {
                currentGroupName = "";
            }
            else
            {
                currentGroupName = GetGroupEntryName(Services.GroupService.Groups.GetValueOrDefault(currentGroup));
            }

            bool stylePopped = false;

            GuiUtils.FilterCombo($"##groupCombo", currentGroupName, () =>
            {
                if (makingNewGroup)
                {
                    ImGui.PopStyleColor();
                    stylePopped = true;
                }

                foreach (var entry in Services.GroupService.EditableGroupEnumerator)
                {
                    using var id = ImRaii.PushId(entry.Key);
                    if (GuiUtils.FilterSelectable(GetGroupEntryName(entry.Value), entry.Key == currentGroup))
                    {
                        SelectGroup(entry.Key);
                        return true;
                    }

                    GuiUtils.DrawGroupTooltip(entry.Key);
                }

                return false;
            });
            if (makingNewGroup && !stylePopped)
            {
                ImGui.PopStyleColor();
            }

            ImGui.SameLine();
            ImGui.BeginDisabled(makingNewGroup || currentGroup == "");
            if (ImGuiComponents.IconButton($"##Duplicate", FontAwesomeIcon.Copy))
            {
                DuplicateGroup(currentGroup);
            }

            ImGui.EndDisabled();
            GuiUtils.HoverTooltip("複製群組");
            ImGui.BeginDisabled(makingNewGroup);
            if (ImGui.Button($"新增標題畫面群組##New Title Screen group"))
            {
                NewGroup(LocationType.TitleScreen);
            }

            ImGui.SameLine();
            if (ImGui.Button($"新增角色選擇畫面群組##New Character Select group"))
            {
                NewGroup(LocationType.CharacterSelect);
            }

            ImGui.EndDisabled();
        }

        private void DrawGroupControls()
        {
            ImGui.BeginDisabled(!makingNewGroup && currentGroup == "");

            ImGui.InputText($"名稱##Group Name", ref group.Name, 256);
            var tableHeight = MathF.Max(1f, MathF.Min(8f, group.PresetFileNames.Count) * (ImGui.CalcTextSize(" ").Y + ImGui.GetStyle().CellPadding.Y * 2 + ImGui.GetStyle().FramePadding.Y * 2));
            using (var table = ImRaii.Table($"群組項目##Group entries", 2, ImGuiTableFlags.ScrollY, new(0f, tableHeight)))
            {
                if (table)
                {
                    var usedPresets = new HashSet<string?>(group.PresetFileNames); // performance?
                    for (int i = 0; i < group.PresetFileNames.Count; i++)
                    {
                        using var id = ImRaii.PushId(i);
                        var presetPath = group.PresetFileNames[i];
                        ImGui.TableNextColumn();

                        ImGui.SetNextItemWidth(16f * ImGui.GetFontSize());
                        //
                        string display;
                        bool invalid = false;
                        if (presetPath == null)
                        {
                            display = "";
                        }
                        else if (Services.PresetService.TryGetPreset(presetPath, out var preset, group.LocationType))
                        {
                            display = preset.Name;
                        }
                        else
                        {
                            display = "無效的預設";
                            invalid = true;
                        }

                        if (invalid)
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, GuiUtils.WarningColor);
                        }

                        GuiUtils.FilterCombo("", display, () =>
                        {
                            foreach (var entry in Services.PresetService.Presets.Where(preset => preset.Value.LocationModel.LocationType == group.LocationType && !group.PresetFileNames.Contains(preset.Key)))
                            {
                                using var id = ImRaii.PushId(entry.Key);
                                if (GuiUtils.FilterSelectable(entry.Value.Name, entry.Key == presetPath))
                                {
                                    group.PresetFileNames[i] = entry.Key;
                                    return true;
                                }

                                GuiUtils.DrawPresetTooltip(entry.Key, group.LocationType);
                            }

                            return false;
                        }, popStyleColor: invalid);
                        if (presetPath != null)
                        {
                            GuiUtils.DrawPresetTooltip(presetPath, group.LocationType);
                        }

                        //
                        ImGui.TableNextColumn();
                        if (ImGuiComponents.IconButton($"##Delete", FontAwesomeIcon.Trash))
                        {
                            group.PresetFileNames.RemoveRange(i, 1);
                        }
                    }
                }
            }

            if (ImGui.Button($"加入預設##Add preset"))
            {
                group.PresetFileNames.Add(null);
            }

            ImGui.EndDisabled();
        }

        private void DrawGroupActions()
        {
            ImGui.BeginDisabled(!makingNewGroup && currentGroup == "");
            if (ImGui.Button($"儲存##Save group"))
            {
                try
                {
                    currentGroup = Services.GroupService.Save(group);
                    makingNewGroup = false;
                    group = Services.GroupService.Groups[currentGroup];
                }
                catch (Exception ex)
                {
                    SetupError(ex);
                }
            }

            ImGui.SameLine();
            if (makingNewGroup)
            {
                if (ImGui.Button($"取消##Cancel group"))
                {
                    ClearGroup();
                }
            }
            else
            {
                if (ImGui.Button($"刪除##Delete group"))
                {
                    SetupDeleteConfirmation();
                }
            }

            ImGui.EndDisabled();
        }

        private string GetGroupEntryName(GroupModel group)
        {
            var prefix = group.LocationType switch
            {
                LocationType.CharacterSelect => "C",
                LocationType.TitleScreen => "T",
                _ => "?"
            };
            return $"[{prefix}] {group.Name}";
        }

        public void SelectGroup(string groupFileName)
        {
            if (Services.GroupService.Groups.TryGetValue(groupFileName, out var groupModel))
            {
                group = groupModel;
                currentGroup = groupFileName;
                makingNewGroup = false;
            }
        }

        public void ClearGroup()
        {
            group = new();
            makingNewGroup = false;
            currentGroup = "";
        }

        public void DuplicateGroup(string groupFileName)
        {
            if (Services.GroupService.Groups.TryGetValue(groupFileName, out var groupModel))
            {
                group = groupModel;
                makingNewGroup = true;
                currentGroup = "";
                group.FileName = "";
            }
        }

        public void NewGroup(LocationType type)
        {
            makingNewGroup = true;
            currentGroup = "";
            group = new();
            group.PresetFileNames.Add(null);
            group.LocationType = type;
        }

        private void SetupDeleteConfirmation() => SetupModal("刪除群組", DrawDeleteConfirmation);

        private void DrawDeleteConfirmation()
        {
            ImGui.TextUnformatted("確定要刪除此群組嗎？此操作無法復原。");
            if (ImGui.Button("是##Confirm delete group"))
            {
                modal = false;
                try
                {
                    Services.GroupService.Delete(currentGroup);
                    ClearGroup();
                }
                catch (Exception ex)
                {
                    SetupError(ex);
                }
            }

            ImGui.SameLine();
            if (ImGui.Button("否##Cancel delete group")) modal = false;
        }
    }
}
