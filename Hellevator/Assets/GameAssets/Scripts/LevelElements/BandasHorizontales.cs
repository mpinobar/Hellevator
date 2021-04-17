using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandasHorizontales : MonoBehaviour
{
    [SerializeField] float velocidadBandas;
    [SerializeField] Transform bandaSuperior;
    [SerializeField] Transform posicionFinalBandaSuperior;
    [SerializeField] Transform bandaInferior;
    [SerializeField] Transform posicionFinalBandaInferior;

    Vector3 posicionInicialSuperior;
    Vector3 posicionInicialInferior;

    private void Start()
    {
        posicionInicialSuperior = bandaSuperior.localPosition;
        posicionInicialInferior = bandaInferior.localPosition;
    }

    public void Activar()
    {
        StartCoroutine(ActivarBandas());
    }

    public void Desactivar()
    {
        StartCoroutine(DesactivarBandas());
    }

    IEnumerator ActivarBandas()
    {
        float tmp = 0;

        while (tmp < 1)
        {
            tmp += Time.deltaTime * velocidadBandas;
            bandaSuperior.localPosition = Vector3.Lerp(bandaSuperior.localPosition, posicionFinalBandaSuperior.localPosition, tmp);
            bandaInferior.localPosition = Vector3.Lerp(bandaInferior.localPosition, posicionFinalBandaInferior.localPosition, tmp);
            yield return null;
        }
    }

    IEnumerator DesactivarBandas()
    {
        float tmp = 0;

        while (tmp < 1)
        {
            tmp += Time.deltaTime * velocidadBandas;
            bandaSuperior.localPosition = Vector3.Lerp(bandaSuperior.localPosition, posicionInicialSuperior, tmp);
            bandaInferior.localPosition = Vector3.Lerp(bandaInferior.localPosition, posicionInicialInferior, tmp);
            yield return null;
        }
    }

}
