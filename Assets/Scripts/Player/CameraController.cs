using UnityEngine;

public class CameraController : MonoBehaviour {
	[Header("Orientation")]
		[SerializeField, Range(1, 10)] int sensX = 5;
		[SerializeField, Range(1, 10)] int sensY = 5;
		[SerializeField] bool invertX = false;
		[SerializeField] bool invertY = false;

	float xRotation;
	float yRotation;
	Transform orientation;

	void Start() {
		Cursor.lockState = CursorLockMode.Locked;
		orientation = transform.parent.gameObject.transform.parent.gameObject.transform.Find("Orientation");
	}

	void Update() {
		if (invertX) sensX *= -1;
		if (invertY) sensY *= -1;

		yRotation += Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX * 100;
		xRotation = Mathf.Clamp(xRotation - Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY * 100, -90f, 90f);

		transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
		orientation.rotation = Quaternion.Euler(0, yRotation, 0);
	}
}