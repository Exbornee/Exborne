using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    private Mesh _mesh;

    private Vector3[] _vertices;
    private int[] _triangles;
    private Color[] _colors;
    private Color[] _gradientColors;
    private float[] _gradientColorTime;

    [SerializeField] private int XSize = 10;
    [SerializeField] private int ZSize = 10;

    [SerializeField] private int XOffset;
    [SerializeField] private int ZOffset;

    [SerializeField] private float NoiseScale = 0.03f;
    [SerializeField] private float HeightMultiplier = 7;

    public int TextureWidth = 1024;
    public int TextureHeight = 1024;

    public float Noise01Scale = 2f;
    public float Noise01Amp = 2f;

    public float Noise02Scale = 4f;
    public float Noise02Amp = 4f;

    public float Noise03Scale = 6f;
    public float Noise03Amp = 6f;

    public Gradient MeshGradient;
    public Texture2D Texture2D;

    private float _minTerrainHeight;
    private float _maxTerrainHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        // GetColors();

        CreateShape();
    }

    private void GetColors()
    {
        _gradientColors = new Color[10001];
        _gradientColorTime = new float[10001];
        int i = 0;
        for (float f = 0f; f < 1f; f += 0.0001f)
        {
            _gradientColors[i] = MeshGradient.Evaluate(f);
            _gradientColorTime[i++] = f;
        }

        Color[] pixel = Texture2D.GetPixels();
        int ti = 0;
        // for (int y = 0; y <= Texture2D.height; y++)
        // {
        //     for (int x = 0; x <= Texture2D.width; x++)
        //     {
        //         pixel[ti] = Texture2D.GetPixel(x, y);
        //         Debug.Log(pixel[ti++]);
        //     }
        // }

        for (ti = 0; ti < pixel.Length; ti++)
        {
            for (int gi = 0; gi < _gradientColors.Length; gi++)
            {
                if ((pixel[ti].r == _gradientColors[gi].r) && (pixel[ti].g == _gradientColors[gi].g) && (pixel[ti++].b == _gradientColors[gi].b))
                {
                    float fy = _gradientColorTime[gi];
                }
            }
        }
    }

    private void Update()
    {
        // UpdateMesh();
        CreateShape();
    }

    private void CreateShape()
    {
        _vertices = new Vector3[(XSize + 1) * (ZSize + 1)];

        // This for loop adds the vertices to each location based on each X and Z axis
        for (int i = 0, z = 0; z <= ZSize; z++)
        {
            for (int x = 0; x <= XSize; x++)
            {
                // float y = 0;
                // int tx = Texture2D.height - 1;
                // int ty = 0;
                // for (float u = x; u <= (x + 1) / (float)XSize; u += 0.001f)
                // {
                //     for (float v = z; v <= (z + 1) / (float)ZSize; v += 0.001f)
                //     {
                //         Color pixel = Texture2D.GetPixel(tx, ty);
                //         float total = 0;
                //         total += pixel.r;
                //         total += pixel.g * 2f;
                //         total += pixel.b * 4f;
                //         y = total;
                //         if (ty < Texture2D.width - 1)
                //         {
                //             ty++;
                //         }
                //     }
                //     if (tx > 0)
                //     {
                //         tx--;
                //     }
                // }

                float y = Mathf.PerlinNoise((x + XOffset) * NoiseScale, (z + ZOffset) * NoiseScale) * HeightMultiplier;
                _vertices[i] = new Vector3(x, y, z);

                if (y > _maxTerrainHeight)
                {
                    _maxTerrainHeight = y;
                }
                if (y < _minTerrainHeight)
                {
                    _minTerrainHeight = y;
                }

                i++;
            }
        }

        _triangles = new int[XSize * ZSize * 6];

        for (int z = 0, vert = 0, tris = 0; z < ZSize; z++)
        {
            for (int x = 0; x < XSize; x++)
            {
                _triangles[tris + 0] = vert + 0;
                _triangles[tris + 1] = vert + XSize + 1;
                _triangles[tris + 2] = vert + 1;
                _triangles[tris + 3] = vert + 1;
                _triangles[tris + 4] = vert + XSize + 1;
                _triangles[tris + 5] = vert + XSize + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }

        _colors = new Color[_vertices.Length];

        for (int i = 0, z = 0; z <= ZSize; z++)
        {
            for (int x = 0; x <= XSize; x++)
            {
                float height = Mathf.InverseLerp(_minTerrainHeight, _maxTerrainHeight, _vertices[i].y);
                _colors[i] = MeshGradient.Evaluate(height);
                i++;
            }
        }


        _mesh.Clear();

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.colors = _colors;

        _mesh.RecalculateNormals();
    }

    private void UpdateMesh()
    {
    }
}
