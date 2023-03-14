using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ItemSystem : MonoBehaviour {
	public MaterialType[] materialList; //? store  type of item in the game
	public ItemType[] itemList; //? store  type of item in the game
	List<ItemHandler> handlers = new List<ItemHandler>();

	public GameObject Spawn(int itemId, Transform parent, int rotIndex, int matId = 0) {
		ItemType type = itemList[itemId];
		GameObject go = Instantiate(type.prefab, parent.position, Quaternion.Euler(0, rotIndex*90, 0), parent);
		handlers.Add(new ItemHandler(type, go));
		if (type.applyMaterial) {
			go.name = $"{materialList[matId].name} {go.name}";
			if (go.transform.childCount == 0)
				go.GetComponent<MeshRenderer>().material = materialList[matId].mat;
			else 
				go.transform.GetChild(0).GetComponent<MeshRenderer>().material = materialList[matId].mat;
		}
		return go;
	}
	public void TransferObj(List<GameObject> objList, Transform destination) {
		CBlock dScript = destination.gameObject.GetComponent<CBlock>();
		if (objList.Count == 0) return;			
		foreach (GameObject obj in objList) {
			obj.transform.position += obj.transform.forward;
			obj.transform.rotation = destination.rotation;
			obj.transform.SetParent(destination);
		}
		dScript.ReceiveObjs(objList);
	}
}
public class ItemHandler {
	public ItemType type;
	public GameObject gObj; //? defined when the item is created
	public ItemHandler(ItemType type, GameObject gObj) {
		this.type = type;
		this.gObj = gObj;
		this.gObj.name = type.name;
	}
}
[Serializable]
public struct ItemType {
	public string name;
	public GameObject prefab;
	public Texture2D icon;
	public bool applyMaterial;
}
[Serializable]
public struct MaterialType {
	public string name;
	public Material mat;
}