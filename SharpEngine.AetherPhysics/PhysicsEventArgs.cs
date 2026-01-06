using JetBrains.Annotations;
using SharpEngine.Core.Utils.EventArgs;
using nkast.Aether.Physics2D.Dynamics.Contacts;

namespace SharpEngine.AetherPhysics;

/// <summary>
/// Event Args for physics
/// </summary>
public class PhysicsEventArgs : BoolEventArgs
{
    /// <summary>
    /// Sender Fixture of Event
    /// </summary>
    [UsedImplicitly]
    public required nkast.Aether.Physics2D.Dynamics.Fixture Sender { get; set; }

    /// <summary>
    /// Other Fixture of Event
    /// </summary>
    [UsedImplicitly]
    public required nkast.Aether.Physics2D.Dynamics.Fixture Other { get; set; }

    /// <summary>
    /// Contact of Event
    /// </summary>
    [UsedImplicitly]
    public required Contact Contact { get; set; }
}
