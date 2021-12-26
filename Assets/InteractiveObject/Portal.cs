public class Portal : InteractiveObject
{ 
    public override void Interaction() => StartCoroutine(GameManager.Instance.EnterPortal());
}