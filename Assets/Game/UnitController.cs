using UnityEngine;

public class UnitController : MonoBehaviour
{
    public int UnitID { get; private set; }
    public int OwnerID { get; private set; }
    public Vector3 Position { get; private set; }

    public void Initialize(int unitID, int ownerID, Vector3 position)
    {
        UnitID = unitID;
        OwnerID = ownerID;
        Position = position;
        transform.position = position;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        Position = targetPosition;
        transform.position = targetPosition;
    }
}
