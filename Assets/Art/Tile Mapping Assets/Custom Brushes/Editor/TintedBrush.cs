using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Coop
{
  [CustomGridBrush(true, false, false, "[Coop] > Tinted Brush")]
  public class TintedBrush : GridBrushBase
  {
    public Color m_Color = Color.white;

    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
      // Do not allow editing palettes
      if (brushTarget.layer == 31)
        return;

      Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
      if (tilemap != null)
      {
        SetColor(tilemap, position, m_Color);
      }
    }

    public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
      // Do not allow editing palettes
      if (brushTarget.layer == 31)
        return;

      Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
      if (tilemap != null)
      {
        SetColor(tilemap, position, Color.white);
      }
    }

    public override void Select(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
    {
      Tilemap tilemap = brushTarget.GetComponent<Tilemap>();

      for (var x = position.xMin; x < position.xMax; x++)
      {
        for (var y = position.yMin; y < position.yMax; y++)
        {
          SetColor(tilemap, new Vector3Int(x, y, 0), m_Color);
        }
      }

      base.Select(gridLayout, brushTarget, position);
    }

    public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
    {
      Tilemap tilemap = brushTarget.GetComponent<Tilemap>();
      if (tilemap != null)
      {
        m_Color = tilemap.GetColor(new Vector3Int((int)position.center.x, (int)position.center.y, 0));
      }
    }

    private static void SetColor(Tilemap tilemap, Vector3Int position, Color color)
    {
      TileBase tile = tilemap.GetTile(position);
      if (tile != null)
      {
        if ((tilemap.GetTileFlags(position) & TileFlags.LockColor) != 0)
        {
          if (tile is Tile)
          {
            Debug.LogWarning("Tint brush cancelled, because Tile (" + tile.name + ") has TileFlags.LockColor set. Unlock it from the Tile asset debug inspector.");
          }
          else
          {
            Debug.LogWarning("Tint brush cancelled. because Tile (" + tile.name + ") has TileFlags.LockColor set. Unset it in GetTileData().");
          }
        }
        if (tilemap.GetColor(position) != color)
          tilemap.SetColor(position, color);

      }
    }
  }


  [CustomEditor(typeof(TintedBrush))]
  public class TintedBrushEditor : GridBrushEditorBase
  {
    public override GameObject[] validTargets
    {
      get
      {
        return GameObject.FindObjectsOfType<Tilemap>().Select(x => x.gameObject).ToArray();
      }
    }
  }
}
