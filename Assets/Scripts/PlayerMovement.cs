using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	[Header("Movement")]
		public Transform orientation;
		public float moveSpeed;
		public float groundDrag;
		public int maxJumps;
		[SerializeField, Range(1, 15f)] public int jumpForce = 8;
		[SerializeField, Range(.1f, 1f)] public float airMovementMultiplier = .5f;

	[Header("Keybinds")]
		public KeyCode jumpKey = KeyCode.Space;
		public KeyCode crouchKey = KeyCode.LeftShift;

	[Header("Ground Check")]
		public float groudnDist;

	bool crouched = false;
	bool grounded;
	bool jumpReady = true;
	int jumpsAvailable;

	Vector2 playerInput;
	Vector3 moveDirection;

	Rigidbody rb;

	private void Start () {
		jumpsAvailable = maxJumps;
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
	}
	private void FixedUpdate () {
		//* crouch controller
		if (Input.GetKey(crouchKey) && !crouched) {
			crouched = true;
			transform.localScale = new Vector3(.9f, .45f, .9f);
			Physics.gravity = new Vector3(0f, -6f, 0f);
			groudnDist /= 2;
		}
		else if (!Input.GetKey(crouchKey) && crouched) {
			crouched = false;
			transform.localScale = new Vector3(.9f, .9f, .9f);
			Physics.gravity = new Vector3(0f, -9.81f, 0f);
			groudnDist *= 2;
		}

		//* jump controller
		if (Input.GetKey(jumpKey) && jumpsAvailable > 0 && jumpReady) {
			jumpReady = false;
			jumpsAvailable--;
			rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
			rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
			Invoke("ResetJumpCooldown", .5f);
		}
		if (jumpsAvailable < maxJumps) {
			Invoke("ResetJumpAvailable", .2f);
		}

		//* limit velocity
		Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		if (flatVel.magnitude > moveSpeed) {
			Vector3 limitedVel = flatVel.normalized * moveSpeed;
			rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
		}

		//* player movement controller
		playerInput.x = Input.GetAxisRaw("Horizontal");
		playerInput.y = Input.GetAxisRaw("Vertical");
		moveDirection = orientation.forward * playerInput.y + orientation.right * playerInput.x;
		grounded = Physics.Raycast(transform.position, Vector3.down, groudnDist);
		if (grounded) {
			rb.drag = groundDrag;
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
		}
		else {
			rb.drag = 0;
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMovementMultiplier, ForceMode.Force);
		}
	}
	private void ResetJumpCooldown () {
		jumpReady = true;
	}
	private void ResetJumpAvailable () {
		if (Physics.Raycast(transform.position, Vector3.down, groudnDist)) {
			jumpsAvailable = maxJumps;
		}
	}
}