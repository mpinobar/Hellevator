using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicaInicialH16 : MonoBehaviour
{
    [SerializeField] PersecucionFinal m_plataformaPersecucion;
    [SerializeField] BandasHorizontales m_bandas;
    [SerializeField] float m_tiempoMirarADerecha = 1.25f;
    [SerializeField] float m_tiempoBandasHorizontales = 1;
    [SerializeField] float m_tiempoHastaMirarIzquierda = 1;
    [SerializeField] float m_tiempoHastaMoverPlataforma = 1;

    bool inCinematic;

    private IEnumerator Start()
    {
        yield return null;
        StartCoroutine(Cinematica());
    }


    private void Update()
    {
        if(inCinematic)
            PossessionManager.Instance.ControlledDemon.CanMove = false;
        
    }
    IEnumerator Cinematica()
    {
        yield return new WaitForSeconds(0.1f);
        inCinematic = true;
        AudioManager.Instance.PauseMusic();
        PossessionManager.Instance.ControlledDemon.MovementDirection = -1;
        yield return new WaitForSeconds(m_tiempoMirarADerecha);
        m_bandas.Activar();
        yield return new WaitForSeconds(m_tiempoBandasHorizontales);
        CameraManager.Instance.CameraShakeMedium();
        yield return new WaitForSeconds(m_tiempoHastaMirarIzquierda);
        PossessionManager.Instance.ControlledDemon.MovementDirection = 1;
        yield return new WaitForSeconds(m_tiempoHastaMoverPlataforma);
        m_bandas.Desactivar();
        PossessionManager.Instance.ControlledDemon.MovementDirection = -1;
        PossessionManager.Instance.ControlledDemon.CanMove = true;
        m_plataformaPersecucion.Activate();
        inCinematic = false;
        AudioManager.Instance.ResumeMusic();

    }
}
