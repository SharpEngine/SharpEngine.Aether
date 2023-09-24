using System;
using SharpEngine.Core.Component;
using SharpEngine.Core.Manager;
using SharpEngine.Core.Math;
using SharpEngine.Core.Utils.Input;

namespace SharpEngine.AetherPhysics;

/// <summary>
/// Physics version of ControlComponent
/// </summary>
public class PhysicsControlComponent : ControlComponent
{
    private PhysicsComponent? _physicsComponent;

    /// <summary>Create Physics Control Component</summary>
    /// <param name="controlType">Control Type (FourDirection)</param>
    /// <param name="speed">Speed (300)</param>
    /// <param name="jumpForce">Jump Force (3)</param>
    /// <param name="useGamePad">Use Game Pad (false)</param>
    /// <param name="gamePadIndex">Game Pad Index (1)</param>
    public PhysicsControlComponent(
        ControlType controlType = ControlType.FourDirection,
        int speed = 300,
        float jumpForce = 2f,
        bool useGamePad = false,
        int gamePadIndex = 1
    )
        : base(controlType, speed, jumpForce, useGamePad, gamePadIndex) { }

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

        var posX = 0f;
        var posY = 0f;
        var jump = false;

        switch (ControlType)
        {
            case ControlType.ClassicJump:
                if (UseGamePad && InputManager.GetGamePadAxis(GamePadIndex, GamePadAxis.LeftX) != 0)
                    posX += InputManager.GetGamePadAxis(GamePadIndex, GamePadAxis.LeftX);
                else
                {
                    if (InputManager.IsKeyDown(GetKey(ControlKey.Left)))
                        posX--;
                    if (InputManager.IsKeyDown(GetKey(ControlKey.Right)))
                        posX++;
                }

                if (
                    InputManager.IsKeyPressed(GetKey(ControlKey.Up))
                    || (
                        UseGamePad
                        && InputManager.IsGamePadButtonPressed(GamePadIndex, GamePadButton.A)
                    )
                )
                {
                    if (_physicsComponent.IsOnGround())
                    {
                        posY -= JumpForce;
                        jump = true;
                    }
                }

                break;
        }

        if (posX != 0 || posY != 0)
        {
            IsMoving = true;
            Direction = jump ? new Vec2(posX, posY) : new Vec2(posX, posY).Normalized();
        }

        if (!IsMoving)
            return;
        var velocity = Direction * Speed;
        _physicsComponent.SetLinearVelocity(
            ControlType == ControlType.ClassicJump
                ? new Vec2(
                    velocity.X,
                    velocity.Y == 0 ? _physicsComponent.GetLinearVelocity().Y : velocity.Y
                )
                : velocity
        );
    }
}
