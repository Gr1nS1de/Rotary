//
// P2DBase.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;

namespace Thinksquirrel.Phys2D
{
    public abstract class P2DBase : MonoBehaviour
    {
        Rigidbody2D m_Rigidbody2D;

        public Rigidbody2D cachedRigidbody2D
        {
            get
            {
                if (!m_Rigidbody2D)
                    m_Rigidbody2D = GetComponent<Rigidbody2D>();

                return m_Rigidbody2D;
            }
        }
    }
}
