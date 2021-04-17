using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersecucionFinal : ActivatedBase
{
    [SerializeField] Transform m_posicionFinal;
    [SerializeField] float m_velocidadMinima;
    [SerializeField] float m_velocidadTramoFinal = 18f;
    [SerializeField] ActivatedBase m_plataformaFinal;
    [SerializeField] Transform m_posicionActivacionPlataformaFinal;
    [SerializeField] Transform m_posicionActivacionVelocidadFinal;
    float velocidadActual;
    float offsetInicialACamara;

    Transform cam;
    private void Start()
    {
        cam = Camera.main.transform;
        offsetInicialACamara = cam.position.x - transform.position.x;
    }
    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            if (cam.position.x - transform.position.x > offsetInicialACamara)
            {
                velocidadActual = m_velocidadMinima * (1 + (cam.position.x - transform.position.x - offsetInicialACamara));
            }
            else
            {
                velocidadActual = m_velocidadMinima;
            }
            transform.position += Vector3.right * Time.deltaTime * velocidadActual;

            if (!m_plataformaFinal.Active && transform.position.x >= m_posicionActivacionPlataformaFinal.position.x)
            {
                m_plataformaFinal.Activate();
                
            }
            if(transform.position.x >= m_posicionActivacionVelocidadFinal.position.x)
                m_velocidadMinima = m_velocidadTramoFinal;
        }
    }
    public override void Deactivate()
    {
        base.Deactivate();
        m_plataformaFinal.enabled = false;
        this.enabled = false;
    }
    public override void Activate()
    {
        if (Active)
        {
            Deactivate();
            return;
        }
        base.Activate();
      
    }
}
