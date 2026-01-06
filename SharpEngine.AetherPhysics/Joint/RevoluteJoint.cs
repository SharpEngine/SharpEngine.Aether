using JetBrains.Annotations;
using SharpEngine.Core.Entity;
using SharpEngine.Core.Math;
using nkast.Aether.Physics2D.Dynamics;
using RJoint = nkast.Aether.Physics2D.Dynamics.Joints.RevoluteJoint;
using SharpEngine.AetherPhysics.Component;

namespace SharpEngine.AetherPhysics.Joint;

/// <summary>
/// Revolution Joint
/// </summary>
/// <remarks>
/// Create Revolution Joint
/// </remarks>
/// <param name="target">Joint Target</param>
/// <param name="fromPosition">Joint From Position</param>
[UsedImplicitly]
public class RevoluteJoint(Entity target, Vec2? fromPosition) : Joint(target, JointType.Revolute, fromPosition ?? Vec2.Zero, fromPosition ?? Vec2.Zero)
{
    internal RJoint ToAetherPhysics(Body from) =>
        new(from, Target.GetComponentAs<PhysicsComponent>()?.Body, FromPosition.ToAetherPhysics());
}
