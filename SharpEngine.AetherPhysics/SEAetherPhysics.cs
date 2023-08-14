using SharpEngine.Core.Manager;
using SharpEngine.Core.Math;
using tainicom.Aether.Physics2D.Common;

namespace SharpEngine.AetherPhysics;

/// <summary>
/// Static class with extensions and add version functions
/// </summary>
public static class SEAetherPhysics
{
    /// <summary>
    /// Change SharpEngine Vec2 to Aether Physics Vector2
    /// </summary>
    /// <param name="vec">SharpEngine Vec2</param>
    /// <returns>Aether Physics Vector 2</returns>
    public static Vector2 ToAetherPhysics(this Vec2 vec) => new(vec.X, vec.Y);
    
    /// <summary>
    /// Change Aether Physics Vector2 to SharpEngine Vec2
    /// </summary>
    /// <param name="vec">Aether Physics Vector2</param>
    /// <returns>SharpEngine Vec2</returns>
    public static Vec2 ToSharpEngine(this Vector2 vec) => new(vec.X, vec.Y);

    /// <summary>
    /// Add versions to DebugManager
    /// </summary>
    public static void AddVersions()
    {
        DebugManager.Versions.Add("Aether.Physics2D", "1.7.0");
        DebugManager.Versions.Add("SharpEngine.AetherPhysics", "1.0.1");
    }
}