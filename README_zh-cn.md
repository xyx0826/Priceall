# Priceall [![AppVeyor build status](https://ci.appveyor.com/api/projects/status/github/xyx0826/Priceall?svg=true)](https://ci.appveyor.com/project/xyx0826/Priceall) 
<img src="https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/Priceall.png" width="128" height="128" />

Priceall 是 EVE Online 的快速估价工具。

如果你不喜欢为了物品估价而切出游戏、使用浏览器，这款工具就是为你打造的！

**Priceall 目前支持 EVE 宁静服务器。**

# 文档目录
- 用法：下载与使用
- 开发：Priceall 代码库信息
- 更新日志：新功能、缺陷修复

# 用法
点[我](https://ci.appveyor.com/project/xyx0826/Priceall/build/artifacts)，然后下载 `build.zip` 就能获取最新版本！

Priceall 可以让你**飞速查询**物品列表的估价，无需把目光移开游戏画面。

**复制** 任意物品列表到剪贴板，然后按快捷键 `Ctrl + Shift + C`，Priceall 就会自动为你查询价格。

**新功能：自动刷新！** 复制到剪贴板后无需按快捷键。Priceall 会自动刷新估价。

![操作](https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/priceall-gif-01-basic.gif)

在设置里，你可以选择显示**精确**价格或**简略**价格。

![格式](https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/priceall-gif-02-format.gif)

你也可以自定义**字体颜色**。可以试试谷歌的[取色器](https://www.google.com/search?q=color%20picker)哦！

![颜色](https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/priceall-gif-03-color.gif)

窗口可被**拖拽**到任意位置，然后右击**锁定**。

![拖拽](https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/priceall-gif-04-drag.gif)

使用鼠标滚轮来调整背景**透明度**。搭配 `Ctrl` 键滚动可以更改窗口**大小**。

![缩放](https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/priceall-gif-05-resize.gif)

如果背景被调为**完全透明**，便可以点击窗口背后的内容。（点击穿透）

![穿透](https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/priceall-gif-06-clickthrough.gif)

**使用须知：**
- Priceall 是一个置顶小窗口——它不是像 Steam 或 Discord 的那种”覆盖层“（Overlay）。所以它可能不兼容全屏游戏。
- Priceall 目前使用 [Evepraisal](http://evepraisal.com) 作为数据源。所以 Evepraisal 支持的物品列表 Priceall 都支持：货柜扫描、合同、蓝图材料等等。
- Priceall 的快捷键（默认为 `Ctrl + Shift + C`）是**全局**的。因为技术限制，其他程序不能捕捉到这个快捷键。

窗口底部有三个按钮：拖拽、设置和关闭。

按住拖拽按钮然后移动鼠标，就能改变窗口位置。右击拖拽按钮便可锁定/解锁窗口位置。

将鼠标在窗口上滚动便可更改窗口透明度。日后会加入窗口颜色自定义。

按住 `Ctrl` 然后将鼠标在窗口上滚动便可缩放窗口大小。

# 开发
Priceall 使用 C# + WPF。欢迎提出建议、Issue 或 PR。

`master` 分支内的代码均为稳定版本。 页面顶端的 AppVeyor 标签和下载链接也使用此分支进行编译。

Priceall 的开发现在位于 `dev` 分支，内含不稳定或未完成的功能。请将所有 PR 发送给此分支。

我的游戏内角色名是 `Sector Sabezan`。欢迎捐助！

*Dataminers: also check out my other repo, `TriExplorer`. It's a modern remake of `TriExporter`, but still in development.*

# 更新日志
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
- Priceall will now check for updates on launch. If you see the settings button turning orange, there is an update available.
- Your settings will no longer be lost when updating Priceall.

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
