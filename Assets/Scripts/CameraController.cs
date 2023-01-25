using UnityEngine;

public class CameraController : MonoBehaviour {
	[Header("Position")]
		public Transform cameraPosition;
		public GameObject cameraHolder;

	[Header("Orientation")]
		[SerializeField, Range(1, 10)] private int sensX;
		[SerializeField, Range(1, 10)] private int sensY;
		[SerializeField] private bool invertX = false;
		[SerializeField] private bool invertY = false;

	float xRotation;
	float yRotation;
	public Transform orientation;

	void Start() {
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update() {
		cameraHolder.transform.position = cameraPosition.position;

		if (invertX)
			sensX *= -1;
		if (invertY)
			sensY *= -1;
		float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX * 100;
		float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY * 100;

		yRotation += mouseX;
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
		orientation.rotation = Quaternion.Euler(0, yRotation, 0);
	}
}