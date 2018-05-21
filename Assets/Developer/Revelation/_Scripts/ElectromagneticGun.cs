using System.Collections.Generic;
using UnityEngine;

namespace Coop
{
  public class ElectromagneticGun : Gun {

    [SerializeField]
    [Tooltip("How far in Unity units that the EM field can affect targets")]
    private float m_EmDistance;
    [SerializeField]
    [Tooltip("How powerful is the EM field?")]
    private int forceAmount;

    private List<Electromagnetic> magnetizedObjects = new List<Electromagnetic>();
    private List<Electrostatic> interruptedObjects = new List<Electrostatic>();
    internal WhichWeapon currentForceType = (WhichWeapon)(-1);

    public override Projectile Fire(WhichWeapon weapType, Vector2? direction = null, Vector2? target = null)
    {
      if (currentForceType == WhichWeapon.Primary || currentForceType == WhichWeapon.Secondary) return null;
      Debug.Log("Firing EM gun.");
      ContactFilter2D filter = new ContactFilter2D();

      // Allocate up to 10 hit results in case other non-electromagnetic things are in the way. If we have more than 10, it will fail, but this seems reasonable.
      RaycastHit2D[] results = new RaycastHit2D[10];
      int hits = Physics2D.Raycast(AmmoSpawnLocation.position, AmmoSpawnLocation.right * Mathf.Sign(AmmoSpawnLocation.lossyScale.x), filter, results, m_EmDistance);
      if(hits > 0)
      {
        currentForceType = weapType;
        // Debugging: int emCount = 0;
        for (var i = 0; i < hits; i++)
        {
          var emComponent = results[i].collider.GetComponent<Electromagnetic>();
          if (emComponent != null)
          {
            // Debugging: emCount++;
            var rb = emComponent.GetComponent<Rigidbody2D>();
            if(rb)
              rb.bodyType = RigidbodyType2D.Dynamic;
            if (weapType == WhichWeapon.Primary && emComponent.canAttract)
            {
              if(emComponent.StartPull(this, currentForceType))
                magnetizedObjects.Add(emComponent);
            }
            else if (weapType == WhichWeapon.Secondary && emComponent.canRepel)
            {
              if(emComponent.StartPush(this, currentForceType))
                magnetizedObjects.Add(emComponent);
            }
          }

          var staticComponent = results[i].collider.GetComponent<Electrostatic>();
          if(staticComponent != null && staticComponent.canInterrupt)
          {
            // TODO: Should particle effect be different for electrostatic?
            staticComponent.StartCharge(this, currentForceType);
            interruptedObjects.Add(staticComponent);
          }
        }
        // Debugging: Debug.Log("Affecting " + emCount + " electromagnetic objects.");
      }
      return null;
    }
    public override void StopFiring()
    {

      Debug.Log("Stopped firing EM gun.");
      foreach (var obj in magnetizedObjects)
      {
        obj.StopPull(this, currentForceType);
        obj.StopPush(this, currentForceType);
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if(!rb) return;

        // TODO: Is this okay? Do we want to dampen the movement instead of stopping it 100%?
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
      }
      magnetizedObjects.Clear();

      foreach(var obj in interruptedObjects)
      {
        obj.StopCharge(this, currentForceType);
      }

      currentForceType = (WhichWeapon)(-1);

    }

    private void Update()
    {
      foreach(var obj in magnetizedObjects)
      {
        var rb = obj.GetComponent<Rigidbody2D>();
        if(!rb) return;
        var direction = AmmoSpawnLocation.right * Mathf.Sign(AmmoSpawnLocation.lossyScale.x); // (AmmoSpawnLocation.position - obj.transform.position).normalized;
        rb.AddForce(direction * (currentForceType == WhichWeapon.Primary ? -forceAmount : forceAmount));
      }
    }

    private void OnDrawGizmos()
    {
      Gizmos.DrawLine(AmmoSpawnLocation.position, AmmoSpawnLocation.position + (AmmoSpawnLocation.right * Mathf.Sign(AmmoSpawnLocation.lossyScale.x) * m_EmDistance));
    }
  }
}