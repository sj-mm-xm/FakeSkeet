using System;
using System.Threading;
using CS2Cheat.Core;
using CS2Cheat.Core.Data;
using CS2Cheat.Data.Entity;
using CS2Cheat.Data.Game;
using CS2Cheat.Utils;
using Process.NET.Memory;
using config;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CS2Cheat.Features
{
    public class bhop : ThreadedServiceBase
    {
        private readonly object _stateLock = new();
        private GameProcess GameProcess { get; set; }
        private GameData GameData { get; set; }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const int VK_SPACE = 0x20;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_SCANCODE = 0x0008;

        public bhop(GameProcess gameProcess, GameData gameData)
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

        private void PressAndReleaseSpace()
        {
            keybd_event(VK_SPACE, 0, 0, UIntPtr.Zero); // Press space
            keybd_event(VK_SPACE, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Release space
        }

        protected override void FrameAction()
        {
            if (!GameProcess.IsValid || !GameData.Player.IsAlive()) return;

            if (true) // config check
            {
                if ((GetAsyncKeyState(VK_SPACE) & 0x8000) != 0) // check if space key is pressed
                {
                    // Implement your bhop logic here
                    // 65665 is on ground
                    // 65664 is in air
                    var playerState = GameData.Player.FFlags; // Get the player's state
                    if (playerState == 65665) // on ground
                    {
                        PressAndReleaseSpace(); // Press and release the space key
                    }
                }
            }
        }
    }
}
