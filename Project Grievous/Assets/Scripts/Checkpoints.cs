using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    [SerializeField] List<Transform> checkpointLocations;

    public void SetOnCheckpoint(Transform source, int index)
    {
        if (checkpointLocations[index])
        {
            source.position = checkpointLocations[index].position;
            source.rotation = checkpointLocations[index].rotation;
        }
    }
}
