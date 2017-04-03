using UnityEngine;

public class TextureGenerater : Singleton<TextureGenerater>
{
	public float[] GenerateHeightMap(int mapWidth, int mapHeight, float[,] PerlinNosiseMap)
	{
		float[] map = new float[mapWidth * mapHeight];

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				map[(mapHeight-y-1)* mapWidth + x] = Mathf.Lerp(0f,1f, PerlinNosiseMap[x, y]);
			}
		}

		return map;
	}
    public Texture2D GenerateGrayscaleTexture(int mapWidth, int mapHeight, float[,] PerlinNosiseMap)
    {
        Texture2D texture = new Texture2D(mapWidth, mapHeight);

        Color[] colors = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
				colors[(mapHeight-y-1)* mapWidth + x] = Color.Lerp(Color.black, Color.white, PerlinNosiseMap[x, y]);
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }

    public Texture2D GenerateColorfulTexture(int mapWidth, int mapHeight, float[,] PerlinNosiseMap,TerrainType [] terrainType)
    {
        Texture2D texture = new Texture2D(mapWidth, mapHeight);

        Color[] colors = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //noise height 0.6
                float height=PerlinNosiseMap[x,y];
                
                // Debug.Log(height);
                
                for(int i=0;i<terrainType.Length;i++)
                {
                    if(height>=terrainType[i].height)
                    {
                        colors[y*mapWidth+x]=terrainType[i].color;
                    }
                    if(height<=0.1&&height>=0)
                    {
                        colors[y*mapWidth+x]=terrainType[0].color;
                    }
                }
            }
        }
        
        texture.SetPixels(colors);

        texture.wrapMode=TextureWrapMode.Clamp;
        
        texture.filterMode=FilterMode.Point;
        
        texture.Apply();

        return texture;
    }
}
