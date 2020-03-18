using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : PersistentSingleton<InputManager>
{

	Controls m_controls = null;

	float m_moveInputValue = 0f;


	public override void Awake()
	{
		base.Awake();

		m_controls = new Controls();
		m_controls.PlayerControls.InputMove.performed += ctx => m_moveInputValue = ctx.ReadValue<float>();
		m_controls.PlayerControls.InputMove.canceled += ctx =>m_moveInputValue = ctx.ReadValue<float>();
		m_controls.PlayerControls.InputJump.performed += ctx => ChangeCamera();
		//m_controls.PlayerControls.InputAbility.performed += ctx =>;
		//m_controls.PlayerControls.InputInteract.performed += ctx =>;
		//m_controls.PlayerControls.InputSuicide.performed += ctx =>;

	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void ChangeCamera()
	{
		PruebasMovement.Instance.ChangeCamera();
	}

	private void OnEnable()
	{
		m_controls.Enable();
	}
	private void OnDisable()
	{
		m_controls.Disable();
	}
}
