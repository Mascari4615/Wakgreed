public class Sohpia : ShopKeeper
{
    public override void FocusOff()
    {
        base.FocusOff();
        DataManager.Instance.CurGameData.talkedOnceNPC[ID] = true;
        DataManager.Instance.CurGameData.rescuedNPC[ID] = true;
    }
}
