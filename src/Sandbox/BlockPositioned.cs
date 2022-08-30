using Godot;
using System;

namespace CrazyEaters.Sandbox
{
   public class BlockPositioned
   {
      public int blockId;
      public float rotationAngle;
      public Vector3 rotationAxis;
      public int currentLife;

      public BlockPositioned() { }
      public BlockPositioned(int blockId, float rotationAngle, Vector3 rotationAxis, int currentLife)
      {
         this.blockId = blockId;
         this.rotationAngle = rotationAngle;
         this.rotationAxis = rotationAxis;
         this.currentLife = currentLife;
      }
   }
}