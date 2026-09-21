// DetailPanelBuilder.Extras.cs — Extras section: Ultimate ASI Loader and future extras.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using RenoDXCommander.Models;
using RenoDXCommander.Services;
using RenoDXCommander.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;

namespace RenoDXCommander;

public partial class DetailPanelBuilder
{
    public void BuildExtrasSection(GameCardViewModel card)
    {
        var __exSw = System.Diagnostics.Stopwatch.StartNew();
        _window.ExtrasPanel.Children.Clear();
        _window.ExtrasContainer.Visibility = Visibility.Visible;

        // ── Collapsible header ────────────────────────────────────────────────
        const string extrasSectionKey = "Extras";
        var exSettings   = _window.ViewModel.Settings;
        bool exCollapsed = exSettings.CollapsedDetailSections.Contains(extrasSectionKey);

        var exArrow = new TextBlock
        {
            Text      = exCollapsed ? "▶" : "▼",
            FontSize  = 10,
            Foreground = UIFactory.Brush(ResourceKeys.TextTertiaryBrush),
            VerticalAlignment = VerticalAlignment.Center,
            Margin    = new Thickness(0, 0, 6, 0),
        };
        var exTitle = new TextBlock
        {
            Text       = "Extras（附加）",
            FontSize   = 13,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = UIFactory.Brush(ResourceKeys.TextPrimaryBrush),
            VerticalAlignment = VerticalAlignment.Center,
        };
        var exHeaderRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 0 };
        exHeaderRow.Children.Add(MakeDragHandle(_window.ExtrasContainer));
        exHeaderRow.Children.Add(exArrow);
        exHeaderRow.Children.Add(exTitle);
        _window.ExtrasPanel.Children.Add(exHeaderRow);

        var exBody = new StackPanel { Spacing = 10, Visibility = exCollapsed ? Visibility.Collapsed : Visibility.Visible };
        _window.ExtrasPanel.Children.Add(exBody);

        exHeaderRow.PointerEntered += (s, e) => exTitle.Foreground = UIFactory.Brush(ResourceKeys.AccentTealBrush);
        exHeaderRow.PointerExited  += (s, e) => exTitle.Foreground = UIFactory.Brush(ResourceKeys.TextPrimaryBrush);
        var exHandCursor  = Microsoft.UI.Input.InputSystemCursor.Create(Microsoft.UI.Input.InputSystemCursorShape.Hand);
        var exArrowCursor = Microsoft.UI.Input.InputSystemCursor.Create(Microsoft.UI.Input.InputSystemCursorShape.Arrow);
        var exCursorProp  = DetailPanelBuilder.CursorProp;
        exHeaderRow.PointerEntered += (s, e) => exCursorProp?.SetValue(exHeaderRow, exHandCursor);
        exHeaderRow.PointerExited  += (s, e) => exCursorProp?.SetValue(exHeaderRow, exArrowCursor);
        exHeaderRow.PointerPressed += (s, e) =>
        {
            bool nowCollapsed = exBody.Visibility == Visibility.Visible;
            exBody.Visibility = nowCollapsed ? Visibility.Collapsed : Visibility.Visible;
            exArrow.Text = nowCollapsed ? "▶" : "▼";
            if (nowCollapsed) exSettings.CollapsedDetailSections.Add(extrasSectionKey);
            else              exSettings.CollapsedDetailSections.Remove(extrasSectionKey);
            _window.ViewModel.SaveSettingsPublic();
        };

        // ── Ultimate ASI Loader row ───────────────────────────────────────────
        var __t0 = __exSw.ElapsedMilliseconds;
        BuildUalRow(card, exBody);
        CrashReporter.Log($"[BuildExtrasSection] UalRow: {__exSw.ElapsedMilliseconds - __t0}ms '{card.GameName}'");

        // ── MFG Unlocks separator ─────────────────────────────────────────────
        exBody.Children.Add(MakeExtrasSeparator("MFG 解锁"));

        // ── RTX 40 MFG Unlock row ─────────────────────────────────────────────
        __t0 = __exSw.ElapsedMilliseconds;
        BuildRtx40MfgRow(card, exBody);
        CrashReporter.Log($"[BuildExtrasSection] Rtx40MfgRow: {__exSw.ElapsedMilliseconds - __t0}ms '{card.GameName}'");

        // ── MFG Ada Unlock row ────────────────────────────────────────────────
        __t0 = __exSw.ElapsedMilliseconds;
        BuildMfgAdaUnlockRow(card, exBody);
        CrashReporter.Log($"[BuildExtrasSection] MfgAdaRow: {__exSw.ElapsedMilliseconds - __t0}ms '{card.GameName}'");

        // ── 20/30 FG Unlock row ───────────────────────────────────────────────
        __t0 = __exSw.ElapsedMilliseconds;
        BuildDlssg2030Row(card, exBody);
        CrashReporter.Log($"[BuildExtrasSection] Dlssg2030Row: {__exSw.ElapsedMilliseconds - __t0}ms '{card.GameName}'");

        // ── Other separator ───────────────────────────────────────────────────
        exBody.Children.Add(MakeExtrasSeparator("Other"));

        // ── OptiScaler row ────────────────────────────────────────────────────
        __t0 = __exSw.ElapsedMilliseconds;
        BuildOsRow(card, exBody);
        CrashReporter.Log($"[BuildExtrasSection] OsRow: {__exSw.ElapsedMilliseconds - __t0}ms '{card.GameName}'");

        // ── DLSS Enabler (standalone) row ─────────────────────────────────────
        __t0 = __exSw.ElapsedMilliseconds;
        BuildDlssEnablerRow(card, exBody);
        CrashReporter.Log($"[BuildExtrasSection] DlssEnablerRow: {__exSw.ElapsedMilliseconds - __t0}ms '{card.GameName}'");

        // ── API Upgrades sub-header + DXVK row ────────────────────────────────
        if (card.IsDxvkToggleVisible)
        {
            exBody.Children.Add(MakeExtrasSeparator("API 升级"));
            __t0 = __exSw.ElapsedMilliseconds;
            BuildDxvkRow(card, exBody);
            CrashReporter.Log($"[BuildExtrasSection] DxvkRow: {__exSw.ElapsedMilliseconds - __t0}ms '{card.GameName}'");
        }

        UpdateOsFeedback(card);
        __exSw.Stop();
        CrashReporter.Log($"[BuildExtrasSection] Total: {__exSw.ElapsedMilliseconds}ms '{card.GameName}'");
    }

    
    public void OnExtrasCardPropertyChanged(GameCardViewModel card, string? propertyName)
    {
        UpdateOsFeedback(card);

        if (propertyName is "IsOsInstalled" or "OsActionLabel" or "OsStatusText"
            or "OsStatusColor" or "OsDeleteVisibility" or "OsRowVisibility"
            or "OsInstallEnabled" or "OsBtnBackground" or "OsInstalledFile" or "Is32Bit")
        {
            RequestExtrasRebuild(card);
        }
    }

    
    public void UpdateOsFeedback(GameCardViewModel card)
    {
        _window.DetailOsProgress.Visibility = card.OsRowVisibility == Visibility.Visible ? card.OsProgressVisibility : Visibility.Collapsed;
        _window.DetailOsProgress.Value = card.OsProgress;
        _window.DetailOsMessage.Visibility = card.OsRowVisibility == Visibility.Visible ? card.OsMessageVisibility : Visibility.Collapsed;
        _window.DetailOsMessage.Text = card.OsActionMessage;
        _window.DetailOsMessage.Foreground = UIFactory.GetBrush(GetMessageColor(card.OsActionMessage));
    }

    
    private bool _extrasRebuildPending;

    public void RequestExtrasRebuild(GameCardViewModel card)
    {
        if (_extrasRebuildPending) return;
        _extrasRebuildPending = true;
        _window.DispatcherQueue.TryEnqueue(() =>
        {
            _extrasRebuildPending = false;
            if (_currentDetailCard != card) return;
            BuildExtrasSection(card);
        });
    }

    private static Grid MakeExtrasSeparator(string label)
    {
        var grid = new Grid { ColumnSpacing = 8, Margin = new Thickness(0, 4, 0, 2) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });

        var text = new TextBlock
        {
            Text = $"———  {label}  ———",
            FontSize = 11,
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetColumn(text, 3);
        grid.Children.Add(text);
        return grid;
    }

    private void BuildUalRow(GameCardViewModel card, StackPanel body)
    {
        var ualSvc    = _window.ViewModel.UalServiceInstance;
        var gameName  = card.GameName;
        var store     = card.Source ?? "";
        var installPath = card.InstallPath ?? "";

        // Detect current install state
        var ualRecord   = string.IsNullOrEmpty(installPath) ? null
            : _auxInstallService.FindRecord(gameName, installPath, UltimateAsiLoaderService.AddonType);
        bool isInstalled = ualRecord != null;
        string? installedAs = ualRecord?.InstalledAs;

        // Status text
        string statusText;
        string statusColor;
        if (isInstalled)
        {
            var staged = card.Is32Bit ? ualSvc.StagedVersion32 : ualSvc.StagedVersion64;
            statusText  = staged ?? "Installed";
            statusColor = "#5ECB7D";
        }
        else
        {
            statusText  = "就绪";
            statusColor = "#A0AABB";
        }

        // ── Row grid matching Components section exactly ───────────────────────
        // Col 0: label (120)  Col 1: status (80)  Col 2: Info (36)
        // Col 3: install (*)  Col 4: cog (36)     Col 5: delete (36)
        var row = new Grid { ColumnSpacing = 8 };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });

        // Col 0 — label
        var label = new TextBlock
        {
            Text = "ASI Loader",
            FontSize = 12,
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            VerticalAlignment = VerticalAlignment.Center,
        };
        ToolTipService.SetToolTip(label, "Ultimate ASI Loader — 将 .asi 插件加载到游戏进程中的代理 DLL。");
        Grid.SetColumn(label, 0);
        row.Children.Add(label);

        // Col 1 — status
        var statusBlock = new TextBlock
        {
            Text = statusText,
            FontSize = 12,
            Foreground = UIFactory.GetBrush(statusColor),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalTextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
            TextDecorations = isInstalled ? Windows.UI.Text.TextDecorations.Underline : Windows.UI.Text.TextDecorations.None,
        };
        if (isInstalled)
        {
            ToolTipService.SetToolTip(statusBlock, $"Installed as: {installedAs}\nClick to open GitHub releases");
            statusBlock.PointerPressed += (s, e) =>
                _ = Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/ThirteenAG/Ultimate-ASI-Loader/releases"));
        }
        Grid.SetColumn(statusBlock, 1);
        row.Children.Add(statusBlock);

        // Col 2 — Info button (matches Components style)
        var infoBtn = new Button
        {
            Content = "Info",
            FontSize = 11,
            Padding = new Thickness(6, 2, 6, 2),
            Width = 36,
            Height = 32,
            Background = UIFactory.Brush(ResourceKeys.AccentBlueBgBrush),
            Foreground = UIFactory.Brush(ResourceKeys.AccentBlueBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
        };
        ToolTipService.SetToolTip(infoBtn, "打开 Ultimate ASI Loader GitHub releases 页面");
        infoBtn.Click += (s, e) =>
            _ = Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/ThirteenAG/Ultimate-ASI-Loader/releases"));
        Grid.SetColumn(infoBtn, 2);
        row.Children.Add(infoBtn);

        // Col 3 — Install button
        var installBtn = new Button
        {
            Content = isInstalled ? "↺  重新安装 ASI Loader" : "⬇  安装 ASI Loader",
            FontSize = 12,
            Height = 32,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            CornerRadius = new CornerRadius(8),
            Background = isInstalled
                ? UIFactory.GetBrush("#182840")
                : UIFactory.Brush(ResourceKeys.AccentBlueBgBrush),
            Foreground = isInstalled
                ? UIFactory.GetBrush("#7AACDD")
                : UIFactory.Brush(ResourceKeys.AccentBlueBrush),
            BorderBrush = isInstalled
                ? UIFactory.GetBrush("#2A4468")
                : UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush),
            BorderThickness = new Thickness(1),
        };
        ToolTipService.SetToolTip(installBtn, isInstalled
            ? $"Reinstall Ultimate ASI Loader (currently '{installedAs}')"
            : "安装 Ultimate ASI Loader — 选择要使用的 DLL 名称");
        installBtn.Click += async (s, e) =>
        {
            if (string.IsNullOrEmpty(installPath)) return;
            var chosen = await ShowUalDllPickerAsync(card, ualRecord?.InstalledAs);
            if (chosen == null) return;

            installBtn.IsEnabled = false;
            installBtn.Content = "安装中...";
            try
            {
                var (success, hookedOriginal) = await ualSvc.InstallAsync(card, chosen);
                if (success)
                {
                    _window.ViewModel.SetUalInstalledAs(gameName, chosen, store);
                    if (hookedOriginal != null)
                    {
                        _ = DialogService.ShowSafeAsync(new ContentDialog
                        {
                            Title = "原始 DLL 已链接",
                            Content = $"The existing '{chosen}' was renamed to '{hookedOriginal}' so ASI Loader can chain-load it automatically.",
                            CloseButtonText = "OK",
                            XamlRoot = _window.Content.XamlRoot,
                        });
                    }
                    RequestExtrasRebuild(card);
                }
                else
                {
                    installBtn.Content = "❌ 安装失败";
                }
            }
            finally { installBtn.IsEnabled = true; }
        };
        Grid.SetColumn(installBtn, 3);
        row.Children.Add(installBtn);

        // Col 4 — Cog (empty for now, matches Components cog size/position)
        var cogBtn = new Button
        {
            Width = 36,
            Height = 32,
            Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.SurfaceOverlayBrush),
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.BorderDefaultBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "⚙", FontSize = 14, HorizontalAlignment = HorizontalAlignment.Center },
            Tag = card,
        };
        ToolTipService.SetToolTip(cogBtn, "ASI Loader 设置（即将推出）");
        cogBtn.Click += async (s, e) =>
        {
            // Placeholder — settings dialog will be added later
            var dlg = new ContentDialog
            {
                Title = "ASI Loader 设置",
                Content = new TextBlock { Text = "暂无设置可用。", FontSize = 12 },
                CloseButtonText = "Close",
                XamlRoot = _window.Content.XamlRoot,
            };
            await DialogService.ShowSafeAsync(dlg);
        };
        Grid.SetColumn(cogBtn, 4);
        row.Children.Add(cogBtn);

        // Col 5 — Remove button
        var removeBtn = new Button
        {
            Width = 36,
            Height = 32,
            Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.AccentRedBgBrush),
            Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.AccentPurpleBorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "✕", FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush) },
            Opacity = isInstalled ? 1.0 : 0,
            IsHitTestVisible = isInstalled,
        };
        ToolTipService.SetToolTip(removeBtn, "从此游戏中移除 Ultimate ASI Loader");
        removeBtn.Click += (s, e) =>
        {
            if (string.IsNullOrEmpty(installPath)) return;
            ualSvc.Uninstall(card);
            _window.ViewModel.SetUalInstalledAs(gameName, null, store);
            RequestExtrasRebuild(card);
        };
        Grid.SetColumn(removeBtn, 5);
        row.Children.Add(removeBtn);

        body.Children.Add(row);
    }

    private void BuildMfgAdaUnlockRow(GameCardViewModel card, StackPanel body)
    {
        var gameName    = card.GameName;
        var store       = card.Source ?? "";
        var installPath = card.InstallPath ?? "";

        const string DeployFileName  = "renodx-mfgunlock.addon64";
        const string StagedFileName  = "MFG Ada Unlock.addon64";
        var stagedPath  = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RHI", "addons", StagedFileName);

        bool isInstalled   = !string.IsNullOrEmpty(installPath) && File.Exists(Path.Combine(installPath, DeployFileName));
        bool rsInstalled   = card.IsRsInstalled;
        var mfgInstalledAs = _window.ViewModel.GetRtx40MfgInstalledAs(card.GameName, card.Source ?? "");
        bool rtx40Conflict = !string.IsNullOrEmpty(mfgInstalledAs) &&
            !string.IsNullOrEmpty(installPath) && File.Exists(Path.Combine(installPath, mfgInstalledAs));
        bool staged        = File.Exists(stagedPath);

        var   addonVersion = AddonPackService.LoadAddonVersion("MFG Ada Unlock");
        string statusText  = isInstalled ? (string.IsNullOrEmpty(addonVersion) ? "Installed" : $"v{addonVersion}") : "就绪";
        string statusColor = isInstalled ? "#5ECB7D" : "#A0AABB";

        var row = new Grid { ColumnSpacing = 8 };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });

        // Col 0 — label
        var label = new TextBlock
        {
            Text = "MFG Ada Unlock",
            FontSize = 12,
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            VerticalAlignment = VerticalAlignment.Center,
        };
        ToolTipService.SetToolTip(label, "MFG Ada Unlock — 在 RTX 40 系列 GPU 上解锁 DLSS 多帧生成（3x/4x+）。需要 ReShade。仅内存中运行，不修改文件。");
        Grid.SetColumn(label, 0);
        row.Children.Add(label);

        // Col 1 — status
        var statusBlock = new TextBlock
        {
            Text = statusText,
            FontSize = 12,
            Foreground = UIFactory.GetBrush(statusColor),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalTextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
            TextDecorations = isInstalled ? Windows.UI.Text.TextDecorations.Underline : Windows.UI.Text.TextDecorations.None,
        };
        if (isInstalled)
        {
            ToolTipService.SetToolTip(statusBlock, "点击打开 GitHub releases");
            statusBlock.PointerPressed += (s, e) =>
                _ = Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/mavismmg/MFGAdaUnlock-RenoDx/releases"));
        }
        Grid.SetColumn(statusBlock, 1);
        row.Children.Add(statusBlock);

        // Col 2 — Info button (always blue)
        var infoBtn = new Button
        {
            Content = "Info",
            FontSize = 11,
            Padding = new Thickness(6, 2, 6, 2),
            Width = 36,
            Height = 32,
            Background = UIFactory.Brush(ResourceKeys.AccentBlueBgBrush),
            Foreground = UIFactory.Brush(ResourceKeys.AccentBlueBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
        };
        ToolTipService.SetToolTip(infoBtn, "打开 MFG Ada Unlock GitHub 页面");
        infoBtn.Click += (s, e) =>
            _ = Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/mavismmg/MFGAdaUnlock-RenoDx"));
        Grid.SetColumn(infoBtn, 2);
        row.Children.Add(infoBtn);

        // Col 3 — Install button
        string btnLabel;
        bool btnEnabled = true;
        if (!rsInstalled)
        {
            btnLabel   = "⚠  ReShade required";
            btnEnabled = false;
        }
        else if (rtx40Conflict)
        {
            btnLabel   = "⚠  RTX 40 MFG 已安装";
            btnEnabled = false;
        }
        else if (!staged)
        {
            btnLabel   = "⬇  安装 MFG Ada Unlock";
            btnEnabled = true; // will download on demand when clicked
        }
        else
        {
            btnLabel = isInstalled ? "↺  重新安装 MFG Ada Unlock" : "⬇  安装 MFG Ada Unlock";
        }

        var installBtn = new Button
        {
            Content = btnLabel,
            FontSize = 12,
            Height = 32,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CornerRadius = new CornerRadius(8),
            Background = isInstalled ? UIFactory.GetBrush("#182840") : UIFactory.Brush(ResourceKeys.AccentBlueBgBrush),
            Foreground = isInstalled ? UIFactory.GetBrush("#7AACDD") : UIFactory.Brush(ResourceKeys.AccentBlueBrush),
            BorderBrush = isInstalled ? UIFactory.GetBrush("#2A4468") : UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush),
            BorderThickness = new Thickness(1),
            IsEnabled = btnEnabled,
            Opacity = btnEnabled ? 1.0 : 0.35,
        };

        if (!rsInstalled)
            ToolTipService.SetToolTip(installBtn, "安装 ReShade first — MFG Ada Unlock requires it");
        else if (rtx40Conflict)
            ToolTipService.SetToolTip(installBtn, "RTX 40 MFG Unlock（ASI 版本）已安装并存在冲突。请先移除它。");

        installBtn.Click += async (s, e) =>
        {
            if (string.IsNullOrEmpty(installPath)) return;
            installBtn.IsEnabled = false;
            installBtn.Content   = "下载中...";
            try
            {
                // Download on demand if not yet staged
                if (!File.Exists(stagedPath))
                {
                    var addonSvc = _window.ViewModel.AddonPackServiceInstance;
                    var entry = addonSvc.AvailablePacks.FirstOrDefault(p =>
                        p.PackageName.Equals("MFG Ada Unlock", StringComparison.OrdinalIgnoreCase));
                    if (entry != null)
                        await addonSvc.DownloadAddonAsync(entry).ConfigureAwait(false);
                }

                if (!File.Exists(stagedPath))
                {
                    CrashReporter.Log("[BuildMfgAdaUnlockRow] Staged file still not found after download attempt");
                    _window.DispatcherQueue?.TryEnqueue(() => installBtn.Content = "下载失败");
                    return;
                }

                var dest = Path.Combine(installPath, DeployFileName);
                File.Copy(stagedPath, dest, overwrite: true);
                CrashReporter.Log($"[BuildMfgAdaUnlockRow] Installed '{DeployFileName}' to '{installPath}'");
                _window.DispatcherQueue?.TryEnqueue(() => RequestExtrasRebuild(card));
            }
            catch (Exception ex)
            {
                CrashReporter.Log($"[BuildMfgAdaUnlockRow] 安装失败 — {ex.Message}");
                _window.DispatcherQueue?.TryEnqueue(() => { installBtn.IsEnabled = true; installBtn.Content = btnLabel; });
            }
        };
        Grid.SetColumn(installBtn, 3);
        row.Children.Add(installBtn);

        // Col 4 — Cog (placeholder for consistency)
        var cogBtn = new Button
        {
            Width = 36,
            Height = 32,
            Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.SurfaceOverlayBrush),
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.BorderDefaultBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "⚙", FontSize = 14, HorizontalAlignment = HorizontalAlignment.Center },
        };
        ToolTipService.SetToolTip(cogBtn, "MFG Ada Unlock 设置");
        cogBtn.Click += async (s, e) =>
        {
            var dlg = new ContentDialog
            {
                Title = "MFG Ada Unlock",
                Content = new TextBlock
                {
                    Text = "MFG Ada Unlock 在 RTX 40 系列 GPU 上解锁 DLSS 多帧生成（3x/4x 及以上）。\n\n" +
                           "在游戏内通过 ReShade 叠加层进行配置。",
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap,
                },
                PrimaryButtonText = "打开 GitHub",
                CloseButtonText = "Close",
                XamlRoot = _window.Content.XamlRoot,
                RequestedTheme = ElementTheme.Dark,
            };
            var result = await DialogService.ShowSafeAsync(dlg);
            if (result == ContentDialogResult.Primary)
                _ = Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/mavismmg/MFGAdaUnlock-RenoDx"));
        };
        Grid.SetColumn(cogBtn, 4);
        row.Children.Add(cogBtn);

        // Col 5 — Remove button
        var removeBtn = new Button
        {
            Width = 36,
            Height = 32,
            Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.AccentRedBgBrush),
            Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.AccentPurpleBorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "✕", FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush) },
            Opacity = isInstalled ? 1.0 : 0,
            IsHitTestVisible = isInstalled,
        };
        ToolTipService.SetToolTip(removeBtn, "从此游戏中移除 MFG Ada Unlock");
        removeBtn.Click += (s, e) =>
        {
            if (string.IsNullOrEmpty(installPath)) return;
            try
            {
                var dest = Path.Combine(installPath, DeployFileName);
                if (File.Exists(dest)) File.Delete(dest);
                CrashReporter.Log($"[BuildMfgAdaUnlockRow] Removed '{DeployFileName}' from '{installPath}'");

                // Also remove from addon selections so SyncGameFolder doesn't redeploy it.
                // MFG Ada Unlock installed via Extras greys out the picker toggle — the user
                // can't deselect it there, so we must clean up the selection here.
                const string PackName = "MFG Ada Unlock";

                // Global selection
                var globalAddons = _window.ViewModel.Settings.EnabledGlobalAddons;
                if (globalAddons.Remove(PackName))
                {
                    _window.ViewModel.SaveSettingsPublic();
                    CrashReporter.Log($"[BuildMfgAdaUnlockRow] Removed '{PackName}' from global addon selection");
                }

                // Per-game selection (composite key first, then legacy name-only)
                var gameNameService = _window.ViewModel.GameNameServiceInstance;
                var compositeKey = Models.GameKey.FromCard(card.GameName, card.Source).ToKey();
                bool perGameChanged = false;
                if (gameNameService.PerGameAddonSelection.TryGetValue(compositeKey, out var perGame))
                    perGameChanged = perGame.Remove(PackName);
                if (!perGameChanged && gameNameService.PerGameAddonSelection.TryGetValue(card.GameName, out var perGameLegacy))
                    perGameChanged = perGameLegacy.Remove(PackName);
                if (perGameChanged)
                {
                    _window.ViewModel.SaveSettingsPublic();
                    CrashReporter.Log($"[BuildMfgAdaUnlockRow] Removed '{PackName}' from per-game addon selection for '{card.GameName}'");
                }

                RequestExtrasRebuild(card);
            }
            catch (Exception ex)
            {
                CrashReporter.Log($"[BuildMfgAdaUnlockRow] Remove failed — {ex.Message}");
            }
        };
        Grid.SetColumn(removeBtn, 5);
        row.Children.Add(removeBtn);

        body.Children.Add(row);
    }

    private void BuildOsRow(GameCardViewModel card, StackPanel body)
    {
        // Only add the row when it should be visible
        if (card.OsRowVisibility != Visibility.Visible) return;

        bool osGreyed = card.Is32Bit;

        // ── Row grid matching Components section exactly ───────────────────────
        // Col 0: label (120)  Col 1: status (80)  Col 2: Info (36)
        // Col 3: install (*)  Col 4: cog (36)     Col 5: delete (36)
        var row = new Grid { ColumnSpacing = 8 };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });

        // Col 0 — label
        var label = new TextBlock
        {
            Text = "OptiScaler",
            FontSize = 12,
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            VerticalAlignment = VerticalAlignment.Center,
            TextDecorations = osGreyed ? Windows.UI.Text.TextDecorations.Strikethrough : Windows.UI.Text.TextDecorations.None,
            Opacity = osGreyed ? 0.35 : 1.0,
            Tag = card,
        };
        Grid.SetColumn(label, 0);
        row.Children.Add(label);

        // Col 1 — status
        var statusBlock = new TextBlock
        {
            Text = card.OsStatusText,
            FontSize = 12,
            Foreground = UIFactory.GetBrush(card.OsStatusColor),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalTextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
            TextDecorations = osGreyed
                ? Windows.UI.Text.TextDecorations.Strikethrough
                : (card.IsOsInstalled ? Windows.UI.Text.TextDecorations.Underline : Windows.UI.Text.TextDecorations.None),
            Opacity = osGreyed ? 0.35 : 1.0,
        };
        if (card.IsOsInstalled && !osGreyed)
        {
            var osVariant = _window.ViewModel.GetOsVariant(card.GameName, card.Source ?? "");
            var osWikiUrl = osVariant == "DlssNr"
                ? "https://github.com/wilsjo2/OptiScaler-DLSSNR-PreSR-Multipass/releases"
                : "https://github.com/optiscaler/OptiScaler/wiki";
            ToolTipService.SetToolTip(statusBlock, "点击打开 OptiScaler releases");
            statusBlock.PointerPressed += async (s, e) =>
                await Windows.System.Launcher.LaunchUriAsync(new Uri(osWikiUrl));
        }
        Grid.SetColumn(statusBlock, 1);
        row.Children.Add(statusBlock);

        // Col 2 — Info button
        var infoBtn = new Button
        {
            Content = "Info",
            FontSize = 11,
            Padding = new Thickness(6, 2, 6, 2),
            Width = 36,
            Height = 32,
            CornerRadius = new CornerRadius(8),
            Tag = card,
            DataContext = AddonType.OptiScaler,
        };
        ApplyInfoButtonStyle(infoBtn, card, AddonType.OptiScaler);
        infoBtn.Click += (s, e) => _window.InfoButton_Click(s, e);
        Grid.SetColumn(infoBtn, 2);
        row.Children.Add(infoBtn);

        // Col 3 — Install button
        var installBtn = new Button
        {
            Height = 32,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            CornerRadius = new CornerRadius(8),
            FontSize = 12,
            Background = UIFactory.GetBrush(card.OsBtnBackground),
            Foreground = UIFactory.GetBrush(card.OsBtnForeground),
            BorderBrush = UIFactory.GetBrush(card.OsBtnBorderBrush),
            BorderThickness = new Thickness(1),
            Tag = card,
            IsEnabled = card.OsInstallEnabled && !osGreyed,
            Opacity = osGreyed ? 0.35 : 1.0,
            IsHitTestVisible = !osGreyed,
        };
        installBtn.Content = WithInfoArrow(card.OsActionLabel, HasRealInfoContent(card, AddonType.OptiScaler), card.OsStatus == GameStatus.UpdateAvailable, installBtn);
        installBtn.Click += (s, e) => _window.InstallOsButton_Click(s, e);
        Grid.SetColumn(installBtn, 3);
        row.Children.Add(installBtn);

        // Col 4 — Cog (⚙) button
        var cogBtn = new Button
        {
            Width = 36,
            Height = 32,
            Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.SurfaceOverlayBrush),
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.BorderStrongBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "⚙", FontSize = 14, HorizontalAlignment = HorizontalAlignment.Center },
            Tag = card,
            IsEnabled = !osGreyed,
            Opacity = osGreyed ? 0.35 : 1.0,
        };
        ToolTipService.SetToolTip(cogBtn, "OptiScaler 设置");
        cogBtn.Click += (s, e) => _window.OsCogButton_ClickInternal(s, e);
        Grid.SetColumn(cogBtn, 4);
        row.Children.Add(cogBtn);

        // Col 5 — Delete (✕) button
        bool osShow = card.OsDeleteVisibility == Visibility.Visible;
        var deleteBtn = new Button
        {
            Width = 36,
            Height = 32,
            Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.AccentRedBgBrush),
            Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.AccentPurpleBorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "✕", FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush) },
            Tag = card,
            Opacity = (osGreyed || !osShow) ? 0 : 1.0,
            IsHitTestVisible = osShow && !osGreyed,
        };
        ToolTipService.SetToolTip(deleteBtn, "移除 OptiScaler");
        deleteBtn.Click += (s, e) => _window.UninstallOsButton_Click(s, e);
        Grid.SetColumn(deleteBtn, 5);
        row.Children.Add(deleteBtn);

        body.Children.Add(row);
    }

    private async Task<string?> ShowUalDllPickerAsync(GameCardViewModel card, string? currentDllName)
    {
        if (string.IsNullOrEmpty(card.InstallPath)) return null;

        var names = card.Is32Bit
            ? UltimateAsiLoaderService.Win32Names
            : UltimateAsiLoaderService.Win64Names;

        // Collect files already in the game folder
        var existingFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            if (Directory.Exists(card.InstallPath))
                foreach (var f in Directory.GetFiles(card.InstallPath, "*.dll"))
                    existingFiles.Add(Path.GetFileName(f));
        }
        catch { }

        // RHI-managed filenames to flag as conflict
        var rhiOwned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(card.RsInstalledFile))  rhiOwned.Add(card.RsInstalledFile);
        if (!string.IsNullOrEmpty(card.OsInstalledFile))  rhiOwned.Add(card.OsInstalledFile);
        if (!string.IsNullOrEmpty(card.DcInstalledFile))  rhiOwned.Add(card.DcInstalledFile);

        string? chosen = null;

        var listPanel = new StackPanel { Spacing = 4 };
        var scrollViewer = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            MaxHeight = 420,
            Content = listPanel,
        };

        foreach (var name in names)
        {
            bool isRecommended = UltimateAsiLoaderService.RecommendedNames.Contains(name, StringComparer.OrdinalIgnoreCase);
            bool isRhiConflict = UltimateAsiLoaderService.RhiConflictNames.Contains(name, StringComparer.OrdinalIgnoreCase);
            bool isTaken       = existingFiles.Contains(name) && !rhiOwned.Contains(name) && name != currentDllName;
            bool isRhiOwned    = rhiOwned.Contains(name);
            bool isCurrent     = string.Equals(name, currentDllName, StringComparison.OrdinalIgnoreCase);

            var btn = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(10, 6, 10, 6),
                CornerRadius = new CornerRadius(6),
                BorderThickness = new Thickness(1),
                IsEnabled = !isRhiOwned,
                Opacity = isRhiOwned ? 0.4 : 1.0,
            };

            // Styling
            if (isCurrent)
            {
                btn.Background   = UIFactory.Brush(ResourceKeys.AccentBlueBgBrush);
                btn.BorderBrush  = UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush);
            }
            else
            {
                btn.Background  = UIFactory.Brush(ResourceKeys.SurfaceOverlayBrush);
                btn.BorderBrush = UIFactory.Brush(ResourceKeys.BorderDefaultBrush);
            }

            // Content: name + badges
            var contentRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            contentRow.Children.Add(new TextBlock
            {
                Text = name,
                FontSize = 12,
                Foreground = isTaken || isRhiOwned
                    ? UIFactory.Brush(ResourceKeys.TextTertiaryBrush)
                    : UIFactory.Brush(ResourceKeys.TextPrimaryBrush),
                VerticalAlignment = VerticalAlignment.Center,
            });

            if (isRecommended)
                contentRow.Children.Add(MakeBadge("推荐", "#1A3A20", "#6AE87A", "#2A5A30"));
            if (isTaken)
                contentRow.Children.Add(MakeBadge("使用中", "#2A1818", "#CC6666", "#5A2828"));
            if (isRhiOwned)
                contentRow.Children.Add(MakeBadge("RHI 使用中", "#2A1818", "#CC6666", "#5A2828"));
            if (isRhiConflict && !isRhiOwned)
                contentRow.Children.Add(MakeBadge("可能与 ReShade/OS 冲突", "#2A1A10", "#CC9955", "#5A3A18"));
            if (isCurrent)
                contentRow.Children.Add(MakeBadge("当前", "#182840", "#7AACDD", "#2A4468"));

            btn.Content = contentRow;

            // Tooltip for taken files
            if (isTaken)
                ToolTipService.SetToolTip(btn, $"'{name}' already exists in the game folder. Selecting it will rename the existing file to '{Path.GetFileNameWithoutExtension(name)}Hooked.dll' so ASI Loader can chain-load it.");
            else if (isRhiOwned)
                ToolTipService.SetToolTip(btn, "此文件名已被 RHI 管理的组件（ReShade、OptiScaler 或 DC）使用。请选择其他名称。");

            btn.Tag = name;
            btn.Click += (s, ev) =>
            {
                chosen = (s as Button)?.Tag as string;
                // Close the dialog by finding and closing it
                if (s is FrameworkElement fe)
                {
                    var dialog = FindParentContentDialog(fe);
                    dialog?.Hide();
                }
            };

            listPanel.Children.Add(btn);
        }

        var dialog = new ContentDialog
        {
            Title = "选择 ASI Loader DLL 名称",
            Content = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock
                    {
                        Text = "选择 ASI Loader 的文件名。大多数游戏可使用 version.dll 或 winmm.dll。",
                        FontSize = 11,
                        Foreground = UIFactory.Brush(ResourceKeys.TextTertiaryBrush),
                        TextWrapping = TextWrapping.Wrap,
                    },
                    scrollViewer,
                }
            },
            CloseButtonText = "Cancel",
            XamlRoot = _window.Content.XamlRoot,
        };

        await DialogService.ShowSafeAsync(dialog);
        return chosen;
    }

    /// <summary>Walks up the visual tree to find the parent ContentDialog.</summary>
    private static ContentDialog? FindParentContentDialog(DependencyObject element)
    {
        var parent = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(element);
        while (parent != null)
        {
            if (parent is ContentDialog d) return d;
            parent = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(parent);
        }
        return null;
    }

    private static Border MakeBadge(string text, string bg, string fg, string border)
    {
        return new Border
        {
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(5, 1, 5, 1),
            Background = UIFactory.GetBrush(bg),
            BorderBrush = UIFactory.GetBrush(border),
            BorderThickness = new Thickness(1),
            VerticalAlignment = VerticalAlignment.Center,
            Child = new TextBlock
            {
                Text = text,
                FontSize = 10,
                Foreground = UIFactory.GetBrush(fg),
            },
        };
    }

    private static readonly string[] MfgDllNames =
    {
        "version.dll", "dinput8.dll", "winmm.dll", "d3d9.dll", "d3d10.dll",
        "d3d11.dll", "d3d12.dll", "dxgi.dll", "dsound.dll", "wininet.dll",
        "winhttp.dll", "binkw64.dll", "bink2w64.dll", "xinput1_1.dll",
        "xinput1_2.dll", "xinput1_3.dll", "xinput1_4.dll", "xinput9_1_0.dll",
        "xinputuap.dll",
    };

    private void BuildDlssg2030Row(GameCardViewModel card, StackPanel body)
    {
        var svc         = App.Services.GetRequiredService<Dlssg20_30Service>();
        var gameName    = card.GameName;
        var store       = card.Source ?? "";
        var installPath = card.InstallPath ?? "";

        var currentDllName = _window.ViewModel.GetDlssg2030InstalledAs(gameName, store);
        bool isInstalled   = svc.IsInstalledIn(installPath, currentDllName);
        bool addonConflict = !string.IsNullOrEmpty(installPath) &&
            File.Exists(Path.Combine(installPath, "renodx-mfgunlock.addon64"));

        string statusText  = isInstalled ? (svc.StagedVersion ?? "Installed") : "就绪";
        string statusColor = isInstalled ? "#5ECB7D" : "#A0AABB";

        var row = new Grid { ColumnSpacing = 8 };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });

        // Col 0 — label
        var label = new TextBlock
        {
            Text = "20/30 FG 解锁",
            FontSize = 12,
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            VerticalAlignment = VerticalAlignment.Center,
        };
        ToolTipService.SetToolTip(label, "20/30 FG 解锁 — enables DLSS Frame Generation on RTX 20 and 30 series GPUs. D3D12 only. No ASI Loader or ReShade required.");
        Grid.SetColumn(label, 0);
        row.Children.Add(label);

        // Col 1 — status (version SHA when installed)
        var statusBlock = new TextBlock
        {
            Text = statusText,
            FontSize = 12,
            Foreground = UIFactory.GetBrush(statusColor),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalTextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
            TextDecorations = isInstalled ? Windows.UI.Text.TextDecorations.Underline : Windows.UI.Text.TextDecorations.None,
        };
        if (isInstalled)
        {
            ToolTipService.SetToolTip(statusBlock, $"Installed as: {currentDllName}\nClick to open GitHub releases");
            statusBlock.PointerPressed += (s, e) =>
                _ = Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sdli1995/dlssg_for_sm86"));
        }
        Grid.SetColumn(statusBlock, 1);
        row.Children.Add(statusBlock);

        // Col 2 — Info button
        var infoBtn = new Button
        {
            Content = "Info",
            FontSize = 11,
            Padding = new Thickness(6, 2, 6, 2),
            Width = 36,
            Height = 32,
            Background = UIFactory.Brush(ResourceKeys.AccentBlueBgBrush),
            Foreground = UIFactory.Brush(ResourceKeys.AccentBlueBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
        };
        ToolTipService.SetToolTip(infoBtn, "Open 20/30 FG 解锁 GitHub page");
        infoBtn.Click += (s, e) =>
            _ = Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sdli1995/dlssg_for_sm86"));
        Grid.SetColumn(infoBtn, 2);
        row.Children.Add(infoBtn);

        // Col 3 — Install button
        var installBtn = new Button
        {
            Content = isInstalled ? "↺  重新安装 20/30 FG" : "⬇  安装 20/30 FG",
            FontSize = 12,
            Height = 32,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CornerRadius = new CornerRadius(8),
            Background = isInstalled ? UIFactory.GetBrush("#182840") : UIFactory.Brush(ResourceKeys.AccentBlueBgBrush),
            Foreground = isInstalled ? UIFactory.GetBrush("#7AACDD") : UIFactory.Brush(ResourceKeys.AccentBlueBrush),
            BorderBrush = isInstalled ? UIFactory.GetBrush("#2A4468") : UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush),
            BorderThickness = new Thickness(1),
        };

        if (addonConflict)
        {
            installBtn.IsEnabled = false;
            installBtn.Opacity   = 0.35;
            installBtn.Content   = "请先卸载 MFG Ada Unlock";
            ToolTipService.SetToolTip(installBtn, "MFG Ada Unlock（附加组件）已安装并存在冲突。请先从附加组件选择器中移除它。");
        }
        else
        {
            ToolTipService.SetToolTip(installBtn, isInstalled
                ? $"重新安装 20/30 FG 解锁 (currently deployed as {currentDllName})"
                : "安装 20/30 FG 解锁 — deploys version.dll under a name you choose");
        }

        installBtn.Click += async (s, e) =>
        {
            if (string.IsNullOrEmpty(installPath)) return;

            var chosen = await ShowDlssg2030DllPickerAsync(card, currentDllName);
            if (chosen == null) return;

            installBtn.IsEnabled = false;
            installBtn.Content   = "安装中...";
            try
            {
                // If reinstalling with a different name, remove the old one first
                if (isInstalled && !string.IsNullOrEmpty(currentDllName)
                    && !currentDllName.Equals(chosen, StringComparison.OrdinalIgnoreCase))
                    svc.Uninstall(installPath, currentDllName);

                var gpuGen = _window.ViewModel.GetDlssg2030GpuGen(gameName, store);
                bool ok = await Task.Run(async () =>
                {
                    if (!svc.IsStagingReady || svc.HasUpdate)
                        await svc.EnsureStagingAsync().ConfigureAwait(false);
                    return svc.Install(installPath, chosen, gpuGen);
                });

                if (ok)
                {
                    _window.ViewModel.SetDlssg2030InstalledAs(gameName, chosen, store);
                    RequestExtrasRebuild(card);
                }
                else
                {
                    installBtn.Content   = "下载失败 — try again";
                    installBtn.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                CrashReporter.Log($"[BuildDlssg2030Row] 安装失败 — {ex.Message}");
                installBtn.Content   = "安装失败";
                installBtn.IsEnabled = true;
            }
        };
        Grid.SetColumn(installBtn, 3);
        row.Children.Add(installBtn);

        // Col 4 — Cog button
        var cogBtn = new Button
        {
            Width = 36, Height = 32, Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.SurfaceOverlayBrush),
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.BorderDefaultBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "⚙", FontSize = 14, HorizontalAlignment = HorizontalAlignment.Center },
        };
        ToolTipService.SetToolTip(cogBtn, "选择 GPU 代次");
        cogBtn.Click += async (s, e) =>
        {
            var currentGen = _window.ViewModel.GetDlssg2030GpuGen(gameName, store);
            var combo = new ComboBox
            {
                ItemsSource = new[] { Dlssg20_30Service.GpuGenRtx30, Dlssg20_30Service.GpuGenRtx20 },
                SelectedItem = currentGen,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            var dlg = new ContentDialog
            {
                Title = "20/30 FG 解锁 — GPU Generation",
                Content = new StackPanel
                {
                    Spacing = 8,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "选择您的 GPU 代次。这控制使用哪种渲染路径。\n\n" +
                                   "RTX 30 Series (SM86) — Ampere\n" +
                                   "RTX 20 Series (SM75) — Turing\n\n" +
                                   "更改后请重新安装以应用新设置。",
                            FontSize = 12,
                            TextWrapping = TextWrapping.Wrap,
                        },
                        combo,
                    },
                },
                PrimaryButtonText = "Save",
                CloseButtonText   = "Cancel",
                XamlRoot          = _window.Content.XamlRoot,
                RequestedTheme    = ElementTheme.Dark,
            };
            var result = await DialogService.ShowSafeAsync(dlg);
            if (result == ContentDialogResult.Primary && combo.SelectedItem is string selected)
            {
                _window.ViewModel.SetDlssg2030GpuGen(gameName, selected, store);
                // If installed, update the INI immediately
                if (isInstalled && !string.IsNullOrEmpty(currentDllName)
                    && !string.IsNullOrEmpty(installPath))
                {
                    var iniPath = System.IO.Path.Combine(installPath, Dlssg20_30Service.IniFileName);
                    if (System.IO.File.Exists(iniPath))
                        Dlssg20_30Service.ApplyRouterToIniPublic(iniPath, selected);
                }
                RequestExtrasRebuild(card);
            }
        };
        Grid.SetColumn(cogBtn, 4);
        row.Children.Add(cogBtn);

        // Col 5 — Remove button
        var removeBtn = new Button
        {
            Width = 36, Height = 32, Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.AccentRedBgBrush),
            Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.AccentPurpleBorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "✕", FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush) },
            Opacity = isInstalled ? 1.0 : 0,
            IsHitTestVisible = isInstalled,
        };
        ToolTipService.SetToolTip(removeBtn, "Remove 20/30 FG 解锁 from this game");
        removeBtn.Click += (s, e) =>
        {
            if (string.IsNullOrEmpty(installPath)) return;
            svc.Uninstall(installPath, currentDllName);
            _window.ViewModel.SetDlssg2030InstalledAs(gameName, null, store);
            RequestExtrasRebuild(card);
        };
        Grid.SetColumn(removeBtn, 5);
        row.Children.Add(removeBtn);

        body.Children.Add(row);
    }

    private async Task<string?> ShowDlssg2030DllPickerAsync(GameCardViewModel card, string? currentDllName)
    {
        if (string.IsNullOrEmpty(card.InstallPath)) return null;

        var rhiOwned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(card.RsInstalledFile))  rhiOwned.Add(card.RsInstalledFile);
        if (!string.IsNullOrEmpty(card.OsInstalledFile))  rhiOwned.Add(card.OsInstalledFile);
        if (!string.IsNullOrEmpty(card.DcInstalledFile))  rhiOwned.Add(card.DcInstalledFile);

        string? chosen = null;

        var listPanel    = new StackPanel { Spacing = 4 };
        var scrollViewer = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            MaxHeight = 360,
            Content   = listPanel,
        };

        foreach (var name in Dlssg20_30Service.ProxyNames)
        {
            bool isRecommended  = string.Equals(name, "version.dll", StringComparison.OrdinalIgnoreCase);
            bool isRhiOwned     = rhiOwned.Contains(name);
            bool isCurrent      = string.Equals(name, currentDllName, StringComparison.OrdinalIgnoreCase);

            var btn = new Button
            {
                HorizontalAlignment        = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Padding         = new Thickness(10, 6, 10, 6),
                CornerRadius    = new CornerRadius(6),
                BorderThickness = new Thickness(1),
                IsEnabled       = !isRhiOwned,
                Opacity         = isRhiOwned ? 0.4 : 1.0,
                Background  = isCurrent ? UIFactory.Brush(ResourceKeys.AccentBlueBgBrush) : UIFactory.Brush(ResourceKeys.SurfaceOverlayBrush),
                BorderBrush = isCurrent ? UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush) : UIFactory.Brush(ResourceKeys.BorderDefaultBrush),
            };

            var contentRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            contentRow.Children.Add(new TextBlock
            {
                Text = name,
                FontSize = 12,
                Foreground = isRhiOwned
                    ? UIFactory.Brush(ResourceKeys.TextTertiaryBrush)
                    : UIFactory.Brush(ResourceKeys.TextPrimaryBrush),
                VerticalAlignment = VerticalAlignment.Center,
            });
            if (isRecommended)
                contentRow.Children.Add(MakeBadge("推荐", "#1A3A20", "#6AE87A", "#2A5A30"));
            if (isRhiOwned)
                contentRow.Children.Add(MakeBadge("RHI 使用中", "#2A1818", "#CC6666", "#5A2828"));
            if (isCurrent)
                contentRow.Children.Add(MakeBadge("当前", "#182840", "#7AACDD", "#2A4468"));

            btn.Content = contentRow;
            if (isRhiOwned)
                ToolTipService.SetToolTip(btn, "此文件名已被 RHI 管理的组件使用。请选择其他名称。");

            btn.Tag    = name;
            btn.Click += (s, ev) =>
            {
                chosen = (s as Button)?.Tag as string;
                if (s is FrameworkElement fe)
                {
                    var dialog = FindParentContentDialog(fe);
                    dialog?.Hide();
                }
            };
            listPanel.Children.Add(btn);
        }

        var pickerDialog = new ContentDialog
        {
            Title = "Choose 20/30 FG 解锁 DLL name",
            Content = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock
                    {
                        Text = "安装前请在齿轮 (⚙) 中选择您的 GPU 代次。然后选择要部署 DLL 的文件名。",
                        FontSize = 11,
                        Foreground = UIFactory.Brush(ResourceKeys.TextTertiaryBrush),
                        TextWrapping = TextWrapping.Wrap,
                    },
                    scrollViewer,
                }
            },
            CloseButtonText = "Cancel",
            XamlRoot        = _window.Content.XamlRoot,
        };

        await DialogService.ShowSafeAsync(pickerDialog);
        return chosen;
    }

    private void BuildRtx40MfgRow(GameCardViewModel card, StackPanel body)
    {
        var mfgSvc       = App.Services.GetRequiredService<Rtx40MfgService>();
        var gameName     = card.GameName;
        var store        = card.Source ?? "";
        var installPath  = card.InstallPath ?? "";

        var currentDllName = _window.ViewModel.GetRtx40MfgInstalledAs(gameName, store);
        bool isInstalled   = !string.IsNullOrEmpty(currentDllName)
                             && !string.IsNullOrEmpty(installPath)
                             && File.Exists(Path.Combine(installPath, currentDllName));
        bool addonConflict = !string.IsNullOrEmpty(installPath) &&
            File.Exists(Path.Combine(installPath, "renodx-mfgunlock.addon64"));

        // Status
        string statusText  = isInstalled ? (mfgSvc.StagedVersion ?? "Installed") : "就绪";
        string statusColor = isInstalled ? "#5ECB7D" : "#A0AABB";

        var row = new Grid { ColumnSpacing = 8 };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });

        // Col 0 — label
        var label = new TextBlock
        {
            Text = "RTX 40 MFG",
            FontSize = 12,
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            VerticalAlignment = VerticalAlignment.Center,
        };
        ToolTipService.SetToolTip(label, "RTX 40 MFG Unlock — 在 RTX 40 系列 GPU 上启用超过 2x（最高 6x）的 DLSS 多帧生成倍数。独立 DLL，无需 ASI Loader。");
        Grid.SetColumn(label, 0);
        row.Children.Add(label);

        // Col 1 — status
        var statusBlock = new TextBlock
        {
            Text = statusText,
            FontSize = 12,
            Foreground = UIFactory.GetBrush(statusColor),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalTextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
            TextDecorations = isInstalled ? Windows.UI.Text.TextDecorations.Underline : Windows.UI.Text.TextDecorations.None,
        };
        if (isInstalled)
        {
            ToolTipService.SetToolTip(statusBlock, $"Installed as: {currentDllName}\nClick to open GitHub releases page");
            statusBlock.PointerPressed += (s, e) =>
                _ = Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/dashdogy/RTX40MFG-Unlock/releases"));
        }
        Grid.SetColumn(statusBlock, 1);
        row.Children.Add(statusBlock);

        // Col 2 — Info button
        var infoBtn = new Button
        {
            Content = "Info",
            FontSize = 11,
            Padding = new Thickness(6, 2, 6, 2),
            Width = 36,
            Height = 32,
            Background = UIFactory.Brush(ResourceKeys.AccentBlueBgBrush),
            Foreground = UIFactory.Brush(ResourceKeys.AccentBlueBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
        };
        ToolTipService.SetToolTip(infoBtn, "打开 RTX 40 MFG Unlock GitHub 页面");
        infoBtn.Click += (s, e) =>
            _ = Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/dashdogy/RTX40MFG-Unlock"));
        Grid.SetColumn(infoBtn, 2);
        row.Children.Add(infoBtn);

        // Col 3 — Install button
        var installBtn = new Button
        {
            Content = isInstalled ? "↺  重新安装 RTX 40 MFG" : "⬇  安装 RTX 40 MFG",
            FontSize = 12,
            Height = 32,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CornerRadius = new CornerRadius(8),
            Background = isInstalled
                ? UIFactory.GetBrush("#182840")
                : UIFactory.Brush(ResourceKeys.AccentBlueBgBrush),
            Foreground = isInstalled
                ? UIFactory.GetBrush("#7AACDD")
                : UIFactory.Brush(ResourceKeys.AccentBlueBrush),
            BorderBrush = isInstalled
                ? UIFactory.GetBrush("#2A4468")
                : UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush),
            BorderThickness = new Thickness(1),
        };

        if (addonConflict)
        {
            installBtn.IsEnabled = false;
            installBtn.Opacity   = 0.35;
            installBtn.Content   = "请先卸载 MFG Ada Unlock";
            ToolTipService.SetToolTip(installBtn, "MFG Ada Unlock（附加组件）已安装，与 RTX 40 MFG Unlock 冲突。请先从附加组件选择器中移除它。");
        }
        else
        {
            ToolTipService.SetToolTip(installBtn, isInstalled
                ? $"Reinstall RTX 40 MFG Unlock (currently deployed as {currentDllName})"
                : "安装 RTX 40 MFG Unlock — 以您选择的名称部署 RTXMFG.dll");
        }

        installBtn.Click += async (s, e) =>
        {
            if (string.IsNullOrEmpty(installPath)) return;

            var chosen = await ShowMfgDllPickerAsync(card, currentDllName);
            if (chosen == null) return;

            installBtn.IsEnabled = false;
            installBtn.Content   = "安装中...";
            try
            {
                // If reinstalling with a different name, remove the old one first
                if (isInstalled && !string.IsNullOrEmpty(currentDllName)
                    && !currentDllName.Equals(chosen, StringComparison.OrdinalIgnoreCase))
                    mfgSvc.Uninstall(installPath, currentDllName);

                bool ok = await Task.Run(async () =>
                {
                    if (!mfgSvc.IsStagingReady || mfgSvc.HasUpdate)
                        await mfgSvc.EnsureStagingAsync().ConfigureAwait(false);
                    return mfgSvc.Install(installPath, chosen);
                });

                if (ok)
                {
                    _window.ViewModel.SetRtx40MfgInstalledAs(gameName, chosen, store);
                    RequestExtrasRebuild(card);
                }
                else
                {
                    installBtn.Content   = "下载失败 — try again";
                    installBtn.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                CrashReporter.Log($"[BuildRtx40MfgRow] 安装失败 — {ex.Message}");
                installBtn.Content   = "安装失败";
                installBtn.IsEnabled = true;
            }
        };
        Grid.SetColumn(installBtn, 3);
        row.Children.Add(installBtn);

        // Col 4 — Cog button
        var cogBtn = new Button
        {
            Width = 36,
            Height = 32,
            Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.SurfaceOverlayBrush),
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.BorderDefaultBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "⚙", FontSize = 14, HorizontalAlignment = HorizontalAlignment.Center },
        };
        ToolTipService.SetToolTip(cogBtn, "RTX 40 MFG 设置 — 配置倍数模式");
        cogBtn.Click += async (s, e) =>
        {
            var installedNote = isInstalled ? $"\n\nCurrently installed as: {currentDllName}" : "";
            var dlg = new ContentDialog
            {
                Title = "RTX 40 MFG 设置",
                Content = new TextBlock
                {
                    Text = "在游戏内按 Backspace 打开 RTX 40 MFG 菜单。\n\n" +
                           "• 跟随游戏 — 使用游戏自身的 MFG 设置\n" +
                           "• 固定 2x–6x — 强制使用特定倍数\n" +
                           "• 动态 — 以显示器刷新率或自定义 FPS 值为目标\n\n" +
                           "如果帧率在 2x 以上冻结，请尝试在 NVIDIA App 中将帧生成设置为预设 B。" +
                           installedNote,
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap,
                },
                PrimaryButtonText = "打开 GitHub",
                CloseButtonText   = "Close",
                XamlRoot          = _window.Content.XamlRoot,
                RequestedTheme    = ElementTheme.Dark,
            };
            var result = await DialogService.ShowSafeAsync(dlg);
            if (result == ContentDialogResult.Primary)
                _ = Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/dashdogy/RTX40MFG-Unlock"));
        };
        Grid.SetColumn(cogBtn, 4);
        row.Children.Add(cogBtn);

        // Col 5 — Remove button
        var removeBtn = new Button
        {
            Width = 36,
            Height = 32,
            Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.AccentRedBgBrush),
            Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.AccentPurpleBorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "✕", FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush) },
            Opacity = isInstalled ? 1.0 : 0,
            IsHitTestVisible = isInstalled,
        };
        ToolTipService.SetToolTip(removeBtn, "从此游戏中移除 RTX 40 MFG Unlock");
        removeBtn.Click += (s, e) =>
        {
            if (string.IsNullOrEmpty(installPath)) return;
            mfgSvc.Uninstall(installPath, currentDllName);
            _window.ViewModel.SetRtx40MfgInstalledAs(gameName, null, store);
            RequestExtrasRebuild(card);
        };
        Grid.SetColumn(removeBtn, 5);
        row.Children.Add(removeBtn);

        body.Children.Add(row);
    }

    private static readonly string[] DeDllNames =
        { "version.dll", "dxgi.dll", "winmm.dll", "dbghelp.dll", "psapi.dll", "winhttp.dll" };

    private void BuildDlssEnablerRow(GameCardViewModel card, StackPanel body)
    {
        var deSvc       = App.Services.GetRequiredService<DlssEnablerService>();
        var gameName    = card.GameName;
        var store       = card.Source ?? "";
        var installPath = card.InstallPath ?? "";

        var currentDllName = _window.ViewModel.GetDeInstalledAs(gameName, store);
        bool isInstalled   = deSvc.IsStandaloneInstalledIn(installPath, currentDllName);

        // Mutual exclusivity with OptiScaler
        bool osConflict = card.IsOsInstalled;

        // Status text
        string statusText  = isInstalled ? (deSvc.StagedVersion ?? "Installed") : "就绪";
        string statusColor = isInstalled ? "#5ECB7D" : "#A0AABB";

        // ── Row grid matching Components section exactly ───────────────────────
        // Col 0: label (120)  Col 1: status (80)  Col 2: Info (36)
        // Col 3: install (*)  Col 4: cog (36)     Col 5: delete (36)
        var row = new Grid { ColumnSpacing = 8 };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });

        // Col 0 — label
        var label = new TextBlock
        {
            Text = "DLSS Enabler",
            FontSize = 12,
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            VerticalAlignment = VerticalAlignment.Center,
        };
        ToolTipService.SetToolTip(label, "DLSS Enabler — 独立的代理 DLL，可在不原生支持 DLSS 的游戏中启用 DLSS。");
        Grid.SetColumn(label, 0);
        row.Children.Add(label);

        // Col 1 — status
        var statusBlock = new TextBlock
        {
            Text = statusText,
            FontSize = 12,
            Foreground = UIFactory.GetBrush(statusColor),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalTextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
            TextDecorations = isInstalled ? Windows.UI.Text.TextDecorations.Underline : Windows.UI.Text.TextDecorations.None,
        };
        if (isInstalled)
        {
            ToolTipService.SetToolTip(statusBlock, $"Installed as: {currentDllName}\nClick to open GitHub releases");
            statusBlock.PointerPressed += (s, e) =>
                _ = Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/RankFTW/rhi-repo/releases"));
        }
        Grid.SetColumn(statusBlock, 1);
        row.Children.Add(statusBlock);

        // Col 2 — Info button
        var infoBtn = new Button
        {
            Content = "Info",
            FontSize = 11,
            Padding = new Thickness(6, 2, 6, 2),
            Width = 36,
            Height = 32,
            Background = UIFactory.Brush(ResourceKeys.AccentBlueBgBrush),
            Foreground = UIFactory.Brush(ResourceKeys.AccentBlueBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
        };
        ToolTipService.SetToolTip(infoBtn, "打开 DLSS Enabler Nexus 页面");
        infoBtn.Click += (s, e) =>
            _ = Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.nexusmods.com/site/mods/757"));
        Grid.SetColumn(infoBtn, 2);
        row.Children.Add(infoBtn);

        // Col 3 — Install button
        string installBtnLabel;
        if (osConflict)
            installBtnLabel = "通过 OptiScaler 安装";
        else
            installBtnLabel = isInstalled ? "↺  重新安装 DLSS Enabler" : "⬇  安装 DLSS Enabler";

        var installBtn = new Button
        {
            Content = installBtnLabel,
            FontSize = 12,
            Height = 32,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CornerRadius = new CornerRadius(8),
            Background = isInstalled
                ? UIFactory.GetBrush("#182840")
                : UIFactory.Brush(ResourceKeys.AccentBlueBgBrush),
            Foreground = isInstalled
                ? UIFactory.GetBrush("#7AACDD")
                : UIFactory.Brush(ResourceKeys.AccentBlueBrush),
            BorderBrush = isInstalled
                ? UIFactory.GetBrush("#2A4468")
                : UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush),
            BorderThickness = new Thickness(1),
        };

        if (osConflict)
        {
            installBtn.IsEnabled = false;
            installBtn.IsHitTestVisible = false;
            installBtn.Opacity = 0.35;
            ToolTipService.SetToolTip(installBtn, "无法与 OptiScaler 一起安装 — DLSS Enabler 已包含在 OptiScaler 中");
        }
        else
        {
            ToolTipService.SetToolTip(installBtn, isInstalled
                ? $"重新安装 DLSS Enabler (currently '{currentDllName}')"
                : "安装 DLSS Enabler — choose which DLL name to use");

            installBtn.Click += async (s, e) =>
            {
                if (string.IsNullOrEmpty(installPath)) return;
                var chosen = await ShowDeDllPickerAsync(card, currentDllName);
                if (chosen == null) return;

                installBtn.IsEnabled = false;
                installBtn.Content   = "安装中...";
                try
                {
                    bool ok = await deSvc.InstallStandaloneAsync(gameName, installPath, store, chosen, currentDllName);
                    if (ok)
                    {
                        _window.ViewModel.SetDeInstalledAs(gameName, chosen, store);
                        RequestExtrasRebuild(card);
                    }
                    else
                    {
                        installBtn.Content   = "安装失败";
                        installBtn.IsEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    CrashReporter.Log($"[BuildDlssEnablerRow] 安装失败 — {ex.Message}");
                    installBtn.Content   = "安装失败";
                    installBtn.IsEnabled = true;
                }
            };
        }
        Grid.SetColumn(installBtn, 3);
        row.Children.Add(installBtn);

        // Col 4 — Cog button
        var cogBtn = new Button
        {
            Width = 36,
            Height = 32,
            Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.SurfaceOverlayBrush),
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.BorderDefaultBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "⚙", FontSize = 14, HorizontalAlignment = HorizontalAlignment.Center },
            IsEnabled = !osConflict,
            IsHitTestVisible = !osConflict,
            Opacity = osConflict ? 0.35 : 1.0,
        };
        ToolTipService.SetToolTip(cogBtn, osConflict ? "无法与 OptiScaler 一起安装" : "DLSS Enabler 设置");
        cogBtn.Click += async (s, e) =>
        {
            var dlg = new ContentDialog
            {
                Title   = "DLSS Enabler 设置",
                Content = new TextBlock { Text = "暂无设置可用。", FontSize = 12 },
                CloseButtonText   = "Close",
                XamlRoot          = _window.Content.XamlRoot,
                RequestedTheme    = ElementTheme.Dark,
            };
            await DialogService.ShowSafeAsync(dlg);
        };
        Grid.SetColumn(cogBtn, 4);
        row.Children.Add(cogBtn);

        // Col 5 — Remove button (hidden when not installed)
        var removeBtn = new Button
        {
            Width = 36,
            Height = 32,
            Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.AccentRedBgBrush),
            Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.AccentPurpleBorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "✕", FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush) },
            Opacity = isInstalled ? 1.0 : 0,
            IsHitTestVisible = isInstalled,
        };
        ToolTipService.SetToolTip(removeBtn, "从此游戏中移除独立的 DLSS Enabler");
        removeBtn.Click += (s, e) =>
        {
            if (string.IsNullOrEmpty(installPath) || string.IsNullOrEmpty(currentDllName)) return;
            deSvc.UninstallStandalone(installPath, currentDllName);
            _window.ViewModel.SetDeInstalledAs(gameName, null, store);
            RequestExtrasRebuild(card);
        };
        Grid.SetColumn(removeBtn, 5);
        row.Children.Add(removeBtn);

        body.Children.Add(row);
    }

    private async Task<string?> ShowMfgDllPickerAsync(GameCardViewModel card, string? currentDllName)
    {
        if (string.IsNullOrEmpty(card.InstallPath)) return null;

        var rhiOwned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(card.RsInstalledFile))  rhiOwned.Add(card.RsInstalledFile);
        if (!string.IsNullOrEmpty(card.OsInstalledFile))  rhiOwned.Add(card.OsInstalledFile);
        if (!string.IsNullOrEmpty(card.DcInstalledFile))  rhiOwned.Add(card.DcInstalledFile);

        string? chosen = null;

        var listPanel    = new StackPanel { Spacing = 4 };
        var scrollViewer = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            MaxHeight = 420,
            Content   = listPanel,
        };

        foreach (var name in MfgDllNames)
        {
            bool isRecommended  = string.Equals(name, "version.dll", StringComparison.OrdinalIgnoreCase);
            bool isRhiOwned     = rhiOwned.Contains(name);
            bool isDxgiConflict = string.Equals(name, "dxgi.dll", StringComparison.OrdinalIgnoreCase)
                               && !string.IsNullOrEmpty(card.RsInstalledFile);
            bool isCurrent      = string.Equals(name, currentDllName, StringComparison.OrdinalIgnoreCase);

            var btn = new Button
            {
                HorizontalAlignment        = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Padding         = new Thickness(10, 6, 10, 6),
                CornerRadius    = new CornerRadius(6),
                BorderThickness = new Thickness(1),
                IsEnabled       = !isRhiOwned,
                Opacity         = isRhiOwned ? 0.4 : 1.0,
                Background  = isCurrent ? UIFactory.Brush(ResourceKeys.AccentBlueBgBrush) : UIFactory.Brush(ResourceKeys.SurfaceOverlayBrush),
                BorderBrush = isCurrent ? UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush) : UIFactory.Brush(ResourceKeys.BorderDefaultBrush),
            };

            var contentRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            contentRow.Children.Add(new TextBlock
            {
                Text = name,
                FontSize = 12,
                Foreground = isRhiOwned
                    ? UIFactory.Brush(ResourceKeys.TextTertiaryBrush)
                    : UIFactory.Brush(ResourceKeys.TextPrimaryBrush),
                VerticalAlignment = VerticalAlignment.Center,
            });
            if (isRecommended)
                contentRow.Children.Add(MakeBadge("推荐", "#1A3A20", "#6AE87A", "#2A5A30"));
            if (isRhiOwned)
                contentRow.Children.Add(MakeBadge("RHI 使用中", "#2A1818", "#CC6666", "#5A2828"));
            else if (isDxgiConflict)
                contentRow.Children.Add(MakeBadge("可能与 ReShade/OS 冲突", "#2A1A10", "#CC9955", "#5A3A18"));
            if (isCurrent)
                contentRow.Children.Add(MakeBadge("当前", "#182840", "#7AACDD", "#2A4468"));

            btn.Content = contentRow;

            if (isRhiOwned)
                ToolTipService.SetToolTip(btn, "此文件名已被 RHI 管理的组件使用。请选择其他名称。");
            else if (isDxgiConflict)
                ToolTipService.SetToolTip(btn, "如果 ReShade 或 OptiScaler 也使用此名称，dxgi.dll 可能会与之冲突。");

            btn.Tag    = name;
            btn.Click += (s, ev) =>
            {
                chosen = (s as Button)?.Tag as string;
                if (s is FrameworkElement fe)
                {
                    var dialog = FindParentContentDialog(fe);
                    dialog?.Hide();
                }
            };

            listPanel.Children.Add(btn);
        }

        var pickerDialog = new ContentDialog
        {
            Title = "选择 RTX 40 MFG DLL 名称",
            Content = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock
                    {
                        Text = "选择部署 RTXMFG.dll 的文件名。请选择游戏较早加载的名称，并避免使用其他模组已使用的名称。",
                        FontSize = 11,
                        Foreground = UIFactory.Brush(ResourceKeys.TextTertiaryBrush),
                        TextWrapping = TextWrapping.Wrap,
                    },
                    scrollViewer,
                }
            },
            CloseButtonText = "Cancel",
            XamlRoot        = _window.Content.XamlRoot,
        };

        await DialogService.ShowSafeAsync(pickerDialog);
        return chosen;
    }

    private async Task<string?> ShowDeDllPickerAsync(GameCardViewModel card, string? currentDllName)
    {
        if (string.IsNullOrEmpty(card.InstallPath)) return null;

        // RHI-managed filenames to flag as conflict
        var rhiOwned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(card.RsInstalledFile))  rhiOwned.Add(card.RsInstalledFile);
        if (!string.IsNullOrEmpty(card.OsInstalledFile))  rhiOwned.Add(card.OsInstalledFile);
        if (!string.IsNullOrEmpty(card.DcInstalledFile))  rhiOwned.Add(card.DcInstalledFile);

        string? chosen = null;

        var listPanel    = new StackPanel { Spacing = 4 };
        var scrollViewer = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            MaxHeight = 420,
            Content   = listPanel,
        };

        foreach (var name in DeDllNames)
        {
            bool isRecommended = string.Equals(name, "version.dll", StringComparison.OrdinalIgnoreCase);
            bool isRhiOwned    = rhiOwned.Contains(name);
            bool isDxgiConflict = string.Equals(name, "dxgi.dll", StringComparison.OrdinalIgnoreCase)
                               && !string.IsNullOrEmpty(card.RsInstalledFile);
            bool isCurrent     = string.Equals(name, currentDllName, StringComparison.OrdinalIgnoreCase);

            var btn = new Button
            {
                HorizontalAlignment        = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Padding         = new Thickness(10, 6, 10, 6),
                CornerRadius    = new CornerRadius(6),
                BorderThickness = new Thickness(1),
                IsEnabled       = !isRhiOwned,
                Opacity         = isRhiOwned ? 0.4 : 1.0,
            };

            if (isCurrent)
            {
                btn.Background  = UIFactory.Brush(ResourceKeys.AccentBlueBgBrush);
                btn.BorderBrush = UIFactory.Brush(ResourceKeys.AccentBlueBorderBrush);
            }
            else
            {
                btn.Background  = UIFactory.Brush(ResourceKeys.SurfaceOverlayBrush);
                btn.BorderBrush = UIFactory.Brush(ResourceKeys.BorderDefaultBrush);
            }

            var contentRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            contentRow.Children.Add(new TextBlock
            {
                Text = name,
                FontSize = 12,
                Foreground = isRhiOwned
                    ? UIFactory.Brush(ResourceKeys.TextTertiaryBrush)
                    : UIFactory.Brush(ResourceKeys.TextPrimaryBrush),
                VerticalAlignment = VerticalAlignment.Center,
            });

            if (isRecommended)
                contentRow.Children.Add(MakeBadge("推荐", "#1A3A20", "#6AE87A", "#2A5A30"));
            if (isRhiOwned)
                contentRow.Children.Add(MakeBadge("RHI 使用中", "#2A1818", "#CC6666", "#5A2828"));
            else if (isDxgiConflict)
                contentRow.Children.Add(MakeBadge("可能与 ReShade/OS 冲突", "#2A1A10", "#CC9955", "#5A3A18"));
            if (isCurrent)
                contentRow.Children.Add(MakeBadge("当前", "#182840", "#7AACDD", "#2A4468"));

            btn.Content = contentRow;

            if (isRhiOwned)
                ToolTipService.SetToolTip(btn, "此文件名已被 RHI 管理的组件（ReShade、OptiScaler 或 DC）使用。请选择其他名称。");
            else if (isDxgiConflict)
                ToolTipService.SetToolTip(btn, "如果 ReShade 或 OS 组件也使用此名称，dxgi.dll 可能会与之冲突。");

            btn.Tag    = name;
            btn.Click += (s, ev) =>
            {
                chosen = (s as Button)?.Tag as string;
                if (s is FrameworkElement fe)
                {
                    var dialog = FindParentContentDialog(fe);
                    dialog?.Hide();
                }
            };

            listPanel.Children.Add(btn);
        }

        var pickerDialog = new ContentDialog
        {
            Title = "选择 DLSS Enabler DLL 名称",
            Content = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock
                    {
                        Text = "选择独立 DLSS Enabler DLL 的文件名。大多数游戏可使用 version.dll。",
                        FontSize = 11,
                        Foreground = UIFactory.Brush(ResourceKeys.TextTertiaryBrush),
                        TextWrapping = TextWrapping.Wrap,
                    },
                    scrollViewer,
                }
            },
            CloseButtonText = "Cancel",
            XamlRoot        = _window.Content.XamlRoot,
        };

        await DialogService.ShowSafeAsync(pickerDialog);
        return chosen;
    }

    private void BuildDxvkRow(GameCardViewModel card, StackPanel body)
    {
        // Col 0: label (120)  Col 1: status (80)  Col 2: Info (36)
        // Col 3: install (*)  Col 4: cog (36)     Col 5: delete (36)
        var row = new Grid { ColumnSpacing = 8 };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });

        bool isDisabled = !card.IsDxvkToggleEnabled;

        // Col 0 — label
        var label = new TextBlock
        {
            Text = "DXVK",
            FontSize = 12,
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            VerticalAlignment = VerticalAlignment.Center,
            Opacity = isDisabled ? 0.35 : 1.0,
            Tag = card,
        };
        if (card.DxvkToggleTooltip != null)
            ToolTipService.SetToolTip(label, card.DxvkToggleTooltip);
        Grid.SetColumn(label, 0);
        row.Children.Add(label);

        // Col 1 — status
        var statusBlock = new TextBlock
        {
            Text = card.DxvkStatusText,
            FontSize = 12,
            Foreground = UIFactory.GetBrush(card.DxvkStatusColor),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalTextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
            TextDecorations = card.IsDxvkInstalled
                ? Windows.UI.Text.TextDecorations.Underline
                : Windows.UI.Text.TextDecorations.None,
            Opacity = isDisabled ? 0.35 : 1.0,
        };
        if (card.IsDxvkInstalled && !isDisabled)
        {
            ToolTipService.SetToolTip(statusBlock, "点击打开 DXVK releases");
            statusBlock.PointerPressed += (s, e) => _window.DetailDxvkStatus_PointerPressed(s, e);
            statusBlock.PointerEntered += (s, e) => _window.LinkText_PointerEntered(s, e);
            statusBlock.PointerExited  += (s, e) => _window.LinkText_PointerExited(s, e);
        }
        Grid.SetColumn(statusBlock, 1);
        row.Children.Add(statusBlock);

        // Col 2 — Info button
        var infoBtn = new Button
        {
            Content = "Info",
            FontSize = 11,
            Padding = new Thickness(6, 2, 6, 2),
            Width = 36,
            Height = 32,
            CornerRadius = new CornerRadius(8),
            Background = UIFactory.Brush(ResourceKeys.SurfaceOverlayBrush),
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.BorderStrongBrush),
            BorderThickness = new Thickness(1),
            Tag = card,
            Opacity = isDisabled ? 0.35 : 1.0,
        };
        infoBtn.Click += (s, e) => _window.DxvkInfoButton_Click(s, e);
        Grid.SetColumn(infoBtn, 2);
        row.Children.Add(infoBtn);

        // Col 3 — Install button
        var installBtn = new Button
        {
            Height = 32,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            CornerRadius = new CornerRadius(8),
            FontSize = 12,
            Background = UIFactory.GetBrush(card.DxvkBtnBackground),
            Foreground = UIFactory.GetBrush(card.DxvkBtnForeground),
            BorderBrush = UIFactory.GetBrush(card.DxvkBtnBorderBrush),
            BorderThickness = new Thickness(1),
            Tag = card,
            IsEnabled = card.DxvkInstallEnabled && !isDisabled,
            Opacity = isDisabled ? 0.35 : 1.0,
            IsHitTestVisible = !isDisabled,
        };
        installBtn.Content = card.DxvkActionLabel;
        installBtn.Click += (s, e) => _window.InstallDxvkButton_Click(s, e);
        Grid.SetColumn(installBtn, 3);
        row.Children.Add(installBtn);

        // Col 4 — Cog button
        var cogBtn = new Button
        {
            Width = 36,
            Height = 32,
            Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.SurfaceOverlayBrush),
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.BorderStrongBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "⚙", FontSize = 14, HorizontalAlignment = HorizontalAlignment.Center },
            Tag = card,
            IsEnabled = !isDisabled,
            Opacity = isDisabled ? 0.35 : 1.0,
        };
        ToolTipService.SetToolTip(cogBtn, "DXVK 设置");
        cogBtn.Click += (s, e) => _window.DxvkCogButton_Click(s, e);
        Grid.SetColumn(cogBtn, 4);
        row.Children.Add(cogBtn);

        // Col 5 — Delete button
        bool showDelete = card.DxvkDeleteVisibility == Visibility.Visible;
        var deleteBtn = new Button
        {
            Width = 36,
            Height = 32,
            Padding = new Thickness(0),
            Background = UIFactory.Brush(ResourceKeys.AccentRedBgBrush),
            Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush),
            BorderBrush = UIFactory.Brush(ResourceKeys.AccentPurpleBorderBrush),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Content = new TextBlock { Text = "✕", FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, Foreground = UIFactory.Brush(ResourceKeys.AccentRedBrush) },
            Tag = card,
            Opacity = showDelete ? 1.0 : 0.0,
            IsHitTestVisible = showDelete,
        };
        ToolTipService.SetToolTip(deleteBtn, "Remove DXVK");
        deleteBtn.Click += (s, e) => _window.UninstallDxvkButton_Click(s, e);
        Grid.SetColumn(deleteBtn, 5);
        row.Children.Add(deleteBtn);

        body.Children.Add(row);
    }
}
