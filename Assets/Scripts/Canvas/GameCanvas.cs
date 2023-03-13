using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCanvas : MonoBehaviour {
	[Header("Icons")]
    	[SerializeField] Texture2D playImg;
		[SerializeField] Texture2D pauseImg;

	RawImage activeBlockImg;
	TextMeshProUGUI questTMP;
	RawImage playPauseImg;
	TextMeshProUGUI dollarTMP;
	Slider lvlSlider;
	void Start() {
		activeBlockImg = transform.Find("Active Block").gameObject.GetComponent<RawImage>();
		questTMP = transform.Find("Quest Info").gameObject.GetComponent<TextMeshProUGUI>();
		playPauseImg = transform.Find("Play Pause").gameObject.GetComponent<RawImage>();
		dollarTMP = transform.Find("Dollars Amount").gameObject.GetComponent<TextMeshProUGUI>();
		lvlSlider = transform.Find("LVL").gameObject.transform.Find("Slider").gameObject.GetComponent<Slider>();
	}
	public void ChangeActiveBlock(Texture2D activeImg) {
		activeBlockImg.texture = activeImg;
	}
	public void DisplayQuest(string description, int val, int tot) {
		questTMP.text = $"{description} ({val}/{tot})";
	}
	public void PlayPause(bool isPlay) {
		if (isPlay)
			playPauseImg.texture = playImg;
		else
			playPauseImg.texture = pauseImg;
	}
	public void DollarsAmount(int amount) {
		dollarTMP.text = $"${amount}";
	}
	public void LevelBar(float val, float tot) {
		lvlSlider.value = val/tot;
	}
}