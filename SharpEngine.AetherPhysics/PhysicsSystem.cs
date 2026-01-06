using System.Collections.Generic;
using SharpEngine.Core.Math;
using SharpEngine.Core.Utils;
using nkast.Aether.Physics2D.Dynamics;
using SharpEngine.Core.Entity;
using System;
using System.Linq;
using JetBrains.Annotations;

namespace SharpEngine.AetherPhysics;

/// <summary>
/// Scene System that adds physics World
/// </summary>
public class PhysicsSystem : ISceneSystem
{
    /// <summary>
    /// Physic World
    /// </summary>
    public readonly World World;

    /// <summary>
    /// If Physics System is Paused
    /// </summary>
    [UsedImplicitly]
    public bool Paused { get; set; }

    private readonly Dictionary<Body, Entity> _bodies = [];
    private float _worldStepTimer;
    private const float WorldStep = 1 / 60f;
    private readonly List<Body> _removeBodies = [];

    /// <summary>
    /// Create a physics system
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
    /// Get Entity from Body
    /// </summary>
    /// <param name="body">Body</param>
    /// <returns>Entity</returns>
    /// <exception cref="ArgumentException">Throw if body doesn't exist in a system</exception>
    [UsedImplicitly]
    public Entity GetEntityForBody(Body body) => _bodies.TryGetValue(body, out var entity) ? entity : throw new ArgumentException("Unknown body.");

    /// <summary>
    /// Get Body from Entity
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>Body</returns>
    [UsedImplicitly]
    public Body GetBodyForEntity(Entity entity) => _bodies.FirstOrDefault(x => x.Value == entity).Key;

    /// <summary>
    /// Create and add Physics Body to System
    /// </summary>
    /// <param name="entity">Entity attached</param>
    /// <param name="position">Position</param>
    /// <param name="rotation">Rotation</param>
    /// <param name="bodyType">Type of Body</param>
    /// <returns>Created Body</returns>
    public Body CreateBody(Entity entity, Vec2 position, float rotation, BodyType bodyType)
    {
        var body = World.CreateBody(position.ToAetherPhysics(), rotation, bodyType);
        _bodies.Add(body, entity);
        return body;
    }

    /// <summary>
    /// Remove Physics Body from System
    /// </summary>
    /// <param name="body">Physics Body</param>
    /// <param name="delay">If remove must be delayed (false)</param>
    [UsedImplicitly]
    public void RemoveBody(Body body, bool delay = false)
    {
        if (delay)
            _removeBodies.Add(body);
        else
        {
            World.Remove(body);
            _bodies.Remove(body);
        }
    }

    /// <inheritdoc />
    public void Load() { }

    /// <inheritdoc />
    public void Unload() { }

    /// <inheritdoc />
    public void Update(float delta)
    {
        foreach (var removeBody in _removeBodies)
        {
            World.Remove(removeBody);
            _bodies.Remove(removeBody);
        }
        _removeBodies.Clear();

        if (Paused) return;
        
        _worldStepTimer += delta;

        while (_worldStepTimer >= WorldStep)
        {
            World.Step(WorldStep);
            _worldStepTimer -= WorldStep;
        }
        World.ClearForces();
    }

    /// <inheritdoc />
    public void Draw() { }

    /// <inheritdoc />
    public void OpenScene() { }

    /// <inheritdoc />
    public void CloseScene() { }
}
