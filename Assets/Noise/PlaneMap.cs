using UnityEngine;
[System.Serializable]
public class PlaneMap
{
    public TextureType textureType=TextureType.Grayscale;
    
    public int mapWidth;
    
    public int mapHeight;
    
    public float scale;
    
    public int octaves;
    
    public float persistance;
    
    public float lacunarity;
    
    public Renderer planeRenderer;
    
    public TerrainType [] terrainType;
	float [,] PerlinNoiseMap;
    public void GenerateMap()
    {
        PerlinNoiseMap=PerlinNoise.GetPerlinNoiseMapArray(mapWidth,mapHeight,scale,octaves,persistance,lacunarity);
		return;
        Texture2D texture=new Texture2D(mapWidth,mapHeight);
        
        switch (textureType)
        {
            case TextureType.Grayscale:
            texture=TextureGenerater.Instance.GenerateGrayscaleTexture(mapWidth,mapHeight,PerlinNoiseMap);
            
            planeRenderer.sharedMaterial.mainTexture=texture;
            planeRenderer.transform.localScale=new Vector3(mapWidth,1,mapHeight);
            
            break;
            
            case TextureType.colorful:
            texture=TextureGenerater.Instance.GenerateColorfulTexture(mapWidth,mapHeight,PerlinNoiseMap,terrainType);
            
            planeRenderer.sharedMaterial.mainTexture=texture;
            planeRenderer.transform.localScale=new Vector3(mapWidth,1,mapHeight);
            
            break;
        }
    }
	public float[] GetHeightMap()
	{
		return TextureGenerater.Instance.GenerateHeightMap(mapWidth,mapHeight,PerlinNoiseMap);
	}
	public Texture2D GetColorfulTex()
	{
		Texture2D texture=new Texture2D(mapWidth,mapHeight);
		texture=TextureGenerater.Instance.GenerateColorfulTexture(mapWidth,mapHeight,PerlinNoiseMap,terrainType);
		return texture;
	}
}

public enum TextureType
{
    Grayscale=0,
    colorful=1,
}

[System.SerializableAttribute]
public struct TerrainType
{
     public string name;
     
     public Color color;
     
     public float height;
}


