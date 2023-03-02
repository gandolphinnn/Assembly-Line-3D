using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	[Header("Movement")]
		[SerializeField] float moveSpeed;
		[SerializeField] float groundDrag;
		[SerializeField, Range(1, 15f)] public int jumpForce = 8;
		[SerializeField, Range(.1f, 1f)] public float airMovementMultiplier = .5f;

	[Header("KeyBinds")]
		public KeyCode jumpKey = KeyCode.Space;
		public KeyCode crouchKey = KeyCode.LeftShift;
	
	Rigidbody rb;
	Transform orientation;
	Vector3[] dimensions;
	float crouchRatio = .75f;
	float groudnDist;
	
	void Start () {
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		orientation = transform.Find("Orientation").gameObject.transform;
		dimensions = new Vector3[] {transform.localScale, new Vector3(transform.localScale.x, transform.localScale.y * crouchRatio, transform.localScale.z)};
		groudnDist = dimensions[0].y+.1f;
	}
	void FixedUpdate () {
		#region crouching
			if (Input.GetKey(crouchKey)) {
				transform.localScale = dimensions[1];
				groudnDist = dimensions[1].y+.1f;
			}
			else if (!Input.GetKey(crouchKey)) {
				transform.localScale = dimensions[0];
				groudnDist = dimensions[0].y+.1f;
			}
		#endregion

		#region jump controller
			if (Input.GetKey(jumpKey) && Physics.Raycast(transform.position, Vector3.down, groudnDist)) {
				rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
				rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
			}
		#endregion

		#region limit velocity
			Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
			if (flatVel.magnitude > moveSpeed) {
				Vector3 limitedVel = flatVel.normalized * moveSpeed;
				rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
			}
		#endregion

		#region player movement controller
			Vector2 playerInput;
			playerInput.x = Input.GetAxisRaw("Horizontal");
			playerInput.y = Input.GetAxisRaw("Vertical");
			Vector3 moveDirection = orientation.forward * playerInput.y + orientation.right * playerInput.x;
			if (Physics.Raycast(transform.position, Vector3.down, groudnDist)) {
				rb.drag = groundDrag;
				rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
			}
			else {
				rb.drag = 0;
				rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMovementMultiplier, ForceMode.Force);
			}
		#endregion
	}		
}