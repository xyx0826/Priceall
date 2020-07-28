# Priceall [![AppVeyor build status](https://ci.appveyor.com/api/projects/status/github/xyx0826/Priceall?svg=true)](https://ci.appveyor.com/project/xyx0826/Priceall)

<img src="Readme/Images/Priceall.png" width="128" height="128"/>

Priceall 专为 EVE Online 快速物品估价而生。

无论你是一名探险家、制造商还是贸易大亨，Priceall
都能帮你**飞速查询**物品列表的价格，无需把目光移开游戏界面。

# 兼容服务

<img src="Readme/Images/evepraisal-logo.png" width="300"/> <img src="Readme/Images/janice-logo.png" width="300"/>

# 目录

- 用法：下载与使用
- 开发：Priceall 代码库信息
- 更新日志：新功能、缺陷修复

# 用法

在[这里](https://ci.appveyor.com/project/xyx0826/Priceall/build/artifacts)
点击 `build.zip` 就可下载最新版。

想要为物品列表估价，将其复制到剪贴板即可。Priceall 便会自动为你查价。

*不喜欢自动查询？打开设置便可创建快捷键。*

支持选择显示**精确**价格或**简略**价格。

![Format](Readme/Images/priceall-gif-02-format.gif)

随意定制**字体颜色**。

*颜色代码看不懂？试试[谷歌取色器](https://www.google.com/search?q=color%20picker)。*

![Color](Readme/Images/priceall-gif-03-color.gif)

按住**拖拽**图标，把窗口拉到任意位置，然后**右击**锁定。

![Drag](Readme/Images/priceall-gif-04-drag.gif)

用滚轮调整**透明度**，搭配 **Ctrl** 键滚动来调整窗口**大小**。

![Resize](Readme/Images/priceall-gif-05-resize.gif)

如果背景被调为**完全透明**，便可以点击窗口背后的内容。（点击穿透）

![Clickthrough](Readme/Images/priceall-gif-06-clickthrough.gif)

# 开发

Priceall 使用 C# + WPF。欢迎提出建议、Issue 或 PR。

`master` 分支内的代码均为稳定版本。 页面顶端的 AppVeyor 标签和下载链接也使用此分支进行编译。

Priceall 的开发现在位于 `dev` 分支，内含不稳定或未完成的功能。请将所有 PR 发送给此分支。

我的游戏内角色名是 `Sector Sabezan`。欢迎捐助！（额其实我退坑了

# 更新日志

## 版本 1.5，版本号 11

- *(karl-kaefer @ GitHub)* 添加了 [Janice](https://janice.e-351.com/) 估价支持。
- 现在支持自定义或移除热键。
- 修复了 Priceall 不会在按 `Alt-F4` 时关闭的问题。
- 在设置内添加了谷歌取色器的链接。
- 大量内部重构。

## 版本 1.4, 版本号 10

- 为 Priceall 加入了一个极简图标。
- 将设置窗口的代码实现改为使用事件模型。

## 版本 1.4, 版本号 9

- *(razaqq @ GitHub)* 现在可以于设置窗口自定义快捷键。
  - 点击编辑器，然后按下你想要的快捷键。
  - 新的快捷键会立刻生效。
  - *已知问题：编辑器内的快捷键显示有误。*

## 版本 1.3, 版本号 8

- 修复了一个导致 Priceall 在启动时不会正确启用自动刷新的问题。

## 版本 1.3, 版本号 7

- *(razaqq @ GitHub)* 你现在可以启用**自动刷新**来监听剪贴板的内容变化。
  - 启用时，只要剪贴板内容变化，Priceall 便会执行一次查价，无需使用快捷键！查询冷却时间仍然有效。
  - *我们无法预估这项功能对 Evepraisal 服务器带来的压力。如果它造成了性能影响，Priceall 可能会请你关掉这项功能。*

## 版本 1.2, 版本号 6

- *(Perry_Swift @ Reddit)* 你现在可以使用**条件颜色**。
  - 指定一个**低位**价格及其颜色，以及**高位**价格与颜色。
  - 如果价格低于低位阈值，它将会以低位颜色显示。
  - 如果价格高于高位阈值，它将会以高位价格显示。
  - 如果两个阈值冲突（例：高位价格比低位价格小），**低位**价格会被优先使用。

## 版本 1.2, 版本号 5

- Priceall 会在启动时检查更新。如果设置标志变红，就表明有更新可供下载。
- 更新 Priceall 时不再会丢失设置了。

## 版本 1.1, 版本号 4

- *(Tragot_Gomndor @ Reddit)* 你现在可以按住 `Ctrl` 并滚动来缩放窗口。
- *(Tragot_Gomndor @ Reddit)* 你现在可以选用简略价格显示 (例：`12.34 Mil`)，或完整数字（例：`12,345,678.90`）
  - *可在设置中启用这项功能。*
- *(Tragot_Gomndor @ Reddit & karl-kaefer @ GitHub)* 你现在可以将窗口背景调为完全透明，允许点击穿透。
  - *窗口上的图标、文本和按钮仍可被点击。*
- 你现在可以选择一个 16 进制颜色（例：`C4B3A2`）作为价格颜色。
  - *如果输入的颜色无效，Priceall 将默认使用白色。*
- 设置窗口中现在有了“重置所有设置“按钮。
  - *已知问题：你需要点击按钮两次来完全重置窗口的大小和位置。*
