using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class GardenDecoration : MonoBehaviour
{

    [SerializeField] Transform pointOne;
    [SerializeField] Transform pointTwo;
    [SerializeField] Transform parentGO;

    [Space]
    [SerializeField] float offsetRandomPosition = 0.5f;
    [SerializeField] float scaleRandomness = 0.02f;
    [SerializeField] float scale = 0.25f;
    [SerializeField] float distanceStep = 2f;
    [SerializeField] Sprite [] decorationArray;

    [SerializeField] bool isVertical;
    [SerializeField] bool isInverted;
    [SerializeField] bool decorate;
    [SerializeField] bool undoLastStep;
    int n = 0;
    List<GameObject> lastStep;
    // Update is called once per frame
    void Update()
    {
        if (decorate)
        {
            Decorate();
            decorate = false;
        }
        if (undoLastStep)
        {
            undoLastStep = false;
            Undo();
        }
    }

    private void Undo()
    {
        for (int i = 0; i < lastStep.Count; i++)
        {
            DestroyImmediate(lastStep[i].gameObject);
            
        }
        lastStep.Clear();
    }

    private void Decorate()
    {
        if(lastStep == null)
        {
            lastStep = new List<GameObject>();
        }
        else
        {
            lastStep.Clear();
        }
        float totalDistance;
        Vector2 direction;
        if (isVertical)
        {
            totalDistance = Mathf.Abs(pointOne.position.y - pointTwo.position.y);
            direction = Vector2.up;
        }
        else
        {
            direction = pointTwo.position - pointOne.position;
            direction.y = 0;
            totalDistance = Mathf.Abs(pointOne.position.x - pointTwo.position.x);
        }
        Vector2 nextPoint = pointOne.position;

        direction.Normalize();
        for (float i = 0; i < totalDistance; i += distanceStep)
        {
            GameObject decorationGO = new GameObject("Decoration batch" + n);
            lastStep.Add(decorationGO);
            decorationGO.transform.position = nextPoint;
            decorationGO.transform.position += (Vector3)(Random.Range(-offsetRandomPosition, offsetRandomPosition) * direction.normalized);
            decorationGO.transform.parent = parentGO;
            decorationGO.AddComponent<SpriteRenderer>();
            decorationGO.GetComponent<SpriteRenderer>().sprite = decorationArray[Random.Range(0, decorationArray.Length)];
            decorationGO.GetComponent<SpriteRenderer>().color = Color.black;
            decorationGO.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
            float finalScale = scale + Random.Range(-scaleRandomness,scaleRandomness);
            float rnd = Random.value;
            rnd = rnd > 0.5f ? -1 : 1;
            if (isInverted)
            {
                decorationGO.transform.localScale = new Vector3(1 * finalScale*rnd, -1 * finalScale, 1 * finalScale);
            }
            else
            {
                decorationGO.transform.localScale = new Vector3(1 * finalScale * rnd, 1 * finalScale, 1 * finalScale);

            }
            if (isVertical)
            {
                decorationGO.transform.localEulerAngles = Vector3.forward * -90f;
            }
            nextPoint += direction.normalized * distanceStep;
        }
        n++;
    }
}
