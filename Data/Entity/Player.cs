﻿using CS2Cheat.Core.Data;
using CS2Cheat.Data.Game;
using CS2Cheat.Graphics;
using CS2Cheat.Utils;
using SharpDX;
using Keys = Process.NET.Native.Types.Keys;

namespace CS2Cheat.Data.Entity;

public class Player : EntityBase
{
    private Matrix MatrixViewProjection { get; set; }
    public Matrix MatrixViewport { get; private set; }
    public Matrix MatrixViewProjectionViewport { get; private set; }
    private Vector3 ViewOffset { get; set; }
    public Vector3 EyePosition { get; private set; }
    public Vector3 ViewAngles { get; set; }
    public Vector2 AimPunchAngle { get; private set; }
    public Vector3 AimDirection { get; private set; }

    public Vector3 localOrigin { get; private set; }

    public Vector3 EyeDirection { get; private set; }

    public float sens { get; private set; }

    public static int Fov => 90;

    public int FFlags { get; private set; }


    protected override IntPtr ReadControllerBase(GameProcess gameProcess)
    {
        return gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwLocalPlayerController);
    }

    protected override IntPtr ReadAddressBase(GameProcess gameProcess)
    {
        return gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwLocalPlayerPawn);
    }

    public override bool Update(GameProcess gameProcess)
    {
        if (!base.Update(gameProcess)) return false;


        MatrixViewProjection = Matrix.Transpose(gameProcess.ModuleClient.Read<Matrix>(Offsets.dwViewMatrix));
        MatrixViewport = Utility.GetMatrixViewport(gameProcess.WindowRectangleClient.Size);
        MatrixViewProjectionViewport = MatrixViewProjection * MatrixViewport;

        localOrigin = gameProcess.Process.Read<Vector3>(AddressBase + Offsets.m_vOldOrigin);

        ViewOffset = gameProcess.Process.Read<Vector3>(AddressBase + Offsets.m_vecViewOffset);
        EyePosition = Origin + ViewOffset;
        ViewAngles = gameProcess.ModuleClient.Read<Vector3>(Offsets.dwViewAngles);



        IntPtr sensBase = gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwSensitivity);

        sens = gameProcess.Process.Read<float>(sensBase+ Offsets.dwSensitivity_sensitivity);

        AimPunchAngle = gameProcess.Process.Read<Vector2>(AddressBase + Offsets.m_AimPunchAngle);
        FFlags = gameProcess.Process.Read<int>(AddressBase + Offsets.m_fFlags);

        EyeDirection =
            GraphicsMath.GetVectorFromEulerAngles(ViewAngles.X.DegreeToRadian(), ViewAngles.Y.DegreeToRadian());
        AimDirection = GraphicsMath.GetVectorFromEulerAngles
        (
            (ViewAngles.X + AimPunchAngle.X * Offsets.WeaponRecoilScale).DegreeToRadian(),
            (ViewAngles.Y + AimPunchAngle.Y * Offsets.WeaponRecoilScale).DegreeToRadian()
        );

        /*
        for bunnyhop to work correctly you need to write this
        alias hop "+jump;-jump"; bind space hop; bind f24 hop; fps_max 32; fps_max 0;
        */

        if (Keys.Space.IsKeyDown() && (FFlags & 1) > 0)
        {
            Task.Delay(1);
            Utility.PressSpace();
            return true;
        }

        return true;
    }

    public bool IsGrenade()
    {
        return new HashSet<string>
        {
            nameof(WeaponIndexes.Smokegrenade), nameof(WeaponIndexes.Flashbang), nameof(WeaponIndexes.Hegrenade),
            nameof(WeaponIndexes.Molotov)
        }.Contains(CurrentWeaponName);
    }
}