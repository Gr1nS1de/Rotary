//
// LineRendererTransforms.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;

namespace Thinksquirrel.Phys2DExamples
{
    [AddComponentMenu("Physics 2D Example/Line Renderer Transforms")]
    [RequireComponent(typeof(LineRenderer))]
    public sealed class LineRendererTransforms : MonoBehaviour
    {
        [SerializeField] Transform[] m_Transforms;
        LineRenderer m_LineRenderer;
        int m_CachedCount;

        void Awake()
        {
            m_LineRenderer = GetComponent<LineRenderer>();
            m_LineRenderer.SetVertexCount(m_Transforms.Length);
        }

        void FixedUpdate()
        {
            if (m_LineRenderer == null)  m_LineRenderer = GetComponent<LineRenderer>();

            if (m_CachedCount != m_Transforms.Length)
            {
                m_CachedCount = m_Transforms.Length;
                m_LineRenderer.SetVertexCount(m_CachedCount);
            }

            m_LineRenderer.useWorldSpace = true;

            for(int i = 0; i < m_CachedCount; ++i)
                m_LineRenderer.SetPosition(i, m_Transforms[i] ? m_Transforms[i].position : Vector3.zero);

        }
    }
}
