using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CS2Cheat.Core;
using CS2Cheat.Core.Data;
using CS2Cheat.Data.Entity;
using CS2Cheat.Data.Game;
using CS2Cheat.Graphics;
using CS2Cheat.Utils;
using SharpDX;
using Vector2 = SharpDX.Vector2;
using Vector3 = SharpDX.Vector3;
using Vector4 = SharpDX.Vector4;
using Point = System.Drawing.Point;

namespace CS2Cheat.Features
{
    public class AimBot : ThreadedServiceBase
    {
        private readonly object _stateLock = new();
        private GameProcess GameProcess { get; set; }
        private GameData GameData { get; set; }

        private static Vector2 oldAngles = new Vector2(0, 0);
        private const float m_pitch = 0.022f;
        private const float m_yaw = 0.022f;

        public AimBot(GameProcess gameProcess, GameData gameData)
        {
            GameProcess = gameProcess;
            GameData = gameData;
        }

        private static string AimBonePos => "head";

        protected override string ThreadName => nameof(AimBot);

        public override void Dispose()
        {
            base.Dispose();
            GameData = null;
            GameProcess = null;
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        const uint MOUSEEVENTF_MOVE = 0x0001;

        protected override void FrameAction()
        {
            if (!config.config.masterAimbotBool) return;

            if (!GameProcess.IsValid || !GameData.Player.IsAlive()) return;

            Vector2 screenCenter = new Vector2(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
            float fovRadius = config.config.aimbot_FOV;

            if (GameData.Player.ShotsFired > 0)
            {
                float closestDistance = float.MaxValue;
                Vector2 closestEnemyScreenPosition = Vector2.Zero;
                Entity closestEnemy = null;

                foreach (var entity in GameData.Entities)
                {
                    if (!IsEnemyWithinFOV(entity, screenCenter, fovRadius))
                        continue;

                    float distanceToPlayer = Vector3.Distance(GameData.Player.EyePosition, entity.BonePos[AimBonePos]);

                    if (distanceToPlayer < closestDistance)
                    {
                        closestDistance = distanceToPlayer;
                        Vector2 screenPosition = WorldToScreen(entity.BonePos[AimBonePos]);
                        closestEnemyScreenPosition = screenPosition;
                        closestEnemy = entity;
                    }
                }

                if (closestEnemy != null && closestEnemyScreenPosition != Vector2.Zero)
                {
                    config.neededInfo.isAiming = true;

                    AdjustAim(closestEnemyScreenPosition);
                }
                else
                {
                    config.neededInfo.isAiming = false;
                }

            }
            else
            {
                config.neededInfo.isAiming = false;
            }
        }

        private bool IsEnemyWithinFOV(Entity enemy, Vector2 screenCenter, float fovRadius)
        {

            if (config.config.aimbot_teamCheck)
            {
                if (enemy.IsAlive() && enemy.AddressBase != GameData.Player.AddressBase && enemy.Team != GameData.Player.Team)
                {
                    Vector2 screenPosition = WorldToScreen(enemy.BonePos[AimBonePos]);

                    float distanceToCenter = Vector2.Distance(screenCenter, screenPosition);

                    return distanceToCenter <= fovRadius;
                }

            }
            else
            {
                if (enemy.IsAlive() && enemy.AddressBase != GameData.Player.AddressBase)
                {
                    Vector2 screenPosition = WorldToScreen(enemy.BonePos[AimBonePos]);

                    float distanceToCenter = Vector2.Distance(screenCenter, screenPosition);

                    return distanceToCenter <= fovRadius;
                }
            }


            return false;
        }

        private void AdjustAim(Vector2 targetScreenPosition)
        {
            Point currentPos = Cursor.Position;
            int dx = (int)(targetScreenPosition.X - currentPos.X);
            int dy = (int)(targetScreenPosition.Y - currentPos.Y);

            Vector2 aimPunch = GameData.Player.AimPunchAngle;
            int shotsFired = GameData.Player.ShotsFired;

            if (shotsFired > 1 && shotsFired != rcsBulletTracker)
            {
                Vector2 newAngles = new Vector2
                {
                    X = (aimPunch.Y - oldAngles.Y) * config.config.rcs_x / (m_pitch * GameData.Player.sens) / 1,
                    Y = -(aimPunch.X - oldAngles.X) * config.config.rcs_y / (m_yaw * GameData.Player.sens) / 1
                };

                dx += (int)newAngles.X;
                dy += (int)newAngles.Y;

                rcsBulletTracker = shotsFired;
                oldAngles = aimPunch;
            }
            else
            {
                oldAngles = new Vector2(0, 0);
            }

            mouse_event(MOUSEEVENTF_MOVE, dx, dy, 0, 0);
        }

        private static int rcsBulletTracker;

        private Vector2 WorldToScreen(Vector3 worldPos)
        {
            // Assume viewMatrix is set correctly from the game process
            Matrix4x4 viewMatrix = Matrix4x4.Transpose(GameProcess.ModuleClient.Read<Matrix4x4>(Offsets.dwViewMatrix));
            Vector4 clipCoords = new Vector4(
                worldPos.X * viewMatrix.M11 + worldPos.Y * viewMatrix.M21 + worldPos.Z * viewMatrix.M31 + viewMatrix.M41,
                worldPos.X * viewMatrix.M12 + worldPos.Y * viewMatrix.M22 + worldPos.Z * viewMatrix.M32 + viewMatrix.M42,
                worldPos.X * viewMatrix.M13 + worldPos.Y * viewMatrix.M23 + worldPos.Z * viewMatrix.M33 + viewMatrix.M43,
                worldPos.X * viewMatrix.M14 + worldPos.Y * viewMatrix.M24 + worldPos.Z * viewMatrix.M34 + viewMatrix.M44
            );

            if (clipCoords.W < 0.1f)
                return Vector2.Zero;


            Vector3 NDC = new Vector3(clipCoords.X / clipCoords.W, clipCoords.Y / clipCoords.W, clipCoords.Z / clipCoords.W);

            float screenX = (Screen.PrimaryScreen.Bounds.Width / 2 * NDC.X) + (NDC.X + Screen.PrimaryScreen.Bounds.Width / 2);
            float screenY = -(Screen.PrimaryScreen.Bounds.Height / 2 * NDC.Y) + (NDC.Y + Screen.PrimaryScreen.Bounds.Height / 2);

            return new Vector2(screenX, screenY);
        }
    }
}
