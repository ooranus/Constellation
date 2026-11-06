# CONSTELLATION

Lightweight live wallpaper for Windows. Particles drift on black desktop, white line to cursor, rainbow when close.

Made by AI, with Meta AI.

## No tray icon

This is normal. There is no tray icon on purpose, to keep it invisible.
You will NOT see an icon near the clock. Do not look for it.

## Build

Double click `compile.bat`.
It finds csc.exe by itself and starts it.

## Run

```
start Constellation.exe
```

## Kill it - pick one

```
Ctrl + Alt + Q
```

Double click `close.bat`

Or in cmd:
```
taskkill /F /IM Constellation.exe
```

## Auto-start when PC starts

By default it does NOT start when PC starts. You have to run it yourself.

If you want it always on:

1. Put the exe anywhere you like, for example:
   ```
   C:\Constellation\Constellation.exe
   ```
2. Create a shortcut for it:
   Right click Desktop > New > Shortcut > Location: your exe path > Name: `Constellation` > Finish
3. Copy that shortcut to Startup folder:
   Press `Win + R`, type `shell:startup`, Enter, Paste the shortcut there

Done. Now it will start every time you log in.

To stop auto-start, delete the shortcut from Startup folder.

## Change it

Open `Constellation.cs`, all instructions are in comments at the top.

- `ParticleCount = 2000` - how many, lower = lighter
- `ConnectDist = 80` - how far cursor connects
- `timer.Interval = 33` - 33 = 30 FPS, 50 = 20 FPS lighter
- `dimBrush` - untouched color
- Line color - search for `255, 255, 255`

After change: save, run `close.bat`, run `compile.bat`.

## Files

- `Constellation.cs` - the code
- `compile.bat` - build
- `close.bat` - close
- `README.md` - this file

## Uninstall

Run `close.bat`, delete the folder. Nothing else left.

## Disclaimer

This program only draws particles on your desktop. It does not collect data, does not connect to internet, does not change system files. If Windows says Unknown publisher, that is because it is not signed, not because it is a virus.
