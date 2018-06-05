using UnityEditor;
using UnityEngine;

namespace Coop
{
  [CustomEditor(typeof(BezierWalk))]
  public class BezierWalkInspector : Editor
  {

    private BezierWalk walker;

    private void Awake()
    {
      walker = (BezierWalk)target;
    }

    private void OnSceneGUI()
    {
      walker.transform.position = walker.spline.GetPoint(walker.startAt);

      Vector3 p0 = walker.spline.transform.TransformPoint(walker.spline.GetControlPoint(0));
      for (int i = 1; i < walker.spline.ControlPointCount; i += 3)
      {
        Vector3 p1 = walker.spline.transform.TransformPoint(walker.spline.GetControlPoint(i));
        Vector3 p2 = walker.spline.transform.TransformPoint(walker.spline.GetControlPoint(i + 1));
        Vector3 p3 = walker.spline.transform.TransformPoint(walker.spline.GetControlPoint(i + 2));

        //Handles.color = Color.gray;
        //Handles.DrawLine(p0, p1);
        //Handles.DrawLine(p2, p3);

        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
        p0 = p3;
      }


    }

  }
}