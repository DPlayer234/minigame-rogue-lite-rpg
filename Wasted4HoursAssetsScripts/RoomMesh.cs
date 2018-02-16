namespace SAE.RoguePG.Main.Dungeon
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Generates a mesh based on all cube meshes attached to the object.
    /// </summary>
    /// <remarks>
    ///     This will not correctly generate meshes, if any of the parents to
    ///     the cubes don't have a scale of (1.0, 1.0, 1.0) or their local position
    ///     (excluding the main root) is not (0.0, 0.0, 0.0).
    /// </remarks>
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [DisallowMultipleComponent]
    public class RoomMesh : MonoBehaviour
    {
        /// <summary> The material for the mesh </summary>
        public Material material;
        
        /// <summary> The UV mappings for the parts </summary>
        public UVMapping[] uvMappings;

        /// <summary>
        ///     The amount of vertices per cube mesh.
        ///     Only meshes with this amount of vertices are considered for generation.
        /// </summary>
        private const int CubeVertexCount = 24;

        /// <summary> The top left corner for the UV map on the cube </summary>
        private static Vector2 cubeUVMapTopLeft = new Vector2(0.0f, 0.0f);

        /// <summary> The bottom right corner for the UV map on the cube </summary>
        private static Vector2 cubeUVMapBottomRight = new Vector2(1.0f, 1.0f);

        /// <summary> The mesh that is being generated. </summary>
        private Mesh mesh;

        /// <summary> The vertices of the new mesh. </summary>
        private List<Vector3> vertices;

        /// <summary> The triangles of the new mesh </summary>
        private List<int> triangles;

        /// <summary> The UV of the new mesh </summary>
        private List<Vector2> uv;

        /// <summary> The <seealso cref="MeshRenderer"/> attached to this GameObject </summary>
        private MeshRenderer meshRenderer;

        /// <summary> The <seealso cref="MeshFilter"/> attached to this GameObject </summary>
        private MeshFilter meshFilter;

        /// <summary>
        ///     Attaches an instance to a room and sets it up based on the dungeon design
        /// </summary>
        /// <param name="gameObject"></param>
        public static void AttachToRoom(GameObject gameObject, DungeonDesign design)
        {
            RoomMesh roomMesh = gameObject.AddComponent<RoomMesh>();

            roomMesh.material = design.material;
            roomMesh.uvMappings = design.uvMappings;
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="RoomMesh"/> when it first becomes active
        /// </summary>
        private void Start()
        {
            this.mesh = new Mesh();
            this.vertices = new List<Vector3>();
            this.triangles = new List<int>();
            this.uv = new List<Vector2>();

            this.meshRenderer = this.GetComponent<MeshRenderer>();
            this.meshFilter = this.GetComponent<MeshFilter>();

            this.GenerateMesh();
            this.DestroyObsoleteMeshRenderers();

            this.meshFilter.mesh = this.mesh;
            this.meshRenderer.material = this.material;
        }

        /// <summary>
        ///     Generates the mesh
        /// </summary>
        private void GenerateMesh()
        {
            // Get anything relevant
            MeshFilter[] meshFilters = this.GetComponentsInChildren<MeshFilter>();

            // Generate Mesh and delete obsolte MeshRenderers
            foreach (MeshFilter meshFilter in meshFilters)
            {
                Mesh mesh = meshFilter.mesh;

                ////Debug.LogFormat("{0} : vertices {1} , triangles {2}", this, mesh.vertexCount, mesh.triangles.Length);

                if (mesh.vertexCount == RoomMesh.CubeVertexCount)
                {
                    this.AddVertices(mesh, meshFilter.transform);
                }

                if (meshFilter != this.meshFilter)
                {
                    MonoBehaviour.Destroy(meshFilter);
                }
            }

            // Set the mesh
            this.mesh.vertices = this.vertices.ToArray();
            this.mesh.triangles = this.triangles.ToArray();
            this.mesh.uv = this.uv.ToArray();

            this.mesh.RecalculateBounds();
            this.mesh.RecalculateNormals();
            this.mesh.RecalculateTangents();
        }

        /// <summary>
        ///     Adds the vertices from <paramref name="mesh"/>, in regards to <paramref name="transform"/>'s local transformation.
        ///     The mesh must have at least one vertex.
        /// </summary>
        /// <param name="mesh">The mesh of which to add the vertices</param>
        /// <param name="transform">The transform to consider</param>
        private void AddVertices(Mesh mesh, Transform transform)
        {
            // Get vertices
            int vertexIndexOffset = this.vertices.Count;
            foreach (Vector3 vertex in mesh.vertices)
            {
                Vector3 absoluteVertex;

                absoluteVertex = VariousCommon.LinearMultiply(vertex, transform.localScale);

                absoluteVertex += transform.localPosition;

                absoluteVertex = VariousCommon.RotateVectorAroundOrigin(absoluteVertex, transform.eulerAngles);
                
                this.vertices.Add(absoluteVertex);
            }

            // Get triangles
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i++)
            {
                this.triangles.Add(triangles[i] + vertexIndexOffset);
            }

            // Get UV
            Vector2[] uv = mesh.uv;
            UVMapping uvMapping = this.uvMappings.GetMappingFor(transform.name);

            for (int i = 0; i < uv.Length; i++)
            {
                Vector2 normalizedUV = VariousCommon.LinearDivide(
                    uv[i] - RoomMesh.cubeUVMapTopLeft,
                    RoomMesh.cubeUVMapBottomRight - RoomMesh.cubeUVMapTopLeft);

                this.uv.Add(VariousCommon.LinearMultiply(normalizedUV, uvMapping.uv[i % uvMapping.uv.Length]));
            }
        }

        /// <summary>
        ///     Destroys obsolte MeshRenderers
        /// </summary>
        private void DestroyObsoleteMeshRenderers()
        {
            MeshRenderer[] meshRenderers = this.GetComponentsInChildren<MeshRenderer>();
            
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                if (meshRenderer != this.meshRenderer)
                {
                    MonoBehaviour.Destroy(meshRenderer);
                }
            }
        }
    }
}
