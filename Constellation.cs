// ============================================================
// CONSTELLATION - Lightweight Live Wallpaper for Windows 10
// Made by AI, with Meta AI
// ------------------------------------------------------------
// HOW TO CHANGE THINGS - READ THIS FIRST, ALL EDITS ARE IN THIS FILE ONLY
// 1. PARTICLE COUNT - search for: ParticleCount
//    Change 2000 to 1000 = lighter, 500 = very light. 2000 = dense like first version.
// 2. CONNECT DISTANCE - search for: ConnectDist
//    Change 80 to 60 = smaller area around cursor, 120 = bigger area.
// 3. PARTICLE SIZE - search for: p.R =
//    Change 1.8f + rnd.Next(3) to bigger numbers for bigger dots. Example: 3.5f + rnd.Next(4)
// 4. PARTICLE SPEED - search for: p.VX =
//    Numbers 1.6 - 0.8 control speed. Smaller = slower drift. Example: 1.0 - 0.5 = slower.
// 5. BACKGROUND COLOR - search for: bgBrush =
//    Right now 0,0,0 = pure black. Change to 10,10,18 for dark blue-black.
// 6. DIM COLOR (untouched particles) - search for: dimBrush =
//    Right now 55,35,35,35 = dim gray almost black. First number is alpha.
//    Try 70,50,50,50 = more visible. Try 35,35,35,35 = more invisible.
// 7. LINE COLOR - search for: Color.FromArgb(alpha, 255, 255, 255)
//    Right now 255,255,255 = white lines. Change to 0,255,65 = terminal green.
//    Change to 0,255,255 = cyan. Keep alpha for fade.
// 8. RAINBOW CONNECTED PARTICLES - search for: double hue =
//    Right now hue = (frameCount * 2 + i * 12) % 360
//    frameCount * 2 = speed of color shift. *1 = slower, *4 = faster.
//    i * 12 = spread between particles. i * 20 = more spread, i * 5 = less spread.
//    To make connected particles white instead of rainbow, replace rainbow brush
//    with white: use Color.FromArgb(255, 255, 255, 255)
// 9. FPS / SPEED - search for: timer.Interval =
//    33 = 30 FPS. 50 = 20 FPS, lighter, less DWM load. 16 = 60 FPS, heavier, do not use on 4GB.
// 10. HOW TO CLOSE - No tray icon by design. Press Ctrl + Alt + Q, or double click close.bat,
//     or taskkill /F /IM Constellation.exe
// ------------------------------------------------------------
// NO TRAY ICON - There is no NotifyIcon in this code on purpose. It stays invisible.
// EXIT - Ctrl + Alt + Q is registered in OnLoad, handled in WndProc.
// ============================================================

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class Particle
{
    public float X;
    public float Y;
    public float VX;
    public float VY;
    public float R;
}

public class ConstellationForm : Form
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string className, string windowName);

    [DllImport("user32.dll")]
    static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, uint flags, uint timeout, out IntPtr result);

    [DllImport("user32.dll")]
    static extern bool EnumWindows(EnumWindowsProc proc, IntPtr param);

    delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr param);

    [DllImport("user32.dll")]
    static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string className, string windowName);

    [DllImport("user32.dll")]
    static extern IntPtr SetParent(IntPtr child, IntPtr newParent);

    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out POINT point);

    [DllImport("user32.dll")]
    static extern bool RegisterHotKey(IntPtr hWnd, int id, uint modifiers, uint key);

    [DllImport("user32.dll")]
    static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [StructLayout(LayoutKind.Sequential)]
    struct POINT
    {
        public int X;
        public int Y;
    }

    private Particle[] particles;
    private Random rnd;
    private Timer timer;
    private Point mouse;
    private SolidBrush dimBrush;
    private SolidBrush bgBrush;
    private int frameCount;
    // HOW TO CHANGE PARTICLE COUNT: Change 2000 below. Lower = lighter, higher = denser.
    private const int ParticleCount = 2000;
    // HOW TO CHANGE CONNECT DISTANCE: Change 80 below. Bigger = cursor connects from farther.
    private const int ConnectDist = 80;

    public ConstellationForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = false;
        StartPosition = FormStartPosition.Manual;
        Bounds = Screen.PrimaryScreen.Bounds;
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        BackColor = Color.FromArgb(0, 0, 0);

        rnd = new Random();
        particles = new Particle[ParticleCount];
        // HOW TO CHANGE DIM COLOR: First number = alpha (visibility), next 3 = R,G,B gray.
        // 55,35,35,35 = dim gray almost black. Try 70,50,50,50 for more visible.
        dimBrush = new SolidBrush(Color.FromArgb(55, 35, 35, 35));
        // HOW TO CHANGE BACKGROUND: 0,0,0 = pure black. Try 10,10,18 for dark blue-black.
        bgBrush = new SolidBrush(Color.FromArgb(0, 0, 0));
        frameCount = 0;

        for (int i = 0; i < ParticleCount; i++)
        {
            Particle p = new Particle();
            p.X = rnd.Next(Width);
            p.Y = rnd.Next(Height);
            // HOW TO CHANGE SPEED: Smaller numbers = slower drift. Right now 1.6 - 0.8 = -0.8 to 0.8
            p.VX = (float)(rnd.NextDouble() * 1.6 - 0.8);
            p.VY = (float)(rnd.NextDouble() * 1.6 - 0.8);
            // HOW TO CHANGE PARTICLE SIZE: 1.8f + rnd.Next(3) = 1.8 to 4.8. Try 2.5f + rnd.Next(4) for bigger.
            p.R = 1.8f + rnd.Next(3);
            particles[i] = p;
        }

        mouse = new Point(Width / 2, Height / 2);

        timer = new Timer();
        // HOW TO CHANGE FPS: 33 = 30 FPS. 50 = 20 FPS lighter. Do not use 16 on 4GB RAM.
        timer.Interval = 33;
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x80;
            cp.ExStyle |= 0x08000000;
            return cp;
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        AttachToDesktop();
        // EXIT HOTKEY: Ctrl + Alt + Q. No tray icon, so this is the main way to close.
        RegisterHotKey(Handle, 1, 0x0002 | 0x0001, 0x51);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x0312 && m.WParam.ToInt32() == 1)
        {
            Application.Exit();
            return;
        }
        base.WndProc(ref m);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        try
        {
            UnregisterHotKey(Handle, 1);
        }
        catch
        {
        }
        base.OnFormClosed(e);
    }

    private void AttachToDesktop()
    {
        try
        {
            IntPtr progman = FindWindow("Progman", null);
            IntPtr result = IntPtr.Zero;
            SendMessageTimeout(progman, 0x052C, IntPtr.Zero, IntPtr.Zero, 0, 1000, out result);

            IntPtr worker = IntPtr.Zero;
            EnumWindows(delegate(IntPtr hWnd, IntPtr param)
            {
                IntPtr shellView = FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shellView != IntPtr.Zero)
                {
                    worker = FindWindowEx(IntPtr.Zero, hWnd, "WorkerW", null);
                }
                return true;
            }, IntPtr.Zero);

            if (worker != IntPtr.Zero)
            {
                SetParent(Handle, worker);
            }
            else
            {
                SetParent(Handle, progman);
            }
        }
        catch
        {
        }
    }

    private Color ColorFromHsv(double hue, double saturation, double value)
    {
        double h = hue;
        while (h < 0)
        {
            h += 360;
        }
        while (h >= 360)
        {
            h -= 360;
        }
        double c = value * saturation;
        double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        double m = value - c;
        double r = 0;
        double g = 0;
        double b = 0;
        if (h < 60)
        {
            r = c;
            g = x;
            b = 0;
        }
        else if (h < 120)
        {
            r = x;
            g = c;
            b = 0;
        }
        else if (h < 180)
        {
            r = 0;
            g = c;
            b = x;
        }
        else if (h < 240)
        {
            r = 0;
            g = x;
            b = c;
        }
        else if (h < 300)
        {
            r = x;
            g = 0;
            b = c;
        }
        else
        {
            r = c;
            g = 0;
            b = x;
        }
        int ri = (int)((r + m) * 255);
        int gi = (int)((g + m) * 255);
        int bi = (int)((b + m) * 255);
        return Color.FromArgb(ri, gi, bi);
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        frameCount++;
        POINT p;
        if (GetCursorPos(out p))
        {
            mouse.X = p.X;
            mouse.Y = p.Y;
        }

        for (int i = 0; i < particles.Length; i++)
        {
            Particle o = particles[i];
            o.X += o.VX;
            o.Y += o.VY;
            if (o.X < -10)
            {
                o.X = Width + 10;
            }
            if (o.X > Width + 10)
            {
                o.X = -10;
            }
            if (o.Y < -10)
            {
                o.Y = Height + 10;
            }
            if (o.Y > Height + 10)
            {
                o.Y = -10;
            }
        }

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.FillRectangle(bgBrush, 0, 0, Width, Height);

        for (int i = 0; i < particles.Length; i++)
        {
            Particle o = particles[i];
            float dx = o.X - mouse.X;
            float dy = o.Y - mouse.Y;
            float distSq = dx * dx + dy * dy;
            bool connected = distSq < ConnectDist * ConnectDist;

            if (connected)
            {
                float dist = (float)Math.Sqrt(distSq);
                int alpha = (int)((1 - dist / ConnectDist) * 180) + 30;
                if (alpha > 255)
                {
                    alpha = 255;
                }
                if (alpha < 0)
                {
                    alpha = 0;
                }
                // HOW TO CHANGE LINE COLOR: Change 255,255,255 below. 255,255,255 = white.
                // Try 0,255,65 = terminal green, 0,255,255 = cyan. Keep alpha for fade.
                using (Pen pen = new Pen(Color.FromArgb(alpha, 255, 255, 255), 1))
                {
                    g.DrawLine(pen, o.X, o.Y, mouse.X, mouse.Y);
                }
                // HOW TO CHANGE RAINBOW: frameCount * 2 = color shift speed. *1 slower, *4 faster.
                // i * 12 = spread between particles. i * 20 more spread, i * 5 less spread.
                // To make connected particles white, replace rainbow brush with white brush.
                double hue = (frameCount * 2 + i * 12) % 360;
                Color rainbow = ColorFromHsv(hue, 1.0, 1.0);
                using (SolidBrush rainbowBrush = new SolidBrush(rainbow))
                {
                    g.FillEllipse(rainbowBrush, o.X - o.R, o.Y - o.R, o.R * 2, o.R * 2);
                }
            }
            else
            {
                // HOW TO CHANGE DIM PARTICLES: They use dimBrush above. Change dimBrush color to change them.
                g.FillEllipse(dimBrush, o.X - o.R, o.Y - o.R, o.R * 2, o.R * 2);
            }
        }
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new ConstellationForm());
    }
}
