# Priceall [![AppVeyor build status](https://ci.appveyor.com/api/projects/status/github/xyx0826/Priceall?svg=true)](https://ci.appveyor.com/project/xyx0826/Priceall) 
Priceall is an app designed for quick EVE Online price-checking.

It currently uses evepraisal.com as the data source.

Ideally it eases the life of gankers, merchants and industrialists: Priceall makes it extremely easy and fast to check the value of any kind of item list.

# Usage
Click [me](https://ci.appveyor.com/project/xyx0826/Priceall/build/artifacts) to get the latest version! Download `build.zip`.
 
Priceall stays as an always-on-top overlay window. Therefore it is **not compatible** with fullscreen game.

Simply **copy** something (D-scan result, cargo scan result, contract etc.) onto clipboard and **click** the window to query for appraisal.

If you don't want to click, there is also a **hotkey**, `Ctrl + Shift + C`. Hotkey customization will come later.

*Note that this is a global hotkey so it will work anywhere. Therefore by design it will "block" the hotkey, preventing other programs from receiving it.*

There are three buttons at the bottom: Drag, Settings and Close.

To drag the window around, drag the Drag button. Right click the Drag button to pin/unpin the window.

To change window background transparency, hover mouse over the window and scroll. Window background color customization will come later.

To resize the window, hold `Ctrl` and scroll on the window.

# Development
Priceall uses C# with WPF. It is in early stage of development. Many features are planned but not implemented.

Issues, suggestions and pull requests are welcome.

My in-game character is `Sector Sabezan`. Feel free to send me ISK donations if you wish!

*Dataminers: also check out my other repo, `TriExplorer`. It's a modern remake of `TriExporter`, but still in development.*

# Changelog
## Version 1.1, build 4
- *(Tragot_Gomndor @ Reddit)* You can now hold `Ctrl` and scroll to change window size.
- *(Tragot_Gomndor @ Reddit)* You can now choose simple price display (e.g. `12.34 Mil`) instead of whole numbers.
    - *Turn on/off this feature in the settings.*
- *(Tragot_Gomndor @ Reddit & karl-kaefer @ GitHub)* You can now tune window opacity down to zero and allow click-through.
    - *Icon, text and buttons on the widget will still be clickable.*
- You can now specify a hex color (e.g. `C4B3A2`) for price tag display.
    - *If you specify an invalid color, Priceall will use white.*
    - *Known issue: when you edit color in settings, the window will become laggy.*
- There is now a "Reset all settings" button in the settings.
    - *Known issue: you need to click the button twice to fully reset the window's position and size.*
