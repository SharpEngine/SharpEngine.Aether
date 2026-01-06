using JetBrains.Annotations;
using SharpEngine.Core.Entity;
using SharpEngine.Core.Math;
using nkast.Aether.Physics2D.Dynamics;
using DJoint = nkast.Aether.Physics2D.Dynamics.Joints.DistanceJoint;
using SharpEngine.AetherPhysics.Component;

namespace SharpEngine.AetherPhysics.Joint;

/// <summary>
/// Distance Joint
/// </summary>
/// <param name="target">Joint Target</param>
/// <param name="fromPosition">Joint From Position</param>
/// <param name="targetPosition">Joint Target Position</param>
/// <param name="length">Joint Length</param>
/// <param name="frequency">Joint Frequency</param>
/// <param name="dampingRatio">Joint Damping Ratio</param>
[UsedImplicitly]
public class DistanceJoint(
    Entity target,
    Vec2? fromPosition = null,
    Vec2? targetPosition = null,
    float length = -1,
    float frequency = -1,
    float dampingRatio = -1
    ) : Joint(target, JointType.Distance, fromPosition ?? Vec2.Zero, targetPosition ?? Vec2.Zero)
{
    /// <summary>
    /// Length of Joint
    /// </summary>
    [UsedImplicitly]
    public float Length { get; set; } = length;

    /// <summary>
    /// Frequency of Joint
    /// </summary>
    [UsedImplicitly]
    public float Frequency { get; set; } = frequency;

    /// <summary>
    /// Damping Ratio of Joint
    /// </summary>
    [UsedImplicitly]
    public float DampingRatio { get; set; } = dampingRatio;

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
