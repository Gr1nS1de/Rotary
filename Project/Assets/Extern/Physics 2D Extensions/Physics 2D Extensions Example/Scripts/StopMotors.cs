//
// StopMotors.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using Thinksquirrel.Phys2D;

namespace Thinksquirrel.Phys2DExamples
{
    [AddComponentMenu("Physics 2D Example/Stop Motors")]
    [RequireComponent(typeof(Collider2D))]
    public sealed class StopMotors : MonoBehaviour
    {
        [SerializeField] WheelJoint2DExt[] m_WheelJoints;

        void OnTriggerEnter2D(Collider2D other)
        {
            foreach(var wheel in m_WheelJoints)
            {
                if (!wheel)
                    continue;

                if (other == wheel.GetComponent<Collider2D>())
                {
                    wheel.useMotor = false;
                }
            }
        }
    }
}
