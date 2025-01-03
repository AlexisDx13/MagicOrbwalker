﻿namespace MagicOrbwalker1.Essentials
{
    class Values
    {
        // Orbwalker Components //
        public static Keys orbKey = Keys.Space;
        public static Keys attackRangeKey = Keys.M;
        public static Point originalMousePosition;
        public static Point EnemyPosition;

        public static Color enemyPix = Color.FromArgb(52, 3, 0); // Enemy Pixel Normal
        public static Color enemyPix1 = Color.FromArgb(53, 3, 0); // Enemy Pixel Normal

        public static Color enemyPixBS = Color.FromArgb(148, 81, 165); // Enemy Pixel BlackShield (Morgana spell)
        public static Color enemyPixBS1 = Color.FromArgb(82, 40, 90); // Enemy Pixel BlackShield (Morgana spell)
        // Orbwalker Components //

        // Main Hodnoty //
        public static float attackRange = 0.0f;
        public static float attackSpeed = 0.0f;
        public static bool ShowAttackRange = true;

        public static bool DrawingsEnabled = false;

        public static float windup = 0.0f;
        public static int extraWindup = 45;

        public static string selectedChamp = "Default";

        // Main Hodnoty //

        // Champion windup //
        public static float Ashe_wu = 21.93f;
        public static float Caitlyn_wu = 17.708f;
        public static float Corki_wu = 10.00f;
        public static float Draven_wu = 15.614f;
        public static float Ezreal_wu = 18.839f;
        public static float Jinx_wu = 16.875f;
        public static float Kaisa_wu = 16.108f;
        public static float Kalista_wu = 36.000f;
        public static float Kayle_wu = 19.355f;
        public static float Kindred_wu = 17.544f;
        public static float Kogmaw_wu = 16.622f;
        public static float Lucian_wu = 15.00f;
        public static float MissFortune_wu = 14.801f;
        public static float Quinn_wu = 17.544f;
        public static float Samira_wu = 15.00f;
        public static float Senna_wu = 31.25f;
        public static float Sivir_wu = 12.00f;
        public static float Tristana_wu = 14.801f;
        public static float Twitch_wu = 20.192f;
        public static float Varus_wu = 17.544f;
        public static float Vayne_wu = 17.544f;
        public static float Xayah_wu = 17.687f;
        public static float Other_wu = 15.0f;
        // Champion windup //

        public static void MakeCorrectWindup()
        {
            var championWindups = new Dictionary<string, float>
            {
            {"Ashe", Ashe_wu},
            {"Caitlyn", Caitlyn_wu},
            {"Corki", Corki_wu},
            {"Draven", Draven_wu},
            {"Ezreal", Ezreal_wu},
            {"Jinx", Jinx_wu},
            {"Kaisa", Kaisa_wu},
            {"Kalista", Kalista_wu},
            {"Kayle", Kayle_wu},
            {"Kindred", Kindred_wu},
            {"Kogmaw", Kogmaw_wu},
            {"Lucian", Lucian_wu},
            {"MissFortune", MissFortune_wu},
            {"Quinn", Quinn_wu},
            {"Samira", Samira_wu},
            {"Senna", Senna_wu},
            {"Sivir", Sivir_wu},
            {"Tristana", Tristana_wu},
            {"Twitch", Twitch_wu},
            {"Varus", Varus_wu},
            {"Vayne", Vayne_wu},
            {"Xayah", Xayah_wu}
            };

            float championSpecificWindup = championWindups.TryGetValue(selectedChamp, out float specificWindup) ? specificWindup : Other_wu;

            windup = championSpecificWindup / 100;
        }

    }
}
