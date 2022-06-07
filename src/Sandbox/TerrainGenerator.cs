namespace CrazyEaters.Sandbox
{
    using Godot;
    using System;
    using Godot.Collections;

    public class TerrainGenerator : Node
    {

        public static Dictionary HabitatGround(Vector3 chunkPosition) {
            Dictionary data = new Dictionary();

            foreach (int x in GD.Range(Chunk.CHUNK_SIZE)) {
                foreach (int z in GD.Range(Chunk.CHUNK_SIZE)) {
                    foreach (int y in GD.Range(Chunk.CHUNK_SIZE)) {
                        data[new Vector3(x, y, z)] = (y + chunkPosition.y) != 0 ? 0 : 7;
                    }
                }
            }

            return data;
        }

    }
}