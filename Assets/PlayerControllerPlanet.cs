using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class PlayerControllerPlanet : MonoBehaviour {
	public float mouseSensitivity = 250.0f;
	public float speed = 10.0f;	
	public float jumpForce = 200.0f;
	public float gravity = 10.0f;
	float rotationVertLook;

	Vector3 move;
	Vector3 velocity;

	Vector3 smoothMoveVelocity;

	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	bool grounded = false;
	Rigidbody rb;
	Transform cameraTransform;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		rb.useGravity = false;
	}
	void Awake()
	{
		cameraTransform = GetComponentInChildren<Camera>().transform;
	}

	void Update()
	{
		//Camera look
		transform.Rotate(mouseSensitivity * Time.deltaTime * Input.GetAxis("Mouse X") * Vector3.up);
		rotationVertLook += mouseSensitivity * Time.deltaTime * Input.GetAxis("Mouse Y");
		rotationVertLook = Mathf.Clamp(rotationVertLook, -90.0f, 90.0f);
		cameraTransform.localEulerAngles = rotationVertLook * Vector3.left;

		Vector3 moveDist = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * speed;
		move = Vector3.SmoothDamp(move, moveDist, ref smoothMoveVelocity, 0.15f);

		if (Input.GetButtonDown("Jump"))
			rb.AddForce(transform.up * jumpForce);

		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;

		grounded = Physics.Raycast(ray, out hit, 1 + .1f);
	}

	void FixedUpdate()
	{
        Vector3 localMove = transform.TransformDirection(move) * Time.fixedDeltaTime;
		rb.MovePosition(rb.position + localMove);
	}
}