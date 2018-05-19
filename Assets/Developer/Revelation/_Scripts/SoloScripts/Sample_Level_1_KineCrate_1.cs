using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public class Sample_Level_1_KineCrate_1 : MonoBehaviour 
  {
    public void StartPushing(Gun gun)
    {
      GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }
    
    public void StopPushing(Gun gun)
    {
      //StartCoroutine(Still());
    }

    // private IEnumerator Still()
    // {
    //   yield return new WaitForSeconds(5);
    //   GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    // }
  }
}