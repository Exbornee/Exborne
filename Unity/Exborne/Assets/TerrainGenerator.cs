using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private TerrainData _terrainData;
    public Texture2D Texture2D;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [MenuItem("Terrain/Apply Heightmap/From Grayscale Texture")]
    static void ApplyHeightmapFromTexture()
    {
        // string heightmapPath = EditorUtility.OpenFilePanel("Texture", System.IO.Directory.GetCurrentDirectory(), ".png");

        Texture2D heightmap = Selection.activeObject as Texture2D;
        if (heightmap == null)
        {
            EditorUtility.DisplayDialog("No texture selected", "Please select a texture.", "Cancel");
            return;
        }

        var terrain = Terrain.activeTerrain.terrainData;
        int w = heightmap.width;
        int h = heightmap.height;
        int w2 = terrain.heightmapResolution;
        float[,] heightmapData = terrain.GetHeights(0, 0, w2, w2);
        Color[] mapColors = heightmap.GetPixels();
        Color[] map = new Color[w2 * w2];

        if (w2 != w || h != w)
        {
            // Resize using nearest-neighbor scaling if texture has no filtering
            if (heightmap.filterMode == FilterMode.Point)
            {
                float dx = (float)w / (float)w2;
                float dy = (float)h / (float)w2;
                for (int y = 0; y < w2; y++)
                {
                    if (y % 20 == 0)
                    {
                        EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0f, w2, y));
                    }

                    int thisY = Mathf.FloorToInt(dy * y) * w;
                    int yw = y * w2;
                    for (int x = 0; x < w2; x++)
                    {
                        map[yw + x] = mapColors[Mathf.FloorToInt(thisY + dx * x)];
                    }
                }
            }
            // Otherwise resize using bilinear filtering
            else
            {
                float ratioX = (1.0f / ((float)w2 / (w - 1)));
                float ratioY = (1.0f / ((float)w2 / (h - 1)));
                for (int y = 0; y < w2; y++)
                {
                    if (y % 20 == 0)
                    {
                        EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0f, w2, y));
                    }

                    int yy = Mathf.FloorToInt(y * ratioY);
                    int y1 = yy * w;
                    int y2 = (yy + 1) * w;
                    int yw = y * w2;

                    for (int x = 0; x < w2; x++)
                    {
                        int xx = Mathf.FloorToInt(x * ratioX);

                        Color bl = mapColors[y1 + xx];
                        Color br = mapColors[y1 + xx + 1];
                        Color tl = mapColors[y2 + xx];
                        Color tr = mapColors[y2 + xx + 1];

                        float xLerp = x * ratioX - xx;
                        map[yw + x] = Color.Lerp(Color.Lerp(bl, br, xLerp), Color.Lerp(tl, tr, xLerp), y * ratioY - (float)yy);
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }
        else
        {
            // Use original if no resize is needed
            map = mapColors;
        }

        // Assign texture data to heightmap

        for (int y = 0; y < w2; y++)
        {
            for (int x = 0; x < w2; x++)
            {
                heightmapData[y, x] = map[y * w2 + x].grayscale;
            }
        }

        terrain.SetHeights(0, 0, heightmapData);
    }

    [MenuItem("Terrain/Apply Heightmap/From Heatmap Texture")]
    static void ApplyHeightmapFromHeatmap()
    {
        //string heightmapPath = EditorUtility.OpenFilePanel("Texture", System.IO.Directory.GetCurrentDirectory(), ".png");

        Texture2D heightmap = Selection.activeObject as Texture2D;
        if (heightmap == null)
        {
            EditorUtility.DisplayDialog("No texture selected", "Please select a texture.", "Cancel");
            return;
        }

        var terrain = Terrain.activeTerrain.terrainData;
        int w = heightmap.width;
        int h = heightmap.height;
        int w2 = terrain.heightmapResolution;
        float[,] heightmapData = terrain.GetHeights(0, 0, w2, w2);
        Color[] mapColors = heightmap.GetPixels();
        Color[] map = new Color[w2 * w2];

        if (w2 != w || h != w)
        {
            // Resize using nearest-neighbor scaling if texture has no filtering
            if (heightmap.filterMode == FilterMode.Point)
            {
                float dx = (float)w / (float)w2;
                float dy = (float)h / (float)w2;
                for (int y = 0; y < w2; y++)
                {
                    if (y % 20 == 0)
                    {
                        EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0f, w2, y));
                    }

                    int thisY = Mathf.FloorToInt(dy * y) * w;
                    int yw = y * w2;
                    for (int x = 0; x < w2; x++)
                    {
                        map[yw + x] = mapColors[Mathf.FloorToInt(thisY + dx * x)];
                    }
                }
            }
            // Otherwise resize using bilinear filtering
            else
            {
                float ratioX = (1.0f / ((float)w2 / (w - 1)));
                float ratioY = (1.0f / ((float)w2 / (h - 1)));
                for (int y = 0; y < w2; y++)
                {
                    if (y % 20 == 0)
                    {
                        EditorUtility.DisplayProgressBar("Resize", "Calculating texture", Mathf.InverseLerp(0.0f, w2, y));
                    }

                    int yy = Mathf.FloorToInt(y * ratioY);
                    int y1 = yy * w;
                    int y2 = (yy + 1) * w;
                    int yw = y * w2;

                    for (int x = 0; x < w2; x++)
                    {
                        int xx = Mathf.FloorToInt(x * ratioX);

                        Color bl = mapColors[y1 + xx];
                        Color br = mapColors[y1 + xx + 1];
                        Color tl = mapColors[y2 + xx];
                        Color tr = mapColors[y2 + xx + 1];

                        float xLerp = x * ratioX - xx;
                        map[yw + x] = Color.Lerp(Color.Lerp(bl, br, xLerp), Color.Lerp(tl, tr, xLerp), y * ratioY - (float)yy);
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }
        else
        {
            // Use original if no resize is needed
            map = mapColors;
        }

        // Assign texture data to heightmap

        for (int y = 0; y < w2; y++)
        {
            for (int x = 0; x < w2; x++)
            {
                var max = Mathf.Max(Mathf.Max(map[y * w2 + x].r, map[y * w2 + x].g), map[y * w2 + x].b);
                var min = Mathf.Min(Mathf.Min(map[y * w2 + x].r, map[y * w2 + x].g), map[y * w2 + x].b);
                var lightness = (max + min) / 2f;
                var invertedLight = 1f - lightness;
                float total = 0;
                total += map[y * w2 + x].r;
                total -= map[y * w2 + x].g / 1.5f;
                total -= map[y * w2 + x].b / 32f;
                if (map[y * w2 + x].r >= map[y * w2 + x].g && map[y * w2 + x].r > map[y * w2 + x].b)
                {
                    total += map[y * w2 + x].r / 32f;
                    total += invertedLight / 2f;
                    if (map[y * w2 + x].r >= 0.5f && map[y * w2 + x].g < 0.33f)
                    {
                        total += (map[y * w2 + x].r - map[y * w2 + x].g) / 4f;
                    }
                    total /= 1.2f;
                }
                if (map[y * w2 + x].g >= map[y * w2 + x].b && map[y * w2 + x].g > map[y * w2 + x].r)
                {
                    total += map[y * w2 + x].g / 16f;
                    total += invertedLight / 2f;
                }
                if (map[y * w2 + x].b >= map[y * w2 + x].g && map[y * w2 + x].b > map[y * w2 + x].r)
                {
                    total += map[y * w2 + x].b / 16f;
                    total += invertedLight / 16f;
                }

                if (map[y * w2 + x].b >= map[y * w2 + x].g && map[y * w2 + x].b > map[y * w2 + x].r && lightness < 0.5f)
                {
                    heightmapData[y, x] = 0;
                }
                else
                {
                    heightmapData[y, x] = total / 16f;
                }
            }
        }

        terrain.SetHeights(0, 0, heightmapData);
    }
}
