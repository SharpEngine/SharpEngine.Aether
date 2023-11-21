using SharpEngine.Core.Entity;
using SharpEngine.Core.Math;
using nkast.Aether.Physics2D.Dynamics;
using RJoint = nkast.Aether.Physics2D.Dynamics.Joints.RevoluteJoint;

namespace SharpEngine.AetherPhysics.Joint;

/// <summary>
/// Revolution Joint
/// </summary>
public class RevoluteJoint : Joint
{
    /// <summary>
    /// Create Revolution Joint
    /// </summary>
    /// <param name="target">Joint Target</param>
    /// <param name="fromPosition">Joint From Position</param>
    public RevoluteJoint(Entity target, Vec2? fromPosition)
        : base(target, JointType.Revolute, fromPosition ?? Vec2.Zero, fromPosition ?? Vec2.Zero) { }

    internal RJoint ToAetherPhysics(Body from) =>
        new(from, Target.GetComponentAs<PhysicsComponent>()?.Body, FromPosition.ToAetherPhysics());
}
