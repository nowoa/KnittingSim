using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualisers : MonoBehaviour
{
    public GameObject PinNeedlePrefab;

    public GameObject CreateVisualiser(GameObject prefab)
    {
        var result = Instantiate(prefab, transform);
        return result;
    }

    public void DestroyVisualiser(GameObject instance)
    {
        Destroy(instance);
    }
}
