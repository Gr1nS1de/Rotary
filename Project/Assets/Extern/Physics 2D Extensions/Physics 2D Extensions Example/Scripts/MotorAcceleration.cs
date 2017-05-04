//
// MotorAcceleration.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using System.Collections;
using Thinksquirrel.Phys2D;

namespace Thinksquirre.Phys2DExamples
{
    [AddComponentMenu("Physics 2D Example/Motor Acceleration")]
    public sealed class MotorAcceleration : MonoBehaviour
    {
        [SerializeField] WheelJoint2DExt m_WheelJoint;
        [SerializeField] float m_MotorSpeed = 350.0f;
        [SerializeField] float m_AccelerationRate = 350.0f;

        void FixedUpdate()
        {
            var motor = m_WheelJoint.motor;

            motor.motorSpeed += Time.deltaTime * m_AccelerationRate;
            motor.motorSpeed = Mathf.Min(motor.motorSpeed, m_MotorSpeed);

            m_WheelJoint.motor = motor;
        }
    }
}
