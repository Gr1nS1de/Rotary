// Phys2DExtensionMethods.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;

namespace Thinksquirrel.Phys2D
{
    /// <summary>
    /// Defines a set of extension methods used throughout Physics 2D Extensions.
    /// </summary>
    public static class Phys2DExtensionMethods
    {
        /// <summary>
        /// Get the world-space size of a pixel, at the specified world point.
        /// </summary>
        /// <param name="camera">The reference camera to use.</param>
        /// <param name="point">The world-space point to use.</param>
        /// <returns>The world-space size of a pixel at that point.</returns>
        public static float GetPixelWidth(this Camera camera, Vector3 point)
        {
            // Square root of 0.5
            const float k = 0.70710678118f;

            // Get the screen coordinate of some point
            var screenPos = camera.WorldToScreenPoint(point);
            var offset = Vector3.zero;

            // Offset by 1 diagonal pixel
            if (screenPos.x > 0) offset.x = -k;
            else offset.x = k;

            if (screenPos.y > 0) offset.x = -k;
            else offset.y = k;

            // Get the world coordinate once offset.
            offset = camera.ScreenToWorldPoint(screenPos + offset);

            return (camera.transform.InverseTransformPoint(point) - camera.transform.InverseTransformPoint(offset)).magnitude;
        }
        public static Vector2 GetPoint(this Transform t, Vector2 point)
        {
            var mat = Matrix4x4.TRS(t.position, t.rotation, Vector3.one).inverse;
            return mat.MultiplyPoint3x4(point);
        }
        public static Vector2 GetPointNoRotation(this Transform t, Vector2 point)
        {
            var mat = Matrix4x4.TRS(t.position, Quaternion.identity, Vector3.one).inverse;
            return mat.MultiplyPoint3x4(point);
        }
        /// <summary>
        /// Gets the relative point of a world-space position from the current transform.
        /// </summary>
        /// <param name="t">The transform to use for calculation.</param>
        /// <param name="relativePoint">The world space point.</param>
        /// <returns></returns>
        public static Vector2 GetRelativePoint(this Transform t, Vector3 relativePoint)
        {
            var mat = Matrix4x4.TRS(t.position, t.rotation, Vector3.one);
            return mat.MultiplyPoint3x4(relativePoint);
        }
        public static Vector2 GetRelativePointNoRotation(this Transform t, Vector2 relativePoint)
        {
            var mat = Matrix4x4.TRS(t.position, Quaternion.identity, Vector3.one);
            return mat.MultiplyPoint3x4(relativePoint);            
        }
        public static Vector2 GetVector(this Transform t, Vector2 vector)
        {
            var mat = Matrix4x4.TRS(t.position, t.rotation, Vector3.one).inverse;
            return mat.MultiplyVector(vector);
        }
        /// <summary>
        /// Gets the relative direction of a world-space vector from the current transform.
        /// </summary>
        /// <param name="t">The transform to use for calculation.</param>
        /// <param name="relativeVector">The world space vector.</param>
        /// <returns></returns>
        public static Vector2 GetRelativeVector(this Transform t, Vector3 relativeVector)
        {
            var mat = Matrix4x4.TRS(t.position, t.rotation, Vector3.one);
            return mat.MultiplyVector(relativeVector);
        }
    }
}