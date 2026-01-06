using JetBrains.Annotations;
using SharpEngine.Core.Component;
using SharpEngine.Core.Math;

namespace SharpEngine.AetherPhysics.Component
{
    /// <summary>
    /// Represents a physics-enabled auto component designed to integrate physics-based behaviors
    /// within the SharpEngine framework. This class is intended to provide automated functionality
    /// for handling physics properties and interactions associated with game objects.
    /// </summary>
    /// <param name="direction">Direction</param>
    /// <param name="rotation">Rotation</param>
    [UsedImplicitly]
    public class PhysicsAutoComponent(Vec2? direction = null, int rotation = 0): AutoComponent(direction, rotation)
    {
        private PhysicsComponent? _physicsComponent;

        /// <inheritdoc />
        public override void Load()
        {
            base.Load();

            _physicsComponent = Entity?.GetComponentAs<PhysicsComponent>();
        }
        
        /// <inheritdoc/>
        [UsedImplicitly]
        public override void Update(float delta)
        {
            base.Update(delta);

            if (_physicsComponent == null)
                return;
            
            _physicsComponent.SetLinearVelocity(Vec2.Zero);

            if (Direction.Length() != 0)
                _physicsComponent.SetLinearVelocity(Direction);
            
            if(Rotation != 0)
                _physicsComponent.SetRotation(Rotation * delta + _physicsComponent.GetRotation());
        }
    }
}
