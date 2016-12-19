using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCam : MonoBehaviour {

	public float m_Sensitivity = 60.0f;
	public float m_MoveSpeed = 10.0f;

	private float m_RotationX = 0;
	private float m_RotationY = 0;
	private Camera m_Camera;

    private byte m_BlockTypeAdd = (byte)Voxel.BlockType.Solid;
    private byte m_BlockTypeRemove = (byte)Voxel.BlockType.Air;

    // Use this for initialization
    void Start () 
	{
		m_Camera = GetComponent<Camera>();
		Cursor.lockState = CursorLockMode.Locked;
	}

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
            UpdateMovementControls();

        // toggle lock/free cursor
        if (Input.GetKeyDown(KeyCode.L))
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void UpdateMovementControls()
    { 
		// Handle Rotation
		m_RotationX += Input.GetAxis("Mouse X") * m_Sensitivity * Time.deltaTime;
		m_RotationY += Input.GetAxis("Mouse Y") * m_Sensitivity * Time.deltaTime;
		m_RotationY = Mathf.Clamp(m_RotationY,-90.0f,90.0f);	// clamp y in range

		transform.localRotation = Quaternion.AngleAxis(m_RotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(m_RotationY, Vector3.left);

		// Handle Movement
		float moveSpeed = m_MoveSpeed;
		float moveForward = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
		float moveRight = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        Vector3 velocity = transform.forward * moveForward;
        velocity += transform.right * moveRight;

       if (Input.GetKey (KeyCode.Q))
            velocity += transform.up * moveSpeed * Time.deltaTime;
		if (Input.GetKey (KeyCode.E))
            velocity -= transform.up * moveSpeed * Time.deltaTime;

        // Simple raytrace movement check
        Ray collisionRay = new Ray(transform.position, velocity);
        if (Physics.Raycast(transform.position, velocity, velocity.magnitude))
        {
            velocity = Vector3.zero;
        }

        transform.position += velocity;
 
		// screen trace
		Ray ray = m_Camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
		{
			// add/remove blocks on mouse click
			if (Input.GetMouseButtonDown(0))
			{
				Voxel.WorldObject.Instance.SetBlockInFrontOfRayHit(hit,m_BlockTypeAdd);
			}

			if (Input.GetMouseButtonDown(1))
			{
				Voxel.WorldObject.Instance.SetBlockBehindRayHit(hit, m_BlockTypeRemove);
			}
			
		}
	}
	
}
