using System.Collections.Generic;

public class SavePointModel
{
    public List<ITeleportGate> CurrentSavePoints { get; private set; }

    public SavePointModel()
    {
        CurrentSavePoints = new List<ITeleportGate>();
    }

    public void Save(ITeleportGate savePoint)
    {
        // if (CurrentSavePoints.Exists(savePoint.GetPos))
    }
}