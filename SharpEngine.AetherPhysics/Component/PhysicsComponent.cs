using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SharpEngine.AetherPhysics.Fixture;
using SharpEngine.AetherPhysics.Joint;
using SharpEngine.Core.Component;
using SharpEngine.Core.Math;
using SharpEngine.Core.Renderer;
using SharpEngine.Core.Utils;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using SharpEngine.Core.Manager;

namespace SharpEngine.AetherPhysics.Component;

/// <summary>
/// Components which add physics
/// </summary>
/// <remarks>
/// Create a PhysicsComponent
/// </remarks>
[UsedImplicitly]
public class PhysicsComponent : Core.Component.Component
{
    /// <summary>
    /// Body Physics
    /// </summary>
    [UsedImplicitly]
    public Body? Body { get; set; }

    /// <summary>
    /// Draw debug information
    /// </summary>
    [UsedImplicitly]
    public bool DebugDraw {
        get;
        set
        {
            if (value)
                DebugManager.Log(LogLevel.Warning, "Debug Physics Draw doesn't take rotation into account.");
            field = value;
        }
    }
    
    /// <summary>
    /// Event which be called when collision
    /// </summary>
    [UsedImplicitly]
    public EventHandler<PhysicsEventArgs>? CollisionCallback { get; set; }

    /// <summary>
    /// Event which be called when separation
    /// </summary>
    [UsedImplicitly]
    public EventHandler<PhysicsEventArgs>? SeparationCallback { get; set; }

    private readonly BodyType _bodyType;
    private readonly List<FixtureInfo> _fixtures = [];
    private readonly List<Joint.Joint> _joints = [];
    private readonly bool _fixedRotation;
    private readonly bool _ignoreGravity;
    private readonly List<List<object>> _debugDrawings = [];
    private readonly List<Contact> _contacts = [];

    private TransformComponent? _transform;

    /// <param name="bodyType">Type of Body</param>
    /// <param name="ignoreGravity">Ignore Gravity</param>
    /// <param name="fixedRotation">Rotation fixed</param>
    /// <param name="debugDraw">Debug Draw</param>
    public PhysicsComponent(
        BodyType bodyType = BodyType.Dynamic,
        bool ignoreGravity = false,
        bool fixedRotation = false,
        bool debugDraw = false
    )
    {
        _bodyType = bodyType;
        _fixedRotation = fixedRotation;
        _ignoreGravity = ignoreGravity;
        DebugDraw = debugDraw;
    }

    /// <summary>
    /// Return Position of Body
    /// </summary>
    /// <returns>Body Position</returns>
    [UsedImplicitly]
    public Vec2 GetPosition() => new(Body!.Position.X * 50, Body.Position.Y * 50);

    /// <summary>
    /// Define Position of Body
    /// </summary>
    /// <param name="position">Body Position</param>
    [UsedImplicitly]
    public void SetPosition(Vec2 position) => Body!.Position = (position * 0.02f).ToAetherPhysics();

    /// <summary>
    /// Return Linear Velocity of Body
    /// </summary>
    /// <returns>Body Linear Velocity</returns>
    public Vec2 GetLinearVelocity() => new(Body!.LinearVelocity.X * 50, Body.LinearVelocity.Y * 50);

    /// <summary>
    /// Define Linear Velocity of Body
    /// </summary>
    /// <param name="velocity">Body Linear Velocity</param>
    public void SetLinearVelocity(Vec2 velocity) =>
        Body!.LinearVelocity = (velocity * 0.02f).ToAetherPhysics();

    /// <summary>
    /// Apply Impulse to Body
    /// </summary>
    /// <param name="impulse">Linear Impulse</param>
    [UsedImplicitly]
    public void ApplyLinearImpulse(Vec2 impulse) => Body!.ApplyLinearImpulse((impulse * 0.02f).ToAetherPhysics());

    /// <summary>
    /// Return Rotation of Body
    /// </summary>
    /// <returns>Body Rotation</returns>
    [UsedImplicitly]
    public float GetRotation() => Body!.Rotation * 180 / MathHelper.Pi;

    /// <summary>
    /// Define Rotation of Body
    /// </summary>
    /// <param name="rotation">Body Rotation</param>
    [UsedImplicitly]
    public void SetRotation(float rotation) => Body!.Rotation = rotation * MathHelper.Pi / 180f;

    /// <summary>
    /// Add Rectangle Collision
    /// </summary>
    /// <param name="size">Collision Size</param>
    /// <param name="offset">Collision Offset</param>
    /// <param name="density">Collision Density</param>
    /// <param name="restitution">Collision Restitution</param>
    /// <param name="friction">Collision Friction</param>
    /// <param name="tag">Collision Tag</param>
    [UsedImplicitly]
    public PhysicsComponent AddRectangleCollision(
        Vec2 size,
        Vec2? offset = null,
        float density = 1f,
        float restitution = 0.5f,
        float friction = 0.5f,
        FixtureTag tag = FixtureTag.Normal
    )
    {
        offset ??= Vec2.Zero;

        var fixture = new FixtureInfo
        {
            Density = density,
            Restitution = restitution,
            Friction = friction,
            Type = FixtureType.Rectangle,
            Parameter = size * 0.02f,
            Offset = offset * 0.02f,
            Tag = tag
        };
        _debugDrawings.Add(["rectangle", size, offset]);
        _fixtures.Add(fixture);
        return this;
    }

    /// <summary>
    /// Add Circle Collision
    /// </summary>
    /// <param name="radius">Collision Radius</param>
    /// <param name="offset">Collision Offset</param>
    /// <param name="density">Collision Density</param>
    /// <param name="restitution">Collision Restitution</param>
    /// <param name="friction">Collision Friction</param>
    /// <param name="tag">Collision Tag</param>
    [UsedImplicitly]
    public PhysicsComponent AddCircleCollision(
        float radius,
        Vec2? offset = null,
        float density = 1f,
        float restitution = 0.5f,
        float friction = 0.5f,
        FixtureTag tag = FixtureTag.Normal
    )
    {
        offset ??= Vec2.Zero;

        var fixture = new FixtureInfo
        {
            Density = density,
            Restitution = restitution,
            Friction = friction,
            Type = FixtureType.Circle,
            Parameter = radius * 0.02f,
            Offset = offset * 0.02f,
            Tag = tag
        };
        _debugDrawings.Add(["circle", radius, offset]);
        _fixtures.Add(fixture);
        return this;
    }

    /// <summary>
    /// Return if entity is on ground
    /// </summary>
    /// <returns>If is on the ground</returns>
    public bool IsOnGround()
    {
        if (GetLinearVelocity().Y != 0)
            return false;
        foreach (var contact in _contacts)
        {
            contact.GetWorldManifold(out var normal, out _);
            if (normal.X == 0 && Math.Abs(normal.Y + 1) < 0.001f)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Add joint
    /// </summary>
    /// <param name="joint">Joint</param>
    [UsedImplicitly]
    public void AddJoint(Joint.Joint joint) => _joints.Add(joint);

    /// <summary>
    /// Remove Body
    /// </summary>
    [UsedImplicitly]
    public void RemoveBody()
    {
        if (Body != null)
            Entity?.Scene?.GetSceneSystem<PhysicsSystem>()?.World.Remove(Body);
        Body = null;
    }

    /// <inheritdoc />
    public override void Load()
    {
        base.Load();

        _transform = Entity?.GetComponentAs<TransformComponent>();

        if (Entity == null || _transform == null)
            return;

        var body = Entity.Scene?.GetSceneSystem<PhysicsSystem>()?
            .CreateBody(Entity, _transform.Position * 0.02f, MathHelper.ToRadians(_transform.Rotation), _bodyType);

        if (body == null)
            return;
        Body = body;
        Body.FixedRotation = _fixedRotation;
        Body.IgnoreGravity = _ignoreGravity;
        Body.OnCollision += OnCollision;
        Body.OnSeparation += OnSeparation;
    }

    /// <inheritdoc />
    public override void Update(float delta)
    {
        base.Update(delta);

        if (Body == null)
            return;

        // Create Fixtures
        foreach (var info in _fixtures)
        {
            nkast.Aether.Physics2D.Dynamics.Fixture fixture;
            switch (info.Type)
            {
                case FixtureType.Rectangle:
                    var size = info.Parameter as Vec2;
                    fixture = Body.CreateRectangle(
                        size!.X,
                        size.Y,
                        info.Density,
                        info.Offset.ToAetherPhysics()
                    );
                    break;
                case FixtureType.Circle:
                    var radius = Convert.ToSingle(info.Parameter);
                    fixture = Body.CreateCircle(
                        radius,
                        info.Density,
                        info.Offset.ToAetherPhysics()
                    );
                    break;
                default:
                    throw new ArgumentException($"Unknown Type of Fixture : {info.Type}");
            }

            fixture.Tag = info.Tag;
            fixture.Restitution = info.Restitution;
            fixture.Friction = info.Friction;
        }
        _fixtures.Clear();

        // Create Joint
        foreach (var joint in _joints)
        {
            switch (joint.Type)
            {
                case JointType.Distance:
                    Entity
                        ?.Scene?.GetSceneSystem<PhysicsSystem>()
                        ?.World.Add(((DistanceJoint)joint).ToAetherPhysics(Body));
                    break;
                case JointType.Revolute:
                    Entity
                        ?.Scene?.GetSceneSystem<PhysicsSystem>()
                        ?.World.Add(((RevoluteJoint)joint).ToAetherPhysics(Body));
                    break;
                case JointType.Rope:
                    Entity
                        ?.Scene?.GetSceneSystem<PhysicsSystem>()
                        ?.World.Add(((RopeJoint)joint).ToAetherPhysics(Body));
                    break;
                default:
                    throw new ArgumentException($"Unknown Type of Joint : {joint.Type}");
            }
        }
        _joints.Clear();

        if (_transform == null)
            return;

        _transform.LocalPosition = (Body.Position * 50f).ToSharpEngine();
        _transform.LocalRotation = (int)MathHelper.ToDegrees(Body.Rotation);
    }

    /// <inheritdoc />
    public override void Draw()
    {
        base.Draw();

        if (_transform == null)
            return;

        if (!DebugDraw) return;
        
        foreach (var drawing in _debugDrawings)
        {
            switch ((string)drawing[0])
            {
                case "rectangle":
                    var size = (Vec2)drawing[1];
                    var offset = (Vec2)drawing[2];
                    var rect = new Rect(
                        _transform.Position.X + offset.X - size.X / 2,
                        _transform.Position.Y + offset.Y - size.Y / 2,
                        size.X,
                        size.Y
                    );
                    SERender.DrawRectangleLines(
                        rect,
                        2,
                        Color.DarkRed,
                        InstructionSource.Entity,
                        int.MaxValue
                    );
                    break;
                case "circle":
                    var radius = (float)drawing[1];
                    var offsetCirc = (Vec2)drawing[2];
                    SERender.DrawCircleLines(
                        (int)(_transform.Position.X + offsetCirc.X),
                        (int)(_transform.Position.Y + offsetCirc.Y),
                        radius,
                        Color.DarkRed,
                        InstructionSource.Entity,
                        int.MaxValue
                    );
                    break;
            }
        }
    }

    private bool OnCollision(
        nkast.Aether.Physics2D.Dynamics.Fixture sender,
        nkast.Aether.Physics2D.Dynamics.Fixture other,
        Contact contact
    )
    {
        var eventArgs = new PhysicsEventArgs
        {
            Sender = sender,
            Other = other,
            Contact = contact,
            Result = true
        };

        CollisionCallback?.Invoke(this, eventArgs);

        if (
            (FixtureTag)sender.Tag == FixtureTag.IgnoreCollisions
            || (FixtureTag)other.Tag == FixtureTag.IgnoreCollisions
        )
            eventArgs.Result = false;

        if (eventArgs.Result)
            _contacts.Add(contact);

        return eventArgs.Result;
    }

    private void OnSeparation(
        nkast.Aether.Physics2D.Dynamics.Fixture sender,
        nkast.Aether.Physics2D.Dynamics.Fixture other,
        Contact contact
    )
    {
        var eventArgs = new PhysicsEventArgs
        {
            Sender = sender,
            Other = other,
            Contact = contact,
            Result = true
        };

        SeparationCallback?.Invoke(this, eventArgs);
        _contacts.Remove(contact);
    }
}
