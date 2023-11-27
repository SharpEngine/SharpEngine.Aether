using SharpEngine.AetherPhysics;
using SharpEngine.Core.Component;
using SharpEngine.Core.Input;
using SharpEngine.Core.Manager;
using SharpEngine.Core.Math;

namespace SharpEngine.AetherPhysics;

/// <summary>
/// Physics version of ControlComponent
/// </summary>
/// <remarks>Create Physics Control Component</remarks>
/// <param name="controlType">Control Type (FourDirection)</param>
/// <param name="speed">Speed (300)</param>
/// <param name="jumpForce">Jump Force (3)</param>
/// <param name="useGamePad">Use Game Pad (false)</param>
/// <param name="gamePadIndex">Game Pad Index (1)</param>
public class PhysicsControlComponent(
    ControlType controlType = ControlType.FourDirection,
    int speed = 300,
    float jumpForce = 2f,
    bool useGamePad = false,
    int gamePadIndex = 1
    ) : ControlComponent(controlType, speed, jumpForce, useGamePad, gamePadIndex)
{
    private PhysicsComponent? _physicsComponent;
    private bool _jump;

    /// <inheritdoc />
    public override void Load()
    {
        base.Load();

        _physicsComponent = Entity?.GetComponentAs<PhysicsComponent>();
    }

    /// <inheritdoc />
    public override void Update(float delta)
    {
        base.Update(delta);

        if (_physicsComponent == null)
            return;

        _physicsComponent.SetLinearVelocity(
            ControlType == ControlType.ClassicJump
                ? new Vec2(0, _physicsComponent.GetLinearVelocity().Y)
                : Vec2.Zero
        );
        
        _jump = false;

        var move = ControlType switch
        {
            ControlType.ClassicJump => GetJumpMovement(),
            _ => Vec2.Zero
        };

        if (!move.IsZero())
        {
            IsMoving = true;
            Direction = _jump ? move : move.Normalized();
        }

        if (!IsMoving)
            return;
        var velocity = Direction * Speed;
        if(ControlType == ControlType.ClassicJump)
            _physicsComponent.SetLinearVelocity(new Vec2(velocity.X, velocity.Y == 0 ? _physicsComponent.GetLinearVelocity().Y : velocity.Y));
        else
            _physicsComponent.SetLinearVelocity(velocity);
    }

    private Vec2 GetJumpMovement()
    {
        var result = Vec2.Zero;
        if (UseGamePad)
            result.X += InputManager.GetGamePadAxis(GamePadIndex, GamePadAxis.LeftX);
        else
        {
            if (InputManager.IsKeyDown(GetKey(ControlKey.Left)))
                result.X--;
            if (InputManager.IsKeyDown(GetKey(ControlKey.Right)))
                result.X++;
        }

        if (
            (InputManager.IsKeyPressed(GetKey(ControlKey.Up)) || ( UseGamePad && InputManager.IsGamePadButtonPressed(GamePadIndex, GamePadButton.A)))
            && _physicsComponent!.IsOnGround()
        )
        {
            if (_physicsComponent!.IsOnGround())
            {
                result.Y -= JumpForce;
                _jump = true;
            }
        }

        return result;
    }
}
