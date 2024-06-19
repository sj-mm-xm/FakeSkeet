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
    public class rcs : ThreadedServiceBase
    {
        private readonly object _stateLock = new();
        private GameProcess GameProcess { get; set; }
        private GameData GameData { get; set; }

        public rcs(GameProcess gameProcess, GameData gameData)
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

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        const uint MOUSEEVENTF_MOVE = 0x0001;

        private static SharpDX.Vector2 oldAngles = new SharpDX.Vector2(0, 0);
        private const float m_pitch = 0.022f;
        private const float m_yaw = 0.022f;
        private const float sens = 1.25f; // Hardcoded mouse sensitivity in settings

        protected override void FrameAction()
        {
            if (!GameProcess.IsValid || !GameData.Player.IsAlive()) return;

            SharpDX.Vector2 aimPunch = GameData.Player.AimPunchAngle;
            int shotsFired = GameData.Player.ShotsFired;

            if (shotsFired > 1)
            {
                aimPunch = GameData.Player.AimPunchAngle;

                SharpDX.Vector2 newAngles = new SharpDX.Vector2
                {
                    X = (aimPunch.Y - oldAngles.Y) * 2f / (m_pitch * sens) / 1,
                    Y = -(aimPunch.X - oldAngles.X) * 2f / (m_yaw * sens) / 1
                };

                // Move the mouse to compensate for recoil
                mouse_event(MOUSEEVENTF_MOVE, (int)(newAngles.X), (int)newAngles.Y, 0, 0);

                // Update oldAngles to the current aimPunch for the next frame
                oldAngles = aimPunch;
            }
            else
            {
                oldAngles = new SharpDX.Vector2(0, 0);
            }
        }
    }
}
