using Godot;
using System;

public class MeshManipulation : Spatial
{
   [Export]
   public int xSize, ySize, zSize;

   [Export]
   public int roundness;

   private MeshInstance meshInstance;
   private Mesh mesh;
   private Vector3[] vertices;
   private Vector3[] normals;

   [Export]
   Material material;

	[Export]
	private bool generate = false;

   public override void _Ready()
   {
      if (generate) Generate();
   }

   private void Generate()
   {
      SurfaceTool surfaceTool = new SurfaceTool();
      surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
      surfaceTool.AddSmoothGroup(true);

      CreateVertices(surfaceTool);
      CreateTriangles(surfaceTool);

      surfaceTool.GenerateNormals();

      ArrayMesh arrayMesh = surfaceTool.Commit();

      meshInstance = new MeshInstance();
      meshInstance.Mesh = arrayMesh;
      meshInstance.MaterialOverride = material;

      CallDeferred("add_child", meshInstance);


   }

   private void CreateVertices(SurfaceTool surfaceTool)
   {
      int cornerVertices = 8;
      int edgeVertices = (xSize + ySize + zSize - 3) * 4;
      int faceVertices = (
         (xSize - 1) * (ySize - 1) +
         (xSize - 1) * (zSize - 1) +
         (ySize - 1) * (zSize - 1)) * 2;
      vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
      normals = new Vector3[vertices.Length];

      int v = 0;
      for (int y = 0; y <= ySize; y++)
      {
         for (int x = 0; x <= xSize; x++)
         {
            SetVertex(v++, x, y, 0);
         }
         for (int z = 1; z <= zSize; z++)
         {
            SetVertex(v++, xSize, y, z);
         }
         for (int x = xSize - 1; x >= 0; x--)
         {
            SetVertex(v++, x, y, zSize);
         }
         for (int z = zSize - 1; z > 0; z--)
         {
            SetVertex(v++, 0, y, z);
         }
      }
      for (int z = 1; z < zSize; z++)
      {
         for (int x = 1; x < xSize; x++)
         {
            SetVertex(v++, x, ySize, z);
         }
      }
      for (int z = 1; z < zSize; z++)
      {
         for (int x = 1; x < xSize; x++)
         {
            SetVertex(v++, x, 0, z);
         }
      }

      for (int iv = 0; iv < vertices.Length; iv++)
      {
         surfaceTool.AddVertex(vertices[iv]);
      }
      for (int iNo = 0; iNo < normals.Length; iNo++)
      {
         // surfaceTool.AddNormal(normals[iNo]);
      }
   }

   private void SetVertex(int i, int x, int y, int z)
   {
      Vector3 inner = vertices[i] = new Vector3(x, y, z);

      if (x < roundness)
      {
         inner.x = roundness;
      }
      else if (x > xSize - roundness)
      {
         inner.x = xSize - roundness;
      }
      if (y < roundness)
      {
         inner.y = roundness;
      }
      else if (y > ySize - roundness)
      {
         inner.y = ySize - roundness;
      }
      if (z < roundness)
      {
         inner.z = roundness;
      }
      else if (z > zSize - roundness)
      {
         inner.z = zSize - roundness;
      }

      normals[i] = (vertices[i] - inner).Normalized();
      vertices[i] = inner + normals[i] * roundness;
   }

   private void CreateTriangles(SurfaceTool surfaceTool)
   {
      int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
      int[] triangles = new int[quads * 6];
      int ring = (xSize + zSize) * 2;
      int t = 0, v = 0;

      for (int y = 0; y < ySize; y++, v++)
      {
         for (int q = 0; q < ring - 1; q++, v++)
         {
            t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
         }
         t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
      }

      t = CreateTopFace(triangles, t, ring);
      t = CreateBottomFace(triangles, t, ring);

      for (int it = 0; it < triangles.Length; it++)
      {
         surfaceTool.AddIndex(triangles[it]);
      }
   }

   private int CreateTopFace(int[] triangles, int t, int ring)
   {
      int v = ring * ySize;
      for (int x = 0; x < xSize - 1; x++, v++)
      {
         t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
      }
      t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

      int vMin = ring * (ySize + 1) - 1;
      int vMid = vMin + 1;
      int vMax = v + 2;

      for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
      {
         t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
         for (int x = 1; x < xSize - 1; x++, vMid++)
         {
            t = SetQuad(
               triangles, t,
               vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
         }
         t = SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
      }

      int vTop = vMin - 2;
      t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
      for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
      {
         t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
      }
      t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

      return t;
   }

   private int CreateBottomFace(int[] triangles, int t, int ring)
   {
      int v = 1;
      int vMid = vertices.Length - (xSize - 1) * (zSize - 1);
      t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
      for (int x = 1; x < xSize - 1; x++, v++, vMid++)
      {
         t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
      }
      t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

      int vMin = ring - 2;
      vMid -= xSize - 2;
      int vMax = v + 2;

      for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
      {
         t = SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
         for (int x = 1; x < xSize - 1; x++, vMid++)
         {
            t = SetQuad(
               triangles, t,
               vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
         }
         t = SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
      }

      int vTop = vMin - 1;
      t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
      for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
      {
         t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
      }
      t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

      return t;
   }

   private static int
   SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
   {
      triangles[i] = v00;
      triangles[i + 1] = triangles[i + 4] = v01;
      triangles[i + 2] = triangles[i + 3] = v10;
      triangles[i + 5] = v11;
      return i + 6;
   }
}
