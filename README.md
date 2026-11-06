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

## Auto-start when PC starts - Shortcut

By default it does NOT start when PC starts. You have to run it yourself.

If you want it always on, put a shortcut in Startup:

1. Press `Win + R`, type `shell:startup`, Enter
2. Right click > New > Shortcut
3. Location: put your exe path, for example:
   ```
   C:\Constellation\Constellation.exe
   ```
4. Click Next, Name: `Constellation`, Finish

Now it will start every time you log in.

To stop auto-start, delete that shortcut:
1. Press `Win + R`, type `shell:startup`, Enter
2. Delete `Constellation` shortcut

Do NOT put the exe in `C:\Windows` or System folder. Keep it in `C:\Constellation` or your own folder.

### Note - If you cannot delete the Startup shortcut

This is a Windows bug, not a virus. Dead Startup shortcuts sometimes get stuck in Explorer, it says file is in use even when it is dead and not running.

If Explorer will not delete it, use CMD:

1. Press `Win + R`, type `cmd`, press `Ctrl + Shift + Enter` for Admin
2. Run this:
   ```
   del /f /q "%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup\Constellation.lnk"
   ```

If it still says in use, kill Explorer first, then delete, then start Explorer again:
```
taskkill /F /IM explorer.exe
del /f /q "%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup\Constellation.lnk"
start explorer.exe
```

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
