using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public enum SplineWalkerMode
  {
    Once,
    Loop,
    PingPong
  }

  public class BezierWalk : MonoBehaviour
  {

    public BezierSpline spline;

    public bool lookForward = false;
    public SplineWalkerMode mode;
    private bool goingForward = true;

    public float duration;

    [SerializeField, Range(0f, 1f)]
    public float startAt;
    private float progress;

    private void Awake()
    {
      progress = startAt;
    }

    private void Update()
    {
      if (goingForward)
      {
        progress += Time.deltaTime / duration;
        if (progress > 1f)
        {
          if (mode == SplineWalkerMode.Once)
          {
            progress = 1f;
          }
          else if (mode == SplineWalkerMode.Loop)
          {
            progress -= 1f;
          }
          else
          {
            progress = 2f - progress;
            goingForward = false;
          }
        }
      }
      else
      {
        progress -= Time.deltaTime / duration;
        if (progress < 0f)
        {
          progress = -progress;
          goingForward = true;
        }
      }
      Vector3 position = spline.GetPoint(progress);
      var rb = GetComponent<Rigidbody2D>();
      if(rb)
        rb.MovePosition(position);
      else
        transform.position = position;
      //position = Vector2.SmoothDamp(transform.position, position, ref vel, .01f);

      if (lookForward)
      {
        transform.LookAt(position + spline.GetDirection(progress));
      }
    }
  }
}