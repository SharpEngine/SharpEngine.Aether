using SharpEngine.AetherPhysics;
using SharpEngine.Core;
using SharpEngine.Core.Component;
using SharpEngine.Core.Entity;
using SharpEngine.Core.Math;
using SharpEngine.Core.Utils;
using nkast.Aether.Physics2D.Dynamics;

namespace Testing;

public class MyScene : Scene
{
    public MyScene()
    {
        AddSceneSystem(new PhysicsSystem());

        var e1 = new Entity();
        e1.AddComponent(new TransformComponent(new Vec2(100)));
        e1.AddComponent(new RectComponent(Color.Blue, new Vec2(50)));
        e1.AddComponent(
                new PhysicsComponent(fixedRotation: true, ignoreGravity: true, debugDraw: true)
            )
            .AddCircleCollision(50, restitution: 0f);
        e1.AddComponent(new PhysicsControlComponent(speed: 300));
        AddEntity(e1);

        for (var x = 0; x < 2; x++)
        {
            for (var y = 0; y < 2; y++)
            {
                var e2 = new Entity();
                e2.AddComponent(new TransformComponent(new Vec2(240 + 120 * x, 80 + 120 * y), rotation: 45));
                e2.AddComponent(new RectComponent(Color.Red, new Vec2(50)));
                e2.AddComponent(new PhysicsComponent(BodyType.Dynamic, true, false, true))
                    .AddRectangleCollision(new Vec2(50));
                AddEntity(e2);
            }
        }
    }
}
