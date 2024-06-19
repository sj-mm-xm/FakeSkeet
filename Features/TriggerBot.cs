using CS2Cheat.Data.Game;
using CS2Cheat.Utils;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace CS2Cheat.Features;

public class TriggerBot(GameProcess gameProcess, GameData gameData) : ThreadedServiceBase
{
    protected override string ThreadName => nameof(TriggerBot);

    private GameProcess GameProcess { get; set; } = gameProcess;

    private GameData GameData { get; set; } = gameData;


    public override void Dispose()
    {
        base.Dispose();

        GameData = default;
        GameProcess = default;
    }


    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
    private const int VK_LMENU = 0xA4;
    public static bool IsHotKeyDown()
    {

        try
        {
            if (config.config.triggerHoldKey.ToString() == "System")
            {
                // why is alt called System

                return (GetAsyncKeyState(VK_LMENU) & 0x8000) != 0;


            }

            bool isKeyDown = false;
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                isKeyDown = Keyboard.IsKeyDown(config.config.triggerHoldKey);
            });
            return isKeyDown;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    protected override void FrameAction()
    {
        if (!config.config.triggerbotBool)
            return;


        var entityId = GameProcess.Process.Read<int>(GameProcess.ModuleClient.Read<IntPtr>(Offsets.dwLocalPlayerPawn) + Offsets.m_iIDEntIndex);

        if (!GameProcess.IsValid || !IsHotKeyDown() || entityId < 0)
            return;

        var entityEntry = GameProcess.Process.Read<IntPtr>(GameProcess.ModuleClient.Read<IntPtr>(Offsets.dwEntityList) +
                                                           0x8 * (entityId >> 9) + 0x10);
        var entity = GameProcess.Process.Read<IntPtr>(entityEntry + 120 * (entityId & 0x1FF));
        var entityTeam = GameProcess.Process.Read<int>(entity + Offsets.m_iTeamNum);

        bool shouldTrigger;

        if (config.config.aimbot_teamCheck)
        {
            shouldTrigger = (GameData.Player.Team != entityTeam.ToTeam() || GameData.Player.FFlags == 65664) &&
                            Math.Abs(GameData.Player.Velocity.Z) <= 18f;
        }
        else
        {
            shouldTrigger = (true || GameData.Player.FFlags == 65664) &&
                            Math.Abs(GameData.Player.Velocity.Z) <= 18f;
        }

        if (!shouldTrigger)
            return;

        Thread.Sleep(config.config.triggerDelay);

        Utility.MouseLeftDown();
        Utility.MouseLeftUp();
    }

}