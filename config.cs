using CS2Cheat.Core.Data;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace config
{
    public class neededInfo
    {
        internal static bool isAiming = false;
    }
    public class config
    {

        internal static bool b_m_thirdperson = false;

        internal static int i_m_fov = 90;

        internal static float f_m_vmfov = 60;


        internal static bool masterGlow = false;

        internal static bool b_e_glow = false;
        internal static bool b_t_glow = false;

        internal static uint enemyGlowColor = 0x800000FF;

        internal static uint teamGlowColor = 0x800000FF;




        // enemy esp values

        internal static bool b_e_Box = false;
        internal static bool b_e_Healthbar = false;
        internal static bool b_e_Healthnum = false;
        internal static bool b_e_skeletonESP = false;
        internal static bool b_e_weaponName = false;
        internal static bool b_e_playerName = false;

        // team esp values


        internal static bool b_t_Box = false;
        internal static bool b_t_Healthbar = false;
        internal static bool b_t_Healthnum = false;
        internal static bool b_t_skeletonESP = false;
        internal static bool b_t_weaponName = false;
        internal static bool b_t_playerName = false;

        // aimbot values

        internal static bool bhop = false;

        internal static Keys k_m_aimKey = Keys.XButton1;

        internal static bool aimbot_teamCheck = false;

        internal static float aimbot_FOV = 45f;

        internal static bool masterAimbotBool = false;

        internal static float rcs_x = 1f;

        internal static float rcs_y = 1f;

        internal static bool triggerbotBool = false;

        internal static int triggerDelay = 40;

        internal static System.Windows.Input.Key triggerHoldKey = 0;

        internal static bool hitsound = false;

        internal static bool autoStop = false;
    }

}
