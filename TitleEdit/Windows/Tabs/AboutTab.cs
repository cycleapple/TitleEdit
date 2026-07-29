using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Linq;
using TitleEdit.Utility;

namespace TitleEdit.Windows.Tabs
{
    internal class AboutTab : AbstractTab
    {
        public override string Title => "關於";

        public override void Draw()
        {
            base.Draw();
            using var textWrapPos = ImRaii.TextWrapPos(ImGui.GetFontSize() * 28);
            ImGui.TextWrapped("歡迎使用 Title Edit V3，這是原始插件的完整重製版本。");
            ImGui.TextWrapped("若遇到「已知問題」中未列出的狀況，或有可協助釐清問題的資訊，可透過插件安裝器的意見回饋按鈕，或 Dalamud Discord 的 #plugin-help-forum 中 Title Edit 貼文回報。");

            ImGui.TextWrapped("若想尋找更多預設，可前往 Dalamud 官方 Discord 的 #preset-sharing 頻道。");

            if (ImGui.CollapsingHeader($"操作提示##Tips and tricks {Title}"))
            {
                WrappedBulletText("大多數滑桿／拖曳輸入可用 Ctrl＋點擊手動輸入數值");
                WrappedBulletText("按住 Alt 拖曳可緩慢調整數值");
                WrappedBulletText("按住 Shift 拖曳可快速調整數值");
                WrappedBulletText("未登入角色時可按 Ctrl＋T 開啟此視窗");
            }

            if (ImGui.CollapsingHeader($"已知問題##Known Issues {Title}"))
            {
                ImGui.TextWrapped("除非另有說明，以下問題只會影響角色選擇或標題畫面。");
                WrappedBulletText("在住宅區時，世界與角色的載入時間較長");
                WrappedBulletText("部分背景音樂不會循環播放");
                WrappedBulletText("啟用實驗性場景配置後，部分 VFX 物件會在每次載入場景時重新播放，副本內尤其常見");
                WrappedBulletText("實驗性場景配置儲存功能整體仍可能不穩定");
            }

            if (ImGui.CollapsingHeader($"規劃中的功能##Planned Features {Title}"))
            {
                ImGui.TextWrapped("以下功能尚未確認可行性，也沒有預定完成日期，僅為作者認為值得加入的構想。");
                WrappedBulletText("角色選擇畫面的寵物支援");
                WrappedBulletText("角色選擇畫面的時尚配飾支援");
                WrappedBulletText("角色選擇畫面的情感動作支援");
                WrappedBulletText("可開關地圖中特定部分的場景配置編輯器");
                WrappedBulletText("預設的季節活動選擇器");
            }

            if (ImGui.CollapsingHeader($"製作人員##Credits {Title}"))
            {
                WrappedBulletText("Speedas - initial Dawntrail update, full plugin rewrite to 3.0");
                WrappedBulletText("attick - Title Edit 1.0 and many functions of 2.0");
                WrappedBulletText("perchbird - Custom title screens and supporting features, maintaining the plugin before Dawntrail");
                WrappedBulletText("ff-meli - BGM now playing code");
                WrappedBulletText("goat - being a caprine individual");
            }
        }

        private void WrappedBulletText(string text)
        {
            ImGui.Bullet();
            ImGui.TextWrapped(text);
        }
    }
}
