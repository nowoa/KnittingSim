using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualisers : MonoBehaviour
{
    public GameObject PinNeedlePrefab;

    public GameObject CreateVisualiser(GameObject prefab, Vector3 position, Quaternion lookRotation)
    {
        var result = Instantiate(prefab, position, lookRotation,transform);
        return result;
    }

    public void DestroyVisualiser(GameObject instance)
    {
        Destroy(instance);
    }
}
