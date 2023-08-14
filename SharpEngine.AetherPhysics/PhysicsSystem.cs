using System.Collections.Generic;
using SharpEngine.Core.Math;
using SharpEngine.Core.Utils;
using tainicom.Aether.Physics2D.Dynamics;

namespace SharpEngine.AetherPhysics;

/// <summary>
/// Scene System which add physics World
/// </summary>
public class PhysicsSystem: ISceneSystem
{
    /// <summary>
    /// Physic World
    /// </summary>
    public readonly World World;
    
    /// <summary>
    /// If Physics System is Paused
    /// </summary>
    public bool Paused { get; set; }
    
    private float _worldStepTimer;
    private const float WorldStep = 1 / 60f;
    private readonly List<Body> _removeBodies = new();

    /// <summary>
    /// Create physics system
    /// </summary>
    /// <param name="gravity">Gravity (Vec2(0, 25))</param>
    public PhysicsSystem(Vec2? gravity = null)
    {
        var gravityFinal = gravity ?? new Vec2(0, 25);
        World = new World(gravityFinal.ToAetherPhysics())
        {
            ContactManager =
            {
                VelocityConstraintsMultithreadThreshold = 256,
                PositionConstraintsMultithreadThreshold = 256,
                CollideMultithreadThreshold = 256
            }
        };
    }

    /// <summary>
    /// Remove Physics Body from System
    /// </summary>
    /// <param name="body">Physics Body</param>
    /// <param name="delay">If remove must be delayed (false)</param>
    public void RemoveBody(Body body, bool delay = false)
    {
        if(delay)
            _removeBodies.Add(body);
        else
            World.Remove(body);
    }

    /// <inheritdoc />
    public void Load()
    {}

    /// <inheritdoc />
    public void Unload()
    {
    }

    /// <inheritdoc />
    public void Update(float delta)
    {
        foreach (var removeBody in _removeBodies)
            World.Remove(removeBody);
        _removeBodies.Clear();
        
        if (!Paused)
        {
            _worldStepTimer += delta;

            while (_worldStepTimer >= WorldStep)
            {
                World.Step(WorldStep);
                _worldStepTimer -= WorldStep;
            }
            World.ClearForces();
        }
    }

    /// <inheritdoc />
    public void Draw()
    {
    }

    /// <inheritdoc />
    public void OpenScene()
    {
    }

    /// <inheritdoc />
    public void CloseScene()
    {
    }
}