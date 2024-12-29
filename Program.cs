using System.Runtime.InteropServices;
using MagicOrbwalker1.Essentials;
using MagicOrbwalker1.Essentials.API;
using static MagicOrbwalker1.Essentials.Keyboard;

namespace MagicOrbwalker1
{
    internal static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);

        static bool wasKeyPressed = false;

        [STAThread]
        static async Task Main()
        {
            AllocConsole();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Menu //
            Thread LobbyHandle = new Thread(() => CNSL.LobbyShow());
            LobbyHandle.Start();

            // Drawings //


            // Orbwalker Initialization //
            var apiClient = new API();

            while (Values.selectedChamp == "Default"){
                if ((GetAsyncKeyState(Values.orbKey) & 0x8000) != 0) {
                    Keyboard.SendKeyDown(ScanCodeShort.KEY_N);
                    Values.selectedChamp = await apiClient.GetChampionNameAsync();
                }
                Thread.Sleep(500);
            }

            Values.MakeCorrectWindup();
            Values.attackRange = await apiClient.GetAttackRangeAsync();

            // Orbwalker Loop //
            while (true)
            {
                bool isKeyPressed = (GetAsyncKeyState(Values.attackRangeKey) & 0x8000) != 0;
                if ((GetAsyncKeyState(Values.orbKey) & 0x8000) != 0)
                {
                    OrbwalkEnemy().Wait();
                }
                else if (!isKeyPressed && wasKeyPressed && Values.ShowAttackRange)
                {
                    Keyboard.SendKeyDown(ScanCodeShort.KEY_N);
                    Keyboard.SendKeyUp(ScanCodeShort.KEY_N);
                    Keyboard.SendKeyDown(ScanCodeShort.KEY_N);
                }
                else {
                    Thread.Sleep(50);
                }
                wasKeyPressed = isKeyPressed;
            }
        }

        private static void makeoverlay()
        {
            using (var overlay = new Drawings())
            {
                overlay.Run();
            }
        }

        private static async Task OrbwalkEnemy()
        {
            if (Environment.TickCount >= SpecialFunctions.nextAaTick)
            {
                Values.EnemyPosition = ScreenCapture.GetEnemyPosition ();
                if (Values.EnemyPosition != Point.Empty)
                {
                    Values.originalMousePosition = new Point(Cursor.Position.X, Cursor.Position.Y);
                    SpecialFunctions.ClickAt(Values.EnemyPosition);
                    Thread.Sleep(9);
                    SpecialFunctions.MoveCursorTo(Values.originalMousePosition.X, Values.originalMousePosition.Y, 2, 5);

                    var (attackWindup, attackDelay) = await SpecialFunctions.GetAttackParameters();//1350;
                    SpecialFunctions.nextAaTick = attackDelay + Environment.TickCount;
                    int windupDelay = attackWindup + Values.extraWindup;
                    Thread.Sleep(windupDelay);

                    SpecialFunctions.HandleAbilityKeys(Values.EnemyPosition);
                }
                else
                {
                    SpecialFunctions.Click();
                    Thread.Sleep(50);
                }
            }
            else
            {
                SpecialFunctions.Click();
                int nextAttack = SpecialFunctions.nextAaTick;
                if (nextAttack >= (Environment.TickCount + 100)) {
                    Thread.Sleep(100);
                }
                else if (nextAttack > Environment.TickCount) {
                    Thread.Sleep(nextAttack - Environment.TickCount);
                }
            }
        }
    }
}
