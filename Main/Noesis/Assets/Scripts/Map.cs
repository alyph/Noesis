using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour 
{
	public Texture2D ProvinceMap;

	// Use this for initialization
	void Start () 
	{
		ProcessProvinceMap();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	private void ProcessProvinceMap()
	{
		int palatteTexSize = 128;
		int padding = 3;
		int provincePerRow = palatteTexSize / padding;

		List<Color32> colorids = new List<Color32>();
		List<Color32> uvs = new List<Color32>();
		Color32[] colors = ProvinceMap.GetPixels32();

		for (int i = 0; i < colors.Length; i++)
		{
			Color32 color = colors[i];

			if (color.r == 255 && color.g == 255 && color.b == 255)
				continue;

			int found = -1;
			for (int c = 0; c < colorids.Count; c++)
			{
				Color32 colorid = colorids[c];
				if (colorid.r == color.r && colorid.g == color.g && colorid.b == color.b)
				{
					found = c;
					break;
				}
			}

			if (found < 0)
			{
				colorids.Add(color);
				found = colorids.Count - 1;
				float u = (float)((found % provincePerRow) * padding + padding / 2) / palatteTexSize;
				float v = (float)(found / provincePerRow) / palatteTexSize;
				Color32 uvColor = new Color32((byte)(u * 255), (byte)(v * 255), 0, 255);
				uvs.Add(uvColor);
			}

			colors[i] = uvs[found];
		}


		Texture2D textureProv = new Texture2D(ProvinceMap.width, ProvinceMap.height, TextureFormat.RGBA32, false);
		textureProv.wrapMode = TextureWrapMode.Clamp;
		textureProv.filterMode = FilterMode.Point;
		textureProv.SetPixels32(colors);
		textureProv.Apply();
		renderer.material.SetTexture("_ProvinceMap", textureProv);


		Texture2D texture = new Texture2D(palatteTexSize, palatteTexSize, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Point;
		Color32[] palatteColors = new Color32[palatteTexSize * palatteTexSize];
		texture.SetPixels32(palatteColors);
		texture.Apply();

		renderer.material.SetTexture("_ProvincePalatte", texture);
	}
}
