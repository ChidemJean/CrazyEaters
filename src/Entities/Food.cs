using Godot;
using System;
using CrazyEaters.Resources;
using CrazyEaters.Managers;
using CrazyEaters.Controllers.Launcher;
using CrazyEaters.DependencyInjection;

namespace CrazyEaters.Entities 
{
    public class Food : RigidBody, IThrowable
    {
        [Export]
        public FoodData foodData;

        RigidBody rb;

        [Inject]
        GameManager gameManager;

        public override void _Ready()
        {
            this.ResolveDependencies();
            rb = this;
        }

        void IThrowable.Drop(Vector3 force, Vector3 origin)
        {
            GD.Print(force);
            GetParent().RemoveChild(this);
            if (gameManager.currentMainNode3D != null) {
                gameManager.currentMainNode3D.AddChild(this);
                GlobalTranslation = origin;
                rb.Mode = RigidBody.ModeEnum.Rigid;
                rb.GravityScale = 2;
                rb.ApplyImpulse(Vector3.Zero, force);
            }
        }

        void IThrowable.Hold(Spatial slot)
        {
            if (gameManager.currentMainNode3D != null) {
                GetParent().RemoveChild(this);
                slot.AddChild(this);
                rb.Mode = RigidBody.ModeEnum.Character;
                rb.GravityScale = 0;
                rb.LinearVelocity = Vector3.Zero;
                GlobalTranslation = slot.GlobalTransform.origin;
                RotationDegrees = Vector3.Zero;
            }
        }
   }
}