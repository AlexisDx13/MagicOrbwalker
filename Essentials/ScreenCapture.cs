using Hazdryx.Drawing;
using System.Drawing.Imaging;

namespace MagicOrbwalker1.Essentials
{
    class ScreenCapture
    {
        public static Point GetEnemyPosition()
        {
            Point playerPos = new Point(Cursor.Position.X, Cursor.Position.Y);
            Rectangle rect = CalculateRectangle(playerPos);

            return PixelSearchEnemy(rect, Values.enemyPix, Values.enemyPix1, Values.enemyPixBS, Values.enemyPixBS1, playerPos);
        }
        private static Rectangle CalculateRectangle(Point playerPos)
        {
            Rectangle rect = Rectangle.Empty;

            if (Values.attackRange != -1) {
                // TODO: Use attackRange
                int rangeXL = 525;
                int rangeXR = 395;
                int rangeYT = 480;
                int rangeYB = 320;
                
                rect = new Rectangle(0, 0, 1920, 1080);
            }

            return rect;
        }
        public static Point PixelSearchEnemy(Rectangle rect, Color pixelColor, Color pixelColor1, Color PixelColorBS, Color PixelColorBS1, Point playerPos)
        {
            if (rect.IsEmpty)
                return Point.Empty;

            List<Point> points = PixelSearchEnemies(rect, pixelColor, pixelColor1, PixelColorBS, PixelColorBS1);
            Point closestPoint = Point.Empty;
            int closestDistance = int.MaxValue;

            foreach (Point p in points)
            {
                int distance = SquareDistance(playerPos, p);
                if (distance < closestDistance)
                {
                    closestPoint = p;
                    closestDistance = distance;
                }
            }

            return closestPoint;
        }
        private static int SquareDistance(Point player, Point enemy)
        {
            int dx = player.X - enemy.X;
            int dy = player.Y - enemy.Y;

            if (enemy.Y < player.Y) {
                dx = (int)(dx * 1.5);     // Patch Y (top/bottom)
            }

            return dx * dx + dy * dy;
        }

        public static List<Point> PixelSearchEnemies(Rectangle rect, Color PixelColor, Color PixelColor1, Color PixelColorBS, Color PixelColorBS1)
        {
            int offsetX = 69;
            int offsetY = 133;
            var primaryScreen = Screen.PrimaryScreen;

            if (primaryScreen != null)
            {
                if (primaryScreen.Bounds.Width != 1920 || primaryScreen.Bounds.Height != 1080)
                {
                    double XRatio = primaryScreen.Bounds.Width / 1920.0;
                    double YRatio = primaryScreen.Bounds.Height / 1080.0;
                    rect.X = (int)(rect.X * XRatio);
                    rect.Y = (int)(rect.Y * YRatio);
                    rect.Width = (int)(rect.Width * XRatio);
                    rect.Height = (int)(rect.Height * YRatio);
                    offsetX = (int)(offsetX * XRatio);
                    offsetY = (int)(offsetY * YRatio);
                }
            }

            int searchvalueNormal = PixelColor.ToArgb(); // enemyPix
            int searchvalueNormal1 = PixelColor1.ToArgb(); // enemyPix1
            int searchvalueBS = PixelColorBS.ToArgb(); // enemyPixBS
            int searchvalueBS1 = PixelColorBS1.ToArgb(); // enemyPixBS1

            List<Point> Points = new List<Point>();
            object lockObj = new object();

            Bitmap BMP = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppRgb);
            using (Graphics GFX = Graphics.FromImage(BMP))
            {
                GFX.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }
            using (FastBitmap bitmap = new FastBitmap(BMP))
            {
                Parallel.For(0, bitmap.Length - 1, i =>
                {
                    int currentPixel = bitmap.GetI(i);
                    int nextPixel = bitmap.GetI(i + 1);

                    // Check for normal enemy pixel followed by its pair
                    if (currentPixel == searchvalueNormal && nextPixel == searchvalueNormal1)
                    {
                        AddPointToList(i, bitmap, rect, offsetX, offsetY, lockObj, Points);
                    }

                    // Check for black shield pixel followed by its pair
                    else if (currentPixel == searchvalueBS && nextPixel == searchvalueBS1)
                    {
                        AddPointToList(i, bitmap, rect, offsetX, offsetY, lockObj, Points);
                    }
                });
            }

            return Points;
        }
        private static void AddPointToList(int index, FastBitmap bitmap, Rectangle rect, int offsetX, int offsetY, object lockObj, List<Point> Points)
        {
            int x = index % bitmap.Width;
            int y = index / bitmap.Width;

            lock (lockObj)
            {
                Points.Add(new Point(x + rect.X + offsetX, y + rect.Y + offsetY));
            }
        }
    }
}
