using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	[Header("KeyBinds")]
		public KeyCode jumpKey = KeyCode.Space;
		public KeyCode crouchKey = KeyCode.LeftShift;
	
	const float crouchRatio = .75f;
	const float moveSpeed = 7;
	const float groundDrag = 5;
	const float airMovementMultiplier = .5f;
	
	Rigidbody rb;
	Transform orientation;
	Vector3[] dimensions;
	int[] jumpForces = new int[] {6, 10};
	int jForce;
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
				jForce = jumpForces[1];
			}
			else if (!Input.GetKey(crouchKey)) {
				transform.localScale = dimensions[0];
				groudnDist = dimensions[0].y+.1f;
				jForce = jumpForces[0];
			}
		#endregion

		#region jump controller
			if (Input.GetKey(jumpKey) && Physics.Raycast(transform.position, Vector3.down, groudnDist)) {
				rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
				rb.AddForce(transform.up * jForce, ForceMode.Impulse);
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