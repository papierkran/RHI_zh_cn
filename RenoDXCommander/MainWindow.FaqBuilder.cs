// MainWindow.FaqBuilder.cs — Builds the FAQ/Quick Start guide content dynamically.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace RenoDXCommander;

public sealed partial class MainWindow
{
    private bool _faqBuilt;

    /// <summary>
    /// Builds the FAQ panel content. Called once when FAQ is first opened.
    /// </summary>
    private void BuildFaqContent()
    {
        if (_faqBuilt) return;
        _faqBuilt = true;

        var panel = FaqContentPanel;
        panel.Children.Clear();

        // Welcome section
        panel.Children.Add(BuildFaqSection(
            null, "欢迎使用 RHI", "AccentTealBrush",
            "RHI 会自动检测您的游戏，让您在一个地方安装 HDR 模组、着色器、帧率限制器，并管理 NVIDIA 驱动设置。以下是入门指南。",
            null));

        // Step 1: Select a Game
        panel.Children.Add(BuildFaqStep(1,
            "选择游戏",
            "您的游戏列在左侧边栏中。单击任意游戏可查看其详细信息和可用操作。使用筛选标签（全部游戏、已安装、Unreal 等）和搜索框查找特定游戏。",
            "提示：双击游戏可直接启动。将游戏的 .exe 文件拖放到 RHI 上可添加未自动检测到的游戏。"));

        // Step 2: Install ReShade
        panel.Children.Add(BuildFaqStep(2,
            "安装 ReShade",
            "RenoDX HDR 模组需要 ReShade 才能工作。在游戏详情面板上单击“安装 ReShade”。RHI 会自动下载并安装具有完整附加组件支持的正确版本。",
            "可以通过游戏覆盖部分为每个游戏更改 ReShade 版本 — 选择 Stable、Nightly、Legacy 或自定义 ReShade DLL。\nVulkan 游戏：Vulkan 游戏（如 Doom Eternal）需要管理员权限。RHI 会在需要时提示提升权限。\n将 ReShade 预设文件（.ini）拖放到游戏上即可自动安装。"));

        // Step 3a: RenoDX
        panel.Children.Add(BuildFaqSection(
            "3a", "RenoDX", "AccentTealBrush",
            "RenoDX 是 RHI 的主要 HDR 模组框架。每个游戏上的 RenoDX 行显示可用内容：\n\n• 命名模组 — 来自 RenoDX 维基、由社区制作的游戏专用 HDR 模组。\n• UE-Extended — 适用于没有命名模组的虚幻引擎游戏。通过通用 UE 附加组件提供原生 HDR 输出。\n• Unity 附加组件 — 同理，适用于 Unity 引擎游戏。为没有命名模组的 Unity 游戏提供 HDR 输出。\n• RTX HDR — 如果没有可用模组，可以通过 RenoDX ⚙ 齿轮启用 NVIDIA 驱动级的 SDR 转 HDR 转换。",
            "RenoDX 旁边的齿轮图标可打开高级设置：峰值尼特、UE-Extended 开关和 RTX HDR 配置。\nEngine.ini 设置（仅虚幻引擎游戏）：切换写入游戏 Engine.ini 的 HDR 键和 LUT 更新频率，以实现准确的 HDR 渲染。\n对于不在维基上的游戏，可将 RenoDX Discord 中的 .addon64 文件直接拖放到 RHI 中的游戏上。"));

        // Step 3b: Luma
        panel.Children.Add(BuildFaqSection(
            "3b", "Luma", "AccentTealBrush",
            "Luma 是由 Pumbo（HDR Den）开发的替代模组框架。根据游戏的不同，Luma 模组可能会添加 HDR、DLAA（深度学习抗锯齿）、游戏专用渲染修复或它们的组合 — 查看“信息”按钮了解每个模组提供的功能。\n\n• 已完成的模组 — 受支持游戏的命名 Luma 模组，显示在 Luma 行上。\n• 通用 Luma — 适用于所有 DX11 虚幻引擎游戏。RHI 会自动安装它，并应用 Luma 维基上列出的任何游戏专用 Engine.ini 调整或启动参数。\n\n您可以在同一个游戏上安装 RenoDX 和 Luma — 但不能保证它们在每个游戏上都能协同工作。首次尝试同时安装两者时，RHI 会向您发出警告。",
            "Luma 需要 ReShade — 在安装 ReShade 之前它将呈灰色不可用。\nLuma ⚙ 齿轮可让您为需要 TAA 的游戏切换 Engine.ini 设置。\n如果游戏需要特定的启动参数（例如 -dx11），RHI 会在安装时自动设置，并在卸载时移除。启动参数仅在通过 RHI 启动游戏时生效。\n查看 Luma 行上的“信息”按钮了解游戏特定说明 — 已完成的模组通常会包含模组添加内容或所需游戏内设置的详细信息。"));

        // Step 4: Choose Shaders
        panel.Children.Add(BuildFaqStep(4,
            "选择着色器（可选）",
            "单击工具栏中的“着色器/附加组件”按钮，然后选择“全局着色器”以选择着色器包。默认选中 Lilium 的 HDR 着色器包。这些适用于所有已安装 ReShade 的游戏。\n\n展开任意包以挑选单个着色器 — 当只选择部分文件时，包会显示一个短划线。使用右侧的配置文件面板保存、加载、重命名和共享命名的着色器选择。将配置文件导出为 zip 以便通过 Discord 共享。",
            "提示：可以使用每个游戏详情卡上的“着色器”按钮设置游戏专用着色器（已安装 ReShade 时）。\n提示：使用“全部展开”/“全部折叠”一次性浏览所有包。“全部取消选择”会清空整个选择。“导出”会将所选着色器的 zip 复制到剪贴板 — 直接粘贴到 Discord 中即可共享。"));

        // Step 5: DOF Fix
        panel.Children.Add(BuildFaqStep(5,
            "DOF Fix（推荐用于 UE5）",
            "DOF Fix 将虚幻引擎 5.7 的景深渲染修复向后移植到运行 UE 5.0–5.6 的游戏。在支持时，它会出现在游戏详情面板的“推荐”部分。请与 RenoDX 一起安装以获得最佳效果。",
            "DOF Fix 仅显示在符合条件的 UE 5.0–5.6 游戏上 — 不会出现在 UE4 或 UE 5.7+ 游戏中。\n安装和卸载与其他任何组件相同。"));

        // Step 6: Frame Limiters
        panel.Children.Add(BuildFaqStep(6,
            "帧率限制器（可选）",
            "ReLimiter 和 Display Commander 是 ReShade 附加组件，可为 VRR 显示器提供精确的帧率限制。从游戏详情面板安装它们。推荐使用 ReLimiter，因为它由与 RHI 相同的团队开发。在设置中、通过齿轮图标为每个游戏设置，或直接在游戏内设置目标 FPS。",
            "VRR cap presets by refresh rate (leave headroom below max for smooth VRR):\n• 60Hz → 59 FPS\n• 120Hz → 116 FPS\n• 144Hz → 138 FPS\n• 165Hz → 157 FPS\n• 240Hz → 224 FPS\n• 360Hz → 324 FPS\nThese values are pre-configured in RHI's FPS dropdown menus."));

        // Step 7: DLSS/Streamline
        panel.Children.Add(BuildFaqStep(7,
            "更新 DLSS / Streamline（可选）",
            "带有 DLSS 或 Streamline DLL 的游戏在详情面板上有专门的区域显示版本信息。单击以更新到最新版本。建议使用最新版本以获得最佳性能和质量。RHI 会自动备份原始文件，因此您随时可以恢复。",
            "当新的 DLSS 或 Streamline 版本发布时，它们会自动出现在 RHI 中。在设置中设置默认的 DLSS 预设。可以在每个游戏的详情面板的 DLSS 区域中更改游戏专用预设。"));

        // OptiScaler
        panel.Children.Add(BuildFaqSpecialSection("⚙", "AccentAmberBrush",
            "OptiScaler（可选）",
            "OptiScaler replaces DLSS/XeSS with alternative upscalers (FSR, XeSS, Intel Arc) or adds/patches frame generation on any GPU. Install it from the game's detail panel when a game has OptiScaler support.\n\nThe ⚙ cog on the OptiScaler row opens per-game settings. For the Nightly build channel these include:\n• Streamline/DLSS Enabler — deploys Streamline and DLSS Enabler to the game folder for DLSS Frame Generation support.\n• Frame Generation — set FG 输入, FG 输出, FG Nvngx 覆盖, and HUD Fix.\n• 其他设置 — DLSS SR/RR preset, render scale, and flip metering.\n• Presets — save and apply named setting presets across games.\n• Engine.ini 设置 (Unreal Engine games) — 膨胀运动矢量, FSR 崩溃修复, FSR-FG 交换链, 超分辨率插件.",
            "在齿轮中为每个游戏在 Stable 和 Nightly 通道之间切换 — Nightly 添加帧生成和其他设置。\nOptiScaler 和 ReShade 可以共存。如果两者都安装后出现崩溃，请尝试使用游戏覆盖面板中的 DLL 命名覆盖将 ReShade 重命名为其他 DLL 名称。\nGPU 类型和 DLSS 输入设置（仅限 AMD/Intel）在安装前于设置 → OptiScaler 设置中配置。\n齿轮中的“部署 OptiScaler.ini”按钮会将您配置的 INI 模板重新部署到游戏文件夹。"));

        // Settings Overview
        panel.Children.Add(BuildFaqInfoSection("Settings",
            "单击工具栏中的“设置”为所有游戏配置默认值：",
            new[]
            {
                "ReLimiter FPS：默认帧率目标",
                "DLSS 预设：默认超分辨率预设",
                "NVIDIA 驱动设置：垂直同步、低延迟、电源模式",
                "峰值尼特：显示器用于 HDR 的峰值亮度",
                "ReShade 热键：自定义叠加层和截图键"
            }));

        // NVIDIA Driver Settings
        panel.Children.Add(BuildFaqInfoSection("NVIDIA 驱动设置",
            "RHI 可以管理游戏专用的 NVIDIA 驱动配置文件。这些设置可直接在每个游戏的详情面板上使用：",
            new[]
            {
                "垂直同步：开、关或自适应（快速同步）",
                "低延迟模式：超高、开或关",
                "平滑运动：多帧生成（仅限每个游戏）",
                "ReBAR：可调整大小的 BAR（需要管理员）"
            },
            "垂直同步、低延迟和电源模式的全局默认值在设置中配置。游戏专用覆盖可直接在每个游戏的详情面板上配置。"));

        // Vulkan Games
        panel.Children.Add(BuildFaqSpecialSection("V", "AccentPurpleBrush",
            "Vulkan 游戏",
            "Vulkan games (shown with a 'Vulkan' badge) use a global ReShade layer installed to C:\\ProgramData\\ReShade. This requires administrator privileges. When you install ReShade on a Vulkan game, RHI will prompt for elevation.",
            "所有 Vulkan 游戏共享同一个 ReShade 安装。在一个 Vulkan 游戏上更新 ReShade 会为所有游戏更新。\n游戏专用的 RenoDX 附加组件和着色器仍会单独安装到每个游戏文件夹。"));

        // Adding Games Manually
        panel.Children.Add(BuildFaqSpecialSection("+", "AccentAmberBrush",
            "手动添加游戏",
            "如果游戏未被自动检测到，请将其 .exe 文件直接拖放到 RHI 窗口上。RHI 会将其添加到您的库中并检测其引擎类型。",
            "您也可以将 RenoDX Discord 中的 .addon64 文件拖放到任意游戏上，以安装尚未在维基上的模组。"));

        // Updating Everything
        panel.Children.Add(BuildFaqSpecialSection("↑", "AccentGreenBrush",
            "更新所有内容",
            "单击工具栏中的“全部更新”可一次性更新所有游戏中已安装的所有组件。这包括 ReShade、RenoDX 模组、ReLimiter、Display Commander 等。",
            "有可用更新的游戏会在边栏中显示绿点。在设置中配置“全部更新”包含哪些组件。"));

        // Troubleshooting - Full Refresh
        panel.Children.Add(BuildFaqSpecialSection("↻", "AccentBlueBrush",
            "疑难解答：完全刷新",
            "如果游戏缺失、安装位置已更改，或 DLSS/Streamline 文件已添加或移除，请使用设置中的“完全刷新”从头重新扫描整个库。",
            "完全刷新会清除缓存的游戏列表并重新检测所有内容。当正常的刷新按钮无法检测到更改时使用它。"));

        // System Tray
        panel.Children.Add(BuildFaqSpecialSection("◰", "AccentPurpleBrush",
            "系统托盘",
            "RHI 可以最小化到系统托盘而不是关闭。右键单击托盘图标可快速启动最近的游戏，而无需打开主窗口。",
            "在设置中启用“关闭到系统托盘”以使 RHI 在后台保持运行。托盘图标可让您快速访问最近玩过的游戏。RHI 在运行时会每 4 小时自动检查一次更新，因此所有内容都保持最新。"));

        // Need More Help
        panel.Children.Add(BuildFaqLinksSection());
    }


    private Border BuildFaqSection(string? badge, string title, string titleBrush, string description, string? tip)
    {
        var stack = new StackPanel { Spacing = 10 };

        var header = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
        if (badge != null)
        {
            var badgeBorder = new Border
            {
                Background = (Brush)Application.Current.Resources[titleBrush],
                CornerRadius = new CornerRadius(12),
                Width = 24,
                Height = 24
            };
            badgeBorder.Child = new TextBlock
            {
                Text = badge,
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 10, 30, 50)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            header.Children.Add(badgeBorder);
        }
        header.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center
        });
        stack.Children.Add(header);

        stack.Children.Add(new TextBlock
        {
            Text = description,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
            LineHeight = 20
        });

        if (tip != null)
        {
            var tipBorder = new Border
            {
                Background = (Brush)Application.Current.Resources["SurfaceToolbarBrush"],
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12, 10, 12, 10),
                BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
                BorderThickness = new Thickness(1)
            };
            tipBorder.Child = new TextBlock
            {
                Text = tip,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
                LineHeight = 18
            };
            stack.Children.Add(tipBorder);
        }

        return new Border
        {
            Background = (Brush)Application.Current.Resources["SurfaceRaisedBrush"],
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20, 16, 20, 16),
            BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
            BorderThickness = new Thickness(1),
            Child = stack
        };
    }


    private Border BuildFaqStep(int step, string title, string description, string? tip)
    {
        var stack = new StackPanel { Spacing = 8 };

        var header = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var badgeBorder = new Border
        {
            Background = (Brush)Application.Current.Resources["AccentTealBrush"],
            CornerRadius = new CornerRadius(12),
            Width = 24,
            Height = 24
        };
        badgeBorder.Child = new TextBlock
        {
            Text = step.ToString(),
            FontSize = 12,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 10, 30, 50)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        header.Children.Add(badgeBorder);
        header.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center
        });
        stack.Children.Add(header);

        stack.Children.Add(new TextBlock
        {
            Text = description,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
            LineHeight = 20,
            Margin = new Thickness(32, 0, 0, 0)
        });

        if (tip != null)
        {
            var tipBorder = new Border
            {
                Background = (Brush)Application.Current.Resources["SurfaceToolbarBrush"],
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(32, 4, 0, 0),
                BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
                BorderThickness = new Thickness(1)
            };
            tipBorder.Child = new TextBlock
            {
                Text = tip,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
                LineHeight = 18
            };
            stack.Children.Add(tipBorder);
        }

        return new Border
        {
            Background = (Brush)Application.Current.Resources["SurfaceRaisedBrush"],
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20, 16, 20, 16),
            BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
            BorderThickness = new Thickness(1),
            Child = stack
        };
    }


    private Border BuildFaqInfoSection(string title, string description, string[] bullets, string? tip = null)
    {
        var stack = new StackPanel { Spacing = 8 };

        var header = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var badgeBorder = new Border
        {
            Background = (Brush)Application.Current.Resources["AccentBlueBrush"],
            CornerRadius = new CornerRadius(12),
            Width = 24,
            Height = 24
        };
        badgeBorder.Child = new TextBlock
        {
            Text = "?",
            FontSize = 12,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 10, 30, 50)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        header.Children.Add(badgeBorder);
        header.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center
        });
        stack.Children.Add(header);

        stack.Children.Add(new TextBlock
        {
            Text = description,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
            LineHeight = 20,
            Margin = new Thickness(32, 0, 0, 0)
        });

        var bulletStack = new StackPanel { Spacing = 6, Margin = new Thickness(32, 4, 0, 0) };
        foreach (var bullet in bullets)
        {
            bulletStack.Children.Add(new TextBlock
            {
                Text = $"• {bullet}",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
                LineHeight = 18
            });
        }
        stack.Children.Add(bulletStack);

        if (tip != null)
        {
            var tipBorder = new Border
            {
                Background = (Brush)Application.Current.Resources["SurfaceToolbarBrush"],
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(32, 4, 0, 0),
                BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
                BorderThickness = new Thickness(1)
            };
            tipBorder.Child = new TextBlock
            {
                Text = tip,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
                LineHeight = 18
            };
            stack.Children.Add(tipBorder);
        }

        return new Border
        {
            Background = (Brush)Application.Current.Resources["SurfaceRaisedBrush"],
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20, 16, 20, 16),
            BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
            BorderThickness = new Thickness(1),
            Child = stack
        };
    }


    private Border BuildFaqSpecialSection(string badge, string badgeBrush, string title, string description, string? tip)
    {
        var stack = new StackPanel { Spacing = 8 };

        var header = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var badgeBorder = new Border
        {
            Background = (Brush)Application.Current.Resources[badgeBrush],
            CornerRadius = new CornerRadius(12),
            Width = 24,
            Height = 24
        };
        var badgeFg = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 10, 30, 50));
        badgeBorder.Child = new TextBlock
        {
            Text = badge,
            FontSize = badge.Length > 1 ? 11 : 14,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = badgeFg,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        header.Children.Add(badgeBorder);
        header.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center
        });
        stack.Children.Add(header);

        stack.Children.Add(new TextBlock
        {
            Text = description,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
            LineHeight = 20,
            Margin = new Thickness(32, 0, 0, 0)
        });

        if (tip != null)
        {
            var tipBorder = new Border
            {
                Background = (Brush)Application.Current.Resources["SurfaceToolbarBrush"],
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(32, 4, 0, 0),
                BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
                BorderThickness = new Thickness(1)
            };
            tipBorder.Child = new TextBlock
            {
                Text = tip,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
                LineHeight = 18
            };
            stack.Children.Add(tipBorder);
        }

        return new Border
        {
            Background = (Brush)Application.Current.Resources["SurfaceRaisedBrush"],
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20, 16, 20, 16),
            BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
            BorderThickness = new Thickness(1),
            Child = stack
        };
    }


    private Border BuildFaqLinksSection()
    {
        var stack = new StackPanel { Spacing = 10 };

        stack.Children.Add(new TextBlock
        {
            Text = "需要更多帮助？",
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = (Brush)Application.Current.Resources["AccentTealBrush"]
        });

        stack.Children.Add(new TextBlock
        {
            Text = "Discord 上提供支持 — 加入社区获取帮助、模组更新和讨论。",
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
            LineHeight = 20
        });

        var linksStack = new StackPanel { Spacing = 6 };

        var discordLink = new HyperlinkButton
        {
            NavigateUri = new Uri("https://discord.gg/ultraplus"),
            Padding = new Thickness(0)
        };
        discordLink.Content = new TextBlock
        {
            Text = "加入 Ultra+ Discord（主社区）",
            Foreground = (Brush)Application.Current.Resources["AccentBlueBrush"],
            FontSize = 12
        };
        linksStack.Children.Add(discordLink);

        var renodxDiscordLink = new HyperlinkButton
        {
            NavigateUri = new Uri("https://discord.gg/renodx"),
            Padding = new Thickness(0)
        };
        renodxDiscordLink.Content = new TextBlock
        {
            Text = "RenoDX Discord（模组开发）",
            Foreground = (Brush)Application.Current.Resources["AccentBlueBrush"],
            FontSize = 12
        };
        linksStack.Children.Add(renodxDiscordLink);

        var wikiLink = new HyperlinkButton
        {
            NavigateUri = new Uri("https://github.com/clshortfuse/renodx/wiki/Mods"),
            Padding = new Thickness(0)
        };
        wikiLink.Content = new TextBlock
        {
            Text = "浏览 RenoDX 模组维基",
            Foreground = (Brush)Application.Current.Resources["AccentBlueBrush"],
            FontSize = 12
        };
        linksStack.Children.Add(wikiLink);

        var githubLink = new HyperlinkButton
        {
            NavigateUri = new Uri("https://github.com/RankFTW/RHI"),
            Padding = new Thickness(0)
        };
        githubLink.Content = new TextBlock
        {
            Text = "RHI GitHub — 报告问题或请求功能",
            Foreground = (Brush)Application.Current.Resources["AccentBlueBrush"],
            FontSize = 12
        };
        linksStack.Children.Add(githubLink);

        stack.Children.Add(linksStack);

        return new Border
        {
            Background = (Brush)Application.Current.Resources["SurfaceRaisedBrush"],
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20, 16, 20, 16),
            BorderBrush = (Brush)Application.Current.Resources["AccentTealBorderBrush"],
            BorderThickness = new Thickness(1),
            Child = stack
        };
    }
}
