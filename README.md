# Gamma Manager

Doesn't need setup, works from .exe file. The only additional .ini file is created in the same folder as .exe file (to save settings presets).

Overrides all current gamma settings.

![image](GammaManager.jpg?raw=true)

Color buttons allow to manage gamma ramp options independently for rgb or altogether.
Checkbox []+++ for extreme contrast values.

Pull trackbar pointer for major changes, keyboard arrows or mouse clicks to sides from selected trackbar pointer for minor changes.

<pre>
Gamma 0.30 — 4.40             [1.00 default]    | CreateGammaRamp function doesn't work with lesser or bigger values<br />
Brightness -1.00 — 1.00       [0.00 default]    | Values out of range don't provide any effect<br />
Contrast 0.10 — 3.00|100.00   [1.00 default]    | 0.10 cap for safety reasons, first upper limit for practical use, second limit for memes
</pre>

Small image for calibration purposes.

Drop-down list with monitors on the left side. Change manually, with mouse wheel or button "Forward" to scroll.

Drop-down list with presets in the center. Choose manually or with mouse wheel. To save preset write a name in empty list and click "Save" to put preset in .ini file. 

Choose preset and click "Delete" to erase preset from .ini file.

Monitor brightness and contrast (if available) trackbars, 0 - 100, same functional as gamma ramp trackbars.

"Reset" changes all values to default positions (won't return to previous settings).

"Hide" minimizes app to tray.

Restore minimized window: double click or right click on tray icon - > Settings.
All monitors should be shown in tray as drop-down lists filled with presets.

Exit: red "x" in upper right or right click on tray icon - > Exit.
