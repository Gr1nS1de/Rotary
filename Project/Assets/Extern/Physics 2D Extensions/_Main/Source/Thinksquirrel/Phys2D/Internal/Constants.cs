//
// Constants.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;

namespace Thinksquirrel.Phys2D.Internal
{
    internal static class Constants
    {
        public const float epsilon = 1.192092896e-07f;
        
        public static float velocityThreshold
        {
            get
            {
                return Physics2D.velocityThreshold;
            }
        }
        
        public static float maxLinearCorrection
        {
            get
            {
                return Physics2D.maxLinearCorrection;
            }
        }
        
        public static float maxAngularCorrection
        {
            get
            {
                return Physics2D.maxAngularCorrection * Mathf.Deg2Rad;
            }
        }
        
        public static float maxTranslationSpeed
        {
            get
            {
                return Physics2D.maxTranslationSpeed;
            }
        }
        
        public static float maxRotationSpeed
        {
            get
            {
                return Physics2D.maxRotationSpeed * Mathf.Deg2Rad;
            }
        }
        
        public static float linearSleepTolerance
        {
            get
            {
                return Physics2D.linearSleepTolerance;
            }
        }
        
        public static float angularSleepTolerance
        {
            get
            {
                return Physics2D.angularSleepTolerance * Mathf.Deg2Rad;
            }
        }
        
        public static float linearSlop
        {
            get
            {
                return 0.0025f;
            }
        }
        
        public static float angularSlop
        {
            get
            {
                return 2.0f * Mathf.Deg2Rad;
            }
        }
    }
}
