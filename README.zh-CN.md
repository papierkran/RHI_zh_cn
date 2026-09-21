# RHI — ReShade HDR 安装程序

**一款工具。畅玩所有游戏。一个窗口搞定完整 HDR、DLSS 和 NVIDIA 驱动管理。**

RHI 会检测你所有游戏库中的所有主流商店，并处理 ReShade、HDR Mod、帧率限制器、DLSS 管理、OptiScaler、着色器包和 NVIDIA 驱动配置——一切只需一次点击。

> **关于本中文翻译版**
>
> 本项目是 RHI 的中文本地化分支，界面文本使用 AI（大语言模型）完成翻译。
>
> - **仅翻译界面文本**：翻译只针对程序中面向用户的英文显示内容（菜单、按钮、提示、说明、日志文案等），逐条直接由英文译为中文。
> - **不改动核心代码**：业务逻辑、功能实现、配置键、资源标识符、枚举值及其他任何非显示内容均保持原样，未被修改。
> - **自动化替换**：通过脚本批量识别 C# 源码与 XAML 界面中可翻译的字符串并做等价替换，确保不触及任何功能代码，保证与上游行为一致。
> - **翻译仅供参考**：译文由 AI 生成，个别术语或表述可能不够精确，欢迎指正与反馈。

---

## 下载

**[最新版本](https://github.com/RankFTW/RenoDXChecker/releases/latest)** · **[Discord](https://discord.gg/ultraplace)**

---

## 功能概述

### 一键为所有游戏开启 HDR

- **[RenoDX](https://github.com/clshortfuse/renodx)** —— 自动检测并为每款游戏安装正确的 HDR Mod。数百款专为特定游戏定制的 Mod，此外还为所有没有专属 Mod 的 Unreal Engine 和 Unity 游戏提供通用 HDR 支持。
- **UE-Extended** —— 所有没有命名 Mod 的 Unreal Engine 游戏的默认选择。自动配置 `reshade.ini` 和 `Engine.ini`，应用正确的 HDR 设置。无需手动编辑 ini 文件。
- **[Luma Framework](https://github.com/Filoppi/Luma-Framework)** —— 库中每款 DX11 Unreal Engine 游戏都会自动获得一行 Luma。支持的游戏有专属 Mod，其余游戏获得通用支持。Engine.ini 键、启动参数和 TAA 设置在安装时自动从 Wiki 应用。
- **[DXVK + Lilium HDR](https://github.com/EndlesslyFlowering/dxvk)** —— 为通常完全没有 HDR 路径的老款 DX9 游戏带来 scRGB HDR 输出。
- **[RenoFX HDR Toolkit](https://github.com/clshortfuse/renofx)** —— 为还没有专属 RenoDX Mod 的任何游戏提供 SDR 转 HDR 转换。

### ReShade —— 完全托管

RHI 完全替代了 ReShade 安装程序。

- 通过 PE 二进制扫描检测每款游戏的图形 API 并正确命名 DLL —— `dxgi.dll`、`d3d9.dll`、`opengl32.dll` —— 全自动。
- 每次启动时预置最新稳定版和每晚构建版。
- 按游戏设置通道：Stable、Nightly、Custom、Legacy 或 No Addons。可以在你的库中自由混合。
- Vulkan 游戏使用全局隐式层 —— 一键安装，无需手动步骤。
- 外部 DLL 检测：如果另一款工具（DXVK、Special K、ENB）已经在游戏文件夹中，在覆盖任何文件之前会发出警告。
- `reshade.ini` 自动生成并合并 —— 快捷键、截图路径、峰值亮度和 DLSS 路径全部设置完毕，无需任何手动配置。

### DLSS 5 与神经渲染

- **DLSS5 Feeder** —— 一键完成完整管线安装：部署 Feeder 加载项、DLSS5 消费者、`nvngx_dlss.dll`、`nvngx_dlssnr.dll`、预配置的 `ReShadePreset.ini` 和所需的着色器包。自动为你的游戏选择正确的方法（32 位、DX11、DX12）。
- **ShortFuse DLSS Tool** —— DX12 游戏的替代神经渲染实现。
- **DX11 Bridge 变体** —— DLSS5 Tool + Bridge，适用于需要 DX11 兼容性的游戏。
- **NR Cost Scaler** —— 将 `nvngx_dlssnr.dll` 封装以较低成本运行神经渲染（默认 75%）。在安装方法前切换即可自动部署。

### DLSS 和 Streamline 版本管理

- 从下拉菜单中按游戏切换 SR、RR、FG 和 Streamline 版本。从 2.x 到最新版本的所有版本，加上 Custom。
- 原始 DLL 自动备份。随时恢复原始版本。
- 只需设置一次全局默认值 —— Quick Apply 一键推送到任意游戏。
- 批量部署：从复选列表中选择多款游戏，一次性推送版本、预设和渲染比例。
- DLSS 预设（SR 为 J/K/L/M，RR 为 D/E，FG 为 A/B）直接写入 NVIDIA 驱动配置。无需 Profile Inspector。
- **DLSS SR 渲染比例覆盖** —— 强制每款游戏的渲染分辨率为 33–100%。
- **NVIDIA Override** —— 直接从版本下拉菜单为每个组件设置驱动的 "Latest DLL" 标志，相当于在 NVIDIA App 或 Profile Inspector 中启用。
- 从 RHI 清单自动更新 DLSS SR、RR 和 FG。按游戏选择加入。

### 多帧生成（MFG）

- **RTX 40 MFG Unlock** —— 通过 Ultimate ASI Loader 在 RTX 40 系列上启用 2x 之外的 DLSS MFG 倍率（最高 6x）。从 Extras 部分一键安装。
- **MFG Ada Unlock** —— 通过加载项管理器为 RTX 40 系列提供的内存中 MFG 解锁加载项。
- **RTX 50 系列 MFG** —— 从 DLSS 部分配置 Fixed 或 Dynamic MFG 模式、帧数（2x–6x）和动态目标帧率。

### OptiScaler

- 将 DLSS API 调用重定向到 AMD 和 Intel GPU 的 FSR4/FSR3/XeSS，并在游戏本身没有帧生成时添加帧生成支持。
- **每游戏 Stable 和 Nightly 通道**，可在齿轮图标中选择。
- **每 GPU INI 模板** —— 六个预配置模板（NVIDIA、AMD+DLSS、AMD no-DLSS，各有 Stable 和 Nightly）。首次启动时注入，用户可编辑，永不覆盖。
- **Streamline + DLSS Enabler 部署**（Nightly）—— 按游戏切换以将 Streamline DLL 和 DLSS Enabler 部署到 OptiScaler 文件夹。按游戏选择 Streamline 版本。
- 帧生成设置：FG Input、FG Output、FG Nvngx Replacement、HUD Fix —— 全部可在齿轮中配置。
- **OptiPatcher** 自动部署到所有安装了 OptiScaler 的游戏。静默自动更新。
- ReShade 共存自动处理 —— 文件名调整，设置 `LoadReshade=true`。

### 帧率限制器

两款按游戏独立的帧率限制器，互斥：

- **[ReLimiter](https://github.com/RankFTW/ReLimiter)** —— 精确的帧时序控制，具备预测性休眠、相位锁定时序网格和闭环校正。适配 VRR，DLSS FG 自适应，集成 Reflex。
- **[Display Commander](https://github.com/pmnoxx/display-commander)** —— 替代限制器，支持 32 位和 64 位游戏。

两者均作为 ReShade 加载项部署。均参与 Update All。

### NVIDIA 驱动配置管理 —— 替代 NVIDIA App 和 Profile Inspector

所有按游戏的驱动设置通过 NvAPI 直接写入。无需 NVIDIA Profile Inspector，无需 NVIDIA App。

**按游戏覆盖：**
- VSync Mode、Tear Control、Low Latency（Off/On/Ultra）
- Smooth Motion：Enable、Allowed APIs、Flip Pacing —— 启用时自动将 Low Latency 设为 Ultra
- 电源管理模式
- G-Sync 按游戏开关
- ReBAR：Enable（Off/On/Auto）、Mode（Standard/Optimized）、Size Limit
- RTX HDR：启用/配置峰值亮度、对比度、饱和度、中灰、去色带
- DLSS 预设和渲染比例覆盖（无需管理员权限）

**全局设置（基础配置）：**
- 着色器缓存大小和预编译
- G-Sync 模式和首选刷新率
- 全局 ReBAR
- FPS 限制（Frame Rate Limiter V3）
- 每显示器数字鲜艳度 —— 每次应用启动时自动恢复

**导出/导入：** 将所有按游戏的 NVIDIA 配置备份到 JSON 文件。导入可在驱动重装后恢复所有设置。

### HDR 自动切换

通过 RHI 启动游戏时启用 Windows HDR，游戏关闭时禁用。支持 Steam、Epic、Xbox 和直接 exe 启动。Launch 旁边有按游戏覆盖按钮 —— 激活时呈紫色。

### 着色器包管理 —— 46 个包

你可能需要的所有着色器包，全部在一个地方管理：

- **必备**：Lilium HDR Shaders（EndlesslyFlowering）—— HDR 色调映射所必需
- **推荐**：crosire reshade-shaders、PumboAutoHDR、MaxG2D Simple HDR、clshortfuse shaders、RenoFX HDR Toolkit、smolbbsoop shaders
- **额外**：39 个额外包 —— SweetFX、OtisFX、qUINT、iMMERSE、Prod80、CorgiFX、ZenteonFX、CRT-Royale、VRToolkit、GShade-Shaders、QD-OLED APL Fixer、LumaBoost 等

全局选择着色器包或按游戏覆盖。展开任意包以选择单个 `.fx` 文件 —— 三态复选框支持部分选择。将选择保存为命名配置，导出为 zip，直接分享到 Discord。

自定义着色器放在 `%LocalAppData%\RHI\Custom\reshade\Shaders\` —— RHI 会与托管包一起部署它们。

### ReShade 加载项管理

通过工具栏的全局加载项管理器。按游戏或全局启用/禁用。`%LocalAppData%\RHI\Custom\Addons\` 中的自定义加载项会自动出现在选择器中。

### Update All（全部更新）

工具栏中的一个按钮可更新整个库中所有游戏的 ReShade、RenoDX、ReLimiter、Display Commander、OptiScaler 和 RE Framework。尊重按游戏的更新排除。有更新可用时变为紫色。

运行期间每 4 小时后台检查 —— 无需保持窗口打开即可从系统托盘工作。

### 清单驱动更新

游戏数据、Mod 链接、Engine.ini 配置、启动修复和功能标志均由每次启动时从 GitHub 获取的清单驱动。这意味着：

- 新游戏支持、修复和调整立即部署到所有用户 —— 无需应用更新
- 按游戏的 Engine.ini 覆盖（如《黑神话：悟空》的自定义 HDR 键）由服务器端提供
- 功能标志通过清单切换为所有用户启用开发预览功能

---

## 支持的商店

Steam · Epic Games · GOG · Xbox / Game Pass · EA App · Ubisoft Connect · Battle.net · Rockstar · itch.io

如果未自动检测到，可以将任意游戏的 `.exe` 拖放到窗口上手动添加。

---

## 要求

- Windows 10 / 11
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
- 驱动配置功能推荐使用 NVIDIA GPU（ReShade 和 RenoDX 支持所有 GPU）
- 需要管理员权限的功能：ReBAR、Smooth Motion、Low Latency（Ultra）、Vulkan ReShade、驱动导出/导入

---

## 第三方组件

| 组件 | 作者 |
|-----------|--------|
| [ReShade](https://reshade.me) | Crosire |
| [RenoDX](https://github.com/clshortfuse/renodx) | clshortfuse & contributors |
| [ReLimiter](https://github.com/RankFTW/ReLimiter) | RankFTW |
| [Display Commander](https://github.com/pmnoxx/display-commander) | pmnoxx |
| [RE Framework](https://github.com/praydog/REFramework-nightly) | praydog |
| [Luma Framework](https://github.com/Filoppi/Luma-Framework) | Pumbo (Filoppi) |
| [OptiScaler](https://github.com/optiscaler/OptiScaler) | OptiScaler contributors |
| [DXVK](https://github.com/doitsujin/dxvk) | doitsujin & contributors |
| [DXVK HDR-mod](https://github.com/EndlesslyFlowering/dxvk) | EndlesslyFlowering (Lilium) |
| [Ultimate ASI Loader](https://github.com/ThirteenAG/Ultimate-ASI-Loader) | ThirteenAG |
