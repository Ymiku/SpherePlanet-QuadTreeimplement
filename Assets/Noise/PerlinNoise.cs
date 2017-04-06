using UnityEngine;

public static class PerlinNoise 
{
    public static float [,] GetPerlinNoiseMapArray(int mapWidth,int mapHeight,float scale,int octaves,float persistance,float lacunarity)
    {
        float [,] PerlinNoiseMap=new float [mapWidth, mapHeight];
        
        float min=float.MaxValue;
        float max=float.MinValue;
        
        for(int y=0;y<mapHeight;y++)
        {
            for(int x=0;x<mapWidth;x++)
            {
                float amplitude=1;
                float frequency=1;
                float noiseValue=0;
                
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale*frequency;
                    float sampleY = y / scale*frequency;
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

                    // Debug.Log(PerlinNoiseMap[x,y]);
                }
            }
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
