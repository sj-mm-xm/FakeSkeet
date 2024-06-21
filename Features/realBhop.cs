using System;
using System.Runtime.InteropServices;
using System.Threading;
using CS2Cheat.Core;
using CS2Cheat.Core.Data;
using CS2Cheat.Data.Entity;
using CS2Cheat.Data.Game;
using CS2Cheat.Utils;
using Process.NET.Memory;
using config;

namespace CS2Cheat.Features
{
    public class realBhop : ThreadedServiceBase
    {
        private readonly object _stateLock = new();
        private GameProcess GameProcess { get; set; }
        private GameData GameData { get; set; }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        private const int VK_W = 0x57;
        private const int VK_S = 0x53;
        private const int VK_A = 0x41;
        private const int VK_D = 0x44;
        private const int VK_SPACE = 0x20;

        public realBhop(GameProcess gameProcess, GameData gameData)
        {
            GameProcess = gameProcess;
            GameData = gameData;
        }

        protected override string ThreadName => nameof(rcs);

        public override void Dispose()
        {
            base.Dispose();
            GameData = null;
            GameProcess = null;
        }

        private void keyDown(IntPtr hWnd, int key)
        {
            PostMessage(hWnd, WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
        }
        private void keyUp(IntPtr hWnd, int key)
        {
            PostMessage(hWnd, WM_KEYUP, (IntPtr)key, IntPtr.Zero);
        }

        bool wasA = false;
        bool wasD = false;
        bool wasS = false;
        bool wasW = false;

        protected override void FrameAction()
        {
            if (!GameProcess.IsValid || !GameData.Player.IsAlive() || !config.config.bhop)
            {
                return;
            }

            IntPtr hWnd = FindWindow(null, "Counter-Strike 2");
            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("Window not found.");
                return;
            }

            while((GetAsyncKeyState(VK_SPACE) & 0x8000) != 0)
            {
                if(GameData.Player.FFlags != 65664)
                {
                    keyUp(hWnd, VK_SPACE);

                    keyDown(hWnd, VK_SPACE);

                }
                Thread.Sleep(1);

            }

        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
    }
}
