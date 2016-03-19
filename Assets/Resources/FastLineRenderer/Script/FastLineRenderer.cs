//
// (c) 2016 Digital Ruby, LLC
// http://www.digitalruby.com
// Code may not be redistributed in source form!
// Using this code in commercial games and apps is fine.
//

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using DigitalRubyShared;

namespace DigitalRuby.FastLineRenderer
{
    [Serializable]
    public struct LineGroupList
    {
        public static LineGroupList Default()
        {
            LineGroupList l = new LineGroupList();

            l.LineRadius = 2.0f;
            l.LineColor = Color.green;
            l.GlowWidthMultiplier = 0.0f;
            l.GlowIntensity = 0.4f;
            l.AddStartCap = true;
            l.AddEndCap = true;
            l.LineJoin = FastLineRendererLineJoin.Round;
            l.Continuous = true;

            return l;
        }

        [Tooltip("Description. Not saved when built.")]
        public string Description;

        [Tooltip("Offset for all lines in this group")]
        public Vector3 Offset;

        [Range(0.25f, 100.0f)]
        [Tooltip("Line radius")]
        public float LineRadius;

        [Tooltip("Line color")]
        public Color32 LineColor;

        [Range(0.0f, 64.0f)]
        [Tooltip("Glow width multiplier")]
        public float GlowWidthMultiplier;

        [Range(0.0f, 4.0f)]
        [Tooltip("Glow intensity")]
        public float GlowIntensity;

        [Tooltip("Whether to add a start cap")]
        public bool AddStartCap;

        [Tooltip("Whether to add an end cap")]
        public bool AddEndCap;

        [Tooltip("Join type")]
        public FastLineRendererLineJoin LineJoin;

        [Tooltip("Continuous. If true, line points append. If false, every two points is an individual line.")]
        public bool Continuous;

        [ReorderableList("List of points for the lines.")]
        public ReorderableList_Vector3 Points;
    }

    /// <summary>
    /// Line join modes
    /// </summary>
    public enum FastLineRendererLineJoin
    {
        /// <summary>
        /// No attempt to join
        /// </summary>
        None,

        /// <summary>
        /// Adjust the position of the line to intersect the previous line
        /// </summary>
        AdjustPosition,

        /// <summary>
        /// Force the vertices of the line to attach to the previous line. This looks worse at sharper angles, and AdjustPosition should be used for those cases.
        /// </summary>
        AttachToPrevious,

        /// <summary>
        /// Round line join
        /// </summary>
        Round
    }

    /// <summary>
    /// Type of line segment
    /// </summary>
    public enum FastLineRendererLineSegmentType
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Full line segment
        /// </summary>
        Full = 2,

        /// <summary>
        /// Start cap
        /// </summary>
        StartCap = 4,

        /// <summary>
        /// End cap
        /// </summary>
        EndCap = 8,

        /// <summary>
        /// Round join
        /// </summary>
        RoundJoin = 16
    }

    /// <summary>
    /// Spline flags
    /// </summary>
    public enum FastLineRendererSplineFlags
    {
        /// <summary>
        /// No flags
        /// </summary>
        None = 0,

        /// <summary>
        /// Loop back to start (close path)
        /// </summary>
        ClosePath = 1,

        /// <summary>
        /// Add a start cap
        /// </summary>
        StartCap = 2,

        /// <summary>
        /// Add an end cap
        /// </summary>
        EndCap = 4
    }

    /// <summary>
    /// Properties for creating lines
    /// </summary>
    public class FastLineRendererProperties
    {
        /// <summary>
        /// Infinite lifetime and no fading. Line exists until manually destroyed.
        /// </summary>
        public static Vector4 LifeTimeInfinite()
        {
            return new Vector4(0.0f, 0.0f, float.MaxValue, 0.0f);
        }

        /// <summary>
        /// Start position
        /// </summary>
        public Vector3 Start;

        /// <summary>
        /// End position (ignored for AppendLine, in which case Start is used)
        /// </summary>
        public Vector3 End;

        /// <summary>
        /// Line radius in world units
        /// </summary>
        public float Radius = 4.0f;

        /// <summary>
        /// Color
        /// </summary>
        public Color32 Color = UnityEngine.Color.white;

        /// <summary>
        /// Glow width multiplier
        /// </summary>
        public float GlowWidthMultiplier = 4.0f;

        /// <summary>
        /// Glow intensity multiplier
        /// </summary>
        public float GlowIntensityMultiplier = 0.5f;

        /// <summary>
        /// Life time parameters. Do not modify, instead call SetLifeTime.
        /// </summary>
        public Vector4 LifeTime = LifeTimeInfinite();

        /// <summary>
        /// Velocity of the line.
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// Angular velocity
        /// </summary>
        public float AngularVelocity { get { return LifeTime.w; } set { LifeTime.w = value; } }

        /// <summary>
        /// Join mode if AppendLine is used
        /// </summary>
        public FastLineRendererLineJoin LineJoin = FastLineRendererLineJoin.AdjustPosition;

        /// <summary>
        /// Line type
        /// </summary>
        public FastLineRendererLineSegmentType LineType = FastLineRendererLineSegmentType.Full;

        /// <summary>
        /// Populates the LifeTime vector
        /// </summary>
        /// <param name="creationTime">Time at which the line is created (where now is Time.timeSinceLevelLoad)</param>
        /// <param name="lifeTime">Total lifetime for the line.
        /// <param name="fadeSeconds">Seconds to fade in and out</param>
        public void SetLifeTime(float creationTime, float lifeTime, float fadeSeconds)
        {
            LifeTime.x = creationTime;
            LifeTime.y = fadeSeconds;
            LifeTime.z = lifeTime;
        }

        /// <summary>
        /// Populates the LifeTime vector
        /// </summary>
        /// <param name="lifeTime">Total lifetime for the line.
        /// <param name="fadeInSeconds">Seconds to fade in and out.</param>
        public void SetLifeTime(float lifeTime, float fadeSeconds)
        {
            LifeTime.x = Time.timeSinceLevelLoad;
            LifeTime.y = fadeSeconds;
            LifeTime.z = lifeTime;
        }

        /// <summary>
        /// Populates the LifeTime vector with no fade
        /// </summary>
        /// <param name="lifeTime">Total lifetime of the line.</param>
        public void SetLifeTime(float lifeTime)
        {
            LifeTime.x = Time.timeSinceLevelLoad;
            LifeTime.y = 0.0f;
            LifeTime.z = lifeTime;
        }
    }

    [ExecuteInEditMode]
    public class FastLineRenderer : MonoBehaviour
    {
        /// <summary>
        /// Maximum number of lines (quads) per mesh
        /// </summary>
        public const int MaxLinesPerMesh = 16250;

        /// <summary>
        /// Maximum vertices per mesh
        /// </summary>
        public const int MaxVerticesPerMesh = 65000;

        /// <summary>
        /// Maximum indices per mesh
        /// </summary>
        public const int MaxIndicesPerMesh = 97500;

        /// <summary>
        /// Number of vertices per line / quad
        /// </summary>
        public const int VerticesPerLine = 4;

        /// <summary>
        /// Contains indices that allow rendering quads with a mesh, using QuadUV* uv coordinates. Array is MaxIndicesPerMesh in size, allowing
        /// you to pull out that exact amount of indices you need into a new array. Unity really needs to provide index and count parameters
        /// to really optimize the use of this array.
        /// </summary>
        public static readonly int[] QuadIndices = new int[MaxIndicesPerMesh];

        /// <summary>
        /// Quad UV1
        /// </summary>
        public static readonly Vector2 QuadUV1 = new Vector2(0.0f, 0.0f);

        /// <summary>
        /// Quad UV2
        /// </summary>
        public static readonly Vector2 QuadUV2 = new Vector2(1.0f, 0.0f);

        /// <summary>
        /// Quad UV3
        /// </summary>
        public static readonly Vector2 QuadUV3 = new Vector2(0.0f, 1.0f);

        /// <summary>
        /// Quad UV4
        /// </summary>
        public static readonly Vector2 QuadUV4 = new Vector2(1.0f, 1.0f);

        private static int mainTexId;
        private static int mainTexStartCapId;
        private static int mainTexEndCapId;
        private static int mainTexRoundJoinId;
        private static int glowTexId;
        private static int glowTexStartCapId;
        private static int glowTexEndCapId;
        private static int glowTexRoundJoinId;
        private static int uvxScaleId;
        private static int uvyScaleId;
        private static int uvxScaleGlowId;
        private static int uvyScaleGlowId;
        private static int tintColorId;
        private static int glowTintColorId;
        private static int glowIntensityMultiplierId;
        private static int glowWidthMultiplierId;
        private static int glowLengthMultiplierId;
        private static int jitterMultiplierId;
        private static int turbulenceMultiplierId;
        private static int time2Id;

        private const int defaultListCapacity = 256;
        private static readonly HashSet<FastLineRenderer> currentLineRenderers = new HashSet<FastLineRenderer>();
        private static readonly LinkedList<FastLineRenderer> cache = new LinkedList<FastLineRenderer>();
        private readonly List<Mesh> meshes = new List<Mesh>();
        private readonly List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
        private readonly List<List<Vector4>> texCoordsAndGlowLists = new List<List<Vector4>>(new[] { new List<Vector4>(defaultListCapacity) });
        private readonly List<List<Vector3>> verticesLists = new List<List<Vector3>>(new[] { new List<Vector3>(defaultListCapacity) });
        private readonly List<List<Vector4>> lineDirsLists = new List<List<Vector4>>(new[] { new List<Vector4>(defaultListCapacity) });
        private readonly List<List<Color32>> colorsLists = new List<List<Color32>>(new[] { new List<Color32>(defaultListCapacity) });
        private readonly List<List<Vector3>> endsLists = new List<List<Vector3>>(new[] { new List<Vector3>(defaultListCapacity) });
        private readonly List<List<Vector4>> lifeTimesLists = new List<List<Vector4>>(new[] { new List<Vector4>(defaultListCapacity) });
        private readonly List<Bounds> boundsList = new List<Bounds>(new[] { new Bounds() });
        private readonly List<Vector3> path = new List<Vector3>();

        private Vector3? lastPoint;
        private int listIndex;
        private List<Vector4> texCoordsAndGlow;
        private List<Vector3> vertices;
        private List<Vector4> lineDirs;
        private List<Color32> colors;
        private List<Vector3> velocities;
        private List<Vector4> lifeTimes;
        private Vector3 currentBoundsMin;
        private Vector3 currentBoundsMax;

        [Tooltip("Material to render the lines with")]
        public Material Material;
        private Material previousMaterial;

        [Tooltip("Whether to clone the material or use a reference to the material. Cloning can be useful if you want the texture and other material global parameters " +
            "to be unique per script. Using a reference may help increase batching and reduce draw calls if you are using the same texture and aren't changing the global " +
            "material parameters per script.")]
        public bool CloneMaterial = true;

        [Tooltip("Camera. Defaults to main camera.")]
        public Camera Camera;

        [Range(0.0f, 64.0f)]
        [Tooltip("Line UV X Scale. If not 1, Ensure your material texture is set to repeat. Applies globally in the Material rather than per vertex.")]
        public float LineUVXScale = 1.0f;

        [Range(0.0f, 64.0f)]
        [Tooltip("Line UV Y Scale. If not 1, Ensure your material texture is set to repeat. Applies globally in the Material rather than per vertex.")]
        public float LineUVYScale = 1.0f;

        [Range(0.0f, 64.0f)]
        [Tooltip("Line glow UV X Scale. If not 1, Ensure your material glow texture is set to repeat. Applies globally in the Material rather than per vertex.")]
        public float GlowUVXScale = 1.0f;

        [Range(0.0f, 64.0f)]
        [Tooltip("Line glow UV Y Scale. IF not 1, Ensure your material glow texture is set to repeat. Applies globally in the Material rather than per vertex.")]
        public float GlowUVYScale = 1.0f;

        [Tooltip("Tint color. Applies globally to all lines and is applied in addition to individual line colors.")]
        public Color32 TintColor = Color.white;

        [Tooltip("Glow color. Applies globally in the Material rather than per vertex.")]
        public Color32 GlowColor = Color.blue;

        [Range(0.0f, 3.0f)]
        [Tooltip("Glow itensity multiplier. Applies globally in the Material rather than per vertex.")]
        public float GlowIntensityMultiplier = 1.0f;

        [Range(0.0f, 16.0f)]
        [Tooltip("Glow width multiplier. Applies globally in the Material rather than per vertex.")]
        public float GlowWidthMultiplier = 1.0f;

        [Range(0.0f, 2.0f)]
        [Tooltip("Glow length multiplier. Applies globally in the Material rather than per vertex.")]
        public float GlowLengthMultiplier = 0.4f;

        [Tooltip("Jitter multiplier. Applies globally in the Material rather than per vertex.")]
        public float JitterMultiplier;

        [Tooltip("Turbulence. Requires lines to have LifeTime setup and works only at runtime. Applies globally in the Material rather than per vertex.")]
        public float Turbulence;

        [Tooltip("Amount to scale the mesh by. If you aren't using GPU properties that modify position (i.e. turbulence, velocity and angular velocity) you can leave as 1. " +
            "If you are using GPU properties, you will want to assign a value that is large enough to scale the mesh size so that vertices are visible for the lifetime of the lines.")]
        public Vector3 BoundsScale = Vector3.one;

        [Tooltip("Line texture")]
        public Texture2D LineTexture;

        [Tooltip("Line texture - start cap")]
        public Texture2D LineTextureStartCap;

        [Tooltip("Line texture - end cap")]
        public Texture2D LineTextureEndCap;

        [Tooltip("Line texture - round join")]
        public Texture2D LineTextureRoundJoin;

        [Tooltip("Glow texture")]
        public Texture2D GlowTexture;

        [Tooltip("Glow texture - start cap")]
        public Texture2D GlowTextureStartCap;

        [Tooltip("Glow texture - end cap")]
        public Texture2D GlowTextureEndCap;

        [Tooltip("Glow texture - round join")]
        public Texture2D GlowTextureRoundJoin;

        // editable via FastLineRendererEditor
        [HideInInspector]
        [Tooltip("Sort layer")]
        public string SortLayerName;

        // editable via FastLineRendererEditor
        [HideInInspector]
        [Tooltip("Order in sort layer")]
        public int SortOrderInLayer;

        [Tooltip("Initial set of lines. Leave empty if you are generating your lines in script.")]
        public LineGroupList[] InitialLineGroups;

        static FastLineRenderer()
        {
            int vertexIndex = 0;
            int index = 0;
            while (vertexIndex != MaxVerticesPerMesh)
            {
                QuadIndices[index++] = vertexIndex++;
                QuadIndices[index++] = vertexIndex++;
                QuadIndices[index++] = vertexIndex;
                QuadIndices[index++] = vertexIndex--;
                QuadIndices[index++] = vertexIndex;
                QuadIndices[index++] = (vertexIndex += 2);
                vertexIndex++;
            }
        }

        private void CreateNewSetOfLists()
        {
            texCoordsAndGlow = new List<Vector4>(defaultListCapacity);
            vertices = new List<Vector3>(defaultListCapacity);
            lineDirs = new List<Vector4>(defaultListCapacity);
            colors = new List<Color32>(defaultListCapacity);
            velocities = new List<Vector3>(defaultListCapacity);
            lifeTimes = new List<Vector4>(defaultListCapacity);

            texCoordsAndGlowLists.Add(texCoordsAndGlow);
            verticesLists.Add(vertices);
            lineDirsLists.Add(lineDirs);
            colorsLists.Add(colors);
            endsLists.Add(velocities);
            lifeTimesLists.Add(lifeTimes);

            listIndex = verticesLists.Count - 1;
        }

        private void AssignLists()
        {
            texCoordsAndGlow = texCoordsAndGlowLists[listIndex];
            vertices = verticesLists[listIndex];
            lineDirs = lineDirsLists[listIndex];
            colors = colorsLists[listIndex];
            velocities = endsLists[listIndex];
            lifeTimes = lifeTimesLists[listIndex];
            currentBoundsMin = boundsList[listIndex].min;
            currentBoundsMax = boundsList[listIndex].max;
        }

        private void UpdateCurrentLists()
        {
            if (vertices.Count == MaxVerticesPerMesh)
            {
                boundsList[listIndex] = new Bounds(Vector3.zero, currentBoundsMax - currentBoundsMin);
                ResetCurrentBounds();

                if (listIndex >= verticesLists.Count - 1)
                {
                    CreateNewSetOfLists();
                }
                else
                {
                    listIndex++;
                    AssignLists();
                }
            }
        }

        private void ResetCurrentBounds()
        {
            currentBoundsMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            currentBoundsMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        }

        private void Cleanup()
        {
            foreach (Mesh mesh in meshes)
            {
                if (mesh != null)
                {
                    mesh.triangles = null;
                    mesh.vertices = null;
                    mesh.colors = null;
                    mesh.tangents = null;
                    mesh.normals = null;
                    mesh.uv = null;
                    mesh.uv2 = null;
                    mesh.uv3 = null;
                    mesh.uv4 = null;
                }
            }
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = false;
                }
            }
            foreach (var list in verticesLists)
            {
                list.Clear();
            }
            foreach (var list in texCoordsAndGlowLists)
            {
                list.Clear();
            }
            foreach (var list in lineDirsLists)
            {
                list.Clear();
            }
            foreach (var list in colorsLists)
            {
                list.Clear();
            }
            foreach (var list in endsLists)
            {
                list.Clear();
            }
            foreach (var list in lifeTimesLists)
            {
                list.Clear();
            }
            for (int i = 0; i < boundsList.Count; i++)
            {
                boundsList[i] = new Bounds();
            }
            ResetCurrentBounds();
            while (gameObject.transform.childCount != 0)
            {
                GameObject obj = gameObject.transform.GetChild(0).gameObject;
                if (obj != null)
                {
                    GameObject.DestroyImmediate(obj);
                }
            }
            meshes.Clear();
            meshRenderers.Clear();

            listIndex = 0;
            lastPoint = null;

            AssignLists();
        }

        private void ApplyListsToMeshes(bool optimize)
        {
            boundsList[listIndex] = new Bounds(Vector3.zero, currentBoundsMax - currentBoundsMin);

            for (int index = 0; index <= listIndex; index++)
            {
                EnsureMeshCount(index);
                Mesh mesh = meshes[index];
                List<Vector3> vertices = verticesLists[index];
                mesh.SetVertices(vertices);
                mesh.SetTangents(lineDirsLists[index]);
                mesh.SetColors(colorsLists[index]);
                mesh.SetUVs(0, texCoordsAndGlowLists[index]);
                mesh.SetNormals(endsLists[index]);
                mesh.SetUVs(1, lifeTimesLists[index]);

                int indicesCount = (int)(vertices.Count * 1.5f);
                if (indicesCount == MaxIndicesPerMesh)
                {
                    mesh.triangles = QuadIndices;
                }
                else
                {
                    int[] indicesArray = new int[indicesCount];
                    Array.Copy(QuadIndices, 0, indicesArray, 0, indicesCount);
                    mesh.triangles = indicesArray;
                }
                if (optimize)
                {
                    mesh.Optimize();
                }
                Bounds b = boundsList[index];
                Vector3 s = b.size;
                s.x *= BoundsScale.x;
                s.y *= BoundsScale.y;
                s.z *= BoundsScale.z;
                b.center = Vector3.zero;
                b.max = s + new Vector3(2.0f, 2.0f, 2.0f); // extra padding just in case of extra glow and optimized bounds function
                mesh.bounds = b;
                MeshRenderer r = meshRenderers[index];
                r.enabled = true;
                r.sharedMaterial = Material;
                r.sortingLayerName = SortLayerName;
                r.sortingOrder = SortOrderInLayer;
            }
        }

        private void CreateInitialLines()
        {
            if (InitialLineGroups == null || InitialLineGroups.Length == 0)
            {
                return;
            }

            bool changes = false;
            FastLineRendererProperties props = new FastLineRendererProperties();
            foreach (LineGroupList list in InitialLineGroups)
            {
                if (list.Points == null || list.Points == null || list.Points.List == null || list.Points.List.Count == 0)
                {
                    continue;
                }

                bool first = true;
                Vector3 offset = list.Offset;
                int lastIndex = list.Points.List.Count - 1;
                props.LineJoin = list.LineJoin;

                for (int i = 1; i < list.Points.List.Count; i++)
                {
                    changes = true;
                    Vector3 nextPoint = list.Points.List[i];
                    props.Radius = list.LineRadius;
                    props.Color = list.LineColor;
                    props.GlowWidthMultiplier = list.GlowWidthMultiplier;
                    props.GlowIntensityMultiplier = list.GlowIntensity;

                    if (first)
                    {
                        first = false;
                        props.Start = list.Points.List[i - 1] + offset;
                        props.End = nextPoint + offset;
                        if (list.Continuous)
                        {
                            if (list.AddStartCap)
                            {
                                props.LineType = FastLineRendererLineSegmentType.StartCap;
                                StartLine(props);
                            }
                            else
                            {
                                AddLine(props);
                            }
                        }
                        else
                        {
                            AddLine(props, list.AddStartCap, list.AddEndCap);
                            first = true;
                            i++;
                        }
                    }
                    else
                    {
                        props.Start = nextPoint + offset;
                        if (list.AddEndCap && i == lastIndex)
                        {
                            props.LineType = FastLineRendererLineSegmentType.EndCap;
                            EndLine(props);
                        }
                        else
                        {
                            AppendLine(props);
                        }
                    }
                }
            }
            if (changes)
            {
                Apply();
            }
        }

        private void CheckInitialLines()
        {

#if UNITY_EDITOR

            // detect increase in array size and reset last item to default instead of Unity's stupid cloning of last item
            if (!Application.isPlaying)
            {
                Cleanup();
                CreateInitialLines();
            }
            else

#endif

            {
                if (InitialLineGroups != null && InitialLineGroups.Length != 0)
                {
                    CreateInitialLines();
                    InitialLineGroups = null;
                }
            }
        }

        private void UpdateCamera()
        {
            if (Camera == null)
            {
                Camera = Camera.main;
                if (Camera == null)
                {
                    Camera = Camera.current;
                }
            }
        }

        private void UpdateMaterial()
        {
            if (Material == null)
            {
                previousMaterial = null;
                return;
            }
            else if (Material != previousMaterial)
            {
                Material = (CloneMaterial ? new Material(Material) : Material);
                previousMaterial = Material;
                foreach (MeshRenderer meshRenderer in meshRenderers)
                {
                    meshRenderer.sharedMaterial = Material;
                }
            }

            Material.SetTexture(mainTexId, LineTexture);
            Material.SetTexture(mainTexStartCapId, LineTextureStartCap);
            Material.SetTexture(mainTexEndCapId, LineTextureEndCap);
            Material.SetTexture(mainTexRoundJoinId, LineTextureRoundJoin);
            Material.SetTexture(glowTexId, GlowTexture);
            Material.SetTexture(glowTexStartCapId, GlowTextureStartCap);
            Material.SetTexture(glowTexEndCapId, GlowTextureEndCap);
            Material.SetTexture(glowTexRoundJoinId, GlowTextureRoundJoin);
            Material.SetFloat(uvxScaleId, LineUVXScale);
            Material.SetFloat(uvyScaleId, LineUVYScale);
            Material.SetFloat(uvxScaleGlowId, GlowUVXScale);
            Material.SetFloat(uvyScaleGlowId, GlowUVYScale);
            Material.SetColor(tintColorId, TintColor);
            Material.SetColor(glowTintColorId, GlowColor);
            Material.SetFloat(glowIntensityMultiplierId, GlowIntensityMultiplier);
            Material.SetFloat(glowWidthMultiplierId, GlowWidthMultiplier);
            Material.SetFloat(glowLengthMultiplierId, GlowLengthMultiplier);
            Material.SetFloat(jitterMultiplierId, JitterMultiplier);
            Material.SetFloat(turbulenceMultiplierId, Turbulence);

            float t = Time.timeSinceLevelLoad;
            Material.SetVector(time2Id, new Vector4(t / 20.0f, t, t * 2.0f, t * 3.0f));
        }

        private void ResetVariables()
        {
            LineUVXScale = LineUVYScale = GlowUVXScale = GlowUVYScale = 1.0f;
            GlowIntensityMultiplier = 0.4f;
            GlowWidthMultiplier = 1.0f;
            GlowLengthMultiplier = 1.0f;
            TintColor = UnityEngine.Color.white;
            GlowColor = UnityEngine.Color.blue;
            BoundsScale = Vector3.one;
            JitterMultiplier = 1.0f;
            Turbulence = 0.0f;
            CloneMaterial = true;
        }

        private void AssignMaterialIds()
        {
            if (mainTexId != 0)
            {
                return;
            }

            mainTexId = Shader.PropertyToID("_MainTex");
            mainTexStartCapId = Shader.PropertyToID("_MainTexStartCap");
            mainTexEndCapId = Shader.PropertyToID("_MainTexEndCap");
            mainTexRoundJoinId = Shader.PropertyToID("_MainTexRoundJoin");
            glowTexId = Shader.PropertyToID("_GlowTex");
            glowTexStartCapId = Shader.PropertyToID("_GlowTexStartCap");
            glowTexEndCapId = Shader.PropertyToID("_GlowTexEndCap");
            glowTexRoundJoinId = Shader.PropertyToID("_GlowTexRoundJoin");
            uvxScaleId = Shader.PropertyToID("_UVXScale");
            uvyScaleId = Shader.PropertyToID("_UVYScale");
            uvxScaleGlowId = Shader.PropertyToID("_UVXScaleGlow");
            uvyScaleGlowId = Shader.PropertyToID("_UVYScaleGlow");
            tintColorId = Shader.PropertyToID("_TintColor");
            glowTintColorId = Shader.PropertyToID("_GlowTintColor");
            glowIntensityMultiplierId = Shader.PropertyToID("_GlowIntensityMultiplier");
            glowWidthMultiplierId = Shader.PropertyToID("_GlowWidthMultiplier");
            glowLengthMultiplierId = Shader.PropertyToID("_GlowLengthMultiplier");
            jitterMultiplierId = Shader.PropertyToID("_JitterMultiplier");
            turbulenceMultiplierId = Shader.PropertyToID("_Turbulence");
            time2Id = Shader.PropertyToID("_Time2");
        }

        private void Awake()
        {
            AssignMaterialIds();
            currentLineRenderers.Add(this);
            ResetCurrentBounds();
            AssignLists();
            CheckInitialLines();
        }

        private void Update()
        {

#if UNITY_EDITOR

            CheckInitialLines();

#endif

            UpdateCamera();
            UpdateMaterial();
        }

        private void OnDestroy()
        {
            currentLineRenderers.Remove(this);
            Cleanup();
            gameObject.transform.parent = null;
            cache.Clear();
        }

        private void SetupMeshRenderer(MeshRenderer meshRenderer)
        {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.useLightProbes = false;
            meshRenderer.receiveShadows = false;
            meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            meshRenderer.enabled = false;
        }

        private GameObject CreateMeshObject()
        {
            GameObject obj = new GameObject();
            obj.name = "FastLineRendererMesh";
            obj.hideFlags = HideFlags.HideAndDontSave;
            obj.transform.parent = gameObject.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.useLightProbes = false;
            meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            meshRenderer.receiveShadows = false;
            meshRenderers.Add(meshRenderer);
            SetupMeshRenderer(meshRenderer);

            return obj;
        }

        private T[] GetArray<T>(List<T> list, int index, int count)
        {
            T[] array = new T[count];
            list.CopyTo(index, array, 0, count);

            return array;
        }

        private void EnsureMeshCount(int index)
        {
            if (index >= meshes.Count)
            {
                Mesh m = new Mesh();
                CreateMeshObject().GetComponent<MeshFilter>().sharedMesh = m;
                meshes.Add(m);
            }
        }

        private IEnumerator SendToCacheAfterCoRoutine(TimeSpan elapsed)
        {
            yield return new WaitForSeconds((float)elapsed.TotalSeconds);

            SendToCache();
        }

        private void UpdateBounds(ref Vector3 point1, ref Vector3 point2)
        {
            // r = y + ((x - y) & ((x - y) >> (sizeof(int) * CHAR_BIT - 1))); // min(x, y)
            // r = x - ((x - y) & ((x - y) >> (sizeof(int) * CHAR_BIT - 1))); // max(x, y)

            unchecked
            {
                {
                    int xCalculation = (int)point1.x - (int)point2.x;
                    xCalculation &= (xCalculation >> 31);
                    int xMin = (int)point2.x + xCalculation;
                    int xMax = (int)point1.x - xCalculation;
                    xCalculation = (int)currentBoundsMin.x - xMin;
                    xCalculation &= (xCalculation >> 31);
                    currentBoundsMin.x = xMin + xCalculation;
                    xCalculation = (int)currentBoundsMax.x - xMax;
                    xCalculation &= (xCalculation >> 31);
                    currentBoundsMax.x = (int)currentBoundsMax.x - xCalculation;
                }
                {
                    int yCalculation = (int)point1.y - (int)point2.y;
                    yCalculation &= (yCalculation >> 31);
                    int yMin = (int)point2.y + yCalculation;
                    int yMax = (int)point1.y - yCalculation;
                    yCalculation = (int)currentBoundsMin.y - yMin;
                    yCalculation &= (yCalculation >> 31);
                    currentBoundsMin.y = yMin + yCalculation;
                    yCalculation = (int)currentBoundsMax.y - yMax;
                    yCalculation &= (yCalculation >> 31);
                    currentBoundsMax.y = (int)currentBoundsMax.y - yCalculation;
                }
                {
                    int zCalculation = (int)point1.z - (int)point2.z;
                    zCalculation &= (zCalculation >> 31);
                    int zMin = (int)point2.z + zCalculation;
                    int zMax = (int)point1.z - zCalculation;
                    zCalculation = (int)currentBoundsMin.z - zMin;
                    zCalculation &= (zCalculation >> 31);
                    currentBoundsMin.z = zMin + zCalculation;
                    zCalculation = (int)currentBoundsMax.z - zMax;
                    zCalculation &= (zCalculation >> 31);
                    currentBoundsMax.z = (int)currentBoundsMax.z - zCalculation;
                }
            }
        }

        private void AddLineInternal(FastLineRendererProperties props, ref Vector4 dirStart, ref Vector4 dirEnd, FastLineRendererLineSegmentType type)
        {
            int lineType = (int)type;

            UpdateCurrentLists();

            Vector4 texCoordAndGlow = new Vector4(((int)QuadUV1.x | lineType), QuadUV1.y, props.GlowWidthMultiplier, props.GlowIntensityMultiplier);
            lastPoint = props.End;

            dirStart.w = props.Radius;
            vertices.Add(props.Start);
            texCoordsAndGlow.Add(texCoordAndGlow);
            lineDirs.Add(dirStart);
            colors.Add(props.Color);
            velocities.Add(props.Velocity);
            lifeTimes.Add(props.LifeTime);

            dirEnd.w = dirStart.w;
            texCoordAndGlow.x = ((int)QuadUV2.x | lineType);
            texCoordAndGlow.y = QuadUV2.y;
            vertices.Add(props.End);
            texCoordsAndGlow.Add(texCoordAndGlow);
            lineDirs.Add(dirEnd);
            colors.Add(props.Color);
            velocities.Add(props.Velocity);
            lifeTimes.Add(props.LifeTime);

            dirStart.w = -props.Radius;
            texCoordAndGlow.x = ((int)QuadUV3.x | lineType);
            texCoordAndGlow.y = QuadUV3.y;
            vertices.Add(props.Start);
            texCoordsAndGlow.Add(texCoordAndGlow);
            lineDirs.Add(dirStart);
            colors.Add(props.Color);
            velocities.Add(props.Velocity);
            lifeTimes.Add(props.LifeTime);

            dirEnd.w = dirStart.w;
            texCoordAndGlow.x = ((int)QuadUV4.x | lineType);
            texCoordAndGlow.y = QuadUV4.y;
            vertices.Add(props.End);
            texCoordsAndGlow.Add(texCoordAndGlow);
            lineDirs.Add(dirEnd);
            colors.Add(props.Color);
            velocities.Add(props.Velocity);
            lifeTimes.Add(props.LifeTime);

            UpdateBounds(ref props.Start, ref props.End);
        }

        private void AppendLineInternal(FastLineRendererProperties props)
        {
            Vector3 prev = lastPoint.Value;
            Vector4 dir = (props.Start - prev);
            props.End = props.Start;

            if (props.LineJoin == FastLineRendererLineJoin.Round)
            {
                // add a line for the join
                Vector3 radius = new Vector3(props.Radius, 0.0f, 0.0f);
                Vector3 end = props.End;
                Vector4 dirRound = new Vector4(props.Radius + props.Radius, 0.0f, 0.0f, 0.0f);
                float glowIntensity = props.GlowIntensityMultiplier;
                props.GlowIntensityMultiplier = 0.0f;
                props.Start = prev - radius;
                props.End = prev + radius;
                AddLineInternal(props, ref dirRound, ref dirRound, FastLineRendererLineSegmentType.RoundJoin);

                // add the regular line
                props.GlowIntensityMultiplier = glowIntensity;
                props.LineJoin = FastLineRendererLineJoin.None;
                props.Start = prev;
                props.End = end;
                AddLineInternal(props, ref dir, ref dir, props.LineType);

                // restore the join
                props.LineJoin = FastLineRendererLineJoin.Round;
            }
            else if (props.LineJoin == FastLineRendererLineJoin.AdjustPosition)
            {
                // move the start position back to approximate a join
                Vector3 offset = new Vector3(dir.x, dir.y, dir.z).normalized * props.Radius;
                prev -= offset;
                props.Start = prev;
                AddLineInternal(props, ref dir, ref dir, props.LineType);
            }
            else if (props.LineJoin == FastLineRendererLineJoin.AttachToPrevious)
            {
                // use the previous end direction for the start direction
                Vector4 prevDir = lineDirs[lineDirs.Count - 1];
                props.Start = prev;
                AddLineInternal(props, ref prevDir, ref dir, props.LineType);
            }
            else
            {
                // no adjustment
                props.Start = prev;
                AddLineInternal(props, ref dir, ref dir, props.LineType);
            }
        }

        private bool AddStartCapLine(FastLineRendererProperties props)
        {
            if (props.LineType == FastLineRendererLineSegmentType.StartCap)
            {
                // make it square
                Vector3 start = props.Start;
                Vector3 end = props.End;
                Vector3 dirNorm = (props.End - props.Start).normalized;
                float width = props.Radius + props.Radius;
                props.Start = start - (dirNorm * width);
                props.End = start;
                Vector4 dir = (props.End - props.Start);
                AddLineInternal(props, ref dir, ref dir, props.LineType);
                props.Start = start;
                props.End = end;
                props.LineType = FastLineRendererLineSegmentType.Full;

                return true;
            }

            return false;
        }

        private bool AddEndCapLine(FastLineRendererProperties props)
        {
            if (props.LineType == FastLineRendererLineSegmentType.EndCap)
            {
                FastLineRendererLineJoin prevJoin = props.LineJoin;
                props.LineJoin = FastLineRendererLineJoin.None;

                // make it square
                Vector4 prevDir = lineDirs[lineDirs.Count - 1];
                Vector3 dirNorm = new Vector3(prevDir.x, prevDir.y, prevDir.z).normalized;
                props.Start = lastPoint.Value + (dirNorm * (props.Radius + props.Radius));
                AppendLineInternal(props);
                props.LineJoin = prevJoin;
                props.LineType = FastLineRendererLineSegmentType.Full;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Reset all FastLineRenderer objects in the scene - this is fast and does not rely on the slow Unity Find* methods.
        /// </summary>
        public static void ResetAll()
        {
            foreach (FastLineRenderer r in currentLineRenderers)
            {
                r.Reset();
            }
        }

        /// <summary>
        /// Set the capacity of all internal lists.
        /// </summary>
        /// <param name="capacity"></param>
        public void SetCapacity(int capacity)
        {
            capacity = Mathf.Clamp(capacity, 0, MaxVerticesPerMesh);

            foreach (var list in verticesLists)
            {
                list.Capacity = capacity;
            }
            foreach (var list in texCoordsAndGlowLists)
            {
                list.Capacity = capacity;
            }
            foreach (var list in lineDirsLists)
            {
                list.Capacity = capacity;
            }
            foreach (var list in colorsLists)
            {
                list.Capacity = capacity;
            }
            foreach (var list in endsLists)
            {
                list.Capacity = capacity;
            }
            foreach (var list in lifeTimesLists)
            {
                list.Capacity = capacity;
            }
        }

        /// <summary>
        /// Determines if lineCount lines can be added without creating a new mesh
        /// </summary>
        /// <param name="lineCount">Line count. Receives the actual available line count before a new mesh will be created.</param>
        /// <returns>True if lines can be added without creating a new mesh, false otherwise</returns>
        public bool CanAddLines(ref int lineCount)
        {
            int currentLines = (vertices.Count % MaxVerticesPerMesh) / VerticesPerLine; 
            lineCount = MaxLinesPerMesh - currentLines;

            return (lineCount > 0);
        }

        /// <summary>
        /// Adds an individual line that is not meant to be joined to other lines and will have no caps
        /// </summary>
        /// <param name="props">Creation properties</param>
        /// <param name="startCap">Whether to add a start cap</param>
        /// <param name="endCap">Whether to add an end cap</param>
        public void AddLine(FastLineRendererProperties props, bool startCap = false, bool endCap = false)
        {
            if (startCap)
            {
                props.LineType = FastLineRendererLineSegmentType.StartCap;
                AddStartCapLine(props);
            }

            Vector4 dir = (props.End - props.Start);
            AddLineInternal(props, ref dir, ref dir, FastLineRendererLineSegmentType.Full);

            if (endCap)
            {
                props.LineType = FastLineRendererLineSegmentType.EndCap;
                AddEndCapLine(props);
            }
        }

        /// <summary>
        /// Start a line. Start and End must be set on the properties. Set LineType to StartCap to cap.
        /// </summary>
        /// <param name="props">Creation properties</param>
        public void StartLine(FastLineRendererProperties props)
        {
            FastLineRendererLineJoin prevJoin = props.LineJoin;
            props.LineJoin = FastLineRendererLineJoin.None;

            if (AddStartCapLine(props))
            {
                props.LineType = FastLineRendererLineSegmentType.Full;
            }
            Vector4 dir = (props.End - props.Start);
            AddLineInternal(props, ref dir, ref dir, props.LineType);

            props.LineJoin = prevJoin;
        }

        /// <summary>
        /// Append a line - End value of props is ignored, use Start field instead.
        /// </summary>
        /// <param name="props">Creation properties</param>
        /// <returns>True if line was joined to a previous line, false if no previous line available</returns>
        public bool AppendLine(FastLineRendererProperties props)
        {
            // if no previous line, fallback to AddLine
            if (lastPoint == null || vertices.Count == 0)
            {
                if (lastPoint == null)
                {
                    lastPoint = props.Start;
                }
                else
                {
                    props.End = props.Start;
                    props.Start = lastPoint.Value;
                    AddLine(props);
                }
                return false;
            }

            props.LineType |= FastLineRendererLineSegmentType.Full;
            AppendLineInternal(props);

            return true;
        }

        /// <summary>
        /// Ends a line. End does not need to be set on the props. Set LineType to EndCap to cap.
        /// </summary>
        /// <param name="props">Creation properties</param>
        /// <returns>True if success, false if a line hasn't been started yet</returns>
        public bool EndLine(FastLineRendererProperties props)
        {
            if (lastPoint == null || vertices.Count == 0)
            {
                return false;
            }

            FastLineRendererLineSegmentType type = props.LineType;
            props.LineType = FastLineRendererLineSegmentType.Full;
            AppendLineInternal(props);
            props.LineType = type;

            AddEndCapLine(props);

            lastPoint = null;

            return true;
        }

        /// <summary>
        /// Append a quad/bezier curve to the fast line renderer. The line curves from props.Start to props.End, using the two control points to curve.
        /// </summary>
        /// <param name="props">Line properties</param>
        /// <param name="ctr1">Control point 1</param>
        /// <param name="ctr2">Control point 2</param>
        /// <param name="numberOfSegments">Number of segments. The higher the better quality but more CPU and GPU usage.</param>
        /// <param name="startCap">Whether to add a start cap</param>
        /// <param name="endCap">Whether to add an end cap</param>
        public void AppendCurve(FastLineRendererProperties props, Vector3 ctr1, Vector3 ctr2, int numberOfSegments, bool startCap, bool endCap)
        {
            PathGenerator.Is2D = Camera.orthographic;
            FastLineRendererLineJoin prevJoin = props.LineJoin;
            props.LineJoin = FastLineRendererLineJoin.AttachToPrevious;
            props.LineType = FastLineRendererLineSegmentType.Full;
            PathGenerator.CreateCurve(path, props.Start, props.End, ctr1, ctr2, numberOfSegments, 0.0f);
            int index = 0;
            int lastIndexMinusOne = path.Count - 1;

            if (startCap)
            {
                index = 2;
                props.Start = path[0];
                props.End = path[1];
                props.LineType = FastLineRendererLineSegmentType.StartCap;
                StartLine(props);
            }

            for (; index < path.Count; index++)
            {
                props.Start = path[index];
                if (endCap && index == lastIndexMinusOne)
                {
                    props.LineType = FastLineRendererLineSegmentType.EndCap;
                    EndLine(props);
                }
                else
                {
                    AppendLine(props);
                }
            }

            path.Clear();
            props.LineJoin = prevJoin;
        }

        /// <summary>
        /// Append a spline to the fast line renderer. Start and End in props is ignored.
        /// </summary>
        /// <param name="props">Line properties</param>
        /// <param name="points">Points for the spline to follow</param>
        /// <param name="numberOfSegments">Total number of line segments for the spline. The higher this number, the higher quality, but more CPU / GPU time.</param>
        /// <param name="flags">Flags determining how the spline behaves</param>
        /// <returns>True if success, false if points length is too small</returns>
        public bool AppendSpline(FastLineRendererProperties props, IList<Vector3> points, int numberOfSegments, FastLineRendererSplineFlags flags)
        {
            PathGenerator.Is2D = Camera.orthographic;
            bool closePath = (flags & FastLineRendererSplineFlags.ClosePath) == FastLineRendererSplineFlags.ClosePath;
            bool startCap = (flags & FastLineRendererSplineFlags.StartCap) == FastLineRendererSplineFlags.StartCap;
            bool endCap = (flags & FastLineRendererSplineFlags.EndCap) == FastLineRendererSplineFlags.EndCap;

            if (!PathGenerator.CreateSpline(path, points, numberOfSegments, closePath))
            {
                return false;
            }

            int index = 0;
            int lastIndexMinusOne = path.Count - 1;

            FastLineRendererLineJoin prevJoin = props.LineJoin;
            props.LineJoin = FastLineRendererLineJoin.AttachToPrevious;
            props.LineType = FastLineRendererLineSegmentType.Full;

            if (startCap)
            {
                index = 2;
                props.Start = path[0];
                props.End = path[1];
                props.LineType = FastLineRendererLineSegmentType.StartCap;
                StartLine(props);
            }

            for (; index < path.Count; index++)
            {
                props.Start = path[index];
                if (endCap && index == lastIndexMinusOne)
                {
                    props.LineType = FastLineRendererLineSegmentType.EndCap;
                    EndLine(props);
                }
                else
                {
                    AppendLine(props);
                }
            }

            path.Clear();
            props.LineJoin = prevJoin;

            return true;
        }

        /// <summary>
        /// Apply all line creations
        /// </summary>
        /// <param name="optimize">True to optimize the mesh, false otherwise. This can provide increased draw performance at a slight one-time CPU cost.</param>
        /// <returns>True if success, false if error</returns>
        public bool Apply(bool optimize = false)
        {
            try
            {
                ApplyListsToMeshes(optimize);
                return true;
            }
            catch
            {
                Cleanup();
                return false;
            }
        }

        /// <summary>
        /// Reset everything, remove all lines
        /// </summary>
        public void Reset()
        {
            Cleanup();
            InitialLineGroups = new LineGroupList[] { LineGroupList.Default() };
        }

        /// <summary>
        /// Copy the properties of this fast line renderer to another fast line renderer
        /// </summary>
        /// <param name="other">FastLineRenderer to copy to</param>
        public void CopyTo(FastLineRenderer other)
        {
            other.BoundsScale = BoundsScale;
            other.Camera = Camera;
            other.CloneMaterial = CloneMaterial;
            other.TintColor = TintColor;
            other.GlowColor = GlowColor;
            other.GlowIntensityMultiplier = GlowIntensityMultiplier;
            other.GlowLengthMultiplier = GlowLengthMultiplier;
            other.GlowWidthMultiplier = GlowWidthMultiplier;
            other.GlowTexture = GlowTexture;
            other.GlowTextureStartCap = GlowTextureStartCap;
            other.GlowTextureEndCap = GlowTextureEndCap;
            other.GlowTextureRoundJoin = GlowTextureRoundJoin;
            other.GlowUVXScale = GlowUVXScale;
            other.GlowUVYScale = GlowUVYScale;
            other.JitterMultiplier = JitterMultiplier;
            other.LineTexture = LineTexture;
            other.LineTextureStartCap = LineTextureStartCap;
            other.LineTextureEndCap = LineTextureEndCap;
            other.LineTextureRoundJoin = LineTextureRoundJoin;
            other.LineUVXScale = LineUVXScale;
            other.LineUVYScale = LineUVYScale;
            other.Material = Material;
            other.SortLayerName = SortLayerName;
            other.SortOrderInLayer = SortOrderInLayer;
            other.Turbulence = Turbulence;
        }

        /// <summary>
        /// Reset and then add this FastLineRenderer to the cache
        /// </summary>
        public void SendToCache()
        {
            Reset();
            cache.AddLast(this);
        }

        /// <summary>
        /// Send this FastLineRenderer back to the cache after a certain time
        /// </summary>
        /// <param name="elapsed">Time to wait before sending to the cache</param>
        public void SendToCacheAfter(TimeSpan elapsed)
        {
            StartCoroutine(SendToCacheAfterCoRoutine(elapsed));
        }

        /// <summary>
        /// Create a new FastLineRenderer or retrieve from cache if available
        /// </summary>
        /// <param name="parent">Parent</param>
        /// <param name="template">Template fast line renderer to use for properties of the new or cached fast line renderer object. Must not be null.
        /// You probably want to create a fast line renderer in you scene view, get it looking how you want, disable it, and then pass that as the template.</param>
        /// <returns>FastLineRenderer from cache or new</returns>
        public static FastLineRenderer CreateWithParent(GameObject parent, FastLineRenderer template)
        {
            FastLineRenderer r;
            if (cache.Count == 0)
            {
                GameObject obj = new GameObject();
                obj.name = "FastLineRenderer_" + Guid.NewGuid().ToString("N");
                obj.hideFlags = HideFlags.HideAndDontSave;
                obj.transform.parent = (parent == null ? null : parent.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
                r = obj.AddComponent<FastLineRenderer>();
                r.Awake();
            }
            else
            {
                r = cache.First.Value;
                cache.RemoveFirst();
            }
            r.InitialLineGroups = null;
            template.CopyTo(r);
            r.Update();
            r.gameObject.transform.parent = (parent == null ? null : parent.transform);

            return r;
        }
    }
}