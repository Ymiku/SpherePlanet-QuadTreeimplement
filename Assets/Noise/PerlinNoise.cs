using UnityEngine;
namespace QTPlanetUtility{
	public static class PerlinNoise 
	{
		public static int GetSeedX(int s)
		{
			return s * 17;
		}
		public static int GetSeedY(int s)
		{
			return s * 23;
		}

		public static float [,] GetPerlinNoiseMapArray(int mapWidth,int mapHeight,float scale,int octaves,float persistance,float lacunarity,int seed =10)
	    {
	        float [,] PerlinNoiseMap=new float [mapWidth, mapHeight];
	        
			float min = float.MaxValue;
	        float max = float.MinValue;
			int seedX = GetSeedX(seed);
			int seedY = GetSeedY(seed);
	        for(int y=0;y<mapHeight;y++)
	        {
	            for(int x=0;x<mapWidth;x++)
	            {
	                float amplitude=1;
	                float frequency=1;
	                float noiseValue=0;
	                for (int i = 0; i < octaves; i++)
	                {
						float sampleX = (x+seedX) / scale*frequency;
						float sampleY = (y+seedY) / scale*frequency;
						float pointHeight = MathExtra.PerlinNoise(sampleX, sampleY)*2-1;
	                    
	                    noiseValue+=pointHeight*amplitude;

	                    PerlinNoiseMap[x, y] = noiseValue;
	                    
	                    amplitude*=persistance;
	                    frequency*=lacunarity;

	                    if (noiseValue < min)
	                    {
	                        min = noiseValue;
	                    }
	                    else if (noiseValue > max)
	                    {
	                        max = noiseValue;
	                    }

	                    
	                }
	            }
	        }
			for (int i = 0; i < mapHeight; i++) {
				PerlinNoiseMap [0, i] = -1f;
				PerlinNoiseMap [mapWidth-1, i] = -1f;
			}
			for (int i = 0; i < mapWidth; i++) {
				PerlinNoiseMap [i, 0] = -1f;
				PerlinNoiseMap [i, mapHeight-1] = -1f;
			}
	        for (int y = 0; y < mapHeight; y++)
	        {
	            for (int x = 0; x < mapWidth; x++)
	            {
	                PerlinNoiseMap[x,y]=Mathf.InverseLerp(min,max,PerlinNoiseMap[x,y]);
	            }
	        }
	        
	        
	        return PerlinNoiseMap;
	    } 
	}
}