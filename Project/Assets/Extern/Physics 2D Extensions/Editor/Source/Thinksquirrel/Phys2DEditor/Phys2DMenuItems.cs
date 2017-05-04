using System.Collections.Generic;
using System.Reflection;
using Thinksquirrel.Phys2D;
using UnityEditor;
using UnityEngine;

namespace Thinksquirrel.Phys2DEditor
{
    static class Phys2DMenuItems
    {
        // This variable controls the location of Physics 2D Extensions menu commands.
        // Do not include the "Physics 2D Extensions" folder in the name.
        // No trailing slashes allowed!
        const string menuToolsLocation = "Tools/Thinksquirrel";

        [MenuItem(menuToolsLocation + "/Product Support", false, 10000)]
        internal static void ProductSupport()
        {
            Application.OpenURL("https://support.thinksquirrel.com");
        }

        [MenuItem(menuToolsLocation + "/Get Update Notifications", false, 10001)]
        internal static void GetUpdateNotifications()
        {
            Application.OpenURL("https://www.thinksquirrel.com/#subscribe");
        }

        // Upgrade all joints to Physics 2D Extensions
        // 1. Find all joints
        // 2. For every joint find all items referencing it
        // 3. Create new joint
        // 4. Convert all references (set each parent object as dirty)
        [MenuItem(menuToolsLocation + "/Physics 2D Extensions/Upgrade Joints in Scene")]
        static void UpgradeJointsInScene()
        {
            if (!EditorUtility.DisplayDialog(
                    "Upgrade Joints to Physics 2D Extensions?",
                    "Are you sure you want to upgrade all joints in this scene? This cannot be undone.", "Upgrade", "Cancel"))
            {
                return;
            }

            var joints = Object.FindObjectsOfType(typeof(Joint2D)) as Joint2D[];

            for (int i = 0; i < joints.Length; ++i)
            {
                var joint = joints[i];
                var spring = joint as SpringJoint2D;
                var distance = joint as DistanceJoint2D;
                var hinge = joint as HingeJoint2D;
                var slider = joint as SliderJoint2D;
                var wheel = joint as WheelJoint2D;

                FieldReferenceInfo[] refs = null;
                Joint2DExt newJoint = null;
                SpringJoint2DExt newSpring;
                DistanceJoint2DExt newDistance;
                HingeJoint2DExt newHinge;
                SliderJoint2DExt newSlider;
                WheelJoint2DExt newWheel;

                if (spring != null)
                {
                    Debug.Log("[Physics 2D Extensions] Upgrading SpringJoint2D", spring.gameObject);

                    newJoint = newSpring = spring.gameObject.AddComponent<SpringJoint2DExt>();
                    newSpring.connectedBody = spring.connectedBody;
                    newSpring.autoConfigureConnectedAnchor = false;
                    newSpring.anchor = spring.anchor;
                    newSpring.connectedAnchor = spring.connectedAnchor;
                    newSpring.autoConfigureDistance = false;
                    newSpring.distance = spring.distance;
                    newSpring.dampingRatio = spring.dampingRatio;
                    newSpring.frequency = spring.frequency;

                    EditorUtility.SetDirty(newJoint);

                    refs = FindObjectsReferencing(spring);
                }
                else if (distance != null)
                {
                    Debug.Log("[Physics 2D Extensions] Upgrading DistanceJoint2D", distance.gameObject);

                    newJoint = newDistance = distance.gameObject.AddComponent<DistanceJoint2DExt>();
                    newDistance.connectedBody = distance.connectedBody;
                    newDistance.autoConfigureConnectedAnchor = false;
                    newDistance.anchor = distance.anchor;
                    newDistance.connectedAnchor = distance.connectedAnchor;
                    newDistance.autoConfigureDistance = false;
                    newDistance.distance = distance.distance;
                    newDistance.maxDistanceOnly = distance.maxDistanceOnly;

                    EditorUtility.SetDirty(newJoint);

                    refs = FindObjectsReferencing(distance);
                }
                else if (hinge != null)
                {
                    Debug.Log("[Physics 2D Extensions] Upgrading HingeJoint2D", hinge.gameObject);

                    newJoint = newHinge = hinge.gameObject.AddComponent<HingeJoint2DExt>();
                    newHinge.connectedBody = hinge.connectedBody;
                    newHinge.autoConfigureConnectedAnchor = false;
                    newHinge.anchor = hinge.anchor;
                    newHinge.connectedAnchor = hinge.connectedAnchor;
                    newHinge.useMotor = hinge.useMotor;
                    newHinge.motor = hinge.motor;
                    newHinge.useLimits = hinge.useLimits;
                    newHinge.limits = hinge.limits;

                    EditorUtility.SetDirty(newJoint);

                    refs = FindObjectsReferencing(hinge);
                }
                else if (slider != null)
                {
                    Debug.Log("[Physics 2D Extensions] Upgrading SliderJoint2D", slider.gameObject);

                    newJoint = newSlider = slider.gameObject.AddComponent<SliderJoint2DExt>();
                    newSlider.connectedBody = slider.connectedBody;
                    newSlider.autoConfigureConnectedAnchor = false;
                    newSlider.anchor = slider.anchor;
                    newSlider.connectedAnchor = slider.connectedAnchor;
                    newSlider.angle = slider.angle;
                    newSlider.useMotor = slider.useMotor;
                    newSlider.motor = slider.motor;
                    newSlider.useLimits = slider.useLimits;
                    newSlider.limits = slider.limits;

                    EditorUtility.SetDirty(newJoint);

                    refs = FindObjectsReferencing(slider);
                }
                else if (wheel != null)
                {
                    Debug.Log("[Physics 2D Extensions] Upgrading WheelJoint2D", wheel.gameObject);

                    newJoint = newWheel = wheel.gameObject.AddComponent<WheelJoint2DExt>();
                    newWheel.connectedBody = wheel.connectedBody;
                    newWheel.autoConfigureConnectedAnchor = false;
                    newWheel.anchor = wheel.anchor;
                    newWheel.connectedAnchor = wheel.connectedAnchor;
                    newWheel.suspension = wheel.suspension;
                    newWheel.useMotor = wheel.useMotor;

                    newWheel.motor = wheel.motor;

                    refs = FindObjectsReferencing(wheel);
                }

                if (refs != null)
                {
                    for (int j = 0; j < refs.Length; ++j)
                    {
                        var reference = refs[j];
                        var obj = reference.obj;
                        var fieldInfo = reference.fieldInfo;

                        try
                        {
                            fieldInfo.SetValue(obj, newJoint);
                            EditorUtility.SetDirty(obj);
                            Debug.Log(string.Format("Upgraded object reference {0} on {1}", fieldInfo.Name, obj.name), obj);
                        }
                        catch
                        {
                            Debug.LogWarning(string.Format("Unable to set field {0} on {1} - this object reference must be changed manually", fieldInfo.Name, obj.name), obj);
                        }
                    }
                }

                Object.DestroyImmediate(joint);
            }
        }

        [MenuItem(menuToolsLocation + "/Physics 2D Extensions/Reference Manual")]
        internal static void OpenReferenceManual()
        {
            Application.OpenURL("http://docs.thinksquirrel.com/p2d/README.txt");
        }

        static FieldReferenceInfo[] FindObjectsReferencing<T>(T component) where T : Component
        {
            var list = new List<FieldReferenceInfo>();
            var objs = Resources.FindObjectsOfTypeAll(typeof (Object)) as Object[];
            if (objs == null || objs.Length == 0) return new FieldReferenceInfo[0];

            for(int i = 0; i < objs.Length; ++i)
            {
                var obj = objs[i];

                var fields =
                    obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                            BindingFlags.Static);

                foreach (FieldInfo fieldInfo in fields)
                {
                    if (FieldReferencesComponent(obj, fieldInfo, component))
                    {
                        list.Add(new FieldReferenceInfo { obj = obj, fieldInfo = fieldInfo } );
                    }
                }
            }

            return list.ToArray();
        }

        static bool FieldReferencesComponent<T>(Object obj, FieldInfo fieldInfo, T component) where T : Component
        {
            if (fieldInfo.FieldType.IsArray)
            {
                var arr = fieldInfo.GetValue(obj) as System.Array;
                if (arr != null)
                {
                    foreach (object elem in arr)
                    {
                        if (elem == null)
                            continue;

                        if (elem.GetType() == component.GetType())
                        {
                            var o = elem as T;
                            if (o == component)
                                return true;
                        }
                    }
                }
            }
            else
            {
                if (fieldInfo.FieldType == component.GetType())
                {
                    var o = fieldInfo.GetValue(obj) as T;
                    if (o == component)
                        return true;
                }
            }
            return false;
        }

        struct FieldReferenceInfo
        {
            public Object obj;
            public FieldInfo fieldInfo;
        }
    }
}
