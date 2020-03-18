using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebasInput : PersistentSingleton<PruebasInput>
{

	InputActionsPruebas m_pruebasInput = null;

	float m_movementInput = 0;

	public override void Awake()
	{
		base.Awake();
		m_pruebasInput = new InputActionsPruebas();
		m_pruebasInput.PruebasAndres.Moverse.performed += ctx => m_movementInput = ctx.ReadValue<float>();
		m_pruebasInput.PruebasAndres.Moverse.canceled += ctx => m_movementInput = 0;
		m_pruebasInput.PruebasAndres.Jump.performed += ctx => Jump();
	}


	// Start is called before the first frame update
	void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
		PruebasMovement.Instance.MoveSquare(m_movementInput);
    }

	void Jump()
	{
		PruebasMovement.Instance.Jump();	
	}

	private void OnEnable()
	{
		m_pruebasInput.Enable();
	}
	private void OnDisable()
	{
		m_pruebasInput.Disable();
	}
}
