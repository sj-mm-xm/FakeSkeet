using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using System.Diagnostics;
using System.Net;
using Process.NET.Assembly.CallingConventions;
using System.Windows.Media.Animation;
using Application = System.Windows.Application;
using System.Windows.Media;
using MessageBox = System.Windows.MessageBox;
using Color = System.Windows.Media.Color;
using System.Text;

namespace CS2Cheat
{
    public partial class Menu : Window
    {
        private bool isDragging = false;
        private Point startPoint;
        private static IntPtr hookId = IntPtr.Zero;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private static LowLevelKeyboardProc keyboardProc = HookCallback;
        private static bool isHooked = false;

        // Import the SetWindowPos function from user32.dll
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        // Define the constant values for SetWindowPos
        private const int HWND_TOPMOST = -1;
        private const uint SWP_SHOWWINDOW = 0x0040;

        private static bool visible = true;

        private static Window winMenu;
        public Menu()
        {
            InitializeComponent();

            winMenu = this;

            if (!isHooked)

            {

                hookId = SetHook(keyboardProc);

                isHooked = true;

            }
        }
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (ProcessModule curModule = System.Diagnostics.Process.GetCurrentProcess().MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if ((Keys)vkCode == Keys.Insert)
                {
                    if (visible)
                    {
                        visible = false;


                        // Fade Out and Scale Down
                        DoubleAnimation opacityAnimation = new DoubleAnimation
                        {
                            To = 0,
                            Duration = TimeSpan.FromSeconds(0.5)
                        };

                        winMenu.BeginAnimation(Window.OpacityProperty, opacityAnimation);


                        opacityAnimation.Completed += (s, e) =>
                        {
                            // Optionally hide or minimize the window here if needed
                        };
                    }
                    else
                    {
                        visible = true;

                        // Fade In and Scale Up
                        DoubleAnimation opacityAnimation = new DoubleAnimation
                        {
                            To = 1,
                            Duration = TimeSpan.FromSeconds(0.5)
                        };
                       
                        winMenu.BeginAnimation(Window.OpacityProperty, opacityAnimation);
                       

                        winMenu.Topmost = true;
                        winMenu.Activate();

                        opacityAnimation.Completed += (s, e) =>
                        {
                        };
                    }
                }
            }
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }







        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isDragging = true;
                startPoint = e.GetPosition(this);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPoint = e.GetPosition(this);
                double offsetX = currentPoint.X - startPoint.X;
                double offsetY = currentPoint.Y - startPoint.Y;

                this.Left += offsetX;
                this.Top += offsetY;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isDragging = false;
            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            config.config.b_e_Box = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            config.config.b_e_Box = false;
        }

        private void eName_Checked(object sender, RoutedEventArgs e)
        {
            config.config.b_e_playerName = true;
        }

        private void eName_Unchecked(object sender, RoutedEventArgs e)
        {
            config.config.b_e_playerName = false;
        }

        private void eHealthint_Checked(object sender, RoutedEventArgs e)
        {
            config.config.b_e_Healthnum = true;
        }

        private void eHealthint_Unchecked(object sender, RoutedEventArgs e)
        {
            config.config.b_e_Healthnum = false;
        }

        private void tBox_Checked(object sender, RoutedEventArgs e)
        {
            config.config.b_t_Box = true;
        }

        private void tBox_Unchecked(object sender, RoutedEventArgs e)
        {
            config.config.b_t_Box = false;
        }

        private void tName_Checked(object sender, RoutedEventArgs e)
        {
            config.config.b_t_playerName = true;
        }

        private void tName_Unchecked(object sender, RoutedEventArgs e)
        {
            config.config.b_t_playerName = false;
        }

        private void tHealthint_Checked(object sender, RoutedEventArgs e)
        {
            config.config.b_t_Healthnum = true;
        }

        private void tHealthint_Unchecked(object sender, RoutedEventArgs e)
        {
            config.config.b_t_Healthnum = false;
        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                config.config.i_m_fov = (int)sliderFov.Value;
                labelFov.Content = "FOV: " + sliderFov.Value;
            }
            catch
            {

            }


        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            config.config.aimbot_teamCheck = true;

        }

        private void uncheck_aimteamcheck(object sender, RoutedEventArgs e)
        {
            config.config.aimbot_teamCheck = false;

        }

        private void aimFovSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                config.config.aimbot_FOV = (int)aimFovSlider.Value;
                aimFOV.Content = $"FOV: {config.config.aimbot_FOV}";
            }
            catch
            {

            }


        }

        private void masterAim_Checked(object sender, RoutedEventArgs e)
        {
            config.config.masterAimbotBool = true;


        }
        private void masterAim_UnChecked(object sender, RoutedEventArgs e)
        {
            config.config.masterAimbotBool = false;


        }

        private void aimrcs_x_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                config.config.rcs_x = (float)aimRcs_x.Value;

                rcsLabelX.Content = "X RCS: "+config.config.rcs_x;

            }
            catch
            {

            }
        }

        private void aimrcs_y_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                config.config.rcs_y = (float)aimRcs_y.Value;

                rcsLabelY.Content = "Y RCS: " + config.config.rcs_y;

            }
            catch
            {

            }

        }

        private void triggerbotBool_Checked(object sender, RoutedEventArgs e)
        {
            config.config.triggerbotBool = true;

        }
        private void triggerbotBool_UnChecked(object sender, RoutedEventArgs e)
        {
            config.config.triggerbotBool = false;

        }

        private void triggerDelayChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                config.config.triggerDelay = (int)triggerBotDelay.Value;
                tbotDelayLabel.Content = "Triggerbot delay(ms): " + config.config.triggerDelay;
            }
            catch
            {

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            triggerBotGetHotKey.Content = "   ??   ";


            System.Windows.Input.KeyEventHandler keyHandler = null;
            keyHandler = new System.Windows.Input.KeyEventHandler((s, keyArgs) =>
            {
                System.Windows.Input.Key plsSet = 0;
                if(keyArgs.Key.ToString() == "System")
                {
                    triggerBotGetHotKey.Content = "Left Alt";
                    plsSet = keyArgs.Key;

                }
                else
                {


                    triggerBotGetHotKey.Content = keyArgs.Key.ToString();
                    plsSet = keyArgs.Key;


                }
                if (keyArgs.Key.ToString() == "Escape")
                {
                    triggerBotGetHotKey.Content = "   [   .  .  .   ]   ";
                    plsSet = 0;

                }

                config.config.triggerHoldKey = plsSet;
                
                
                this.KeyDown -= keyHandler;
            });

            this.KeyDown += keyHandler;
        }

        private void hitsound_Checked(object sender, RoutedEventArgs e)
        {
            config.config.hitsound = true;

        }
        private void hitsound_UnChecked(object sender, RoutedEventArgs e)
        {
            config.config.hitsound = false;

        }

        private void enemyGlow_Checked(object sender, RoutedEventArgs e)
        {
            config.config.b_e_glow = true;

        }
        private void enemyGlow_Unchecked(object sender, RoutedEventArgs e)
        {
            config.config.b_e_glow = false;

        }

        private void teamGlow_Checked(object sender, RoutedEventArgs e)
        {
            config.config.b_t_glow = true;


        }
        private void teamGlow_Unchecked(object sender, RoutedEventArgs e)
        {
            config.config.b_t_glow = false;


        }

        private void teamGlowColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {

            }
            catch
            {

            }

        }

        private void enemyGlowColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            try
            {

            }
            catch
            {

            }

        }

        private void autoStop_Checked(object sender, RoutedEventArgs e)
        {
            config.config.autoStop = true;


        }
        private void autoStop_UnChecked(object sender, RoutedEventArgs e)
        {
            config.config.autoStop = false;


        }

        // enemy
        private void txt_teamGlow_Copy_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            
            StringBuilder filteredText = new StringBuilder();

            foreach (char c in txt_teamGlow_Copy.Text)
            {
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                {
                    filteredText.Append(c);
                }
            }

            txt_teamGlow_Copy.Text = filteredText.ToString();

            if (txt_teamGlow_Copy.Text.Length > 8)
            {
                txt_teamGlow_Copy.Text = txt_teamGlow_Copy.Text.Substring(0, 8);
            }

            txt_teamGlow_Copy.CaretIndex = txt_teamGlow_Copy.Text.Length;

            if (txt_teamGlow_Copy.Text.Length == 8)
            {
                if (uint.TryParse(txt_teamGlow_Copy.Text, System.Globalization.NumberStyles.HexNumber, null, out uint result))
                {
                    config.config.enemyGlowColor = result;

                    SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(
                        (byte)((config.config.enemyGlowColor >> 24) & 0xFF),
                        (byte)((config.config.enemyGlowColor >> 16) & 0xFF),
                        (byte)((config.config.enemyGlowColor >> 8) & 0xFF),
                        (byte)(config.config.enemyGlowColor & 0xFF)));

                    // Swap R and B values in config.config.enemyGlowColor
                    config.config.enemyGlowColor = (config.config.enemyGlowColor & 0xFF00FF00) | ((config.config.enemyGlowColor & 0x00FF0000) >> 16) | ((config.config.enemyGlowColor & 0x000000FF) << 16);

                    enemyglowcolorlabel.Foreground = brush;
                }
            }

        }

        private void txt_teamGlow_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            StringBuilder filteredText = new StringBuilder();

            foreach (char c in txt_teamGlow.Text)
            {
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                {
                    filteredText.Append(c);
                }
            }

            txt_teamGlow.Text = filteredText.ToString();

            if (txt_teamGlow.Text.Length > 8)
            {
                txt_teamGlow.Text = txt_teamGlow.Text.Substring(0, 8);
            }

            txt_teamGlow.CaretIndex = txt_teamGlow.Text.Length;

            if (txt_teamGlow.Text.Length == 8)
            {
                if (uint.TryParse(txt_teamGlow.Text, System.Globalization.NumberStyles.HexNumber, null, out uint result))
                {
                    config.config.teamGlowColor = result;

                    SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(
                        (byte)((config.config.teamGlowColor >> 24) & 0xFF),
                        (byte)((config.config.teamGlowColor >> 16) & 0xFF),
                        (byte)((config.config.teamGlowColor >> 8) & 0xFF),
                        (byte)(config.config.teamGlowColor & 0xFF)));

                    // Swap R and B values in config.config.enemyGlowColor
                    config.config.teamGlowColor = (config.config.teamGlowColor & 0xFF00FF00) | ((config.config.teamGlowColor & 0x00FF0000) >> 16) | ((config.config.teamGlowColor & 0x000000FF) << 16);

                    teamglowcolorlabel.Foreground = brush;
                }
            }



        }

        private void bhop_Checked(object sender, RoutedEventArgs e)
        {
            config.config.bhop = true;

        }

        private void bhop_UnChecked(object sender, RoutedEventArgs e)
        {
            config.config.bhop = false;

        }
    }
}
