using UnityEngine;
using QTPlanetUtility;
namespace QTPlanetUtility{
	[System.Serializable]
	public class PlaneMap
	{
	    [HideInInspector]
	    public int mapWidth;
		[HideInInspector]
	    public int mapHeight;
		[HideInInspector]
	    public float scale;
	    
	    public int octaves;
	    
	    public float persistance;
	    
	    public float lacunarity;
	    
	    public TerrainType [] terrainType;
		float [,] PerlinNoiseMap;
		public void GenerateMap(int seed)
	    {
	        PerlinNoiseMap=PerlinNoise.GetPerlinNoiseMapArray(mapWidth,mapHeight,scale,octaves,persistance,lacunarity,seed);
			return;
	    }
		public float[] GetHeightMap()
		{
			float[] map = new float[mapWidth * mapHeight];

			for (int y = 0; y < mapHeight; y++)
			{
				for (int x = 0; x < mapWidth; x++)
				{
					map[(mapHeight-y-1)* mapWidth + x] = Mathf.Lerp(0f,1f, PerlinNoiseMap[x, y]);
				}
			}
			return map;
		}
	}
		

	[System.SerializableAttribute]
	public struct TerrainType
	{
	     public string name;
	     
	     public Color color;
	     
	     public float height;
	}

}
