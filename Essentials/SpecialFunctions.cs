using System.Diagnostics;
using System.Runtime.InteropServices;
using static MagicOrbwalker1.Essentials.Keyboard;

namespace MagicOrbwalker1.Essentials
{
    class SpecialFunctions
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys key);

        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        struct KeyScanCode
        {
            public Keys Key;
            public ScanCodeShort ScanCode;

            public KeyScanCode(Keys key, ScanCodeShort scanCode)
            {
                Key = key;
                ScanCode = scanCode;
            }
        }

        public static bool IsTargetProcessFocused(string processName)
        {
            IntPtr activeWindowHandle = GetForegroundWindow();
            GetWindowThreadProcessId(activeWindowHandle, out int activeProcId);

            try
            {
                Process activeProcess = Process.GetProcessById(activeProcId);
                return activeProcess != null && activeProcess.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                Console.WriteLine("League of Legends Game not found!!");
                return false;
            }
        }
        public static void ClickAt(Point location)
        {
            SetCursorPos(location.X, location.Y);
            Keyboard.SendKeyDown(ScanCodeShort.KEY_X);
            Keyboard.SendKeyUp(ScanCodeShort.KEY_X);
            Keyboard.SendKeyDown(ScanCodeShort.KEY_X);  // Patch
            Keyboard.SendKeyUp(ScanCodeShort.KEY_X);    // Patch
        }

        public static void Click()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN);
            mouse_event(MOUSEEVENTF_RIGHTUP);
        }

        public static void HandleAbilityKeys(System.Drawing.Point cursorPosition)
        {
            int spellSleep = 0;
            KeyScanCode[] keysToCheck = {
                new KeyScanCode(Keys.Q, ScanCodeShort.KEY_Q),
                new KeyScanCode(Keys.W, ScanCodeShort.KEY_W),
                new KeyScanCode(Keys.E, ScanCodeShort.KEY_E),
                new KeyScanCode(Keys.R, ScanCodeShort.KEY_R),
                new KeyScanCode(Keys.T, ScanCodeShort.KEY_T),
                new KeyScanCode(Keys.F, ScanCodeShort.KEY_F)
            };

            SetCursorPos(cursorPosition.X, cursorPosition.Y);

            // Simula la pulsación de la tecla
            Keyboard.SendKeyDown(ScanCodeShort.SHIFT);

            foreach (var keyScan in keysToCheck)
            {
                // Verifica si la tecla está presionada
                if ((GetAsyncKeyState(keyScan.Key) & 0x8000) != 0)
                {
                    Keyboard.SendKeyUp(keyScan.ScanCode);

                    Keyboard.SendKeyDown(keyScan.ScanCode);

                    spellSleep += 335;
                }
            }

            Keyboard.SendKeyUp(ScanCodeShort.SHIFT);

            Thread.Sleep(spellSleep == 335 ? 100 : spellSleep);
        }

        public static int nextAaTick;

        public static async Task<(int attackWindup, int attackDelay)> GetAttackParameters()
        {
            var apiClient = new API.API();
            Values.attackSpeed = await apiClient.GetAttackSpeedAsync();

            // Check for invalid attack speed
            if (Values.attackSpeed <= 0)
            {
                Console.WriteLine("Invalid attack speed.");
                return (-1, -1);
            }

            int attackWindup = (int)((1 / Values.attackSpeed * 1000) * Values.windup);
            int attackDelay = (int)(1000.0f / Values.attackSpeed);

            return (attackWindup, attackDelay);
        }
    }
}
