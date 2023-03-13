using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {
	[Header("KeyBinds")]
		public KeyCode menuKey = KeyCode.Escape;
		public KeyCode inventoryKey = KeyCode.Tab;
	Dictionary<string, GameObject> objs = new Dictionary<string, GameObject>();
	Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();
	GameObject activePanel;

	#region scripts
		GameCanvas gameCv;
		MenuCanvas menuCv;
	#endregion
	void Start() {
		foreach (Transform panel in gameObject.transform) {
			panels[panel.gameObject.name] = panel.gameObject;
		}
		objs["player"] = GameObject.Find("Player");
		objs["camera"] = objs["player"].transform.Find("CameraHolder").GetChild(0).gameObject;
		EnablePanel("Game");
	}
	public void PlayPause(bool b) {

	} 
	void Update() {
		if (Input.GetKeyDown(inventoryKey)) InventoryPanel();
		if (Input.GetKeyDown(menuKey)) MenuPanel();
	}
	void UnlockPlayer(bool enable) {
		objs["player"].GetComponents<PlayerMovement>()[0].enabled = enable;
		objs["player"].GetComponents<PlayerInteractions>()[0].enabled = enable;
		objs["camera"].GetComponents<CameraController>()[0].enabled = enable;
	}
	void EnablePanel(string panelName) {
		if (activePanel != null)
			activePanel.SetActive(false);
		activePanel = panels[panelName];
		activePanel.SetActive(true);
	}
	#region Panels
		void MenuPanel() {
			if (activePanel.name == "Game") {
				UnlockPlayer(false);
				EnablePanel("Menu");
			}
			else {
				UnlockPlayer(true);
				EnablePanel("Game");
			}
		}
		void InventoryPanel() {
			if (activePanel.name == "Game") {
				UnlockPlayer(false);
				EnablePanel("Inventory");
			}
			else {
				UnlockPlayer(true);
				EnablePanel("Game");
			}
		}
		public void EditPanel(BlockHandler block) {
			if (activePanel.name == "Game") {
				UnlockPlayer(false);
				EnablePanel("Edit");
			}
		}
	#endregion
}