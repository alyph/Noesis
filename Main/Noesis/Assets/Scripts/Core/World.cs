using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Province CreateProvince(ProvinceInitData initData)
	{
		var province = new Province();
		province.DisplayName = initData.DisplayName;
		Provinces.Add(province);
		province.Init(this);
		return province;
	}

	private List<Province> Provinces = new List<Province>();
}
