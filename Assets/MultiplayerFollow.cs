using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MultiplayerFollow : MonoBehaviour
{
    private Coop.Platformer2DUserControl[] players;

    [SerializeField]
    private Vector3 offset;

    private void Awake()
    {
        players = FindObjectsOfType<Coop.Platformer2DUserControl>();
    }

    private void LateUpdate()
    {
        Vector3 avgPos = players.Select(p => p.transform.position).Aggregate((total, next) => total += next) / players.Length;
        transform.position = avgPos + offset;
    }
}
