// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Drawing;

using Prowl.Vector;

namespace Prowl.PaperUI;

    /// <summary>
    /// Provides utility methods for theme-related calculations such as
    /// determining text color contrast against backgrounds.
    /// </summary>
    internal static class ThemeUtils
    {
        /// <summary>
        /// Determines if black text should be used against the specified background color
        /// based on WCAG contrast ratio calculations.
        /// </summary>
        /// <param name="pBgColor">The background color as a Vector4 (RGBA) value.</param>
        /// <returns>True if black text provides better contrast, false if white text is preferred.</returns>
        public static bool UseBlackText(Vector4 pBgColor)
        {
            double luminance = GetLuminance(pBgColor);
            double contrastWithBlack = (luminance + 0.05) / 0.05;
            double contrastWithWhite = 1.05 / (luminance + 0.05);

            return contrastWithBlack >= contrastWithWhite;
        }

        /// <summary>
        /// Calculates the relative luminance of a color according to WCAG 2.0 standards.
        /// </summary>
        /// <param name="pColor">The color as a Vector4 (RGBA) value.</param>
        /// <returns>The relative luminance value between 0 (black) and 1 (white).</returns>
        private static double GetLuminance(Vector4 pColor)
        {
            double r = GetLinearChannel(pColor.x);
            double g = GetLinearChannel(pColor.y);
            double b = GetLinearChannel(pColor.z);
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }


        /// <summary>
        /// Converts a Color struct to a vector4 to use for math calculations
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Vector4 ColorToVector4(Color color)
        {
            return new Vector4(color.R/255f, color.G/255f, color.B/255f, color.A/255f);
        }

        /// <summary>
        /// Converts a color channel from sRGB to linear RGB space.
        /// This is necessary for proper luminance calculations.
        /// </summary>
        /// <param name="pChannel">The color channel value (typically between 0.0 and 1.0).</param>
        /// <returns>The linearized channel value.</returns>
        private static double GetLinearChannel(double pChannel)
        {
            return pChannel <= 0.03928 ? pChannel / 12.92 : Math.Pow((pChannel + 0.055) / 1.055, 2.4);
        }

        /// <summary>
        /// A simple lerp function to blend between two colors
        /// </summary>
        /// <param name="colorA"></param>
        /// <param name="colorB"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Color LerpColor(Color colorA, Color colorB, float t)
        {
            t = Math.Clamp(t, 0f, 1f);

            int r = (int)(colorA.R + (colorB.R - colorA.R) * t);
            int g = (int)(colorA.G + (colorB.G - colorA.G) * t);
            int b = (int)(colorA.B + (colorB.B - colorA.B) * t);
            int a = (int)(colorA.A + (colorB.A - colorA.A) * t);

            return Color.FromArgb(a, r, g, b);
        }
    }
