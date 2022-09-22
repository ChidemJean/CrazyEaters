using Godot;
using System;
using System.Collections.Generic;

namespace CrazyEaters.Tools
{
   public class LineRenderer : ImmediateGeometry
   {

      [Export] List<Vector3> points = new List<Vector3> { new Vector3(0,0,0), new Vector3(0,5,0) };
      [Export] float startThickness = 0.1f;
      [Export] float endThickness = 0.1f;
      [Export] int cornerSmooth = 5;
      [Export] int capSmooth = 5;
      [Export] bool drawCaps = true;
      [Export] bool drawCorners = true;
      [Export] bool globalCoords = true;
      [Export] bool scaleTexture = true;

      Camera camera;
      Vector3 cameraOrigin;

      public override void _Ready()
      {
         
      }

      public void UpdatePoints(List<Vector3> points) 
      {
         this.points = points;
      }

      public override void _Process(float delta)
      {
         if (points.Count < 2) {
            return;
         }

         camera = GetViewport().GetCamera();
         if (camera == null) {
            return;
         }
         cameraOrigin = ToLocal(camera.GlobalTransform.origin);

         float progressStep = 1.0f / points.Count;
         float progress = 0.0f;
         float thickness = Mathf.Lerp(startThickness, endThickness, progress);
         float nextThickness = Mathf.Lerp(startThickness, endThickness, progress + progressStep);

         Clear();
         Begin(Mesh.PrimitiveType.Triangles);

         foreach (int i in GD.Range(points.Count - 1)) {
            Vector3 A = points[i];
            Vector3 B = points[i+1];

            if (globalCoords) {
               A = ToLocal(A);
               B = ToLocal(B);
            }

            Vector3 AB = B - A;
            Vector3 orthogonalABStart = (cameraOrigin - ((A + B) / 2)).Cross(AB).Normalized() * thickness;
            Vector3 orthogonalABEnd = (cameraOrigin - ((A + B) / 2)).Cross(AB).Normalized() * nextThickness;

            Vector3 AtoABStart = A + orthogonalABStart;
            Vector3 AfromABStart = A - orthogonalABStart;
            Vector3 BtoABEnd = B + orthogonalABEnd;
            Vector3 BfromABEnd = B - orthogonalABEnd;

            if (i == 0) {
               if (drawCaps) {
                  Cap(A, B, thickness, capSmooth);
               }
            }

            if (scaleTexture) {
               var ABLen = AB.Length();
               var ABFloor = Mathf.Floor(ABLen);
               var ABFrac = ABLen - ABFloor;
               
               SetUv(new Vector2(ABFloor, 0));
               AddVertex(AtoABStart);
               SetUv(new Vector2(-ABFrac, 0));
               AddVertex(BtoABEnd);
               SetUv(new Vector2(ABFloor, 1));
               AddVertex(AfromABStart);
               SetUv(new Vector2(-ABFrac, 0));
               AddVertex(BtoABEnd);
               SetUv(new Vector2(-ABFrac, 1));
               AddVertex(BfromABEnd);
               SetUv(new Vector2(ABFloor, 1));
               AddVertex(AfromABStart);
            } else {
               SetUv(new Vector2(1, 0));
               AddVertex(AtoABStart);
               SetUv(new Vector2(0, 0));
               AddVertex(BtoABEnd);
               SetUv(new Vector2(1, 1));
               AddVertex(AfromABStart);
               SetUv(new Vector2(0, 0));
               AddVertex(BtoABEnd);
               SetUv(new Vector2(0, 1));
               AddVertex(BfromABEnd);
               SetUv(new Vector2(1, 1));
               AddVertex(AfromABStart);
            }

            if (i == points.Count - 2) {
               if (drawCaps) {
                  Cap(A, B, nextThickness, capSmooth);
               }
            } else {
               if (drawCaps) {
                  Vector3 C = points[i+2];
                  if (globalCoords) {
                     C = ToLocal(C);
                  }

                  Vector3 BC = C - B;
                  Vector3 orthogonalBCStart = (cameraOrigin - ((B + C) / 2)).Cross(BC).Normalized() * nextThickness;
                  float angleDot = AB.Dot(orthogonalBCStart);
                  if (angleDot > 0) {
                     Corner(B, BtoABEnd, B + orthogonalBCStart, cornerSmooth);
                  } else {
                     Corner(B, B - orthogonalBCStart, BfromABEnd, cornerSmooth);
                  }
               }
            }

            progress += progressStep;
            thickness = Mathf.Lerp(startThickness, endThickness, progress);
            nextThickness = Mathf.Lerp(startThickness, endThickness, progress + progressStep);

         }

         End();
      }

      public void Cap(Vector3 center, Vector3 pivot, float thickness, int smoothing)
      {
         Vector3 orthogonal = (cameraOrigin - center).Cross(center - pivot).Normalized() * thickness;
         Vector3 axis = (center - cameraOrigin).Normalized();
         
         List<Vector3> array = new List<Vector3>();
         foreach (int i in GD.Range(smoothing + 1)) {
            array.Add(new Vector3(0,0,0));
         }
         array[0] = center + orthogonal;
         array[smoothing] = center - orthogonal;
         
         foreach (int i in GD.Range(1, smoothing)) {
            array[i] = center + (orthogonal.Rotated(axis, Mathf.Lerp(0, (float) Math.PI, (float) i / smoothing)));
         }
         
         foreach (int i in GD.Range(1, smoothing + 1)) {
            SetUv(new Vector2(0, (i - 1) / smoothing));
            AddVertex(array[i - 1]);
            SetUv(new Vector2(0, (i - 1) / smoothing));
            AddVertex(array[i]);
            SetUv(new Vector2(0.5f, 0.5f));
            AddVertex(center);
         }
      }

      public void Corner(Vector3 center, Vector3 start, Vector3 end, int smoothing)
      {
         List<Vector3> array = new List<Vector3>();
         foreach (int i in GD.Range(smoothing + 1)) {
            array.Add(new Vector3(0,0,0));
         }
         array[0] = start;
         array[smoothing] = end;
         
         var axis = start.Cross(end).Normalized();
         var offset = start - center;
         var angle = offset.AngleTo(end - center);
         
         foreach (int i in GD.Range(1, smoothing)) {
            array[i] = center + offset.Rotated(axis, Mathf.Lerp(0, angle, (float) i / smoothing));
         }
         
         foreach (int i in GD.Range(1, smoothing + 1)) {
            SetUv(new Vector2(0, (i - 1) / smoothing));
            AddVertex(array[i - 1]);
            SetUv(new Vector2(0, (i - 1) / smoothing));
            AddVertex(array[i]);
            SetUv(new  Vector2(0.5f, 0.5f));
            AddVertex(center);
         }
      }

   }
}