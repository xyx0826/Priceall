# Priceall
Priceall is an app for quick EVE Online price-checking.

If you don't like tabbing out and pasting to browser, then this app is for you!

# Usage
Priceall is designed for checking item list prices **as quick as possible**, without moving your eyes away from the game.

**Copy** an item list and hit `Ctrl + Shift + C` to let Priceall do the work for you.

![Basic](https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/priceall-gif-01-basic.gif)

From settings, you can choose to show **numeric** price or **formatted** price.

![Format](https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/priceall-gif-02-format.gif)

And you can also customize the price tag **color**. Google [color picker](https://www.google.com/search?q=color%20picker) will come in handy!

![Color](https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/priceall-gif-03-color.gif)

**Drag** the widget to anywhere you want and **pin** it down.

![Drag](https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/priceall-gif-04-drag.gif)

Use mouse scroll to change background **transparency**, or hold `Ctrl` and scroll to change widget **size**.

![Resize](https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/priceall-gif-05-resize.gif)

If you make the widget **fully transparent**, it can support **click-through**.

![Clickthrough](https://raw.githubusercontent.com/xyx0826/Priceall/master/Readme/Images/priceall-gif-06-clickthrough.gif)

**Things to know:**
- Priceall works as an always-on-top window - it's not an actual "overlay" like the one Steam or Discord have. Therefore it might not work for fullscreen game.
- Priceall currently uses [Evepraisal](http://evepraisal.com) for price checking. Thus, Priceall supports all kinds of item list that Evepraisal supports - cargo scan results, contracts, blueprint material lists, etc.
- Priceall's defeault hotket, `Ctrl + Shift + C`, is *global* and by design will not be passed to other apps. Hotkey customization will come later, but in the mean time if you use this hotkey in EVE, you might want to set it to something else.

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
## Version 1.2, build 5
- Priceall will now check for updates on launch. If you see the settings button turning orange, there is an update available.
- Your settings will no longer be lost when updating Priceall.

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
