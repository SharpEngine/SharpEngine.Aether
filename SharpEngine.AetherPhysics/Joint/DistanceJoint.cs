using SharpEngine.Core.Entity;
using SharpEngine.Core.Math;
using nkast.Aether.Physics2D.Dynamics;
using DJoint = nkast.Aether.Physics2D.Dynamics.Joints.DistanceJoint;

namespace SharpEngine.AetherPhysics.Joint;

/// <summary>
/// Distance Joint
/// </summary>
public class DistanceJoint : Joint
{
    /// <summary>
    /// Length of Joint
    /// </summary>
    public float Length { get; set; }

    /// <summary>
    /// Frequency of Joint
    /// </summary>
    public float Frequency { get; set; }

    /// <summary>
    /// Damping Ratio of Joint
    /// </summary>
    public float DampingRatio { get; set; }

    /// <summary>
    /// Create Distance Joint
    /// </summary>
    /// <param name="target">Joint Target</param>
    /// <param name="fromPosition">Joint From Position</param>
    /// <param name="targetPosition">Joint Target Position</param>
    /// <param name="length">Joint Length</param>
    /// <param name="frequency">Joint Frequency</param>
    /// <param name="dampingRatio">Joint Damping Ratio</param>
    public DistanceJoint(
        Entity target,
        Vec2? fromPosition = null,
        Vec2? targetPosition = null,
        float length = -1,
        float frequency = -1,
        float dampingRatio = -1
    )
        : base(target, JointType.Distance, fromPosition ?? Vec2.Zero, targetPosition ?? Vec2.Zero)
    {
        Length = length;
        Frequency = frequency;
        DampingRatio = dampingRatio;
    }

    internal DJoint ToAetherPhysics(Body from)
    {
        var joint = new DJoint(
            from,
            Target.GetComponentAs<PhysicsComponent>()?.Body,
            FromPosition.ToAetherPhysics(),
            TargetPosition.ToAetherPhysics()
        );
        if (System.Math.Abs(Length + 1) > 0.001f)
            joint.Length = Length;
        if (System.Math.Abs(Frequency + 1) > 0.001f)
            joint.Frequency = Frequency;
        if (System.Math.Abs(DampingRatio + 1) > 0.001f)
            joint.DampingRatio = DampingRatio;
        return joint;
    }
}
