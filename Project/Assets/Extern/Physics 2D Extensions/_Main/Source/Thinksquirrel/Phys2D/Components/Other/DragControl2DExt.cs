//
// DragControl2DExt.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2013-2016 Thinksquirrel Software, LLC
//
using UnityEngine;
using System.Collections.Generic;
using Thinksquirrel.Phys2D.Internal.Joints;
using Thinksquirrel.Phys2D.Internal;

namespace Thinksquirrel.Phys2D
{
    [AddComponentMenu("Physics 2D/Other/Drag Control (P2D Extensions)")]
    public sealed class DragControl2DExt : P2DPhysicsBase
    {
        public delegate InputInfo CustomInputDelegate(DragControl2DExt dragControl);

        public enum InputState
        {
            Started,
            Dragging,
            Ended
        }
        public struct InputInfo
        {
            public Vector2 worldPosition { get; internal set; }
            public float worldDistance { get; internal set; }
            public InputState state { get; internal set; }

            internal Touch? _touch;
            internal bool _beginPhase;
            internal bool _endPhase;
            internal P2DFixedMouseJoint _mouseJoint;

			/*
            public InputInfo(InputState state, Vector2 worldPosition, float worldDistance)
            {
                this.state = state;
                switch(state)
                {
                    case InputState.Started:
                        _beginPhase = true;
                        _endPhase = false;
                        break;
                    case InputState.Dragging:
                        _beginPhase = false;
                        _endPhase = false;
                        break;
                    case InputState.Ended:
                        _beginPhase = false;
                        _endPhase = true;
                        break;
                }

                this.worldPosition = worldPosition;
                this.worldDistance = worldDistance;
            }*/
        }

        const int touchCount = 20;
        List<InputInfo> m_Input = new List<InputInfo>(touchCount + 1);
        List<CustomInputDelegate> m_OnCustomInputList = new List<CustomInputDelegate>();
        List<int> m_ToRemove = new List<int>(touchCount);

        [SerializeField] Camera m_InputCamera;
        [SerializeField] bool m_UseMouseEvents = true;
        [SerializeField] bool m_UseTouchEvents = true;
        [SerializeField] int m_MouseButton;
        [SerializeField] int m_MouseDistance = 10;
        [SerializeField] int m_TouchDistance = 60;
        [SerializeField] float m_MaxForce = 300;
        [SerializeField] LayerMask m_EnabledOnLayers = ~0;

        public Camera inputCamera { get { return m_InputCamera ? m_InputCamera : Camera.main; } set { m_InputCamera = value; } }
        public bool useMouseEvents { get { return m_UseMouseEvents; } set { m_UseMouseEvents = value; } }
        public bool useTouchEvents { get { return m_UseTouchEvents; } set { m_UseTouchEvents = value; } }
        public int mouseButton { get { return m_MouseButton; } set { m_MouseButton = value; } }
        public int mouseDistance { get { return m_MouseDistance; } set { m_MouseDistance = value; } }
        public int touchDistance { get { return m_TouchDistance; } set { m_TouchDistance = value; } }
        public float maxForce { get { return m_MaxForce; } set { m_MaxForce = value;} }
        public LayerMask enabledOnLayers { get { return m_EnabledOnLayers; } set { m_EnabledOnLayers = value; } }

        public event CustomInputDelegate OnCustomInput { add { m_OnCustomInputList.Add(value); } remove { m_OnCustomInputList.Remove(value); } }

        const int MaxQuerySize = 256;
        Collider2D[] m_QueryColliders = new Collider2D[MaxQuerySize];

        protected override sealed void OnEnable()
        {
            base.OnEnable();

            ResetEventData();
        }

        internal override sealed void PhysicsUpdate()
        {
            GetInput();
            CheckMouseJoint();
            SolveJoints();
        }

        public void ResetEventData()
        {
            m_Input.Clear();
            for(int i = 0; i < touchCount + 1; i++)
            {
                var input = new InputInfo();
                input._endPhase = true;
                m_Input.Add(input);
            }
        }

        void GetInput()
        {
            InputInfo t;
            int ind = 0;
            Camera cam = m_InputCamera ? m_InputCamera : Camera.main;

            if (cam)
            {
                // Mouse event
                t = m_Input[ind];

                if (m_UseMouseEvents)
                {
                    if (Input.GetMouseButton(m_MouseButton))
                    {
                        t.worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
                        t.worldDistance = cam.GetPixelWidth(t.worldPosition) * m_MouseDistance;

                        if (t._endPhase)
                        {
                            // Starting a click
                            t._beginPhase = true;
                            t._endPhase = false;
                        }
                        else if (t._beginPhase)
                        {
                            // Holding down a click
                            t._beginPhase = false;
                        }
                    }
                    else
                    {
                        // Released a click
                        t._endPhase = true;
                        t._beginPhase = false;
                    }
                }
                else
                {
                    t._beginPhase = false;
                    t._endPhase = false;
                }

                m_Input[ind] = t;

                // Touch events
                for(int i = 1; i < touchCount + 1; i++)
                {
                    m_ToRemove.Add(i);
                }

                if (m_UseTouchEvents)
                {
                    // Add new touches
                    int tc = Input.touchCount;
                    for(int i = 0; i < tc; ++i)
                    {
                        var touch = Input.GetTouch(i);

                        ind = touch.fingerId + 1;
                        t = m_Input[ind];

                        if (t._endPhase)
                        {
                            // Starting a new touch
                            t._beginPhase = true;
                            t._endPhase = false;
                        }
                        else if (t._beginPhase)
                        {
                            // Holding down a touch
                            t._beginPhase = false;
                        }

                        t._touch = touch;
                        t.worldPosition = cam.ScreenToWorldPoint(touch.position);
                        t.worldDistance = cam.GetPixelWidth(t.worldPosition) * m_TouchDistance;
                        m_Input[ind] = t;

                        m_ToRemove.Remove(ind);
                    }
                }

                // Remove old touches
                int l = m_ToRemove.Count;
                for(int i = 0; i < l; i++)
                {
                    ind = m_ToRemove[i];
                    InputInfo rem = m_Input[ind];

                    rem._endPhase = true;
                    rem._beginPhase = false;

                    m_Input[ind] = rem;
                }

                m_ToRemove.Clear();
            }

            // Custom events
            for(int i = m_Input.Count - 1; i >= touchCount + 1; --i)
            {
                m_Input.RemoveAt(i);
            }

            for (int i = 0; i < m_OnCustomInputList.Count; i++)
            {
                var del = m_OnCustomInputList[i];
                m_Input.Add(del(this));
            }
        }

        Rigidbody2D GetBodyAtTouch(ref InputInfo info)
        {
            // Make a box
            var lowerBound = new Vector2(info.worldPosition.x - info.worldDistance, info.worldPosition.y - info.worldDistance);
            var upperBound = new Vector2(info.worldPosition.x + info.worldDistance, info.worldPosition.y + info.worldDistance);

            Rigidbody2D body = null;

            // Query the world for overlapping shapes
            int queryCount =
                Physics2D.OverlapAreaNonAlloc(
                    lowerBound,
                    upperBound,
                    m_QueryColliders,
                    m_EnabledOnLayers);

            for(int i = 0; i < queryCount; ++i)
            {
                body = m_QueryColliders[i].attachedRigidbody;

                if (body)
                    break;
            }

            return body;
        }

        void CheckMouseJoint()
        {
            int l = m_Input.Count;
            for(int i = 0; i < l; i++)
            {
                InputInfo info = m_Input[i];

                // Press
                if(info._beginPhase)
                {
                    var body = GetBodyAtTouch(ref info);
                    if(body != null)
                    {
                        if (info._mouseJoint == null)
                        {
                            info._mouseJoint = new P2DFixedMouseJoint(body, info.worldPosition);
                            m_Input[i] = info;
                        }
                        if (info._mouseJoint._bodyA != body)
                        {
                            info._mouseJoint._bodyA = body;
                            info._mouseJoint.InitializeLocalAnchor(info.worldPosition);                            
                        }
                        else
                        {
                            info._mouseJoint.worldAnchorB = info.worldPosition;
                        }
                        info._mouseJoint._maxForce = m_MaxForce * body.mass;
                        info._mouseJoint._enabled = true;
                        body.WakeUp();
                    }
                }
                else if (info._endPhase)
                {
                    // Release
                    if (info._mouseJoint != null && info._mouseJoint._enabled)
                    {
                        info._mouseJoint._enabled = false;
                    }
                }
                else if (info._mouseJoint != null && info._mouseJoint._enabled)
                {
                    // Drag
                    info._mouseJoint.worldAnchorB = info.worldPosition;
                    info._mouseJoint._maxForce = m_MaxForce * info._mouseJoint._bodyA.mass;
                }
            }
        }

        void SolveJoints()
        {
            int l = m_Input.Count;
            for(int i = 0; i < l; ++i)
            {
                InputInfo info = m_Input[i];

                if (info._mouseJoint != null && info._mouseJoint._enabled)
                {
                    info._mouseJoint.InitVelocityConstraints();

                    for(int j = 0; j < Physics2D.velocityIterations; ++j)
                    {
                        info._mouseJoint.SolveVelocityConstraints();
                    }
                    for(int j = 0; j < Physics2D.positionIterations; ++j)
                    {
                        info._mouseJoint.SolvePositionConstraints();
                    }
                }
            }
        }

        protected override sealed int executionLevel { get { return 0; } }
    }
}
