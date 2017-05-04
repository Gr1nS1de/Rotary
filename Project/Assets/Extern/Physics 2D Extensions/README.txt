##########################################################################
Physics 2D Extensions
##########################################################################

Thank you for purchasing Physics 2D Extensions! This asset contains a set of add-ons for Unity's 2D physics engine and requires Unity 4.3 or above to function correctly.

==========================================================================
Installation
==========================================================================

Importing the Package:
    Before installing Physics 2D Extensions, make sure that there are no compiler errors in your project. Extract the package into your assets folder - make sure to include all files.

Installing Physics 2D Extensions:
    Once the package has been extracted, a window will appear. Follow the instructions in the install window to complete the installation. If you accidentially close the window or do not see an installation window, please go to Window > Physics 2D Extensions > Install Physics 2D Extensions.

Registration:
    You can optionally register your copy of Physics 2D Extensions. This ensures full product support - you can also sign up for an email notification whenever a new update is released.
    You can register at:
        https://thinksquirrel.com/product-registration
    See the "Product Support" section below for more information.

Uninstalling Physics 2D Extensions:
    Physics 2D Extensions should always be uninstalled prior to upgrading. To uninstall, remove the following folders:
    C# Installation folder:
        Assets/Physics 2D Extensions
    JavaScript installation folders:
        Assets/Physics 2D Extensions
        Assets/Plugins/Physics 2D Extensions

==========================================================================
Getting Started
==========================================================================

There are a number of new components that can be added. You can find all of them under Component > Physics 2D > Extended Joints, Component > Physics 2D > Controllers, and Component > Physics 2D > Other.

Joints work like Unity's physics joints - they provide a constraint of motion between two rigidbodies. These components should be placed on one of the two rigidbodies they affect. Joints can also be fixed to the world by not providing a second body. This is different from Unity's 2D physics implementation (which sets the world anchor to 0,0) and more similar to Unity's 3D physics implementation (which sets the world anchor to the joint's current position).

Controllers apply a set of constraints to an entire group of rigidbodies, and only affect a certain area. Controllers can be added to objects with or without rigidbodies. They are used to represent forces like wind and buoyancy.

The controller filter is a component that allows more fine-grained control over whether a body will be affected by a controller. This component requires a Rigidbody2D to be present. For more information, check out the "Controller Filter" topic.

Finally, a drag control component is included, for dragging rigidbodies around with mouse or touch input.

An example scene can be found in the "Assets/Physics 2D Extensions/P2D Example Project/Unity##" folder.

All API classes are in the following namespace:

    Thinksquirrel.Phys2D

==========================================================================
Component Reference - Joints
==========================================================================

--------------------------------------------------------------------------
All Joints - These properties are shared with all joints.
--------------------------------------------------------------------------
Note:
    There is no way to include the "Collide Connected" toggle in Unity 4.3 - all joints will collide by default. Using layer collision ("Collide Connected Layers") will work around the issue.

Properties:
    Collide Connected Layers - [Unity 4.3 only] If enabled, the layers associated with the bodies attached to this joint will collide.
    Collide Connected - [Post-Unity 4.3 only] If enabled, the bodies attached to this joint will collide.
    Connected Rigid Body - The other Rigidbody2D object that the one with the joint is connected to. If this is null then the other end of the joint will be fixed at a point in space.
    Break Force (only some joints) - The force that needs to be applied for this joint to break.
    Break Torque (only some joints) - The torque that needs to be applied for this joint to break.

API:
    bool collideConnectedLayers { get; set; } - [Unity 4.3 only] See above.
    bool collideConnected { get; set; } - [Post-Unity 4.3 only] See above.
    float connectedBody { get; set; } - See above.
    float breakForce { get; set; } - See above.
    float breakTorque { get; set; } - See above.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Anchored Joints - These properties are shared with all anchored joints.
--------------------------------------------------------------------------
Properties:
    Anchor - Coordinate in local space where the end point of the joint is attached.
    Connected Anchor - Coordinate in the other object's local space where its end of the joint is attached.
    Auto Configure Connected Anchor - If enabled, automatically set the connected anchor.

API:
    Vector2 anchor { get; set; } - See above.
    Vector2 connectedAnchor { get; set; } - See above.
    bool autoConfigureConnectedAnchor { get; set; } - See above.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Angle Joint - Constrains the angle between two rigidbodies.
--------------------------------------------------------------------------
Properties:
    Target Angle - The target angle for the joint, in world space.
    Bias Factor - A 0-1 value that determines how fast the joint moves to the target angle.
    Softness - The overall softness of the joint (how fast the joint slows down).
    Max Impulse - The maximum impulse that can be applied to the joint in order to reach the target angle.

API:
    float targetAngle { get; set; } - See above.
    float biasFactor { get; set; } - See above.
    float softness { get; set; } - See above.
    float maxImpulse { get; set; } - See above.
    float GetReactionTorque(float timeStep); - Gets the reaction torque of the joint given the specified timestep.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Distance Joint - Connects two rigidbodies with an enforced maximum (and optionally minimum) distance.
--------------------------------------------------------------------------
Properties:
    Distance - The distance separating the two ends of the joint.
    Auto Configure Distance - If enabled, automatically set the distance.
    Max Distance Only - Whether to maintain a maximum distance only or not. If not then the absolute distance will be maintained instead.

API:
    float distance { get; set; } - See above.
    bool autoConfigureDistance { get; set; } - See above.
    bool maxDistanceOnly { get; set; } - See above.
    Vector2 GetReactionForce(float timeStep); - Gets the reaction force of the joint given the specified timestep.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Friction Joint - Provides 2D linear and angular friction (useful for top-down games).
--------------------------------------------------------------------------
Properties:
    Max Force - The maximum amount of linear force to apply.
    Max Torque - The maximum amount of rotational torque to apply.

API:
    float maxForce { get; set; } - See above.
    float maxTorque { get; set; } - See above.
    Vector2 GetReactionForce(float timeStep); - Gets the reaction force of the joint given the specified timestep.
    float GetReactionTorque(float timeStep); - Gets the reaction torque of the joint given the specified timestep.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Gear Joint - Connects two hinge joints, or one hinge joint and one slider joint with a gear ratio. It is used to simulate the motion between gears.
--------------------------------------------------------------------------
Note:
    The gear joint is unique in that it connects two joints instead of two rigidbodies. It cannot be anchored to the world (fixed) like other joints.

Properties:
    Local Joint - The first hinge or slider joint to connect (must be on the same game object).
    Connected Joint - The second hinge or slider joint to connect.
    Gear Ratio - The gear ratio (R = wA/wB = tB/tA, where w = angular velocity or t = number of teeth) between the two gears.

API:
    HingeOrSliderJoint2DExt localJoint { get; set; } - See above.
    HingeOrSliderJoint2DExt connectedJoint { get; set; } - See above.
    float gearRatio { get; set; } - See above.
    Vector2 GetReactionForce(float timeStep); - Gets the reaction force of the joint given the specified timestep.
    float GetReactionTorque(float timeStep); - Gets the reaction torque of the joint given the specified timestep.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Hinge Joint - Allows a rigidbody to rotate around a point in space or a point on another object.
--------------------------------------------------------------------------
Properties:
    Use Motor - Should the joint be rotated automatically by a motor torque?
    Motor - Parameters for the motor force applied to the joint.
        Motor Speed - Target motor speed (degrees/sec).
        Maximum Motor Force - The maximum torque the motor can apply while attempting to reach the target speed.
    Use Limits - Should limits be placed on the range of rotation?
    Angle Limits - Limit of angular rotation on the joint.
        Lower Angle - Lower angular limit of rotation.
        Upper Angle - Upper angular limit of rotation.

API:
    bool useMotor { get; set; } - See above.
    JointMotor2D motor { get; set; } - See above.
        float motorSpeed { get; set; } - See above.
        float maxMotorTorque { get; set; } - See above.
    bool useLimits { get; set; } - See above.
    JointAngleLimits2D limits { get; set; } - See above.
        float min { get; set; } - See above.
        float max { get; set; } - See above.
    float jointAngle { get; } - The current joint angle with respect to the reference angle.
    float jointSpeed { get; } - The current joint speed.
    JointLimitState2D limitState { get; } - Gets the state of the joint limit.
    float referenceAngle { get; } - The angle referenced between the two bodies used as the constraint for the joint.
    float GetMotorTorque(float timeStep); - Gets the motor torque of the joint given the specified timestep.
    Vector2 GetReactionForce(float timeStep); - Gets the reaction force of the joint given the specified timestep.
    float GetReactionTorque(float timeStep); - Gets the reaction torque of the joint given the specified timestep.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Pulley Joint - Connects two rigidbodies and two anchors to form a pulley.
--------------------------------------------------------------------------
Note:
    The pulley joint must have two valid ground anchors, and cannot be anchored to the world (fixed) like other joints.

Properties:
    Ground Anchor A - A transform representing an anchor that is connected to the attached rigidbody.
    Ground Anchor B - A transform representing an anchor that is connected to the connected rigidbody.
    Ratio - The ratio between the two ends of the pulley.

API:
    Transform groundAnchorA { get; set; } - See above.
    Transform groundAnchorB { get; set; } - See above.
    float ratio { get; set; } - See above.
    Vector2 GetReactionForce(float timeStep); - Gets the reaction force of the joint given the specified timestep.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Slider Joint - Joint that restricts the motion of a rigidbody to a single line.
--------------------------------------------------------------------------
Properties:
    Angle - The angle of the line in space.
    Use Motor - Should a motor force be applied automatically to the rigidbody?
    Motor - Parameters for a motor force that is applied automatically to the rigidbody along the line.
        Motor Speed - Target motor speed.
        Maximum Motor Force - The maximum force the motor can apply while attempting to reach the target speed.
    Use Limits - Should motion limits be used?
    Translation Limits - Restrictions on how far the joint can slide in each direction along the line.
        Lower Translation - Maximum distance the rigidbody can move from the joint's anchor.
        Upper Translation - Minimum distance the rigidbody can move from the joint's anchor.

API:
    float angle { get; set; } - See above.
    bool useMotor { get; set; } - See above.
    JointMotor2D motor { get; set; } - See above.
        float motorSpeed { get; set; } - See above.
        float maxMotorTorque { get; set; } - See above.
    bool useLimits { get; set; } - See above.
    JointTranslationLimits2D limits { get; set; } - See above.
        float min { get; set; } - See above.
        float max { get; set; } - See above.
    float jointTranslation { get; } - The current joint translation.
    float jointSpeed { get; } - The current joint speed.
    JointLimitState2D limitState { get; } - Gets the state of the joint limit.
    float referenceAngle { get; } - The angle referenced between the two bodies used as the constraint for the joint.
    float GetMotorForce(float timeStep); - Gets the motor force of the joint given the specified timestep.
    Vector2 GetReactionForce(float timeStep); - Gets the reaction force of the joint given the specified timestep.
    float GetReactionTorque(float timeStep); - Gets the reaction torque of the joint given the specified timestep.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Spring Joint - Joint that attempts to keep two rigidbodies a set distance apart by applying a force between them.
--------------------------------------------------------------------------
Properties:
    Distance - The distance separating the two ends of the joint.
    Auto Configure Distance - If enabled, automatically set the distance.
    Damping Ratio - The amount by which the spring force is reduced in proportion to the movement speed.
    Frequency - The frequency at which the spring oscillates around the distance between the objects.

API:
    float distance { get; set; } - See above.
    bool autoConfigureDistance { get; set; } - See above.
    float dampingRatio { get; set; } - See above.
    float frequency { get; set; } - See above.
    Vector2 GetReactionForce(float timeStep); - Gets the reaction force of the joint given the specified timestep.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Weld Joint - Connects two rigidbodies with a weld between them. This joint is only stable with objects that are close together, and provides an easy way to "stick" one object to another.
--------------------------------------------------------------------------
Properties:
    Damping Ratio - The amount by which the weld joint's force is reduced in proportion to the movement speed. Only has effect if the frequency is greater than 0.
    Frequency - The frequency at which the weld joint oscillates. A frequency of 0 will keep the joint rigid.

API:
    float dampingRatio { get; set; } - See above.
    float frequency { get; set; } - See above.
    float referenceAngle { get; } - The angle referenced between the two bodies used as the constraint for the joint.
    Vector2 GetReactionForce(float timeStep); - Gets the reaction force of the joint given the specified timestep.
    float GetReactionTorque(float timeStep); - Gets the reaction torque of the joint given the specified timestep.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Wheel Joint - Provides both rotation and suspension needed to produce a physically accurate wheel in 2D space.
--------------------------------------------------------------------------
Properties:
    Suspension
        Angle - The world suspension angle for the wheel.
        Damping Ratio - The degree to which spring oscillation is suppressed.
        Frequency - The frequency at which the suspension spring oscillates while the rigidbodies are approaching the desired separation distance (cycles per sec).
    Use Motor - Should the wheel motor be enabled?
    Motor
        Motor Speed - Target motor speed (degrees/sec).
        Maximum Motor Force - The maximum torque the motor can apply while attempting to reach the target speed.

API:
    JointSuspension2D suspension { get; set; } - See above.
        float angle { get; set; } - See above.
        float dampingRatio { get; set; } - See above.
        float frequency { get; set; } - See above.
    bool useMotor { get; set; } - See above.
    JointMotor2D motor { get; set; } - See above.
        float motorSpeed { get; set; } - See above.
        float maxMotorTorque { get; set; } - See above.
    float jointTranslation { get; } - The current joint translation.
    float jointSpeed { get; } - The current joint speed.
    float GetMotorTorque(float timeStep); - Gets the motor torque of the joint given the specified timestep.
    Vector2 GetReactionForce(float timeStep); - Gets the reaction force of the joint given the specified timestep.
    float GetReactionTorque(float timeStep); - Gets the reaction torque of the joint given the specified timestep.
--------------------------------------------------------------------------

==========================================================================
Component Reference - Controllers
==========================================================================

--------------------------------------------------------------------------
All Controllers - These properties are shared with all controllers.
--------------------------------------------------------------------------
Note:
    Controllers will only affect bodies with a Collider2D component attached, unless the controller's dimensions are set to (0, 0). Controllers have a limit of 256 Collider2D components (and up to 256 Rigidbody2D components) that can be affected at any given frame. This is for performance reasons.

Properties:
    Rigidbodies - The list of rigidbodies that can be affected by this controller.
    Enabled on Layers - A layer mask that controls which layers a controller will affect.
    Offset - The local space position of the controller's influence area.
    Dimensions - The local space size of the controller's influence area.

API:
    LayerMask enabledOnLayers { get; set; } - See above.
    Vector2 offset { get; set; } - See above.
    Vector2 dimensions { get; set; } - See above.

    Rigidbody2D[] GetRigidbodies() { get; } - Returns the array of rigidbodies that this controller will affect. Note that this method will allocate a new array, so it should not be called every frame.
    void AddRigidbody(Rigidbody2D body); - Adds a body to the controller's list, allowing it to be affected by the controller.
    bool RemoveRigidbody(Rigidbody2D body); - Removes a body from the controller's list. Returns false if the body cannot be successfully removed or was not found in the controller's list.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Buoyancy Controller - Provides buoyancy simulation, with the option to provide a direction of flow.
--------------------------------------------------------------------------
Properties:
    Velocity - The velocity of flow for the buoyancy controller. This can be used to simulate a liquid's flow.
    Density - The density of the buoyancy area. This will affect how much and how strongly other objects float.
    Linear Drag Coefficient - A multiplier applied to linear drag in the buoyancy equation. Higher values will slow down the linear movement of objects in the buoyancy area.
    Angular Drag Coefficient - A multiplier added to the angular drag in the buoyancy equation. Higher values will slow down the angular movement of objects in the buoyancy area.

API:
    Vector2 velocity { get; set; } - See above.
    float density { get; set; } - See above.
    float linearDragCoefficient { get; set; } - See above.
    float angularDragCoefficient { get; set; } - See above.
    void RecalculateMassData(Rigidbody2D body); - Recalculates the mass data for a rigidbody in the controller's rigidbody list. Please see above for more information.
    void RecalculateMassDataAll(); - Recalculates the mass data for all rigidbodies in the controller's rigidbody list. Please see above for more information.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Gravity Controller - Provides point force (gravity) simulation.
--------------------------------------------------------------------------
Properties:
    Gravity Type - Controls how gravitational falloff is calculated. Can be either linear distance or based on the distance squared (the latter is more realistic).
    Strength - A multiplier that controls the strength of gravitational forces.
    Min Radius - The minimum radius from a body's position at which gravitational forces are present.
    Max Radius - The maximum radius from a body's position at which gravitational forces are present.
    Points - A set of points, or singularities that exert gravitational forces on objects. These points are not influenced by other rigidbodies or points.

API:
    GravityType gravityType { get; set; } - See above.
    float strength { get; set; } - See above.
    minRadius { get; set; } - See above.
    float maxRadius { get; set; } - See above.
    List<Vector2> points { get; } - See above.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Velocity Limit Controller - Limits the linear and angular velocity of rigidbodies within its area.
--------------------------------------------------------------------------
Properties:
    Limit Linear Velocity - If true, linear velocity will be limited.
    Limit Angular Velocity - If true, angular velocity will be limited.
    Max Linear Velocity - The maximum linear velocity of affected rigidbodies.
    Max Angular Velocity - The maximum angular velocity of affected rigidbodies.

API:
    bool limitLinearVelocity { get; set; } - See above.
    bool limitAngularVelocity { get; set; } - See above.
    float maxLinearVelocity { get; set; } - See above.
    float maxAngularVelocity { get; set; } - See above.
--------------------------------------------------------------------------

--------------------------------------------------------------------------
Wind Controller - Provides a basic wind simulation, with support for curves.
--------------------------------------------------------------------------
Properties:
    Wind Type - Controls the type of wind force. This can be constant, or based on a curve.
    Force Type - Controls how the wind force is exterted on objects. Wind can emit from either a point, or along a direction.
    Position - Controls the wind force's position, in local space.
    Direction - Controls the wind force's direction, in local space. Point forces are not affected by this.
    Ignore Position - If enabled, the wind force will ignore the body's position for directional force calculation. When disabled, objects "behind" the controller will not be affected by wind. Point forces are not affected by this.
    Strength - The strength of the wind force.
    Strength Curve - Controls how the wind force's strength changes over time. This only takes effect if the Wind Type is set to Curve.
    Decay Mode - Controls how the wind force's strength decays over distance. This can be constant (None), instant (Step), linear, based on the inverse square distance, or based on a custom curve.
    Decay Curve - Controls how the wind force's strength changes over distance. This only takes effect if the Wind Type is set to Curve.
    Decay Start - All rigidbodies less than this distance will be affected by the full wind force.
    Decay End - All rigidbodies greater than this distance will not be affected by the full wind force.
    Divergence - Controls the amount of directional turbulence. Can be a range from 0 to 1.
    Variation - Controls the amount of strength variation.
    Time Scale - Determines the time scale of all curves. This is a quick way to slow down or speed up changes in the wind.

API:
    WindType windType { get; set; } - See above.
    ForceType forceType { get; set; } - See above.
    Vector2 position { get; set; } - See above.
    Vector2 direction { get; set; } - See above.
    bool ignorePosition { get; set; } - See above.
    float strength { get; set; } - See above.
    AnimationCurve strengthCurve { get; set; } - See above.
    DecayMode decayMode { get; set; } - See above.
    AnimationCurve decayCurve { get; set; } - See above.
    float decayStart { get; set; } - See above.
    float decayEnd { get; set; } - See above.
    float divergence { get; set; } - See above.
    float variation { get; set; } - See above.
    float timeScale { get; set; } - See above.
--------------------------------------------------------------------------

==========================================================================
Component Reference - Controller Filter
==========================================================================

--------------------------------------------------------------------------
Controller Filter - Allows for more fine-grained filtering of rigidbodies with different controllers.
--------------------------------------------------------------------------
Note:
    The controller filter filters rigidbodies in this order:
        * If the body is destroyed, it will not be affected by a controller
        * If the body's game object is not active, it will not be affected by a controller
        * If a controller filter component not present or not enabled:
            - The "Enabled On Layers" property is checked
        * If a controller filter is present:
            - The controller filter's flags are checked
            - If the controller is not ignored, the "Enabled On Layers" property is checked

Properties:
    Ignored controllers
        A set of flags representing the controllers that should ignore this rigidbody.

API:
    public void IgnoreControllerType(ControllerType controllerType); - Ignores the controller type. All controllers of this type will no longer affect this rigidbody.
    public void IgnoreControllerType(Controller2DExt controller); - Ignores the controller type. This method accepts a controller object as a parameter.
    public void RestoreControllerType(ControllerType controllerType); - Restores the controller type. All controllers of this type will now affect this rigidbody.
    public void RestoreControllerType(Controller2DExt controller); - Restores the controller type. This method accepts a controller object as a parameter.
    public bool IsControllerTypeIgnored(Controller2DExt controller); - Returns true if this controller type is currently ignored.
    public bool IsControllerTypeIgnored(ControllerType controllerType); Returns true if this controller type is currently ignored. This method accepts a controller object as a parameter.
--------------------------------------------------------------------------

==========================================================================
Component Reference - Drag Control
==========================================================================

--------------------------------------------------------------------------
Drag Control - Provides an easy way to pick up and drag objects. Works with mouse, touch, or custom input events and can filter by layer.
--------------------------------------------------------------------------
Properties:
    Input Camera - The camera to use to calculate input position. This defaults to the main camera.
    Use Mouse Events - If true, listen to mouse events.
    Use Touch Events - If true, listen to touch (and multitouch) events.
    Mouse Button - The mouse button to use for picking up objects.
    Mouse Distance - The margin of error (in pixels) to use when picking up objects with the mouse.
    Touch Distance - The margin of error (in pixels) to use when picking up objects using touch.
    Max Force - The maximum amount of force that can be exerted on the object while dragging.
    Enabled On Layers - A layer mask that controls which layers can be dragged.

API:
    Camera inputCamera { get; set; } - See above.
    bool useMouseEvents { get; set; } - See above.
    bool useTouchEvents { get; set; } - See above.
    int mouseButton { get; set; } - See above.
    int mouseDistance { get; set; } - See above.
    int touchDistance { get; set; } - See above.
    float maxForce { get; set; } - See above.
    LayerMask enabledOnLayers { get; set; } - See above.
    void ResetEventData() - Discards all current input data and resets everything to a default, unpressed state.
    event CustomInputDelegate OnCustomInput { add; remove; } - This event fires every physics frame to determine the state of any custom input devices, after mouse and touch input has been processed. Subscribe to this event in order to control dragging with a custom input device, such as a keyboard, controller, existing event system, etc.
        Arguments:
            DragControl2DExt - The DragControl2DExt component that called the custom input event.
        Returns:
            InputInfo - A custom input struct describing the state of the input device.
                Vector2 worldPosition { get; } - The world position of the input device.
                Vector2 worldDistance { get; } - The maximum margin of error for the input device, in world space.
                InputState state { get; } - The current state of the input device (Started, Dragging, or Ended).

--------------------------------------------------------------------------

==========================================================================
Product Support
==========================================================================

You can register your product for access to additional support, product update notifications and beta releases.

To register, please visit the following link:
https://thinksquirrel.com/product-registration

Also make sure to check out the Thinksquirrel support forum:
https://thinksquirrel.com/forum

==========================================================================
Acknowledgments/Open Source Licenses
==========================================================================

Joint math is derived from the open-source Box2D project.
http://box2d.org/

Copyright (c) 2006-2013 Erin Catto http://www.gphysics.com

This software is provided 'as-is', without any express or implied
warranty.  In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

1. The origin of this software must not be misrepresented; you must not
claim that you wrote the original software. If you use this software
in a product, an acknowledgment in the product documentation would be
appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be
misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.

----------------------

Controller math is derived from the open-source Farseer Physics library.
http://farseerphysics.codeplex.com/

Microsoft Permissive License (Ms-PL) v1.1
Microsoft Permissive License (Ms-PL)

This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.

1. Definitions

The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.

A "contribution" is the original software, or any additions or changes to the software.

A "contributor" is any person that distributes its contribution under this license.

"Licensed patents" are a contributor's patent claims that read directly on its contribution.

2. Grant of Rights

(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.

(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations

(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.

(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.

(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.

(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.

(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.

----------------------
Convex decomposition code is derived from jBox2D.
http://http://www.jbox2d.org

Modified for Unity by Josh Montoute 2013-2016

C# Version Ported by Matt Bettcher and Ian Qvist 2009-2010

Original C++ Version Copyright (c) 2007 Eric Jordan

This software is provided 'as-is', without any express or implied
warranty.  In no event will the authors be held liable for any damages
arising from the use of this software.
Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:
1. The origin of this software must not be misrepresented; you must not
claim that you wrote the original software. If you use this software
in a product, an acknowledgment in the product documentation would be
appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be
misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.


----------------------

Editor scripts use the DotNetZip library for downloading and extracting documentation.
http://dotnetzip.codeplex.com/

Software Licenses that apply to DotNetZip
This software, the DotNetZip library and tools is provided for your use under several licenses.  One license applies to DotNetZip, and several other licenses apply to work that DotNetZip derives from. To use the software, you must accept the licenses. If you do not accept the licenses, do not use the software.
The following license, the Microsoft Public License (Ms-PL), applies to the original intellectual property in DotNetZip: 
1. Definitions
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.
A "contribution" is the original software, or any additions or changes to the software.
A "contributor" is any person that distributes its contribution under this license.
"Licensed patents" are a contributor's patent claims that read directly on its contribution.
2. Grant of Rights
(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
3. Conditions and Limitations
(A) No Trademark License - This license does not grant you rights to use any contributor’s name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement. 


The managed ZLIB code included in Ionic.Zlib.dll and Ionic.Zip.dll is derived from the jzlib, which is Copyright (c) 2000,2001,2002,2003 ymnk, JCraft, Inc., and is licensed under the following terms: 
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
1.	Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
2.	Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
3.	The names of the authors may not be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL JCRAFT, INC. OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


The jzlib library, itself, is based on the C-language ZLIB library, v1.1.3.  The following notice and license applies to zlib:
ZLIB is Copyright (C) 1995-2004 Jean-loup Gailly and Mark Adler
The ZLIB software is provided 'as-is', without any express or implied warranty.  In no event will the authors be held liable for any damages arising from the use of this software.
Permission is granted to anyone to use this software for any purpose, including commercial applications, and to alter it and redistribute it freely, subject to the following restrictions:
1.	The origin of this software must not be misrepresented; you must not claim that you wrote the original software. If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required. 
2.	Altered source versions must be plainly marked as such, and must not be misrepresented as being the original software. 
3.	This notice may not be removed or altered from any source distribution.
  Jean-loup Gailly      jloup@gzip.org
  Mark Adler      madler@alumni.caltech.edu


The managed BZIP2 code included in Ionic.BZip2.dll and Ionic.Zip.dll is modified code, based on the bzip2 code in the Apache commons compress library.
The original BZip2 was created by Julian Seward, and is licensed under the BSD license.
The following license applies to the Apache code:
Licensed to the Apache Software Foundation (ASF) under one or more contributor license agreements.  See the NOTICE file distributed with this work for additional information regarding copyright ownership.  The ASF licenses this file to you under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.  You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the License for the specific language governing permissions and limitations under the License.