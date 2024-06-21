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

        /*
         *     old_punch = { 0, 0, 0 }
    while ur_shit_is_running {
        if shots_fired > 1 and ur_holding_rcs_button_lmb_or_whatever {
            new_angles = ((localplayer_viewangles + old_punch) - (punch_angle * 2.f)).normalized()
            old_punch = punch_angle * 2.f
            localplayer_viewangles = new_angles
        } else
            old_punch = { 0, 0, 0 }
    }

        */

        private static SharpDX.Vector3 oPunch = new SharpDX.Vector3(0, 0, 0);
        private static int rcsBulletTracker = 0;
        protected override void FrameAction()
        {
            if (!GameProcess.IsValid || !GameData.Player.IsAlive()) return;

            if(GameData.Player.ShotsFired > 1 && GameData.Player.ShotsFired != rcsBulletTracker)
            {
                SharpDX.Vector2 newAngles = new SharpDX.Vector2
                {
                    X = (GameData.Player.AimPunchAngleVec3.Y - oPunch.Y) * 2f,
                    Y = -(GameData.Player.AimPunchAngleVec3.X - oPunch.X) * 2f

                };

                Console.WriteLine($"[SHOT {GameData.Player.ShotsFired}] newangles: {newAngles}");

                newAngles.Normalize();

                Console.WriteLine($"[SHOT {GameData.Player.ShotsFired}] newangles normalized: {newAngles}");


                oPunch = GameData.Player.AimPunchAngleVec3 * 2f;

                SetViewAngle(newAngles.X, newAngles.Y);
                Console.WriteLine($"[SHOT {GameData.Player.ShotsFired}] set angles: {newAngles}");

                rcsBulletTracker = GameData.Player.ShotsFired;

            }
            else
            {
                if (GameData.Player.ShotsFired != rcsBulletTracker)
                {
                    if(oPunch != SharpDX.Vector3.Zero)
                    {
                        oPunch = SharpDX.Vector3.Zero;
                        Console.WriteLine("reset opunch");
                    }


                }


            }


        }


        public void SetViewAngle(float Yaw, float Pitch)
        {
            Vector2 Angle = new Vector2(Pitch, Yaw);
            GameProcess.ModuleClient.Write<Vector2>(Offsets.dwViewAngles, Angle);
        }
    }

}