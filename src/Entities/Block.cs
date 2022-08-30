namespace CrazyEaters.Entities 
{
    using Godot;
    using System;
    using CrazyEaters.Resources;

    public class Block : Spatial
    {
        [Export]
        public BlockData data;

        [Export]
        public NodePath areaPath;

        [Export]
        public NodePath fbMeshPath;

        [Export]
        public NodePath collisionShapePath;

        public CollisionShape collisionShape;

        Area area;
        MeshInstance fbMesh;
        ShaderMaterial fbMat;

        public bool overlaping = false;

        public bool placed = false;
        public override void _Ready()
        {
            // if (areaPath != null) {
            //     area = GetNode<Area>(areaPath);
            //     area.Connect("body_entered", this, nameof(OnBodyEnter));
            //     area.Connect("body_exited", this, nameof(OnBodyExit));
            // }
            if (fbMeshPath != null) { 
                fbMesh = GetNode<MeshInstance>(fbMeshPath);
                fbMat = (ShaderMaterial) fbMesh.GetSurfaceMaterial(0);
            }
            collisionShape = GetNodeOrNull<CollisionShape>(collisionShapePath);
        }

        public CollisionShape GetCollisionShape()
        {
            return collisionShape;
        }

        public void Blocked() 
        {
            if (!placed) {
                overlaping = true;
                fbMat.SetShaderParam("overlap", overlaping);
            }
        }

        public void Free() 
        {
            if (!placed) {
                overlaping = false;
                fbMat.SetShaderParam("overlap", overlaping);
            }
        }

        public void UpdateOverlap(bool canPlace) {
            if (!canPlace) {
                Blocked();
                return;
            }
            Free();
        }

        public void SetPlaced(bool placed) 
        {
            this.placed = placed;
            if (placed) {
                // area.Monitoring = false;
                overlaping = false;
            } else {
                // area.Monitoring = true;
            }
        }

    }
}
