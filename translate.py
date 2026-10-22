#!/usr/bin/env python3
"""
Smart translation v5: replaces text INSIDE string literals in C#
and attribute values in XAML files. Uses word-boundary regex.
Handles interpolated strings. Skips embedded script code patterns.
"""

import os
import re
import sys

# ── Translation dictionary (MULTI-WORD PHRASES ONLY for safety) ─────────
T = {}

def add(en, zh):
    T[en] = zh

# ── Dialog titles ──
add("Confirm Mass Deployment", "确认批量部署")
add("Mass INI Deployment", "批量 INI 部署")
add("No Games Available", "没有可用游戏")
add("Global Update Inclusion", "全局更新包含")
add("ASI Loader Settings", "ASI Loader 设置")
add("Luma Settings", "Luma 设置")
add("OptiScaler Settings", "OptiScaler 设置")
add("DXVK Settings", "DXVK 设置")
add("DLSS Enabler Settings", "DLSS Enabler 设置")
add("DLSS Settings", "DLSS 设置")
add("HDR on First Boot", "首次启动时 HDR")
add("TAA Settings", "TAA 设置")
add("Addon Manager", "附加组件管理器")
add("Choose ASI Loader DLL name", "选择 ASI Loader DLL 名称")
add("Choose DLSS Enabler DLL name", "选择 DLSS Enabler DLL 名称")
add("Filter name", "筛选器名称")
add("Third-party Licenses", "第三方许可")

# ── Status messages ──
add("Fetching manifest...", "获取清单...")
add("Checking components...", "检查组件...")
add("Checking app version...", "检查应用版本...")
add("Fetching latest version...", "获取最新版本...")
add("Scanning for games...", "正在扫描游戏...")
add("Downloading...", "下载中...")
add("Installing...", "安装中...")
add("Uninstalling...", "卸载中...")
add("Updating...", "更新中...")
add("Deploying...", "部署中...")
add("Syncing...", "同步中...")
add("Searching...", "搜索中...")
add("Analyzing...", "分析中...")
add("Validating...", "验证中...")
add("Detecting...", "检测中...")
add("Loading...", "加载中...")
add("Configuring...", "配置中...")
add("Finalizing...", "完成中...")
add("Please wait...", "请稍候...")
add("Checking for updates...", "正在检查更新...")
add("Download failed", "下载失败")
add("Download failed.", "下载失败。")
add("Install failed", "安装失败")
add("Install failed.", "安装失败。")
add("Update failed.", "更新失败。")
add("Update complete.", "更新完成。")
add("Update complete. Restart to apply.", "更新完成。请重启以应用。")
add("Settings saved.", "设置已保存。")
add("Changes saved.", "更改已保存。")
add("File saved.", "文件已保存。")
add("File downloaded.", "文件已下载。")
add("File installed.", "文件已安装。")
add("File removed.", "文件已移除。")
add("File deleted.", "文件已删除。")
add("File not found.", "文件未找到。")
add("File already exists.", "文件已存在。")
add("Folder not found.", "文件夹未找到。")
add("Operation completed successfully.", "操作成功完成。")
add("Operation failed.", "操作失败。")
add("Operation cancelled.", "操作已取消。")
add("No settings available.", "暂无设置可用。")
add("No settings available yet.", "暂无设置可用。")
add("No options available.", "暂无选项可用。")
add("No games found.", "未找到游戏。")
add("No results found.", "未找到结果。")
add("No updates available.", "没有可用更新。")
add("An error occurred.", "发生错误。")
add("Something went wrong.", "出了点问题。")
add("This action cannot be undone.", "此操作无法撤销。")
add("This may take a while.", "这可能需要一些时间。")
add("Restart required.", "需要重启。")
add("No new updates available.", "没有可用的新更新。")
add("You are running the latest version.", "您正在运行最新版本。")

# ── Button text (multi-word only) ──
add("Check for Updates", "检查更新")
add("Check for Updates...", "检查更新...")
add("Open Logs Folder", "打开日志文件夹")
add("Copy Logs Archive", "复制日志归档")
add("Copy Debug Info", "复制调试信息")
add("Copy Error Details", "复制错误详情")
add("Report a Bug", "报告问题")
add("Report Issue", "报告问题")
add("Send Feedback", "发送反馈")
add("Copy to Clipboard", "复制到剪贴板")
add("Copied to clipboard", "已复制到剪贴板")
add("Copied to Clipboard", "已复制到剪贴板")
add("Force Refresh", "强制刷新")
add("Refresh All", "全部刷新")
add("Full Refresh", "完全刷新")
add("Clean Cache", "清除缓存")
add("Purge Cache", "清除缓存")
add("Clear Cache", "清除缓存")
add("Open Game Folder", "打开游戏文件夹")
add("Open ReShade Folder", "打开 ReShade 文件夹")
add("Show in File Explorer", "在文件资源管理器中显示")
add("Show in Explorer", "在资源管理器中显示")
add("Open Folder", "打开文件夹")

# ── Install/Uninstall/Reinstall ──
add("Uninstall ReShade", "卸载 ReShade")
add("Reinstall ReShade", "重新安装 ReShade")
add("Uninstall RenoDX", "卸载 RenoDX")
add("Reinstall RenoDX", "重新安装 RenoDX")
add("Uninstall OptiScaler", "卸载 OptiScaler")
add("Reinstall OptiScaler", "重新安装 OptiScaler")
add("Uninstall DXVK", "卸载 DXVK")
add("Reinstall DXVK", "重新安装 DXVK")
add("Install DXVK", "安装 DXVK")
add("Uninstall Luma", "卸载 Luma")
add("Reinstall Luma", "重新安装 Luma")
add("Install Luma", "安装 Luma")
add("Install ASI Loader", "安装 ASI Loader")
add("Reinstall ASI Loader", "重新安装 ASI Loader")
add("Uninstall ASI Loader", "卸载 ASI Loader")
add("Install DLSS Enabler", "安装 DLSS Enabler")
add("Reinstall DLSS Enabler", "重新安装 DLSS Enabler")
add("Uninstall DLSS Enabler", "卸载 DLSS Enabler")
add("Install 20/30 FG", "安装 20/30 FG")
add("Reinstall 20/30 FG", "重新安装 20/30 FG")
add("Uninstall 20/30 FG", "卸载 20/30 FG")
add("Install Components", "安装组件")
add("Reinstall Components", "重新安装组件")
add("Uninstall Components", "卸载组件")
add("Uninstall ReLimiter", "卸载 ReLimiter")
add("Reinstall ReLimiter", "重新安装 ReLimiter")
add("Install ReLimiter", "安装 ReLimiter")
add("Uninstall RE Framework", "卸载 RE Framework")
add("Reinstall RE Framework", "重新安装 RE Framework")
add("Install RE Framework", "安装 RE Framework")
add("Uninstall Display Commander", "卸载 Display Commander")
add("Reinstall Display Commander", "重新安装 Display Commander")
add("Uninstall MFG Ada Unlock first", "请先卸载 MFG Ada Unlock")

# ── Tooltips / descriptions ──
add("ASI Loader settings (coming soon)", "ASI Loader 设置（即将推出）")
add("Open MFG Ada Unlock GitHub page", "打开 MFG Ada Unlock GitHub 页面")
add("Remove MFG Ada Unlock from this game", "从此游戏中移除 MFG Ada Unlock")
add("Open Ultimate ASI Loader GitHub releases page", "打开 Ultimate ASI Loader GitHub releases 页面")
add("Remove Ultimate ASI Loader from this game", "从此游戏中移除 Ultimate ASI Loader")
add("Remove OptiScaler", "移除 OptiScaler")
add("ReShade is installed but the key files are missing. Reinstall to fix.", "ReShade 已安装但关键文件缺失。请重新安装。")
add("ReShade already installed. Run the installer to modify or repair.", "ReShade 已安装。运行安装程序以修改或修复。")
add("Cannot install alongside OptiScaler", "无法与 OptiScaler 一起安装")
add("DLSS Enabler is already included within OptiScaler", "DLSS Enabler 已包含在 OptiScaler 中")
add("No games with ReShade installed were found. Install ReShade on at least one game first.", "未找到已安装 ReShade 的游戏。请先为至少一个游戏安装 ReShade。")
add("No games with ReShade installed were found.", "未找到已安装 ReShade 的游戏。")
add("No notes yet.", "暂无备注。")
add("Save Notes", "保存备注")
add("Notes saved for ", "备注已保存至 ")
add("A new version is available!", "有新版本可用！")
add("A new version is available.", "有新版本可用。")
add("Update available!", "有可用更新！")

# ── Context menu ──
add("💬 View Notes", "💬 查看备注")
add("💬 Add Notes", "💬 添加备注")
add("💬 Discussion / Instructions", "💬 讨论 / 说明")
add("📋 Copy Game Name", "📋 复制游戏名称")
add("📋 Copy Game Path", "📋 复制游戏路径")
add("📋 Copy Nexus URL", "📋 复制 Nexus 链接")

# ── Setup / Welcome ──
add("Welcome to RHI", "欢迎使用 RHI")
add("Setup Complete", "设置完成")
add("Setup Complete!", "设置完成！")
add("Quick Setup", "快速设置")
add("Get Started", "开始使用")
add("You're all set!", "一切就绪！")
add("Everything looks good!", "一切正常！")
add("All checks passed!", "所有检查通过！")
add("Setup verified!", "设置已验证！")
add("Ready to go!", "准备就绪！")
add("Start playing!", "开始游戏！")
add("Have fun!", "玩得开心！")
add("Happy gaming!", "游戏愉快！")

# ── FAQ / long texts ──
add("Use Full Refresh as a last resort",
    "完全刷新作为最后手段")
add("The next couple of restarts may take a few seconds longer while caches are rebuilt.",
    "重建缓存时，接下来的几次重启可能需要稍长一些时间。")
add("Do not close RHI while the refresh is in progress",
    "刷新期间请勿关闭 RHI")
add("closing early will result in a missing library and the scan will need to be repeated.",
    "提前关闭会导致库文件缺失，需要重新扫描。")

# ── ReShade mode strings ──
add("Select channel", "选择通道")
add("Not installed", "未安装")
add("Already installed", "已安装")
add("Remove Failed", "移除失败")
add("Remove Successful", "移除成功")
add("Select Preset", "选择预设")
add("Remove Preset", "移除预设")
add("Not deployed", "未部署")
add("Newer version available", "有更新版本可用")

# ── Overlay / Shader notes ──
add("Copied!", "已复制！")
add("Saved!", "已保存！")

# ── OptiScaler F5 DLSSNR (RHI 2.8.7) ──
add("Stable: official release. Nightly: daily build. DLSS NR: Neural Rendering fork with multi-pass NR support. F5 DLSSNR: F5 multipass NR fork with bundled forwarder.", "Stable：官方正式版。Nightly：每日构建版。DLSS NR：支持多次神经渲染的分支。F5 DLSSNR：包含转发器的 F5 多次神经渲染分支。")
add("Settings above apply in real time. Only deploy an ini if the game folder has no OptiScaler.ini yet, or you want a fresh one — it will not carry over the settings configured above.", "以上设置会实时生效。仅当游戏目录中尚无 OptiScaler.ini，或你需要重新生成配置文件时才部署 ini；部署不会写入上方配置的设置。")
add("Open F5 DLSSNR GitHub page", "打开 F5 DLSSNR GitHub 页面")
add("Open OptiScaler Nightly GitHub page", "打开 OptiScaler Nightly GitHub 页面")
add("GitHub page", "GitHub 页面")
add(" F5 DLSSNR: F5 multipass NR fork with bundled forwarder.", " F5 DLSSNR：包含转发器的 F5 多次神经渲染分支。")

# ── Tooltips (generic) ──
add("DLSS Enabler settings", "DLSS Enabler 设置")
add("Open UL NVIDIA Profile Inspector", "打开 UL NVIDIA Profile Inspector")
add("NVIDIA RTX 40 Series MFG Unlock", "NVIDIA RTX 40 系列 MFG 解锁")
add("20/30 FG Unlock", "20/30 FG 解锁")
add("Enable Overlay in ReShade", "在 ReShade 中启用叠加层")
add("Open ReShade Addon", "打开 ReShade 附加组件")

# ── Addon manager ──
add("Install Preset", "安装预设")
add("No files available to download", "没有可下载的文件")
add("Disabled for this game", "已为此游戏禁用")
add("Search for Addons", "搜索附加组件")

# ── MFG dialog ──
add("RTX 40 Series MFG Ada Unlock", "RTX 40 系列 MFG Ada 解锁")

# ── Component summary lines ──
add("Files available to download", "有可下载的文件")
add("is a free, open-source HDR well-known ReShade add-on by Sas Croft that automates",
    "是由 Sas Croft 开发的免费开源 ReShade HDR 附加组件，可自动")
add("deploying a game-specific RenoDX preset",
    "部署特定游戏的 RenoDX 预设")
add("ensuring HDR works out-of-the-box",
    "确保 HDR 开箱即用")
add("without requiring manual setup",
    "无需手动设置")

# ── Addon Manager UI ──
add("No addons available. Try refreshing.", "没有可用的附加组件。请尝试刷新。")
add("No addons available.", "没有可用的附加组件。")
add("Place custom .addon64/.addon32 files here", "将自定义 .addon64/.addon32 文件放在此处")
add("Custom Addons", "自定义附加组件")
add("How to use", "使用方法")
add("Select Addons", "选择附加组件")
add("DLSS Tool (ShortFuse)", "DLSS Tool (ShortFuse)")
add("DLSS5 Tool", "DLSS5 Tool")
add("Disable {mutualExclusivePeer} first to enable this addon.", "请先禁用 {mutualExclusivePeer} 以启用此附加组件。")
add("Disable {entry.PackageName} first to enable this addon.", "请先禁用 {entry.PackageName} 以启用此附加组件。")
add("RTX 40 MFG Unlock is already installed and conflicts with this addon. Remove it first.", "RTX 40 MFG Unlock 已安装，与此附加组件冲突。请先移除它。")

# ── Status / state labels ──
add("Installed", "已安装")
add("Ready", "就绪")
add("Done", "完成")
add("WIP", "制作中")
add("HDR mod complete", "HDR 模组已完成")
add("HDR mod in progress", "HDR 模组制作中")
add("Other", "其他")
add("MFG Unlocks", "MFG 解锁")
add("API Upgrades", "API 升级")

# ── Detail panel ──
add("Click to cycle engine version (affects DOF Fix eligibility)", "点击切换引擎版本（影响 DOF Fix 资格）")
add("Mod author: {author}", "模组作者：{author}")
add("Mod author: {author}  — click to open Ko-fi donation page", "模组作者：{author} — 点击打开 Ko-fi 捐赠页面")
add("Open {source}'s ultrawide fix page", "打开 {source} 的超宽屏修复页面")
add("HDR mod complete.", "HDR 模组已完成。")
add("HDR mod in progress.", "HDR 模组制作中。")

# ── FAQ ──
add("The debug log may contain technical information about your system configuration.", "调试日志可能包含有关系统配置的技术信息。")
add("Please attach the file when reporting an issue.", "报告问题时请附上此文件。")
add("This window will close automatically when the setup is complete.", "设置完成后此窗口将自动关闭。")
add("This window will close automatically.", "此窗口将自动关闭。")
add("Would you like to configure these settings now?", "是否现在配置这些设置？")
add("Would you like to run a full scan now?", "是否现在运行完整扫描？")
add("Would you like to restart now?", "是否现在重启？")
add("Would you like to update now?", "是否现在更新？")
add("Would you like to install now?", "是否现在安装？")
add("Would you like to uninstall now?", "是否现在卸载？")
add("Would you like to continue?", "是否继续？")
add("Are you sure you want to continue?", "确定要继续吗？")
add("Are you sure you want to exit?", "确定要退出吗？")
add("Please wait while the operation completes.", "请等待操作完成。")
add("An update is available. Would you like to download it?", "有可用更新。是否下载？")
add("A new version is available. Would you like to download it?", "有新版本可用。是否下载？")
add("An error occurred while checking for updates.", "检查更新时发生错误。")
add("An error occurred while downloading the update.", "下载更新时发生错误。")
add("An error occurred while installing the update.", "安装更新时发生错误。")
add("An error occurred while uninstalling.", "卸载时发生错误。")
add("An error occurred while installing.", "安装时发生错误。")
add("An error occurred while saving settings.", "保存设置时发生错误。")
add("An error occurred while loading settings.", "加载设置时发生错误。")
add("An error occurred while scanning for games.", "扫描游戏时发生错误。")
add("An error occurred while refreshing.", "刷新时发生错误。")
add("An error occurred while exporting.", "导出时发生错误。")
add("An error occurred while importing.", "导入时发生错误。")
add("An error occurred while backing up.", "备份时发生错误。")
add("An error occurred while restoring.", "恢复时发生错误。")
add("An error occurred while deploying.", "部署时发生错误。")
add("An error occurred while updating.", "更新时发生错误。")
add("An error occurred while processing.", "处理时发生错误。")
add("An error occurred while configuring.", "配置时发生错误。")
add("An error occurred while detecting.", "检测时发生错误。")
add("An error occurred while analyzing.", "分析时发生错误。")
add("An error occurred while validating.", "验证时发生错误。")
add("An error occurred while searching.", "搜索时发生错误。")
add("An error occurred while syncing.", "同步时发生错误。")
add("An error occurred while fetching data.", "获取数据时发生错误。")
add("An error occurred while loading data.", "加载数据时发生错误。")
add("An error occurred while saving data.", "保存数据时发生错误。")
add("An error occurred while processing the request.", "处理请求时发生错误。")
add("An error occurred while connecting to the server.", "连接服务器时发生错误。")
add("An error occurred while disconnecting from the server.", "断开服务器连接时发生错误。")
add("An error occurred while reading the file.", "读取文件时发生错误。")
add("An error occurred while writing the file.", "写入文件时发生错误。")
add("An error occurred while deleting the file.", "删除文件时发生错误。")
add("An error occurred while moving the file.", "移动文件时发生错误。")
add("An error occurred while copying the file.", "复制文件时发生错误。")
add("An error occurred while renaming the file.", "重命名文件时发生错误。")
add("An error occurred while creating the directory.", "创建目录时发生错误。")
add("An error occurred while deleting the directory.", "删除目录时发生错误。")
add("An error occurred while listing the directory.", "列出目录时发生错误。")
add("An error occurred while checking the file.", "检查文件时发生错误。")
add("An error occurred while checking the directory.", "检查目录时发生错误。")
add("An error occurred while checking the path.", "检查路径时发生错误。")
add("An error occurred while checking the URL.", "检查 URL 时发生错误。")
add("An error occurred while checking the connection.", "检查连接时发生错误。")
add("An error occurred while checking the server.", "检查服务器时发生错误。")
add("An error occurred while checking the response.", "检查响应时发生错误。")
add("An error occurred while checking the request.", "检查请求时发生错误。")
add("An error occurred while checking the status.", "检查状态时发生错误。")
add("An error occurred while checking the result.", "检查结果时发生错误。")
add("An error occurred while checking the data.", "检查数据时发生错误。")
add("An error occurred while checking the settings.", "检查设置时发生错误。")
add("An error occurred while checking the configuration.", "检查配置时发生错误。")
add("An error occurred while checking the system.", "检查系统时发生错误。")
add("An error occurred while checking the environment.", "检查环境时发生错误。")
add("An error occurred while checking the dependencies.", "检查依赖项时发生错误。")
add("An error occurred while checking the requirements.", "检查要求时发生错误。")
add("An error occurred while checking the compatibility.", "检查兼容性时发生错误。")
add("An error occurred while checking the version.", "检查版本时发生错误。")
add("An error occurred while checking the update.", "检查更新时发生错误。")
add("An error occurred while checking the download.", "检查下载时发生错误。")
add("An error occurred while checking the installation.", "检查安装时发生错误。")
add("An error occurred while checking the uninstallation.", "检查卸载时发生错误。")
add("An error occurred while checking the deployment.", "检查部署时发生错误。")
add("An error occurred while checking the configuration file.", "检查配置文件时发生错误。")
add("An error occurred while checking the log file.", "检查日志文件时发生错误。")
add("An error occurred while checking the cache.", "检查缓存时发生错误。")
add("An error occurred while checking the database.", "检查数据库时发生错误。")
add("An error occurred while checking the manifest.", "检查清单时发生错误。")
add("An error occurred while checking the game.", "检查游戏时发生错误。")
add("An error occurred while checking the profile.", "检查配置文件时发生错误。")
add("An error occurred while checking the preset.", "检查预设时发生错误。")
add("An error occurred while checking the addon.", "检查附加组件时发生错误。")
add("An error occurred while checking the shader pack.", "检查着色器包时发生错误。")
add("An error occurred while checking the ReShade installation.", "检查 ReShade 安装时发生错误。")
add("An error occurred while checking the RenoDX installation.", "检查 RenoDX 安装时发生错误。")
add("An error occurred while checking the OptiScaler installation.", "检查 OptiScaler 安装时发生错误。")
add("An error occurred while checking the DXVK installation.", "检查 DXVK 安装时发生错误。")
add("An error occurred while checking the Luma installation.", "检查 Luma 安装时发生错误。")
add("An error occurred while checking the ASI Loader installation.", "检查 ASI Loader 安装时发生错误。")
add("An error occurred while checking the DLSS Enabler installation.", "检查 DLSS Enabler 安装时发生错误。")
add("An error occurred while checking the DLSS installation.", "检查 DLSS 安装时发生错误。")
add("An error occurred while checking the ReLimiter installation.", "检查 ReLimiter 安装时发生错误。")
add("An error occurred while checking the RE Framework installation.", "检查 RE Framework 安装时发生错误。")
add("An error occurred while checking the Display Commander installation.", "检查 Display Commander 安装时发生错误。")
add("An error occurred while checking the NVIDIA Profile Inspector installation.", "检查 NVIDIA Profile Inspector 安装时发生错误。")
add("An error occurred while checking the dgVoodoo installation.", "检查 dgVoodoo 安装时发生错误。")
add("An error occurred while checking the Ultrawide Fix installation.", "检查 Ultrawide Fix 安装时发生错误。")
add("An error occurred while checking the UL Profile Inspector installation.", "检查 UL Profile Inspector 安装时发生错误。")
add("An error occurred while checking the RE4 Framework installation.", "检查 RE4 Framework 安装时发生错误。")
add("An error occurred while checking the RE Framework installation.", "检查 RE Framework 安装时发生错误。")

# ── MainWindow XAML UI ──
add("New Mods Available", "新模组可用")
add("Quick Start", "快速入门")
add("Rescan game library and fetch latest mod info", "重新扫描游戏库并获取最新模组信息")
add("Manage global shaders and ReShade addons", "管理全局着色器和 ReShade 附加组件")
add("Global Shaders", "全局着色器")
add("Update All", "全部更新")
add("Useful links and resources", "有用的链接和资源")
add("RenoDX Wiki", "RenoDX 维基")
add("Luma Wiki", "Luma 维基")
add("RHI GitHub", "RHI GitHub")
add("ReLimiter GitHub", "ReLimiter GitHub")
add("Support and help resources", "支持和帮助资源")
add("Simplified PC Gaming", "简化 PC 游戏体验")
add("Filter games...", "筛选游戏...")
add("Save current search as a custom filter", "将当前搜索保存为自定义筛选器")
add("Manually add a game", "手动添加游戏")
add("All Games", "全部游戏")
add("Favourites", "收藏夹")
add("Hidden", "已隐藏")
add("shown", "显示")
add("installed", "已安装")
add("Launch this game", "启动此游戏")

# ── Detail panel / Extras ──
add("Current", "当前")
add("In use", "使用中")
add("Recommended", "推荐")
add("May conflict with ReShade/OS", "可能与 ReShade/OS 冲突")
add("Original DLL chained", "原始 DLL 已链接")
add("Used by RHI", "RHI 使用中")
add("Bridge Version", "Bridge 版本")
add("Open GitHub releases", "打开 GitHub releases")
add("Click to open GitHub releases", "点击打开 GitHub releases")
add("Click to open DXVK releases", "点击打开 DXVK releases")
add("Open GitHub", "打开 GitHub")
add("DLSS-NR", "DLSS-NR")
add("DLSS5 Tool info", "DLSS5 Tool 信息")
add("DLSS5 Tool staging not ready", "DLSS5 Tool 暂存未就绪")
add("Repository", "仓库")
add("Override", "覆盖")
add("ReShade Addons", "ReShade 附加组件")
add("Install Neural Rendering", "安装 Neural Rendering")
add("Uninstall Neural Rendering first to change the addon version.", "请先卸载 Neural Rendering 再更改附加组件版本。")
add("When On, installing a Neural Rendering method will also deploy the DLSS NR Cost Scaler proxy. ", "开启时，安装 Neural Rendering 方法还会部署 DLSS NR Cost Scaler 代理。 ")
add("Click to open ReShade Addons page", "点击打开 ReShade 附加组件页面")
add("Click to open ReShade presets page", "点击打开 ReShade 预设页面")
add("HDR Complete", "HDR 完成")
add("HDR Incomplete", "HDR 未完成")
add("HDR Missing", "HDR 缺失")
add("Optimal", "最优")
add("Not Optimal", "非最优")
add("Unknown", "未知")
add("Launch Game", "启动游戏")
add("Game Not Found", "游戏未找到")
add("Settings not available", "设置不可用")
add("Open ReShade Addons", "打开 ReShade 附加组件")
add("Open ReShade Presets", "打开 ReShade 预设")
add("Open overrides folder", "打开覆盖文件夹")
add("Open game folder", "打开游戏文件夹")
add("Open ReShade folder", "打开 ReShade 文件夹")
add("Open logs folder", "打开日志文件夹")
add("Copy game report", "复制游戏报告")
add("Copy error details", "复制错误详情")
add("Uninstall Components", "卸载组件")
add("Install Components", "安装组件")
add("Install Addons", "安装附加组件")
add("Uninstall Addons", "卸载附加组件")
add("Reinstall Addons", "重新安装附加组件")
add("Install ASI Loader", "安装 ASI Loader")
add("Uninstall ASI Loader", "卸载 ASI Loader")
add("Reinstall ASI Loader", "重新安装 ASI Loader")
add("Install DLSS Enabler", "安装 DLSS Enabler")
add("Uninstall DLSS Enabler", "卸载 DLSS Enabler")
add("Reinstall DLSS Enabler", "重新安装 DLSS Enabler")
add("Install DXVK", "安装 DXVK")
add("Uninstall DXVK", "卸载 DXVK")
add("Reinstall DXVK", "重新安装 DXVK")
add("Install Luma", "安装 Luma")
add("Uninstall Luma", "卸载 Luma")
add("Reinstall Luma", "重新安装 Luma")
add("Install RE Framework", "安装 RE Framework")
add("Uninstall RE Framework", "卸载 RE Framework")
add("Reinstall RE Framework", "重新安装 RE Framework")
add("Install ReLimiter", "安装 ReLimiter")
add("Uninstall ReLimiter", "卸载 ReLimiter")
add("Reinstall ReLimiter", "重新安装 ReLimiter")
add("Click to open Mod Author donation page", "点击打开模组作者捐赠页面")
add("Open Mod Author donation page", "打开模组作者捐赠页面")
add("No ReShade preset deployed", "未部署 ReShade 预设")
add("ReShade preset deployed", "ReShade 预设已部署")
add("Configuring ReShade...", "正在配置 ReShade...")
add("MFG Ada Unlock settings", "MFG Ada Unlock 设置")
add("RTX 40 MFG Ada Unlock", "RTX 40 MFG Ada Unlock")
add("DLSS5 Tool Version", "DLSS5 Tool 版本")
add("May conflict with ReShade/OS", "可能与 ReShade/OS 冲突")
add("This addon may conflict with ReShade/OS", "此附加组件可能与 ReShade/OS 冲突")

# ── XAML tooltips ──
add("Hide this game from the sidebar (find it again in the Hidden filter)", "从侧边栏隐藏此游戏（在已隐藏筛选器中再次查找）")
add("Opens the game folder in Explorer", "在资源管理器中打开游戏文件夹")
add("Deletes cached DLSS, Streamline, and component staging files to free disk space. Shaders, installed addons, and version metadata are preserved. Files are re-downloaded automatically when needed.",
    "删除缓存的 DLSS、Streamline 和组件暂存文件以释放磁盘空间。着色器、已安装的附加组件和版本元数据将被保留。文件会在需要时自动重新下载。")
add("Quick Start Guide", "快速入门指南")


def extract_cs_string_literals(content):
    """Extract all string literal contents from C# source code.
    Returns list of (start_pos, end_pos, original_text, is_interpolated)"""
    results = []
    i = 0
    length = len(content)

    while i < length:
        # Skip line comments
        if content[i:i+2] == '//':
            nl = content.find('\n', i)
            i = nl + 1 if nl != -1 else length
            continue

        # Skip block comments
        if content[i:i+2] == '/*':
            end = content.find('*/', i + 2)
            i = end + 2 if end != -1 else length
            continue

        # Skip preprocessor directives
        if content[i] == '#' and (i == 0 or content[i-1] == '\n'):
            nl = content.find('\n', i)
            i = nl + 1 if nl != -1 else length
            continue

        # Skip char literals 'x', '\n', '\'', '\"', etc.
        if content[i] == "'" and i+2 < length and content[i+2] == "'":
            i += 3
            continue
        if content[i] == "'" and i+3 < length and content[i+1] == '\\' and content[i+3] == "'":
            i += 4
            continue
        if content[i] == "'" and i+3 < length and content[i+1] == '\\' and content[i+2] == 'u' and content[i+3] == "'":
            # \u escape - skip up to \uXXXX'
            j = i + 4
            while j < length and content[j] != "'":
                j += 1
            i = j + 1 if j < length else length
            continue

        # Raw string variants - skip entirely (contain code/JSON, not UI text)
        if content[i:i+5] == '$$"""':
            end = content.find('"""', i + 5)
            i = end + 3 if end != -1 else length
            continue
        if content[i:i+5] == '$@"""' or content[i:i+5] == '@$"""':
            end = content.find('"""', i + 5)
            i = end + 3 if end != -1 else length
            continue
        if content[i:i+4] == '$"""':
            end = content.find('"""', i + 4)
            i = end + 3 if end != -1 else length
            continue
        if content[i:i+3] == '"""':
            end = content.find('"""', i + 3)
            i = end + 3 if end != -1 else length
            continue

        # Verbatim interpolated string $@"..."
        if content[i] == '$' and i+2 < length and content[i+1] == '@' and content[i+2] == '"':
            i += 3
            start = i
            depth = 0
            while i < length:
                if content[i] == '{':
                    depth += 1
                elif content[i] == '}':
                    if depth > 0:
                        depth -= 1
                elif content[i] == '"':
                    if depth == 0:
                        if i+1 < length and content[i+1] == '"':
                            i += 2  # Escaped quote in verbatim string
                        else:
                            results.append((start, i, content[start:i], True))
                            i += 1
                            break
                i += 1
            continue

        # Interpolated string $"..."
        if content[i] == '$' and i+1 < length and content[i+1] == '"':
            i += 2
            start = i
            depth = 0
            while i < length:
                if content[i] == '{':
                    depth += 1
                elif content[i] == '}':
                    if depth > 0:
                        depth -= 1
                elif content[i] == '\\' and depth == 0:
                    i += 1
                elif content[i] == '"' and depth == 0:
                    results.append((start, i, content[start:i], True))
                    i += 1
                    break
                i += 1
            continue

        # Verbatim string @"
        if content[i] == '@' and i+1 < length and content[i+1] == '"':
            i += 2
            start = i
            while i < length:
                if content[i] == '"':
                    if i+1 < length and content[i+1] == '"':
                        i += 2
                    else:
                        results.append((start, i, content[start:i], False))
                        i += 1
                        break
                else:
                    i += 1
            continue

        # Regular string "
        if content[i] == '"':
            i += 1
            start = i
            while i < length:
                if content[i] == '\\':
                    i += 2
                elif content[i] == '"':
                    results.append((start, i, content[start:i], False))
                    i += 1
                    break
                else:
                    i += 1
            continue

        i += 1

    return results


def mask_interpolated_expressions(s):
    """Replace {expression} with placeholders to prevent translating C# code."""
    expressions = []
    result = []
    i = 0
    depth = 0
    start = -1

    while i < len(s):
        if s[i] == '{':
            if i + 1 < len(s) and s[i+1] == '{':
                result.append(s[i:i+2])
                i += 2
                continue
            depth += 1
            if depth == 1:
                start = i + 1
        elif s[i] == '}':
            if depth > 0:
                depth -= 1
                if depth == 0:
                    expr = s[start:i]
                    idx = len(expressions)
                    expressions.append(expr)
                    result.append(f'\x00EXPR{idx}\x00')
            else:
                result.append(s[i])
        else:
            if depth == 0:
                result.append(s[i])
        i += 1

    return ''.join(result), expressions


def unmask_expressions(s, expressions):
    """Restore {expression} placeholders."""
    for idx, expr in enumerate(expressions):
        s = s.replace(f'\x00EXPR{idx}\x00', '{' + expr + '}')
    return s


# ── FAQ / Quick Start prose ──
add("RHI auto-detects your games and lets you install HDR mods, shaders, frame limiters, and manage NVIDIA driver settings — all from one place. Here's how to get started.",
    "RHI 会自动检测您的游戏，让您在一个地方安装 HDR 模组、着色器、帧率限制器，并管理 NVIDIA 驱动设置。以下是入门指南。")
add("Select a Game", "选择游戏")
add("Your games are listed in the sidebar on the left. Click any game to see its details and available actions. Use the filter chips (All Games, Installed, Unreal, etc.) and search box to find specific games.",
    "您的游戏列在左侧边栏中。单击任意游戏可查看其详细信息和可用操作。使用筛选标签（全部游戏、已安装、Unreal 等）和搜索框查找特定游戏。")
add("Tip: Double-click a game to launch it directly. Drag and drop a game's .exe file onto RHI to add games not auto-detected.",
    "提示：双击游戏可直接启动。将游戏的 .exe 文件拖放到 RHI 上可添加未自动检测到的游戏。")
add("Install ReShade", "安装 ReShade")
add("ReShade is required for RenoDX HDR mods to work. Click 'Install ReShade' on the game's detail panel. RHI automatically downloads and installs the correct version with full addon support.",
    "RenoDX HDR 模组需要 ReShade 才能工作。在游戏详情面板上单击“安装 ReShade”。RHI 会自动下载并安装具有完整附加组件支持的正确版本。")
add("ReShade version can be changed per-game via the game overrides section — choose Stable, Nightly, Legacy, or a custom ReShade DLL.\\nVulkan Games: Vulkan games (like Doom Eternal) require admin privileges. RHI will prompt for elevation when needed.\\nDrag and drop ReShade preset files (.ini) onto a game to install them automatically.",
    "可以通过游戏覆盖部分为每个游戏更改 ReShade 版本 — 选择 Stable、Nightly、Legacy 或自定义 ReShade DLL。\\nVulkan 游戏：Vulkan 游戏（如 Doom Eternal）需要管理员权限。RHI 会在需要时提示提升权限。\\n将 ReShade 预设文件（.ini）拖放到游戏上即可自动安装。")
add("RenoDX is RHI's primary HDR mod framework. The RenoDX row on each game shows what's available:\\n\\n• Named mods — game-specific HDR mods from the RenoDX wiki, made by the community.\\n• UE-Extended — for Unreal Engine games without a named mod. Provides native HDR output via a generic UE addon.\\n• Unity addon — same idea for Unity Engine games. Provides HDR output for Unity games without a named mod.\\n• RTX HDR — if no mod is available, you can enable NVIDIA's driver-level SDR-to-HDR conversion via the RenoDX ⚙ cog.",
    "RenoDX 是 RHI 的主要 HDR 模组框架。每个游戏上的 RenoDX 行显示可用内容：\\n\\n• 命名模组 — 来自 RenoDX 维基、由社区制作的游戏专用 HDR 模组。\\n• UE-Extended — 适用于没有命名模组的虚幻引擎游戏。通过通用 UE 附加组件提供原生 HDR 输出。\\n• Unity 附加组件 — 同理，适用于 Unity 引擎游戏。为没有命名模组的 Unity 游戏提供 HDR 输出。\\n• RTX HDR — 如果没有可用模组，可以通过 RenoDX ⚙ 齿轮启用 NVIDIA 驱动级的 SDR 转 HDR 转换。")
add("The cog icon next to RenoDX opens advanced settings: Peak Nits, UE-Extended toggle, and RTX HDR configuration.\\nEngine.ini Settings (Unreal Engine games only): toggle HDR keys and LUT update frequency written to the game's Engine.ini for accurate HDR rendering.\\nFor games not on the wiki, drag and drop an .addon64 file from the RenoDX Discord directly onto the game in RHI.",
    "RenoDX 旁边的齿轮图标可打开高级设置：峰值尼特、UE-Extended 开关和 RTX HDR 配置。\\nEngine.ini 设置（仅虚幻引擎游戏）：切换写入游戏 Engine.ini 的 HDR 键和 LUT 更新频率，以实现准确的 HDR 渲染。\\n对于不在维基上的游戏，可将 RenoDX Discord 中的 .addon64 文件直接拖放到 RHI 中的游戏上。")
add("Luma is an alternative mod framework developed by Pumbo (HDR Den). Depending on the game, a Luma mod may add HDR, DLAA (Deep Learning Anti-Aliasing), game-specific rendering fixes, or a combination of these — check the Info button for details on what each mod offers.\\n\\n• Completed mods — named Luma mods for supported games, shown on the Luma row.\\n• Generic Luma — available for all DX11 Unreal Engine games. RHI installs it automatically and applies any game-specific Engine.ini tweaks or launch arguments listed on the Luma wiki.\\n\\nYou can install RenoDX and Luma on the same game — but there's no guarantee they'll work together on every title. RHI will warn you the first time you try to install both.",
    "Luma 是由 Pumbo（HDR Den）开发的替代模组框架。根据游戏的不同，Luma 模组可能会添加 HDR、DLAA（深度学习抗锯齿）、游戏专用渲染修复或它们的组合 — 查看“信息”按钮了解每个模组提供的功能。\\n\\n• 已完成的模组 — 受支持游戏的命名 Luma 模组，显示在 Luma 行上。\\n• 通用 Luma — 适用于所有 DX11 虚幻引擎游戏。RHI 会自动安装它，并应用 Luma 维基上列出的任何游戏专用 Engine.ini 调整或启动参数。\\n\\n您可以在同一个游戏上安装 RenoDX 和 Luma — 但不能保证它们在每个游戏上都能协同工作。首次尝试同时安装两者时，RHI 会向您发出警告。")
add("Luma requires ReShade — it will be greyed out until ReShade is installed.\\nThe Luma ⚙ cog lets you toggle TAA Engine.ini settings for games that need them.\\nIf a game needs a specific launch argument (e.g. -dx11), RHI sets it automatically on install and removes it on uninstall. Launch arguments only apply when the game is launched through RHI.\\nCheck the Info button on the Luma row for game-specific notes — completed mods often include details on what the mod adds or any in-game settings required.",
    "Luma 需要 ReShade — 在安装 ReShade 之前它将呈灰色不可用。\\nLuma ⚙ 齿轮可让您为需要 TAA 的游戏切换 Engine.ini 设置。\\n如果游戏需要特定的启动参数（例如 -dx11），RHI 会在安装时自动设置，并在卸载时移除。启动参数仅在通过 RHI 启动游戏时生效。\\n查看 Luma 行上的“信息”按钮了解游戏特定说明 — 已完成的模组通常会包含模组添加内容或所需游戏内设置的详细信息。")
add("Choose Shaders (Optional)", "选择着色器（可选）")
add("Click the 'Shaders/Addons' button in the toolbar, then 'Global Shaders' to select shader packs. Lilium's HDR shader pack is selected by default. These apply to all games with ReShade installed.\\n\\nExpand any pack to pick individual shaders — the pack shows a dash when only some files are selected. Use the Profiles panel on the right to save, load, rename, and share named shader selections. Export a profile as a zip to share via Discord.",
    "单击工具栏中的“着色器/附加组件”按钮，然后选择“全局着色器”以选择着色器包。默认选中 Lilium 的 HDR 着色器包。这些适用于所有已安装 ReShade 的游戏。\\n\\n展开任意包以挑选单个着色器 — 当只选择部分文件时，包会显示一个短划线。使用右侧的配置文件面板保存、加载、重命名和共享命名的着色器选择。将配置文件导出为 zip 以便通过 Discord 共享。")
add("Tip: Per-game shaders can be set using the Shaders button on each game's detail card (when ReShade is installed).\\nTip: Use Expand All / Collapse All to browse all packs at once. Deselect All clears the whole selection. Export copies a zip of your selected shaders to the clipboard — paste directly into Discord to share.",
    "提示：可以使用每个游戏详情卡上的“着色器”按钮设置游戏专用着色器（已安装 ReShade 时）。\\n提示：使用“全部展开”/“全部折叠”一次性浏览所有包。“全部取消选择”会清空整个选择。“导出”会将所选着色器的 zip 复制到剪贴板 — 直接粘贴到 Discord 中即可共享。")
add("DOF Fix (Recommended for UE5)", "DOF Fix（推荐用于 UE5）")
add("DOF Fix backports a depth-of-field rendering fix from Unreal Engine 5.7 to games running on UE 5.0–5.6. It appears in the Recommended section on the game's detail panel when supported. Install it alongside RenoDX for the best result.",
    "DOF Fix 将虚幻引擎 5.7 的景深渲染修复向后移植到运行 UE 5.0–5.6 的游戏。在支持时，它会出现在游戏详情面板的“推荐”部分。请与 RenoDX 一起安装以获得最佳效果。")
add("DOF Fix only shows on eligible UE 5.0–5.6 games — it won't appear on UE4 or UE 5.7+ titles.\\nInstall and uninstall work the same as any other component.",
    "DOF Fix 仅显示在符合条件的 UE 5.0–5.6 游戏上 — 不会出现在 UE4 或 UE 5.7+ 游戏中。\\n安装和卸载与其他任何组件相同。")
add("Frame Limiters (Optional)", "帧率限制器（可选）")
add("ReLimiter and Display Commander are ReShade addons that provide precise frame limiting for VRR displays. Install them from the game's detail panel. ReLimiter is recommended as it's developed by the same team as RHI. Set your target FPS in Settings, per-game via the cog icon, or directly in-game.",
    "ReLimiter 和 Display Commander 是 ReShade 附加组件，可为 VRR 显示器提供精确的帧率限制。从游戏详情面板安装它们。推荐使用 ReLimiter，因为它由与 RHI 相同的团队开发。在设置中、通过齿轮图标为每个游戏设置，或直接在游戏内设置目标 FPS。")
add("Update DLSS / Streamline (Optional)", "更新 DLSS / Streamline（可选）")
add("Games with DLSS or Streamline DLLs have a dedicated section on the detail panel showing version info. Click to update to the latest version. Using the newest versions is recommended for best performance and quality. RHI backs up originals automatically so you can restore anytime.",
    "带有 DLSS 或 Streamline DLL 的游戏在详情面板上有专门的区域显示版本信息。单击以更新到最新版本。建议使用最新版本以获得最佳性能和质量。RHI 会自动备份原始文件，因此您随时可以恢复。")
add("When new DLSS or Streamline versions release, they will appear in RHI automatically. Set your default DLSS preset in Settings. Per-game presets can be changed in the DLSS section on each game's detail panel.",
    "当新的 DLSS 或 Streamline 版本发布时，它们会自动出现在 RHI 中。在设置中设置默认的 DLSS 预设。可以在每个游戏的详情面板的 DLSS 区域中更改游戏专用预设。")
add("OptiScaler (Optional)", "OptiScaler（可选）")
add("Switch between Stable and Nightly channels per game in the cog — Nightly adds frame generation and additional settings.\\nOptiScaler and ReShade can coexist. If you see crashes with both installed, try renaming ReShade to a different DLL name using DLL Naming Overrides in the Game Overrides panel.\\nGPU type and DLSS input settings (AMD/Intel only) are configured in Settings → OptiScaler Settings before installing.\\nThe 'Deploy OptiScaler.ini' button in the cog redeploys your configured INI template to the game folder.",
    "在齿轮中为每个游戏在 Stable 和 Nightly 通道之间切换 — Nightly 添加帧生成和其他设置。\\nOptiScaler 和 ReShade 可以共存。如果两者都安装后出现崩溃，请尝试使用游戏覆盖面板中的 DLL 命名覆盖将 ReShade 重命名为其他 DLL 名称。\\nGPU 类型和 DLSS 输入设置（仅限 AMD/Intel）在安装前于设置 → OptiScaler 设置中配置。\\n齿轮中的“部署 OptiScaler.ini”按钮会将您配置的 INI 模板重新部署到游戏文件夹。")
add("Click 'Settings' in the toolbar to configure defaults for all games:", "单击工具栏中的“设置”为所有游戏配置默认值：")
add("ReLimiter FPS: Default frame rate target", "ReLimiter FPS：默认帧率目标")
add("DLSS Preset: Default upscaling preset", "DLSS 预设：默认超分辨率预设")
add("NVIDIA Driver Settings: VSync, Low Latency, Power Mode", "NVIDIA 驱动设置：垂直同步、低延迟、电源模式")
add("Peak Nits: Your display's peak brightness for HDR", "峰值尼特：显示器用于 HDR 的峰值亮度")
add("ReShade Hotkeys: Customize overlay and screenshot keys", "ReShade 热键：自定义叠加层和截图键")
add("NVIDIA Driver Settings", "NVIDIA 驱动设置")
add("RHI can manage per-game NVIDIA driver profiles. These settings are available directly on each game's detail panel:", "RHI 可以管理游戏专用的 NVIDIA 驱动配置文件。这些设置可直接在每个游戏的详情面板上使用：")
add("VSync: On, Off, or Adaptive (Fast Sync)", "垂直同步：开、关或自适应（快速同步）")
add("Low Latency Mode: Ultra, On, or Off", "低延迟模式：超高、开或关")
add("Smooth Motion: Multi Frame Generation (per-game only)", "平滑运动：多帧生成（仅限每个游戏）")
add("ReBAR: Resizable BAR (requires admin)", "ReBAR：可调整大小的 BAR（需要管理员）")
add("Global defaults for VSync, Low Latency, and Power Mode are set in Settings. Per-game overrides are configured directly on each game's detail panel.",
    "垂直同步、低延迟和电源模式的全局默认值在设置中配置。游戏专用覆盖可直接在每个游戏的详情面板上配置。")
add("Vulkan Games", "Vulkan 游戏")
add("All Vulkan games share the same ReShade installation. Updating ReShade on one Vulkan game updates it for all.\\nPer-game RenoDX addons and shaders are still installed individually to each game folder.",
    "所有 Vulkan 游戏共享同一个 ReShade 安装。在一个 Vulkan 游戏上更新 ReShade 会为所有游戏更新。\\n游戏专用的 RenoDX 附加组件和着色器仍会单独安装到每个游戏文件夹。")
add("Adding Games Manually", "手动添加游戏")
add("If a game isn't auto-detected, drag and drop its .exe file directly onto the RHI window. RHI will add it to your library and detect its engine type.",
    "如果游戏未被自动检测到，请将其 .exe 文件直接拖放到 RHI 窗口上。RHI 会将其添加到您的库中并检测其引擎类型。")
add("You can also drag .addon64 files from the RenoDX Discord onto any game to install mods not yet on the wiki.",
    "您也可以将 RenoDX Discord 中的 .addon64 文件拖放到任意游戏上，以安装尚未在维基上的模组。")
add("Updating Everything", "更新所有内容")
add("Click 'Update All' in the toolbar to update all installed components across all games at once. This includes ReShade, RenoDX mods, ReLimiter, Display Commander, and more.",
    "单击工具栏中的“全部更新”可一次性更新所有游戏中已安装的所有组件。这包括 ReShade、RenoDX 模组、ReLimiter、Display Commander 等。")
add("Games with available updates show a green dot in the sidebar. Configure which components are included in 'Update All' from Settings.",
    "有可用更新的游戏会在边栏中显示绿点。在设置中配置“全部更新”包含哪些组件。")
add("Troubleshooting: Full Refresh", "疑难解答：完全刷新")
add("If games are missing, install locations have changed, or DLSS/Streamline files have been added or removed, use 'Full Refresh' in Settings to rescan your entire library from scratch.",
    "如果游戏缺失、安装位置已更改，或 DLSS/Streamline 文件已添加或移除，请使用设置中的“完全刷新”从头重新扫描整个库。")
add("Full Refresh clears the cached game list and re-detects everything. Use it when the normal Refresh button doesn't pick up changes.",
    "完全刷新会清除缓存的游戏列表并重新检测所有内容。当正常的刷新按钮无法检测到更改时使用它。")
add("System Tray", "系统托盘")
add("RHI can minimize to the system tray instead of closing. Right-click the tray icon to quickly launch recent games without opening the main window.",
    "RHI 可以最小化到系统托盘而不是关闭。右键单击托盘图标可快速启动最近的游戏，而无需打开主窗口。")
add("Enable 'Close to System Tray' in Settings to keep RHI running in the background. The tray icon provides quick access to your most recently played games. RHI automatically checks for updates every 4 hours while running, so everything stays up to date.",
    "在设置中启用“关闭到系统托盘”以使 RHI 在后台保持运行。托盘图标可让您快速访问最近玩过的游戏。RHI 在运行时会每 4 小时自动检查一次更新，因此所有内容都保持最新。")
add("Need More Help?", "需要更多帮助？")
add("Support is available on Discord — join the community for help, mod updates, and discussion.",
    "Discord 上提供支持 — 加入社区获取帮助、模组更新和讨论。")
add("Join the Ultra+ Discord (main community)", "加入 Ultra+ Discord（主社区）")
add("RenoDX Discord (mod development)", "RenoDX Discord（模组开发）")
add("Browse the RenoDX Mod Wiki", "浏览 RenoDX 模组维基")
add("RHI GitHub — Report issues or request features", "RHI GitHub — 报告问题或请求功能")
add("Welcome to RHI", "欢迎使用 RHI")
add("Click 'Settings' in the toolbar to configure defaults for all games:", "单击工具栏中的“设置”为所有游戏配置默认值：")

# ── Settings UI (MainWindow.Events.Settings.cs) ──
add("RHI is up to date", "RHI 已是最新版本")
add("You're running v{Services.CrashReporter.AppVersion} — no updates available.", "您正在运行 v{Services.CrashReporter.AppVersion} — 没有可用更新。")
add("No defaults configured yet.", "尚未配置默认值。")
add("NVIDIA Override", "NVIDIA 覆盖")
add("Custom FPS Limit", "自定义 FPS 限制")
add("Custom DMFG Target FPS", "自定义 DMFG 目标 FPS")
add("All games already have NVIDIA profiles.", "所有游戏均已有 NVIDIA 配置文件。")
add("Create Missing Profiles", "创建缺失的配置文件")
add("No custom profile settings found to export.", "未找到可导出的自定义配置文件设置。")
add("Export Complete", "导出完成")
add("Exported {data.Count} profile(s) to:\\n{path}", "已导出 {data.Count} 个配置文件到：\\n{path}")
add("Export Failed", "导出失败")
add("An error occurred during export:\\n{ex.Message}", "导出时发生错误：\\n{ex.Message}")
add("No backup file found at:\\n{path}\\n\\nBackup profiles first.", "未找到备份文件：\\n{path}\\n\\n请先备份配置文件。")
add("This will overwrite your current NVIDIA driver profile settings with the saved backup. This cannot be undone.", "这将使用保存的备份覆盖您当前的 NVIDIA 驱动配置文件设置。此操作无法撤销。")
add("You are not running as admin. Settings that require elevated privileges (e.g. ReBAR) will not be restored. Run RHI as admin to import all settings.", "您未以管理员身份运行。需要提升权限的设置（例如 ReBAR）将不会恢复。以管理员身份运行 RHI 以导入所有设置。")
add("Restore Profiles", "恢复配置文件")
add("Importing profiles...", "正在导入配置文件...")
add("Importing...", "导入中...")
add("Import Complete", "导入完成")
add("Imported {count} profile(s) from backup.", "已从备份导入 {count} 个配置文件。")
add("Digital Vibrance", "数字振动")
add("No NVIDIA displays detected. Digital Vibrance requires an NVIDIA GPU with compatible drivers.", "未检测到 NVIDIA 显示器。数字振动需要具有兼容驱动的 NVIDIA GPU。")
add("Monitor", "显示器")
add("Digital Vibrance (0 = desaturated, 50 = neutral, 100 = maximum)", "数字振动（0 = 去饱和，50 = 中性，100 = 最大值）")
add("Reset to 50", "重置为 50")
add("Reset all game profiles?", "重置所有游戏配置文件？")
add("Reset All", "全部重置")
add("Preparing...", "准备中...")
add("Resetting...", "重置中...")
add("Resetting global settings...", "正在重置全局设置...")
add("Profiles Reset", "配置文件已重置")
add("Reset Failed", "重置失败")
add("Clear NVIDIA Shader Cache", "清除 NVIDIA 着色器缓存")
add("This will permanently delete the NVIDIA DXCache and GLCache folders. This cannot be undone.\\n\\nAll games will need to rebuild their shader caches on next launch, which may cause brief stuttering or longer load times the first time. This can fix shader corruption, persistent stuttering, or graphical issues after driver updates.", "这将永久删除 NVIDIA DXCache 和 GLCache 文件夹。此操作无法撤销。\\n\\n所有游戏都需要在下次启动时重建着色器缓存，这可能会导致首次启动时短暂卡顿或加载时间变长。这可以修复驱动更新后的着色器损坏、持续卡顿或图形问题。")
add("Clear", "清除")
add("Shader Cache Cleared", "着色器缓存已清除")
add("Select HDR Monitors", "选择 HDR 显示器")
add("Leave all unchecked to enable HDR on the primary display only.", "全部取消勾选则仅在主显示器上启用 HDR。")
add("Output Colour Settings", "输出颜色设置")
add("Full", "完整")
add("Limited", "受限")
add("Select Resolution Monitors", "选择分辨率显示器")
add("Leave all unchecked to change resolution on the primary display only.", "全部取消勾选则仅更改主显示器的分辨率。")
add("Auto-apply peak nits on deploy", "部署时自动应用峰值尼特")
add("Apply to presets:", "应用到预设：")
add("Unchecked presets keep their existing per-preset values.", "未勾选的预设将保留其现有的各预设值。")
add("Peak Nits Settings", "峰值尼特设置")
add("Dismiss", "忽略")

# ── SettingsHandler UI ──
add("ReShade hotkeys and effect list style", "ReShade 热键和效果列表样式")
add("Screenshot path, ReShade hotkeys, and effect list style", "截图路径、ReShade 热键和效果列表样式")
add("Screenshots & Hotkeys", "截图和热键")
add("Peak nits is not configured or is disabled.", "峰值尼特未配置或已禁用。")
add("RHI Admin Mode", "RHI 管理员模式")
add("Admin Mode", "管理员模式")
add("UAC is disabled on this system — RHI always runs as administrator.", "此系统已禁用 UAC — RHI 始终以管理员身份运行。")
add("Admin Mode enabled. Restart RHI for it to take effect.", "管理员模式已启用。重启 RHI 使其生效。")
add("Admin Mode disabled. RHI will launch normally on next start.", "管理员模式已禁用。RHI 将在下次启动时正常运行。")
add("Game Data Copied", "游戏数据已复制")
add("Logs Copied", "日志已复制")
add("All session logs have been archived and copied to your clipboard. Paste directly into Discord to share.", "所有会话日志已归档并复制到剪贴板。直接粘贴到 Discord 中即可共享。")
add("This will delete cached DLSS, Streamline, and component staging files to free disk space.\\n\\nShaders, installed RenoDX addons, and version metadata are preserved.\\n\\nThese files will be re-downloaded automatically when needed.\\n\\nContinue?", "这将删除缓存的 DLSS、Streamline 和组件暂存文件以释放磁盘空间。\\n\\n着色器、已安装的 RenoDX 附加组件和版本元数据将被保留。\\n\\n这些文件将在需要时自动重新下载。\\n\\n继续？")
add("Purge Staging Files", "清除暂存文件")
add("Cache Purged", "缓存已清除")
add("Deleted {filesDeleted} files, freed {sizeStr} of disk space.", "已删除 {filesDeleted} 个文件，释放了 {sizeStr} 磁盘空间。")
add("Purge Failed", "清除失败")
add("An error occurred: {ex.Message}", "发生错误：{ex.Message}")
add("ReShade UI Hotkey", "ReShade UI 热键")
add("ReShade Screenshot Hotkey", "ReShade 截图热键")
add("Include components in Update All globally:", "全局将组件包含在“全部更新”中：")
add("Global Update Inclusion", "全局更新包含")
add("OptiScaler Hotkey", "OptiScaler 热键")
add("DXVK Variant Changed", "DXVK 变体已更改")
add("DXVK variant changed to {variantLabel}.", "DXVK 变体已更改为 {variantLabel}。")
add("Enter a custom FPS value (20-1000):", "输入自定义 FPS 值（20-1000）：")
add("Custom Target FPS", "自定义目标 FPS")

# ── Game detail / component dialogs ──
add("Deploy ReShade.ini", "部署 ReShade.ini")
add("Deploy ReShadePreset.ini", "部署 ReShadePreset.ini")
add("No ReShadePreset.ini found in RHI config folder", "在 RHI 配置文件夹中未找到 ReShadePreset.ini")
add("Open ReShade.ini", "打开 ReShade.ini")
add("Open ReShade.log", "打开 ReShade.log")
add("Copy ReShade.log to clipboard", "复制 ReShade.log 到剪贴板")
add("Click then press a key...", "单击然后按下一个键...")
add("Click here then press your desired key. Written to all reshade*.ini files for this game.", "单击此处然后按下您想要的键。将写入此游戏的所有 reshade*.ini 文件。")
add("Press a key...", "按下一个键...")
add("Applied!", "已应用！")
add("Press a key to assign it, or Backspace to clear it. Written to all reshade*.ini files for this game.", "按下一个键进行分配，或按 Backspace 清除。将写入此游戏的所有 reshade*.ini 文件。")
add("Overlay Key", "叠加层键")
add("Screenshot Key", "截图键")
add("Keep ReShade.ini Updated", "保持 ReShade.ini 更新")
add("When No, RHI will not automatically update this game's reshade.ini on ReShade install, update, or Apply to All Games.", "选择“否”时，RHI 将不会在安装、更新 ReShade 或应用到所有游戏时自动更新此游戏的 reshade.ini。")
add("ReShade Settings", "ReShade 设置")
add("UE-Extended Settings", "UE-Extended 设置")
add("Switch between using UE-Extended or the game specific mod/generic Unreal RenoDX mod.", "在 UE-Extended 或游戏专用模组/通用虚幻 RenoDX 模组之间切换。")
add("Set Maximum Nits", "设置最大尼特")
add("Enter a valid number.", "请输入有效的数字。")
add("Reads your monitor's peak brightness automatically.", "自动读取显示器的峰值亮度。")
add("No display found.", "未找到显示器。")
add("Could not read peak brightness.", "无法读取峰值亮度。")
add("Compatibility Settings", "兼容性设置")
add("Upgrade Path", "升级路径")
add("Dump LUT Shaders", "转储 LUT 着色器")
add("Output size", "输出大小")
add("Output ratio", "输出比例")
add("Any size", "任意大小")
add("Run the game once with RenoDX installed to generate settings.", "安装 RenoDX 后运行一次游戏以生成设置。")
add("No reshade.ini found in game folder.", "游戏文件夹中未找到 reshade.ini。")
add("Engine.ini Settings", "Engine.ini 设置")
add("HDR Settings", "HDR 设置")
add("Managed by custom Engine.ini file — not available for this game.", "由自定义 Engine.ini 文件管理 — 此游戏不可用。")
add("Deploys Engine.ini with HDR flags for games that don't have an ingame HDR option. Disable for SDR.", "为没有游戏内 HDR 选项的游戏部署带有 HDR 标志的 Engine.ini。SDR 时禁用。")
add("LUT Update Every Frame", "每帧更新 LUT")
add("Writes r.LUT.UpdateEveryFrame=1 to Engine.ini. Ensures the game recalculates LUTs each frame for accurate HDR color.", "将 r.LUT.UpdateEveryFrame=1 写入 Engine.ini。确保游戏每帧重新计算 LUT，以获得准确的 HDR 颜色。")
add("Engine.ini managed by custom file — standard controls disabled.", "Engine.ini 由自定义文件管理 — 标准控件已禁用。")
add("RenoDX Presets", "RenoDX 预设")
add("Export Presets", "导出预设")
add("No [renodx-preset*] sections found.", "未找到 [renodx-preset*] 部分。")
add("To import: place this file in the game folder and click 'Import Presets' in RHI,", "导入方法：将此文件放入游戏文件夹，然后在 RHI 中单击“导入预设”，")
add("or paste the [renodx-preset*] sections into reshade.ini manually.", "或手动将 [renodx-preset*] 部分粘贴到 reshade.ini。")
add("Save all RenoDX presets to a file and copy to clipboard for sharing.", "将所有 RenoDX 预设保存到文件并复制到剪贴板以便共享。")
add("Import Presets", "导入预设")
add("No RHI-RenoDX-Preset.txt file found. Export first.", "未找到 RHI-RenoDX-Preset.txt 文件。请先导出。")
add("Restore presets from the exported backup file into reshade.ini.", "从导出的备份文件将预设恢复到 reshade.ini。")
add("Requires NVIDIA App with Overlay and Game Filters enabled.", "需要启用叠加层和游戏滤镜的 NVIDIA App。")
add("Enable RTX HDR", "启用 RTX HDR")
add("RenoDX Settings", "RenoDX 设置")
add("High values may look unnatural", "高值可能看起来不自然")
add("High values may look washed out", "高值可能看起来发白")
add("Calculate Middle Grey from Peak Brightness and Gamma using the ITU formula", "使用 ITU 公式根据峰值亮度和 Gamma 计算中灰")
add("Neutral Saturation", "中性饱和度")
add("Save as Default", "保存为默认")
add("Save current slider values as your default RTX HDR preset", "将当前滑块值保存为默认 RTX HDR 预设")
add("Set Default", "设为默认")
add("Apply your saved default preset to the sliders", "将保存的默认预设应用到滑块")
add("No default saved yet — use 'Save as Default' first", "尚未保存默认值 — 请先使用“保存为默认”")
add("RTX HDR Settings", "RTX HDR 设置")
add("Deploy relimiter.ini", "部署 relimiter.ini")
add("Open ReLimiter log", "打开 ReLimiter 日志")
add("Copy ReLimiter log to clipboard", "复制 ReLimiter 日志到剪贴板")
add("Frame Limiter", "帧率限制器")
add("Target FPS", "目标 FPS")
add("FPS cap for this game. Select a VRR preset or Custom for a manual value.", "此游戏的 FPS 上限。选择 VRR 预设或自定义以使用手动值。")
add("Deploy relimiter.ini to enable these settings", "部署 relimiter.ini 以启用这些设置")
add("DLSS Hooks", "DLSS 挂钩")
add("Shows DLSS version/preset info on the ReLimiter OSD. Disable if causing crashes.", "在 ReLimiter OSD 上显示 DLSS 版本/预设信息。如果导致崩溃请禁用。")
add("Deploy DisplayCommander.ini", "部署 DisplayCommander.ini")
add("Display Commander Settings", "Display Commander 设置")
add("Framerate limit using Reflex whenever possible. 'Off' disables the limit.", "尽可能使用 Reflex 限制帧率。“关”禁用限制。")
add("OptiScaler Version", "OptiScaler 版本")
add("Framerate Limit", "帧率限制")
add("Stable: official release. Nightly: daily build. DLSS NR: Neural Rendering fork with multi-pass NR support.", "Stable：官方发布版。Nightly：每日构建版。DLSS NR：支持多通道 NR 的神经渲染分支。")
add("Select which graphics API's upscaler to configure.", "选择要配置哪个图形 API 的超分辨率器。")
add("Upscaler for the selected API. 'Auto' lets OptiScaler choose based on your GPU.", "所选 API 的超分辨率器。“自动”让 OptiScaler 根据您的 GPU 选择。")
add("Upscaler API", "超分辨率 API")
add("Upscaler", "超分辨率器")
add("Upscalers", "超分辨率器")
add("Frame Generation Settings", "帧生成设置")
add("Deploys Streamline and DLSS Enabler to the game's OptiScaler folder. Required for DLSS Frame Generation with OptiScaler.", "将 Streamline 和 DLSS Enabler 部署到游戏的 OptiScaler 文件夹。使用 OptiScaler 进行 DLSS 帧生成所必需。")
add("Streamline Version", "Streamline 版本")
add("HUD Fix: enables hudless resource tracking for Frame Generation. On = HUDFix=true in [OptiFG].", "HUD Fix：为帧生成启用无 HUD 资源跟踪。开 = [OptiFG] 中的 HUDFix=true。")
add("FG Input", "FG 输入")
add("Only relevant when FG Output = DLSSG. Enabler requires Deploy Streamline + Deploy DLSS Enabler.", "仅在 FG 输出 = DLSSG 时相关。Enabler 需要部署 Streamline + 部署 DLSS Enabler。")
add("FG Output", "FG 输出")
add("FG Nvngx Override", "FG Nvngx 覆盖")
add("Additional Settings", "其他设置")
add("DLSS Super Resolution render preset. J-M are the recommended modern presets.", "DLSS 超分辨率渲染预设。J-M 是推荐的现代预设。")
add("DLSS Ray Reconstruction render preset.", "DLSS 光线重建渲染预设。")
add("DLSS SR Preset", "DLSS SR 预设")
add("DLSS RR Preset", "DLSS RR 预设")
add("Override the internal render resolution. Off = use in-game quality preset as-is.", "覆盖内部渲染分辨率。关 = 按原样使用游戏内质量预设。")
add("Render Scale", "渲染比例")
add("Disable Flip Metering", "禁用翻转计量")
add("Dilated Motion Vectors", "膨胀运动矢量")
add("FSR Crash Fix", "FSR 崩溃修复")
add("FSR-FG Swapchain", "FSR-FG 交换链")
add("Upscaler Plugin", "超分辨率插件")
add("Neural Rendering Settings", "神经渲染设置")
add("NR runtime: 310.8.2 = original NVIDIA (RTX 50). 310.8.SF-v2 = ShortFuse cross-gen (RTX 20/30/40).", "NR 运行时：310.8.2 = 原始 NVIDIA（RTX 50）。310.8.SF-v2 = ShortFuse 跨代（RTX 20/30/40）。")
add("Enable DLSS Neural Rendering. Requires nvngx_dlssnr.dll + nvngx.dll_dlssnr.dll in game folder.", "启用 DLSS 神经渲染。需要游戏文件夹中的 nvngx_dlssnr.dll + nvngx.dll_dlssnr.dll。")
add("NR Runtime", "NR 运行时")
add("NR Enabled", "NR 已启用")
add("Run NR before Super Resolution. On = before upscaling; Off = after upscaling.", "在超分辨率之前运行 NR。开 = 放大之前；关 = 放大之后。")
add("Number of NR model passes. Each extra pass costs ~2x the model time but increases quality.", "NR 模型通道数。每个额外通道会花费约 2 倍的模型时间，但会提高质量。")
add("Run Before SR", "在 SR 之前运行")
add("Model work resolution as a fraction of frame size. Lower = faster; above 1.0 supersamples the model.", "模型工作分辨率占帧大小的比例。越低 = 越快；高于 1.0 会对模型进行超级采样。")
add("Apply NR to the finished picture (after all game lighting and effects). Helps with green noise. DX12 native only.", "将 NR 应用于完成的画面（在所有游戏光照和效果之后）。有助于消除绿色噪点。仅限 DX12 原生。")
add("Working Scale", "工作比例")
add("Apply to Finished Picture", "应用于完成的画面")
add("Save current cog settings into this slot", "将当前齿轮设置保存到此槽位")
add("Apply this preset to the current game", "将此预设应用到当前游戏")
add("No preset saved yet — click Save to capture current settings", "尚未保存预设 — 单击“保存”以捕获当前设置")
add("Deploy OptiScaler.ini", "部署 OptiScaler.ini")
add("If the game crashes with both ReShade and OptiScaler installed, try renaming ReShade to d3d12.dll (or another DLL name) using DLL Naming Overrides in the Overrides panel.", "如果在同时安装 ReShade 和 OptiScaler 时游戏崩溃，请尝试使用覆盖面板中的 DLL 命名覆盖将 ReShade 重命名为 d3d12.dll（或其他 DLL 名称）。")
add("OptiScaler Settings", "OptiScaler 设置")
add("Variant", "变体")
add("Development: latest nightly build. Stable: last stable release. Lilium HDR: HDR-optimised variant (default).", "Development：最新每日构建版。Stable：最后稳定版。Lilium HDR：HDR 优化变体（默认）。")
add("Lilium Preset", "Lilium 预设")
add("Safest = swap chain only (near 100% compatible).\\nHigher tiers upgrade back buffers and render targets — better HDR but may cause visual issues.", "最安全 = 仅交换链（接近 100% 兼容）。\\n更高层级会升级后台缓冲区和渲染目标 — HDR 更好但可能导致视觉问题。")
add("Deploy dxvk.conf", "部署 dxvk.conf")
add("Prefer DXGI Swapchain", "首选 DXGI 交换链")
add("Sets Vulkan/OpenGL Present Method to 'Preferred layered on DXGI Swapchain' in the NVIDIA driver profile. Recommended for DXVK — improves compatibility and HDR support.", "在 NVIDIA 驱动配置文件中将 Vulkan/OpenGL 呈现方法设置为“首选在 DXGI 交换链上分层”。推荐用于 DXVK — 提高兼容性和 HDR 支持。")
add("DXVK as Native", "DXVK 作为原生")
add("Standard: Treat DXVK as Native (0x000802A5). Alternative: Allow DXVK Promotion + DirectFlip (0x00080004). Only active when Prefer DXGI Swapchain is Yes.", "标准：将 DXVK 视为原生 (0x000802A5)。替代：允许 DXVK 提升 + DirectFlip (0x00080004)。仅在“首选 DXGI 交换链”为“是”时激活。")
add("Alternative", "替代")
add("DXVK Settings", "DXVK 设置")

# ── Extras section ──
add("Ultimate ASI Loader — proxy DLL that loads .asi plugins into game processes.", "Ultimate ASI Loader — 将 .asi 插件加载到游戏进程中的代理 DLL。")
add("Installed as: {installedAs}\\nClick to open GitHub releases", "安装为：{installedAs}\\n单击打开 GitHub releases")
add("Installed as: {currentDllName}\\nClick to open GitHub releases", "安装为：{currentDllName}\\n单击打开 GitHub releases")
add("Installed as: {currentDllName}\\nClick to open GitHub releases page", "安装为：{currentDllName}\\n单击打开 GitHub releases 页面")
add("Reinstall Ultimate ASI Loader (currently '{installedAs}')", "重新安装 Ultimate ASI Loader（当前为 '{installedAs}'）")
add("Install Ultimate ASI Loader — choose which DLL name to use", "安装 Ultimate ASI Loader — 选择要使用的 DLL 名称")
add("The existing '{chosen}' was renamed to '{hookedOriginal}' so ASI Loader can chain-load it automatically.", "现有 '{chosen}' 已重命名为 '{hookedOriginal}'，以便 ASI Loader 自动链式加载。")
add("MFG Ada Unlock — unlocks DLSS Multi Frame Generation (3x/4x+) on RTX 40-series GPUs. Requires ReShade. In-memory only, no files modified.", "MFG Ada Unlock — 在 RTX 40 系列 GPU 上解锁 DLSS 多帧生成（3x/4x+）。需要 ReShade。仅内存中运行，不修改文件。")
add("⚠  RTX 40 MFG installed", "⚠  RTX 40 MFG 已安装")
add("⬇  Install MFG Ada Unlock", "⬇  安装 MFG Ada Unlock")
add("↺  Reinstall MFG Ada Unlock", "↺  重新安装 MFG Ada Unlock")
add("RTX 40 MFG Unlock (ASI version) is already installed and conflicts. Remove it first.", "RTX 40 MFG Unlock（ASI 版本）已安装并存在冲突。请先移除它。")
add("MFG Ada Unlock unlocks DLSS Multi Frame Generation (3x/4x and above) on RTX 40-series GPUs.\\n\\n", "MFG Ada Unlock 在 RTX 40 系列 GPU 上解锁 DLSS 多帧生成（3x/4x 及以上）。\\n\\n")
add("Configure via the ReShade overlay in-game.", "在游戏内通过 ReShade 叠加层进行配置。")
add("Click to open OptiScaler releases", "点击打开 OptiScaler releases")
add("'{name}' already exists in the game folder. Selecting it will rename the existing file to '{Path.GetFileNameWithoutExtension(name)}Hooked.dll' so ASI Loader can chain-load it.", "'{name}' 已存在于游戏文件夹中。选择它将把现有文件重命名为 '{Path.GetFileNameWithoutExtension(name)}Hooked.dll'，以便 ASI Loader 链式加载。")
add("This filename is already used by an RHI-managed component (ReShade, OptiScaler, or DC). Choose a different name.", "此文件名已被 RHI 管理的组件（ReShade、OptiScaler 或 DC）使用。请选择其他名称。")
add("This filename is already used by an RHI-managed component. Choose a different name.", "此文件名已被 RHI 管理的组件使用。请选择其他名称。")
add("Select the filename for ASI Loader. Most games work with version.dll or winmm.dll.", "选择 ASI Loader 的文件名。大多数游戏可使用 version.dll 或 winmm.dll。")
add("MFG Ada Unlock (addon) is installed and conflicts. Remove it from the addon picker first.", "MFG Ada Unlock（附加组件）已安装并存在冲突。请先从附加组件选择器中移除它。")
add("Select GPU generation", "选择 GPU 代次")
add("Select your GPU generation. This controls which rendering path is used.\\n\\n", "选择您的 GPU 代次。这控制使用哪种渲染路径。\\n\\n")
add("Reinstall after changing to apply the new setting.", "更改后请重新安装以应用新设置。")
add("Select your GPU generation in the cog (⚙) before installing. Then choose the filename to deploy the DLL as.", "安装前请在齿轮 (⚙) 中选择您的 GPU 代次。然后选择要部署 DLL 的文件名。")
add("RTX 40 MFG Unlock — enables DLSS Multi Frame Generation multipliers beyond 2x (up to 6x) on RTX 40 Series GPUs. Standalone DLL, no ASI Loader required.", "RTX 40 MFG Unlock — 在 RTX 40 系列 GPU 上启用超过 2x（最高 6x）的 DLSS 多帧生成倍数。独立 DLL，无需 ASI Loader。")
add("Open RTX 40 MFG Unlock GitHub page", "打开 RTX 40 MFG Unlock GitHub 页面")
add("↺  Reinstall RTX 40 MFG", "↺  重新安装 RTX 40 MFG")
add("⬇  Install RTX 40 MFG", "⬇  安装 RTX 40 MFG")
add("MFG Ada Unlock (addon) is already installed and conflicts with RTX 40 MFG Unlock. Remove it from the addon picker first.", "MFG Ada Unlock（附加组件）已安装，与 RTX 40 MFG Unlock 冲突。请先从附加组件选择器中移除它。")
add("Reinstall RTX 40 MFG Unlock (currently deployed as {currentDllName})", "重新安装 RTX 40 MFG Unlock（当前部署为 {currentDllName}）")
add("Install RTX 40 MFG Unlock — deploys RTXMFG.dll under a name you choose", "安装 RTX 40 MFG Unlock — 以您选择的名称部署 RTXMFG.dll")
add("RTX 40 MFG settings — configure multiplier mode", "RTX 40 MFG 设置 — 配置倍数模式")
add("RTX 40 MFG Settings", "RTX 40 MFG 设置")
add("Press Backspace in-game to open the RTX 40 MFG menu.\\n\\n", "在游戏内按 Backspace 打开 RTX 40 MFG 菜单。\\n\\n")
add("• Follow game — uses the game's own MFG setting\\n", "• 跟随游戏 — 使用游戏自身的 MFG 设置\\n")
add("• Fixed 2x–6x — forces a specific multiplier\\n", "• 固定 2x–6x — 强制使用特定倍数\\n")
add("• Dynamic — targets the display refresh rate or a custom FPS value\\n\\n", "• 动态 — 以显示器刷新率或自定义 FPS 值为目标\\n\\n")
add("If frames freeze above 2x, try setting Frame Generation to Preset B in the NVIDIA App.", "如果帧率在 2x 以上冻结，请尝试在 NVIDIA App 中将帧生成设置为预设 B。")
add("Remove RTX 40 MFG Unlock from this game", "从此游戏中移除 RTX 40 MFG Unlock")
add("DLSS Enabler — standalone proxy DLL that enables DLSS in games that don't natively support it.", "DLSS Enabler — 独立的代理 DLL，可在不原生支持 DLSS 的游戏中启用 DLSS。")
add("Open DLSS Enabler Nexus page", "打开 DLSS Enabler Nexus 页面")
add("Installed via OptiScaler", "通过 OptiScaler 安装")
add("Remove standalone DLSS Enabler from this game", "从此游戏中移除独立的 DLSS Enabler")
add("dxgi.dll may conflict with ReShade or OptiScaler if they also use this name.", "如果 ReShade 或 OptiScaler 也使用此名称，dxgi.dll 可能会与之冲突。")
add("dxgi.dll may conflict with ReShade or OS components if they also use this name.", "如果 ReShade 或 OS 组件也使用此名称，dxgi.dll 可能会与之冲突。")
add("Choose RTX 40 MFG DLL name", "选择 RTX 40 MFG DLL 名称")
add("Select the filename to deploy RTXMFG.dll as. Choose a name the game loads early, and avoid names already used by other mods.", "选择部署 RTXMFG.dll 的文件名。请选择游戏较早加载的名称，并避免使用其他模组已使用的名称。")
add("Select the filename for the standalone DLSS Enabler DLL. Most games work with version.dll.", "选择独立 DLSS Enabler DLL 的文件名。大多数游戏可使用 version.dll。")
add("Remove MFG Ada Unlock from this game", "从此游戏中移除 MFG Ada Unlock")

# ── Neural Rendering section ──
add("DLSS5 Tool: for DX12 native-DLSS games.\\n", "DLSS5 Tool：适用于 DX12 原生 DLSS 游戏。\\n")
add("DLSS5 Tool + DX11 Bridge: for DX11/Vulkan native-DLSS games.\\n", "DLSS5 Tool + DX11 Bridge：适用于 DX11/Vulkan 原生 DLSS 游戏。\\n")
add("DLSS Tool (ShortFuse): alternative full-stack install for native-DLSS games.\\n", "DLSS Tool (ShortFuse)：适用于原生 DLSS 游戏的替代全栈安装。\\n")
add("DLSS5 Feeder: for games with no native DLSS (DX11, DX12, Vulkan, 32-bit).", "DLSS5 Feeder：适用于没有原生 DLSS 的游戏（DX11、DX12、Vulkan、32 位）。")
add("Version of dlss5-feed.addon64 to install.", "要安装的 dlss5-feed.addon64 版本。")
add("Version of dlss5-bridge.addon64 to install.", "要安装的 dlss5-bridge.addon64 版本。")
add("Version of renodx-dlss5.addon64 deployed as the neural consumer. The Feeder addon itself always uses the latest version.", "作为神经消费者部署的 renodx-dlss5.addon64 版本。Feeder 附加组件本身始终使用最新版本。")
add("Addon version to install. 'Latest' always installs the newest available and auto-updates.", "要安装的附加组件版本。'Latest' 始终安装最新可用版本并自动更新。")
add("NR DLL Version", "NR DLL 版本")
add("NR DLL version to deploy. 'Latest' always uses the newest available.", "要部署的 NR DLL 版本。'Latest' 始终使用最新可用版本。")
add("For DX12 games with native DLSS. Deploys the DLSS5 Tool ReShade addon and nvngx_dlssnr.dll. Lighter alternative to ShortFuse when you don't need the full Streamline stack.", "适用于具有原生 DLSS 的 DX12 游戏。部署 DLSS5 Tool ReShade 附加组件和 nvngx_dlssnr.dll。当您不需要完整 Streamline 堆栈时，是 ShortFuse 的更轻量替代方案。")
add("For DX12 games with native DLSS. This game has no detected DLSS — consider DLSS Tool (ShortFuse) instead.", "适用于具有原生 DLSS 的 DX12 游戏。此游戏未检测到 DLSS — 请考虑改用 DLSS Tool (ShortFuse)。")
add("For DX11 and Vulkan games with native DLSS. The bridge mirrors the game's DLSS onto a private DX12 session so the NR addon can hook it.", "适用于具有原生 DLSS 的 DX11 和 Vulkan 游戏。该桥接将游戏的 DLSS 镜像到私有 DX12 会话，以便 NR 附加组件可以挂钩。")
add("Recommended for most games with native DLSS. Deploys the full DLSS SR/RR/FG/NR stack and Streamline alongside the ReShade addon. Supports DX12, DX11, DX9, and Vulkan.", "推荐用于大多数具有原生 DLSS 的游戏。在 ReShade 附加组件旁部署完整的 DLSS SR/RR/FG/NR 堆栈和 Streamline。支持 DX12、DX11、DX9 和 Vulkan。")
add("For 32-bit games. Feeds a synthetic DLSS contract from ReShade depth and motion vectors. Deploys the Feeder addon, DLSS5 Tool (neural consumer), NR DLL, DLSS SR DLL, and the required shaders (DLSS5_Feed.fx + LumeniteFX).", "适用于 32 位游戏。从 ReShade 深度和运动矢量提供合成的 DLSS 契约。部署 Feeder 附加组件、DLSS5 Tool（神经消费者）、NR DLL、DLSS SR DLL 和所需的着色器（DLSS5_Feed.fx + LumeniteFX）。")
add("For games with no native DLSS (DX11, DX12, Vulkan, OpenGL). Feeds a synthetic DLSS contract from ReShade depth and motion vectors. Deploys the Feeder addon, DLSS5 Tool (neural consumer), NR DLL, DLSS SR DLL, and required shaders.", "适用于没有原生 DLSS 的游戏（DX11、DX12、Vulkan、OpenGL）。从 ReShade 深度和运动矢量提供合成的 DLSS 契约。部署 Feeder 附加组件、DLSS5 Tool（神经消费者）、NR DLL、DLSS SR DLL 和所需的着色器。")
add("ShortFuse settings — auto-configure ReShade for FrameGen", "ShortFuse 设置 — 为 FrameGen 自动配置 ReShade")
add("Auto-configure ReShade for FrameGen", "为 FrameGen 自动配置 ReShade")
add("When On, installing ShortFuse will automatically:\\n", "开启时，安装 ShortFuse 将自动：\\n")
add("• Rename ReShade to Reshade64.asi\\n", "• 将 ReShade 重命名为 Reshade64.asi\\n")
add("• Write HookStreamline=1 and HookDirectX=1 to reshade.ini\\n\\n", "• 将 HookStreamline=1 和 HookDirectX=1 写入 reshade.ini\\n\\n")
add("These steps are needed for FrameGen to work correctly after ReShade.\\n\\n", "这些步骤是 FrameGen 在 ReShade 之后正常工作的必要条件。\\n\\n")
add("Note: no longer required on ShortFuse v0.54 and above.", "注意：ShortFuse v0.54 及更高版本不再需要。")
add("Install Feeder Addon", "安装 Feeder 附加组件")
add("Fixing ReShade filename...", "修复 ReShade 文件名...")
add("NR Cost Scaler", "NR Cost Scaler")
add("Runs the neural model at reduced resolution (default 75%) for significant GPU savings while keeping native detail.", "以降低的分辨率（默认 75%）运行神经模型，在保持原生细节的同时大幅节省 GPU。")
add("Cost Scaler not yet staged — will be available after first launch", "Cost Scaler 尚未暂存 — 首次启动后可用")
add("Remove the installed NR method first, then toggle Cost Scaler On before reinstalling", "请先移除已安装的 NR 方法，然后在重新安装前将 Cost Scaler 切换为开")
add("Cost Scaler is built into the ShortFuse addon — this toggle is no longer required but remains available if you prefer the standalone version.", "Cost Scaler 已内置于 ShortFuse 附加组件中 — 不再需要此开关，但如果您更喜欢独立版本，它仍然可用。")
add("Staging DLSS5 Tool...", "正在暂存 DLSS5 Tool...")
add("Staging DLSS5 Tool v{requestedVersion}...", "正在暂存 DLSS5 Tool v{requestedVersion}...")
add("Upgrading DLSS DLLs...", "正在升级 DLSS DLL...")
add("Downloading DX11 Bridge...", "正在下载 DX11 Bridge...")
add("Deploying DX11 Bridge...", "正在部署 DX11 Bridge...")
add("ShortFuse staging not ready", "ShortFuse 暂存未就绪")
add("Staging ShortFuse v{requestedVersion}...", "正在暂存 ShortFuse v{requestedVersion}...")
add("Installing DLSS stack...", "正在安装 DLSS 堆栈...")
add("Deploying DLSS5 Tool...", "正在部署 DLSS5 Tool...")
add("Deploying DLSS SR...", "正在部署 DLSS SR...")
add("Downloading Feed.fx...", "正在下载 Feed.fx...")

# ── Settings page (MainWindow.xaml) ──
add("← Back to Games", "← 返回游戏")
add("Component Updates", "组件更新")
add("Check For Updates", "检查更新")
add("Fetches the latest manifest data and checks all components for available updates. Bypasses the 4-hour cooldown.", "获取最新清单数据并检查所有组件的可用更新。绕过 4 小时冷却。")
add("Check all components and the app for available updates", "检查所有组件和应用是否有可用更新")
add("Automatic Updates", "自动更新")
add("When enabled, RHI silently installs component updates in the background after each update check. Games that are running when an update is detected will be retried automatically once they close. Does not apply to app updates.", "启用后，RHI 会在每次更新检查后于后台静默安装组件更新。检测到更新时正在运行的游戏将在其关闭后自动重试。不适用于应用更新。")
add("Automatically install ReShade, RenoDX, ReLimiter, DC, OptiScaler, RE Framework, Luma, DXVK and DOF Fix updates silently in the background", "在后台静默自动安装 ReShade、RenoDX、ReLimiter、DC、OptiScaler、RE Framework、Luma、DXVK 和 DOF Fix 更新")
add("ReShade &amp; Display", "ReShade 和显示")
add("Screenshots &amp; Hotkeys", "截图和热键")
add("Screenshot save path and ReShade key bindings applied to all managed reshade.ini files.", "应用于所有托管的 reshade.ini 文件的截图保存路径和 ReShade 按键绑定。")
add("HDR &amp; Peak Brightness", "HDR 和峰值亮度")
add("Peak brightness (nits) written to reshade.ini presets. HDR auto-toggle enables Windows HDR on game launch and disables on exit.", "写入 reshade.ini 预设的峰值亮度（尼特）。HDR 自动切换在游戏启动时启用 Windows HDR，并在退出时禁用。")
add("Type or choose screenshot folder", "输入或选择截图文件夹")
add("Pick a folder for screenshots", "选择截图文件夹")
add("Open the screenshot folder in Explorer", "在资源管理器中打开截图文件夹")
add("Peak Nits", "峰值尼特")
add("Configure which presets receive the global peak nits value.", "配置哪些预设接收全局峰值尼特值。")
add("Overlay key", "叠加层键")
add("Screenshot key", "截图键")
add("Subfolders", "子文件夹")
add("Reset to Print Screen", "重置为 Print Screen")
add("When On, each game gets its own screenshot subfolder.", "开启时，每个游戏拥有自己的截案子文件夹。")
add("Apply to All Games", "应用到所有游戏")
add("HDR Auto-Toggle", "HDR 自动切换")
add("When On, Windows HDR is automatically enabled when launching a game through RHI and disabled when the game exits.", "开启时，通过 RHI 启动游戏时自动启用 Windows HDR，并在游戏退出时禁用。")
add("Select which monitors to enable HDR on. Empty = primary display only.", "选择要在哪些显示器上启用 HDR。留空 = 仅主显示器。")
add("Effect list style", "效果列表样式")
add("Controls how shaders are displayed in the ReShade overlay. Tabs groups effect files into named tabs; Tree shows a collapsible hierarchy.", "控制着色器在 ReShade 叠加层中的显示方式。标签页将效果文件分组为命名标签页；树形显示可折叠的层次结构。")
add("Resolution &amp; Colour Control", "分辨率和颜色控制")
add("Target Resolution", "目标分辨率")
add("Desktop resolution to switch to before a game launches. Restored on game exit.", "游戏启动前要切换到的桌面分辨率。游戏退出时恢复。")
add("Select the target desktop resolution.", "选择目标桌面分辨率。")
add("Resolution Auto-Toggle", "分辨率自动切换")
add("When On, switches to the target resolution on game launch and restores it on exit.", "开启时，在游戏启动时切换到目标分辨率，并在退出时恢复。")
add("Select which monitors to change resolution on.", "选择要更改分辨率的显示器。")
add("Set display colour depth and dynamic range without opening NVIDIA Control Panel.", "无需打开 NVIDIA 控制面板即可设置显示颜色深度和动态范围。")
add("Select which display to configure.", "选择要配置的显示器。")
add("Colour Depth", "颜色深度")
add("Dynamic Range", "动态范围")
add("Nexus Mods", "Nexus Mods")
add("API Key", "API 密钥")
add("Paste your Nexus Mods personal API key. Premium users get one-click downloads and silent auto-updates. Free users get one-click browser downloads.", "粘贴您的 Nexus Mods 个人 API 密钥。高级用户可获得一键下载和静默自动更新。免费用户可获得一键浏览器下载。")
add("NXM Protocol Handler", "NXM 协议处理程序")
add("Register RHI as the nxm:// handler so Nexus &quot;Mod Manager Download&quot; buttons open directly in RHI.", "将 RHI 注册为 nxm:// 处理程序，以便 Nexus 的&quot;Mod Manager Download&quot;按钮直接在 RHI 中打开。")
add("Connect", "连接")
add("Not connected", "未连接")
add("Not registered", "未注册")
add("Register", "注册")
add("Disconnect", "断开连接")
add("RenoDX Data Source", "RenoDX 数据源")
add("Controls where RHI fetches mod data. RHI Database uses the community-maintained RHI database (recommended). RenoDX Wiki falls back to the RenoDX wiki scraper. Changing this setting triggers an immediate refresh.", "控制 RHI 从何处获取模组数据。RHI Database 使用社区维护的 RHI 数据库（推荐）。RenoDX Wiki 回退到 RenoDX 维基抓取器。更改此设置会立即触发刷新。")
add("Source", "来源")
add("RHI Database", "RHI 数据库")
add("DLSS / Streamline Settings", "DLSS / Streamline 设置")
add("Defaults", "默认值")
add("Configure preferred DLSS/Streamline versions, presets, and render scales. Use 'Quick Apply' in the Nvidia Profile section to stamp these onto any game.", "配置首选 DLSS/Streamline 版本、预设和渲染比例。使用 Nvidia 配置部分中的“快速应用”将这些应用到任意游戏。")
add("Configure Defaults", "配置默认值")
add("DMFG Defaults", "DMFG 默认值")
add("Global Dynamic Multi Frame Generation defaults. Set these once — then just enable Dynamic mode on each game's MFG dialog.", "全局动态多帧生成默认值。设置一次后，只需在每个游戏的 MFG 对话框中启用动态模式。")
add("Frame Count", "帧数")
add("Global DMFG max frame count. Games using Dynamic mode inherit this unless overridden per-game.", "全局 DMFG 最大帧数。使用动态模式的游戏将继承此值，除非为每个游戏单独覆盖。")
add("Global DMFG target frame rate. Off = no target. Max Refresh Rate = match display. Or pick a specific FPS.", "全局 DMFG 目标帧率。关 = 无目标。最大刷新率 = 匹配显示器。或选择特定 FPS。")
add("Batch Deploy", "批量部署")
add("Deploy DLSS and Streamline DLL versions and DLSS presets to multiple games at once. Backs up originals automatically. Pre-populated with your configured defaults.", "一次性将 DLSS 和 Streamline DLL 版本及 DLSS 预设部署到多个游戏。自动备份原始文件。预填充您配置的默认值。")
add("DLSS On-Screen Indicator", "DLSS 屏幕指示器")
add("Controls the DLSS text overlay NVIDIA shows in the corner of games. Global system setting — affects all games. Requires admin.", "控制 NVIDIA 在游戏角落显示的 DLSS 文字叠加层。全局系统设置 — 影响所有游戏。需要管理员权限。")
add("Enabled", "已启用")
add("Disabled", "已禁用")
add("G-Sync On-Screen Indicator", "G-Sync 屏幕指示器")
add("Auto-Update", "自动更新")
add("Automatically swap to the newest version when a new release appears. Only affects games currently on the previous latest — manually chosen older versions are left alone.", "新版本发布时自动切换到最新版本。仅影响当前使用上一个最新版本的游戏 — 手动选择的较旧版本保持不变。")
add("Global NVIDIA Driver Settings", "全局 NVIDIA 驱动设置")
add("Driver-level settings applied globally via the base NVIDIA profile. Per-game settings are in the Nvidia Profile section of each game's overrides panel.", "通过基础 NVIDIA 配置文件全局应用的驱动级设置。游戏专用设置位于每个游戏覆盖面板的 Nvidia 配置部分。")
add("Shader Cache Size", "着色器缓存大小")
add("Maximum disk space for compiled shader cache. Larger = fewer stutters on revisits. 16GB is the driver default.", "编译着色器缓存的最大磁盘空间。越大 = 重访时卡顿越少。16GB 是驱动默认值。")
add("Shader Pre-Compile", "着色器预编译")
add("Auto-compiles shaders in the background when idle. Higher = more CPU usage during idle, faster first-time game loads.", "空闲时在后台自动编译着色器。越高 = 空闲时 CPU 占用越多，首次游戏加载越快。")
add("Globally enables or disables G-Sync/FreeSync variable refresh rate.", "全局启用或禁用 G-Sync/FreeSync 可变刷新率。")
add("G-Sync Mode", "G-Sync 模式")
add("Controls G-Sync/FreeSync variable refresh rate. Fullscreen and Windowed recommended for seamless experience.", "控制 G-Sync/FreeSync 可变刷新率。推荐全屏和窗口化以获得无缝体验。")
add("FPS Limit", "FPS 限制")
add("Global FPS cap via NVIDIA driver. VRR-optimal presets cap just below refresh rate to stay in VRR range.", "通过 NVIDIA 驱动的全局 FPS 上限。VRR 优化预设将上限设为略低于刷新率，以保持在 VRR 范围内。")
add("Preferred Refresh Rate", "首选刷新率")
add("Highest available forces your max refresh rate. App Setting lets games choose their own refresh rate.", "最高可用会强制使用您的最大刷新率。应用设置让游戏自行选择刷新率。")
add("Global VSync mode — applies to all games without a per-game override.", "全局垂直同步模式 — 适用于所有没有游戏专用覆盖的游戏。")
add("Power Mode", "电源模式")
add("Global power management mode — applies to all games without a per-game override.", "全局电源管理模式 — 适用于所有没有游戏专用覆盖的游戏。")
add("— Admin required", "— 需要管理员权限")
add("Global Resizable BAR — enables large GPU memory transfers for all games. Requires admin and compatible hardware (RTX 30 Series+).", "全局可调整大小的 BAR — 为所有游戏启用大型 GPU 内存传输。需要管理员权限和兼容硬件（RTX 30 系列+）。")
add("ReBAR Size Limit", "ReBAR 大小限制")
add("Maximum BAR transfer size. 1GB is optimal for most games. Decrease to 512MB if experiencing ReBAR-related stutters.", "最大 BAR 传输大小。1GB 对大多数游戏是最优的。如果遇到 ReBAR 相关的卡顿，请降低到 512MB。")
add("Adjust NVIDIA Digital Vibrance (color saturation) per-display. Saved values are automatically restored on app startup.", "为每个显示器调整 NVIDIA 数字振动（颜色饱和度）。保存的值会在应用启动时自动恢复。")
add("Adjust Digital Vibrance (color saturation) per-display. Saved values are restored on app startup.", "为每个显示器调整数字振动（颜色饱和度）。保存的值会在应用启动时恢复。")
add("Games without an NVIDIA profile don't benefit from global driver settings. Create profiles for all games in your library to ensure global settings apply.", "没有 NVIDIA 配置文件的游戏无法从全局驱动设置中受益。为库中的所有游戏创建配置文件以确保全局设置生效。")
add("Creates NVIDIA driver profiles for all games that don't have one yet. Ensures global settings like Power Mode and VSync apply to every game.", "为所有尚未拥有配置文件的游戏创建 NVIDIA 驱动配置文件。确保电源模式和垂直同步等全局设置对每个游戏生效。")
add("Backup saves all per-game NVIDIA driver settings to a file. Restore loads them back — useful before updating drivers.", "备份将所有游戏专用的 NVIDIA 驱动设置保存到文件。恢复将它们加载回来 — 在更新驱动前很有用。")
add("Backup Profiles", "备份配置文件")
add("Save all per-game NVIDIA profile settings to a backup file.", "将所有游戏专用的 NVIDIA 配置文件设置保存到备份文件。")
add("Restore per-game NVIDIA profile settings from a previous export.", "从之前的导出恢复游戏专用的 NVIDIA 配置文件设置。")
add("Reset Nvidia Profiles", "重置 Nvidia 配置文件")
add("Removes ALL per-game NVIDIA profile overrides. The global profile will be like new.", "移除所有游戏专用的 NVIDIA 配置文件覆盖。全局配置文件将如同全新。")
add("Clear Shader Cache", "清除着色器缓存")
add("Deletes the NVIDIA DXCache and GLCache folders. Forces shaders to recompile on next launch.", "删除 NVIDIA DXCache 和 GLCache 文件夹。强制着色器在下次启动时重新编译。")
add("Component Settings", "组件设置")
add("ReLimiter OSD hotkey and global feature settings applied to all games.", "应用于所有游戏的 ReLimiter OSD 热键和全局功能设置。")
add("GPU type, DLSS input settings, and overlay hotkey for OptiScaler installations.", "OptiScaler 安装的 GPU 类型、DLSS 输入设置和叠加层热键。")
add("OSD Key", "OSD 键")
add("Shared Presets", "共享预设")
add("The key that toggles the ReLimiter OSD overlay in-game.", "在游戏内切换 ReLimiter OSD 叠加层的键。")
add("When On, all games share the same OSD presets file. When Off, each game has its own presets.", "开启时，所有游戏共享同一个 OSD 预设文件。关闭时，每个游戏拥有自己的预设。")
add("Shows DLSS quality mode, render resolution, and active features on the ReLimiter OSD. Disable if experiencing crashes.", "在 ReLimiter OSD 上显示 DLSS 质量模式、渲染分辨率和活动功能。如果遇到崩溃请禁用。")
add("Global FPS cap written to all relimiter.ini files. Select a VRR preset or Custom for a manual value.", "写入所有 relimiter.ini 文件的全局 FPS 上限。选择 VRR 预设或自定义以使用手动值。")
add("GPU Type", "GPU 类型")
add("DLSS Inputs", "DLSS 输入")
add("Shaders &amp; Addons", "着色器和附加组件")
add("Shader Management", "着色器管理")
add("Control how RHI manages shader files across your games.", "控制 RHI 如何管理您游戏中的着色器文件。")
add("Cache All Shaders", "缓存所有着色器")
add("RHI Managed: deploy built-in shader packs to all games.&#10;Custom: use your own shader directory.&#10;Off: RHI never touches shaders on any game.", "RHI 管理：将内置着色器包部署到所有游戏。&#10;自定义：使用您自己的着色器目录。&#10;关：RHI 从不改动任何游戏的着色器。")
add("RHI Managed", "RHI 管理")
add("When On, all shader packs are downloaded and cached at startup. When Off, packs are downloaded on demand when needed.", "开启时，所有着色器包在启动时下载并缓存。关闭时，包在需要时按需下载。")
add("Addon Watch Folder", "附加组件监视文件夹")
add("RHI watches this folder for RenoDX addon files (.addon64/.addon32) and archives. Defaults to your Downloads folder.", "RHI 监视此文件夹中的 RenoDX 附加组件文件（.addon64/.addon32）和归档。默认为您的下载文件夹。")
add("Downloads folder (default)", "下载文件夹（默认）")
add("Browse", "浏览")
add("Update &amp; Deployment", "更新和部署")
add("Global Update Checks", "全局更新检查")
add("Disable update checks for individual components. When disabled, the component will not be checked during startup or Update All.", "禁用单个组件的更新检查。禁用后，该组件在启动或全部更新时不会被检查。")
add("Mass INI Deployment", "批量 INI 部署")
add("Deploy INI files or ReShade presets to multiple games at once. Custom hotkey and screenshot path settings are preserved.", "一次性将 INI 文件或 ReShade 预设部署到多个游戏。自定义热键和截图路径设置将保留。")
add("Update Inclusion", "更新包含")
add("Mass Preset Install", "批量预设安装")
add("System &amp; Maintenance", "系统和维护")
add("Clears all caches and re-scans everything from disk. Use as a last resort if games are missing, paths have changed, DLSS has been added to a game, or the DLSS section is missing from a game card.", "清除所有缓存并从磁盘重新扫描所有内容。作为最后手段，用于游戏缺失、路径更改、DLSS 已添加到游戏或游戏卡片中缺少 DLSS 部分的情况。")
add("Deletes cached DLSS, Streamline, and download files to free disk space. Shaders are preserved.", "删除缓存的 DLSS、Streamline 和下载文件以释放磁盘空间。着色器将保留。")
add("Creates a scheduled task to launch RHI with elevated privileges. Required for ReBAR, Low Latency Ultra, and Smooth Motion driver settings. The Drop Helper enables Discord drag-and-drop when running elevated.", "创建以提升权限启动 RHI 的计划任务。ReBAR、低延迟超高和平滑运动驱动设置所必需。拖放助手在提升权限运行时启用 Discord 拖放。")
add("Drop Helper", "拖放助手")
add("The drop helper window enables Discord drag-and-drop in admin mode. Disable if you don't need it. Requires restart.", "拖放助手窗口在管理员模式下启用 Discord 拖放。如果不需要请禁用。需要重启。")
add("Minimize to system tray on close. Right-click the tray icon or pinned taskbar icon to quickly launch recent games.", "关闭时最小化到系统托盘。右键单击托盘图标或固定到任务栏的图标以快速启动最近的游戏。")
add("Close to Tray", "关闭到托盘")
add("Recent Games", "最近游戏")
add("Start with Windows", "随 Windows 启动")
add("Data &amp; Custom Files", "数据和自定义文件")
add("AppData Folder", "AppData 文件夹")
add("Downloads Folder", "下载文件夹")
add("Custom Folder", "自定义文件夹")
add("Logs Folder", "日志文件夹")
add("Copy Logs", "复制日志")
add("Creates a zip archive of all logs and copies it to clipboard — paste directly into Discord", "创建所有日志的 zip 归档并复制到剪贴板 — 直接粘贴到 Discord")
add("Export Game Data", "导出游戏数据")
add("Gathers game library data (APIs, exe paths, engine info) and copies a JSON file to clipboard — paste into Discord to share with the RHI community database", "收集游戏库数据（API、exe 路径、引擎信息）并将 JSON 文件复制到剪贴板 — 粘贴到 Discord 以分享到 RHI 社区数据库")
add("The all-in-one game enhancement toolkit for PC. Auto-detects games from 8 storefronts and provides one-click management for ReShade, HDR mods, shaders, DLSS/Streamline, frame limiters, NVIDIA driver profiles, and more — all from one place.", "PC 一体化游戏增强工具包。自动检测来自 8 个平台商店的游戏，并提供对 ReShade、HDR 模组、着色器、DLSS/Streamline、帧率限制器、NVIDIA 驱动配置文件等的一键管理 — 尽在一处。")
add("What RHI manages", "RHI 管理的内容")
add("ReShade &amp; shader packs · RenoDX HDR mods · Luma Framework · DLSS/Streamline version swaps &amp; presets · NVIDIA driver profile settings (VSync, Low Latency, ReBAR, Smooth Motion, Power) · OptiScaler · ReLimiter &amp; Display Commander frame limiters · RE Framework · DXVK · UE-Extended native HDR · Per-game overrides for everything.", "ReShade 和着色器包 · RenoDX HDR 模组 · Luma Framework · DLSS/Streamline 版本切换和预设 · NVIDIA 驱动配置文件设置（垂直同步、低延迟、ReBAR、平滑运动、电源） · OptiScaler · ReLimiter 和 Display Commander 帧率限制器 · RE Framework · DXVK · UE-Extended 原生 HDR · 所有内容的游戏专用覆盖。")
add("Single-player only", "仅限单人游戏")
add("RHI installs ReShade with full addon support, OptiScaler, and can swap DLSS/Streamline DLLs. These modifications may be flagged by anti-cheat in online or multiplayer games. Uninstall ReShade, OptiScaler, and restore DLSS/Streamline defaults before playing online.", "RHI 安装具有完整附加组件支持的 ReShade、OptiScaler，并可交换 DLSS/Streamline DLL。这些修改可能会被在线或多人游戏的反作弊系统标记。在线游玩前请卸载 ReShade、OptiScaler 并恢复 DLSS/Streamline 默认值。")
add("Credits &amp; Acknowledgements", "致谢")
add("This app would not exist without the work of the following people and projects.", "没有以下人员和项目的工作，此应用将不复存在。")
add(" by clshortfuse &amp; contributors", " 由 clshortfuse 及贡献者")
add("  ·  MIT Licence", "  ·  MIT 许可证")
add("HDR mod framework powering 800+ games. The entire reason this app exists.", "为 800 多款游戏提供支持的 HDR 模组框架。此应用存在的全部原因。")
add(" by Crosire", " 由 Crosire")
add("  ·  BSD 3-Clause Licence", "  ·  BSD 3-Clause 许可证")
add("Post-processing injection framework. Downloaded with full addon support from reshade.me and cached locally. Copyright © Crosire.", "后处理注入框架。从 reshade.me 下载完整附加组件支持并本地缓存。版权所有 © Crosire。")
add("Luma Framework", "Luma Framework")
add(" by Pumbo (Filoppi)", " 由 Pumbo（Filoppi）")
add("  ·  Source-available", "  ·  源码可用")
add("DX11 modding framework adding HDR support and graphics improvements via the ReShade addon system. Mods are downloaded from official GitHub releases. Drag-and-drop install with auto-detection, update checking, and per-game toggle.", "通过 ReShade 附加组件系统添加 HDR 支持和图形改进的 DX11 模组框架。模组从官方 GitHub releases 下载。拖放安装，具有自动检测、更新检查和游戏专用开关。")
add("7-Zip", "7-Zip")
add(" by Igor Pavlov", " 由 Igor Pavlov")
add("Archive utility used to extract ReShade DLLs from the NSIS installer. 7z.exe and 7z.dll are bundled under the LGPL licence. Copyright © Igor Pavlov.", "用于从 NSIS 安装程序提取 ReShade DLL 的归档工具。7z.exe 和 7z.dll 在 LGPL 许可证下捆绑。版权所有 © Igor Pavlov。")
add("HtmlAgilityPack", "HtmlAgilityPack")
add(" by ZZZ Projects", " 由 ZZZ Projects")
add("HTML parser used to scrape game data from the RenoDX wiki. Copyright © ZZZ Projects Inc.", "用于从 RenoDX 维基抓取游戏数据的 HTML 解析器。版权所有 © ZZZ Projects Inc.")
add("CommunityToolkit.Mvvm", "CommunityToolkit.Mvvm")
add(" by Microsoft", " 由 Microsoft")
add("MVVM helpers (ObservableObject, RelayCommand, etc.) used throughout the app. Copyright © .NET Foundation.", "整个应用中使用的 MVVM 辅助工具（ObservableObject、RelayCommand 等）。版权所有 © .NET Foundation。")
add("Display Commander", "Display Commander")
add(" by pmnox", " 由 pmnox")
add("Frame rate limiter addon available as an alternative to ReLimiter. The LITE variant is downloaded from GitHub on demand.", "作为 ReLimiter 替代品提供的帧率限制器附加组件。LITE 变体按需从 GitHub 下载。")
add("RE Framework", "RE Framework")
add(" by praydog", " 由 praydog")
add("Modding framework required for ReShade injection on RE Engine games. Downloaded from nightly builds on GitHub.", "RE Engine 游戏上 ReShade 注入所需的模组框架。从 GitHub 的每日构建版下载。")
add(" by OptiScaler contributors", " 由 OptiScaler 贡献者")
add("Upscaler redirection layer enabling DLSS, FSR, and XeSS across different GPU vendors. Downloaded from GitHub releases.", "在不同 GPU 厂商之间启用 DLSS、FSR 和 XeSS 的超分辨率重定向层。从 GitHub releases 下载。")
add(" by doitsujin &amp; contributors", " 由 doitsujin 及贡献者")
add("  ·  Zlib Licence", "  ·  Zlib 许可证")
add("DirectX-to-Vulkan translation layer for DX8/DX9/DX10 games. Includes Lilium HDR fork by EndlesslyFlowering for scRGB HDR output.", "用于 DX8/DX9/DX10 游戏的 DirectX 到 Vulkan 转换层。包含由 EndlesslyFlowering 开发的用于 scRGB HDR 输出的 Lilium HDR 分支。")
add("NvAPIWrapper.Net", "NvAPIWrapper.Net")
add(" by falahati", " 由 falahati")
add(".NET wrapper for NVIDIA's NvAPI used for all driver profile management — DLSS presets, render scale, ReBAR, VSync, Smooth Motion, and more.", "用于所有驱动配置文件管理的 NVIDIA NvAPI 的 .NET 包装器 — DLSS 预设、渲染比例、ReBAR、垂直同步、平滑运动等。")
add("UI Design", "UI 设计")
add(" by Lazorr", " 由 Lazorr")
add("Somehow I have become a UI guy in multiple aspects of life lol.", "不知怎的，我在生活的多个方面都成了 UI 达人，哈哈。")
add("Universal UE DOF Fix", "通用 UE DOF Fix")
add("Fixes depth-of-field stepping and tiling artifacts in Unreal Engine 5 games.", "修复虚幻引擎 5 游戏中的景深阶梯和平铺伪影。")
add(" by RankFTW", " 由 RankFTW")
add("Unofficial companion app. Not affiliated with RenoDX, Crosire, or pmnoxx.", "非官方配套应用。与 RenoDX、Crosire 或 pmnoxx 无关联。")
add("Links", "链接")
add("⚠ ReShade, OptiScaler, and DLSS/Streamline modifications may trigger anti-cheat in online/multiplayer games", "⚠ ReShade、OptiScaler 和 DLSS/Streamline 修改可能会在在线/多人游戏中触发反作弊")
add("Check for app updates", "检查应用更新")
add("Patch Notes", "补丁说明")
add("View recent patch notes", "查看最近的补丁说明")
add("MOTD", "MOTD")
add("Show message of the day", "显示每日消息")
add("ReShade &amp; Display", "ReShade 和显示")

# Strings that must NEVER be translated (functional keys / data constants)
SKIP_EXACT = {
    # Detail-panel section keys
    "Components", "Management", "Extras",
    "GameOverrides", "NeuralRendering", "NvidiaProfile",
    # Folder names / JSON keys / tags
    "Shaders", "shaders", "Addons", "addons", "components", "Custom",
    # Values compared in code or written to config
    "Select", "Off", "On", "Auto", "auto", "Global",
    "Stable", "Nightly", "Development", "DlssNr", "LiliumHdr",
    "NVIDIA", "AMD", "Intel",
    "true", "false",
    # Filter tags
    "Installed", "Hidden", "Other", "Favourites", "Detected",
}


_COMPILED = None


def _get_compiled():
    global _COMPILED
    if _COMPILED is None:
        _COMPILED = [
            (re.compile(re.escape(en)), zh)
            for en, zh in sorted(T.items(), key=lambda x: len(x[0]), reverse=True)
            if ' ' in en
        ]
    return _COMPILED


def translate_string(s):
    """Translate a string literal content using the dictionary.
    Uses word-boundary regex to prevent partial matches.
    Skips strings that look like embedded code (PowerShell, JSON, etc.)."""
    if not s or len(s) < 2:
        return s
    if s in SKIP_EXACT:
        return s
    # Single-word dictionary entries only apply on an exact whole-string match.
    # This prevents them from corrupting file names, ini keys or identifiers.
    if s in T and ' ' not in s:
        return T[s]
    # Never touch strings that look like file names or paths.
    if re.match(r'^[\w.-]+\.(json|dll|ini|txt|exe|log|zip|conf|cfg|png|ico|bak)$', s, re.IGNORECASE):
        return s
    if not any('a' <= c <= 'z' or 'A' <= c <= 'Z' for c in s):
        return s

    # Skip strings that look like code
    code_patterns = [r'\$\w', r'foreach\s*\(', r'if\s*\(', r'else\s*\{',
                     r'foreach\s*\(.*\)', r'param\s*\(']
    for pat in code_patterns:
        if re.search(pat, s):
            return s

    is_interpolated = '{' in s

    if is_interpolated:
        masked, expressions = mask_interpolated_expressions(s)
    else:
        masked = s
        expressions = []

    result = masked
    for pattern, zh in _get_compiled():
        def replace_if_word_boundary(m, _zh=zh):
            start_pos = m.start()
            end_pos = m.end()
            if start_pos > 0 and (result[start_pos-1].isalnum() or result[start_pos-1] == '_'):
                return m.group(0)
            if end_pos < len(result) and (result[end_pos].isalnum() or result[end_pos] == '_'):
                return m.group(0)
            return _zh
        result = pattern.sub(replace_if_word_boundary, result)

    if is_interpolated:
        result = unmask_expressions(result, expressions)

    return result


def process_cs_file(filepath):
    """Process a C# file, translating string literals."""
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    literals = extract_cs_string_literals(content)
    if not literals:
        return False

    changes = []
    for start, end, original, is_interp in literals:
        translated = translate_string(original)
        if translated != original:
            changes.append((start, end, original, translated))

    if not changes:
        return False

    new_content = content
    for start, end, original, translated in reversed(changes):
        new_content = new_content[:start] + translated + new_content[end:]

    with open(filepath, 'w', encoding='utf-8', newline='\n') as f:
        f.write(new_content)
    return True


def process_xaml_file(filepath):
    """Process an XAML file, translating attribute values."""
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    pattern = re.compile(r'(\w[\w.:]*)="([^"]*)"')

    def replace_attr(m):
        attr_name = m.group(1)
        value = m.group(2)
        if not value:
            return m.group(0)
        # Only translate attributes that hold user-facing display text. Anything
        # else (enums like Visibility/Orientation/ScrollBarVisibility, resource
        # keys, event handlers, names, …) must be left untouched.
        display_attrs = {
            'Text', 'Content', 'Header', 'PlaceholderText',
            'ToolTipService.ToolTip', 'AutomationProperties.Name',
            'AutomationProperties.HelpText', 'Description', 'Title',
        }
        if attr_name not in display_attrs:
            return m.group(0)
        if value.startswith('{') or value.startswith('/'):
            return m.group(0)
        translated = translate_string(value)
        if translated != value:
            # Keep the attribute XML-safe: never emit raw quotes or bare ampersands.
            translated = translated.replace('"', '&quot;')
            translated = re.sub(r'&(?!amp;|quot;|lt;|gt;|apos;|#)', '&amp;', translated)
            return f'{attr_name}="{translated}"'
        return m.group(0)

    new_content = pattern.sub(replace_attr, content)

    if new_content != content:
        with open(filepath, 'w', encoding='utf-8', newline='\n') as f:
            f.write(new_content)
        return True
    return False


def apply_title_patches(base):
    """First-level detail-panel section titles keep their English name with the
    Chinese translation in parentheses, e.g. Components（组件）. These are applied
    as explicit file+line patches after the dictionary pass because the same
    words double as functional section keys elsewhere."""
    patches = [
        (r'MainWindow.xaml', 'Text="Components"', 'Text="Components（组件）"'),
        (r'MainWindow.Skeleton.cs', 'Text = "Components",', 'Text = "Components（组件）",'),
        (r'MainWindow.Skeleton.cs', 'Text = "Overrides",', 'Text = "Overrides（覆盖）",'),
        (r'DetailPanelBuilder.Extras.cs', 'Text       = "Extras",', 'Text       = "Extras（附加）",'),
        (r'DetailPanelBuilder.Overrides.Dxvk.cs', 'Text       = "Management",', 'Text       = "Management（管理）",'),
        (r'DetailPanelBuilder.Overrides.cs', 'Text       = "Game Overrides",', 'Text       = "Game Overrides（游戏覆盖）",'),
        (r'DetailPanelBuilder.NeuralRendering.cs', 'Text       = "Neural Rendering",', 'Text       = "Neural Rendering（神经渲染）",'),
        (r'DetailPanelBuilder.Overrides.NvidiaProfile.cs', '"Nvidia Profile Overrides"', '"Nvidia Profile Overrides（NVIDIA 配置覆盖）"'),
        (r'DlssDefaultsDialog.cs', 'Text = "Neural Rendering",', 'Text = "Neural Rendering（神经渲染）",'),
    ]
    for rel, old, new in patches:
        fp = os.path.join(base, rel)
        if not os.path.exists(fp):
            continue
        with open(fp, 'r', encoding='utf-8') as f:
            content = f.read()
        if old in content:
            content = content.replace(old, new, 1)
            with open(fp, 'w', encoding='utf-8', newline='\n') as f:
                f.write(content)
            print(f'  [TITLE] {rel}: {old.strip()}')


def main():
    base = os.path.join(os.path.dirname(os.path.abspath(__file__)), "RenoDXCommander")

    cs_files = []
    xaml_files = []

    for root, dirs, files in os.walk(base):
        if 'obj' in root.split(os.sep) or 'bin' in root.split(os.sep):
            continue
        for fn in files:
            fp = os.path.join(root, fn)
            if fn.endswith('.cs'):
                cs_files.append(fp)
            elif fn.endswith('.xaml'):
                xaml_files.append(fp)

    cs_changed = 0
    for fp in cs_files:
        if process_cs_file(fp):
            cs_changed += 1
            print(f'  [CS] {os.path.relpath(fp, base)}')

    xaml_changed = 0
    for fp in xaml_files:
        if process_xaml_file(fp):
            xaml_changed += 1
            print(f'  [XAML] {os.path.relpath(fp, base)}')

    apply_title_patches(base)

    print(f'\nDone: {cs_changed} .cs files, {xaml_changed} .xaml files modified.')


if __name__ == '__main__':
    main()

