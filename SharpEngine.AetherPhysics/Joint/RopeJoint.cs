using SharpEngine.Core.Entity;
using SharpEngine.Core.Math;
using nkast.Aether.Physics2D.Dynamics;
using RJoint = nkast.Aether.Physics2D.Dynamics.Joints.RopeJoint;

namespace SharpEngine.AetherPhysics.Joint;

/// <summary>
/// Rope Joint
/// </summary>
/// <param name="target">Joint Target</param>
/// <param name="fromPosition">Joint From Position</param>
/// <param name="targetPosition">Joint Target Position</param>
/// <param name="maxLength">Joint Max Length</param>
public class RopeJoint(
    Entity target,
    Vec2? fromPosition = null,
    Vec2? targetPosition = null,
    float maxLength = -1
    ) : Joint(target, JointType.Rope, fromPosition ?? Vec2.Zero, targetPosition ?? Vec2.Zero)
{
    /// <summary>
    /// Max Length of Joint
    /// </summary>
    public float MaxLength { get; set; } = maxLength;

    internal RJoint ToAetherPhysics(Body from)
    {
        var joint = new RJoint(
            from,
            Target.GetComponentAs<PhysicsComponent>()?.Body,
            FromPosition.ToAetherPhysics(),
            TargetPosition.ToAetherPhysics()
        );
        if (System.Math.Abs(MaxLength + 1) > 0.001f)
            joint.MaxLength = MaxLength;
        return joint;
    }
}
