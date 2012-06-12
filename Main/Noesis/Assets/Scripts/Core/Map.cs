using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Map : MonoBehaviour 
{
	public Texture2D ProvinceMap;
	public List<ProvinceInitData> ProvinceInits = new List<ProvinceInitData>();

	public float Width { get; private set; }
	public float Height { get; private set; }


	public Province SelectedProvince
	{
		get { return SelectedProvinceIdx >= 0 ? Provinces[SelectedProvinceIdx] : null; }
	}

	// Use this for initialization
	void Start () 
	{
		World = GetComponent<World>();

		Width = transform.localScale.x * 10;
		Height = transform.localScale.z * 10;

		SelectedProvinceIdx = -1;

		ProcessProvinceMap();
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateProvinceSelection();
	}

	void OnGUI()
	{
		GUI.Box(new Rect(10, 10, 300, 500), "");
		if (SelectedProvince != null)
			GUI.Label(new Rect(20, 20, 200, 30), SelectedProvince.DisplayName);
	
	}

	private void ProcessProvinceMap()
	{

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
				//float u = (float)((found % ProvincesPerRow) * PalattePadding + PalattePadding / 2) / PalatteTexSize;
				//float v = (float)(found / ProvincesPerRow) / PalatteTexSize;
				byte u = (byte)(found % PalatteTexSize);
				byte v = (byte)(found / PalatteTexSize);
				Color32 uvColor = new Color32(u, v, 0, 255);
				uvs.Add(uvColor);

				var provinceInit = ProvinceInits.First(p => IsSameColorId(p.ColorId, color));
				var province = World.CreateProvince(provinceInit);
				Provinces.Add(province);
			}

			colors[i] = uvs[found];
		}

		Texture2D texProv = new Texture2D(ProvinceMap.width, ProvinceMap.height, TextureFormat.RGBA32, false);
		texProv.wrapMode = TextureWrapMode.Clamp;
		texProv.filterMode = FilterMode.Point;
		texProv.SetPixels32(colors);
		texProv.Apply();
		renderer.material.SetTexture("_ProvinceMap", texProv);

		Texture2D texPalatte = new Texture2D(PalatteTexSize, PalatteTexSize, TextureFormat.RGBA32, false);
		texPalatte.wrapMode = TextureWrapMode.Clamp;
		texPalatte.filterMode = FilterMode.Point;
		Color32[] palatteColors = new Color32[PalatteTexSize * PalatteTexSize];
		texPalatte.SetPixels32(palatteColors);
		texPalatte.Apply();
		renderer.material.SetTexture("_ProvincePalatte", texPalatte);

		ProvinceMap = texProv;
		ProvincePalatte = texPalatte;
	}

	private void UpdateProvinceSelection()
	{
		if (Input.GetMouseButtonUp(0))
		{
			Vector3 mousePos = Input.mousePosition;
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
			float u = Mathf.Clamp(worldPos.x / Width + 0.5f, 0.0f, 1.0f);
			float v = Mathf.Clamp(worldPos.z / Height + 0.5f, 0.0f, 1.0f);
			var provinceColor = ProvinceMap.GetPixel((int)(ProvinceMap.width * u), (int)(ProvinceMap.height * v));
			int provinceIdx = ProvinceColorToIndex(provinceColor);
			ChangeSelectedProvince(provinceIdx);
		}
	}

	private void ChangeSelectedProvince(int _new_idx)
	{
		if (SelectedProvinceIdx != _new_idx && _new_idx >= 0 && _new_idx < Provinces.Count)
		{
			if (SelectedProvinceIdx >= 0)
				SetProvinceColor(SelectedProvinceIdx, new Color(0, 0, 0, 0));

			if (_new_idx >= 0)
				SetProvinceColor(_new_idx, new Color(1, 1, 1, 0.8f));

			SelectedProvinceIdx = _new_idx;
		}
	}

	private bool IsSameColorId(Color32 id1, Color32 id2)
	{
		return id1.r == id2.r && id1.g == id2.g && id1.b == id2.b;
	}

	private int ProvinceColorToIndex(Color color)
	{
		byte u = (byte)(color.r * 255);
		byte v = (byte)(color.g * 255);
		return v * PalatteTexSize + u;
	}

	private void SetProvinceColor(int provinceIdx, Color color)
	{
		int x = provinceIdx % PalatteTexSize;
		int y = provinceIdx / PalatteTexSize;
		ProvincePalatte.SetPixel(x, y, color);
		ProvincePalatte.Apply();
	}

	const int PalatteTexSize = 64;
	//const int PalattePadding = 2;
	//const int ProvincesPerRow = PalatteTexSize / PalattePadding;

	private World World;
	private List<Province> Provinces = new List<Province>();
	private int SelectedProvinceIdx = -1;
	private Texture2D ProvincePalatte;
}

[Serializable]
public class ProvinceInitData
{
	public Color32 ColorId;
	public string DisplayName;
}
