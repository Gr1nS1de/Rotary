using System;
using UnityEngine;

namespace SplatterSystem {
        
    public static class BitmapDraw {
        private static Color32[] colorsArray;

        /// <summary>
        /// Implementation of Bresenham's circle algorithm.
        /// </summary>
        public static void DrawFilledCircle(Texture2D texture, int x, int y, int radius, Color color) {
            int cx = radius;
            int cy = 0;
            int radiusError = 1 - cx;

            while (cx >= cy) {
                DrawLine(texture, cx + x, cy + y, -cx + x, cy + y, color);
                DrawLine(texture, cy + x, cx + y, -cy + x, cx + y, color);
                DrawLine(texture, -cx + x, -cy + y, cx + x, -cy + y, color);
                DrawLine(texture, -cy + x, -cx + y, cy + x, -cx + y, color);
             
                cy++;

                if (radiusError < 0) {
                    radiusError += 2 * cy + 1;
                } else {
                    cx--;
                    radiusError += 2 * (cy - cx + 1);
                }
            }
        }

        /// <summary>
        /// Fills the given rectangle area with a solid color.
        /// </summary>
        public static void DrawFilledSquare(Texture2D texture, int x, int y, int sideSize, Color color)
        {
            if (colorsArray == null || colorsArray.Length < sideSize * sideSize || colorsArray.Length <= 0 || 
                    colorsArray[0] != color) {
                colorsArray = new Color32[sideSize * sideSize * 4];
                for (int i = 0; i < colorsArray.Length; i++) {
                    colorsArray[i] = color;
                }
            }

            texture.SetPixels32(x, y, sideSize * 2, sideSize * 2, colorsArray);
        }

        public static void DrawLine(Texture2D texture, Vector3 start, Vector3 end, Color color) {
            DrawLine(texture, (int)start.x, (int)start.y, (int)end.x, (int)end.y, color);
        }

        public static void DrawLine(Texture2D texture, Vector2 start, Vector2 end, Color color) {
            DrawLine(texture, (int)start.x, (int)start.y, (int)end.x, (int)end.y, color);
        }

        /// <summary>
        /// Draws a line between two points. Implementation of Bresenham's line algorithm.
        /// </summary>
        public static void DrawLine(Texture2D texture, int x0, int y0, int x1, int y1, Color color) {
            bool isSteep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (isSteep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            int deltax = x1 - x0;
            int deltay = Math.Abs(y1 - y0);

            int error = deltax / 2;
            int ystep;
            int y = y0;

            if (y0 < y1) ystep = 1;
            else ystep = -1;

            for (int x = x0; x < x1; x++)
            {
                if (isSteep) texture.SetPixel(y, x, color);
                else texture.SetPixel(x, y, color);

                error = error - deltay;
                if (error < 0)
                {
                    y = y + ystep;
                    error = error + deltax;
                }
            }
        }


        /// <summary>
        /// Swap two ints by reference.
        /// </summary>
        private static void Swap(ref int x, ref int y) {
            int temp = x;
            x = y;
            y = temp;
        }
    }

}
