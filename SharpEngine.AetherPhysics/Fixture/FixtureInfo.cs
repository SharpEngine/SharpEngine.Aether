using JetBrains.Annotations;
using SharpEngine.Core.Math;

namespace SharpEngine.AetherPhysics.Fixture;

/// <summary>
/// Information of Fixture
/// </summary>
public struct FixtureInfo
{
    /// <summary>
    /// Density of Fixture
    /// </summary>
    [UsedImplicitly]
    public float Density { get; set; }

    /// <summary>
    /// Restitution of Fixture
    /// </summary>
    [UsedImplicitly]
    public float Restitution { get; set; }

    /// <summary>
    /// Friction of Fixture
    /// </summary>
    [UsedImplicitly]
    public float Friction { get; set; }

    /// <summary>
    /// Type of Fixture
    /// </summary>
    [UsedImplicitly]
    public FixtureType Type { get; set; }

    /// <summary>
    /// Additional parameter of Fixture
    /// </summary>
    [UsedImplicitly]
    public object Parameter { get; set; }

    /// <summary>
    /// Offset of Fixture
    /// </summary>
    [UsedImplicitly]
    public Vec2 Offset { get; set; }

    /// <summary>
    /// Tag of Fixture
    /// </summary>
    [UsedImplicitly]
    public FixtureTag Tag { get; set; }
}
