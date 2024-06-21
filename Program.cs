using CS2Cheat.Data.Game;
using CS2Cheat.Features;
using CS2Cheat.Graphics;
using CS2Cheat.Utils;
using SharpDX.Direct3D9;
using System.Windows.Forms;
using static CS2Cheat.Core.User32;
using Application = System.Windows.Application;

namespace CS2Cheat;

public class Program :
    Application,
    IDisposable
{
    private Program()
    {
        Offsets.UpdateOffsets();
        Startup += (_, _) => InitializeComponent();
        Exit += (_, _) => Dispose();
    }

    private GameProcess GameProcess { get; set; } = null!;

    private GameData GameData { get; set; } = null!;

    private WindowOverlay WindowOverlay { get; set; } = null!;

    private Graphics.Graphics Graphics { get; set; } = null!;

    private TriggerBot Trigger { get; set; } = null!;

    private AimBot AimBot { get; set; } = null!;
    private Misc Misc { get; set; } = null!;

    private rcs rcs { get; set; } = null!;

    private bhop bhop { get; set; } = null!;

    private realBhop realBhop { get; set; } = null!;


    private BombTimer BombTimer { get; set; } = null!;

    public void Dispose()
    {
        GameProcess.Dispose();
        GameProcess = default!;

        GameData.Dispose();
        GameData = default!;

        WindowOverlay.Dispose();
        WindowOverlay = default!;

        Graphics.Dispose();
        Graphics = default!;

        Trigger.Dispose();
        Trigger = default!;

        AimBot.Dispose();
        AimBot = default!;

        BombTimer.Dispose();
        BombTimer = default!;
    }

    [STAThread]


    public static void Main()
    {

        Console.WriteLine("[Cheat init]");

        new Program().Run();
    }

    private void InitializeComponent()
    {



        Thread thread = new Thread(() =>
        {
            GameProcess = new GameProcess();
            GameProcess.Start();

            GameData = new GameData(GameProcess);
            GameData.Start();

            WindowOverlay = new WindowOverlay(GameProcess);
            WindowOverlay.Start();

            Graphics = new Graphics.Graphics(GameProcess, GameData, WindowOverlay);
            Graphics.Start();

            Trigger = new TriggerBot(GameProcess, GameData);
            Trigger.Start();

            AimBot = new AimBot(GameProcess, GameData);
            AimBot.Start();

            Misc = new Misc(GameProcess, GameData);
            Misc.Start();

            bhop = new bhop(GameProcess, GameData);
            bhop.Start();

            realBhop = new realBhop(GameProcess, GameData);
            realBhop.Start();
            //rcs = new rcs(GameProcess, GameData);
            //rcs.Start();

            //BombTimer = new BombTimer(Graphics);
            //BombTimer.Start();

            Console.WriteLine("[Classes initialized ...]");

            Console.WriteLine("[Starting menu ...]");
            Menu menu = new Menu();
            menu.ShowDialog();



        });

        thread.SetApartmentState(ApartmentState.STA);

        thread.Start();

        //SetWindowDisplayAffinity(WindowOverlay!.Window.Handle, 0x00000011); //obs bypass
    }
}