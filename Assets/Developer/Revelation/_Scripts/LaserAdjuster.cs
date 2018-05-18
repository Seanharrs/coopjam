using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  [ExecuteInEditMode]
  public class LaserAdjuster : MonoBehaviour {
    #if UNITY_EDITOR

    [SerializeField]
    private bool autoAdjust = false;
    [SerializeField]
    private GameObject topEmitter;
    [SerializeField]
    private GameObject bottomEmitter;
    

    void Update () {
      if( autoAdjust && (transform.hasChanged || topEmitter.transform.hasChanged || bottomEmitter.transform.hasChanged) )
      {

        var distance = (topEmitter.transform.position - bottomEmitter.transform.position).magnitude;
        Debug.Log("Transform has changed. Distance: " + distance);
        transform.localScale = new Vector3(1, distance / 4, 1);
        transform.localPosition = new Vector3(0, bottomEmitter.transform.localPosition.y + distance / 2, 0);
        Debug.Log("Changed me.");

        var tPosition = topEmitter.transform.localPosition;
        tPosition.x = 0;
        tPosition.z = 0;
        topEmitter.transform.localPosition = tPosition;
        var bPosition = bottomEmitter.transform.localPosition;
        bPosition.x = 0;
        bPosition.z = 0;
        bottomEmitter.transform.localPosition = bPosition;

      } else{ Debug.LogWarning("No cahnge."); }
    }
    #endif
  }
}