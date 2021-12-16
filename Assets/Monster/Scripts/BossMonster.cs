using FMODUnity;
using System.Collections;
using UnityEngine;

public abstract class BossMonster : Monster
{
    [SerializeField] private BoolVariable isShowingSomething;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Ready());
    }

    private IEnumerator Ready()
    {
        isShowingSomething.RuntimeValue = true;
        yield return StartCoroutine(UIManager.Instance.SpeedWagon_BossOn(this));
        isShowingSomething.RuntimeValue = false;

        yield return new WaitForSeconds(2f);
        StartCoroutine(nameof(Attack));
    }

    protected abstract IEnumerator Attack();
    
    protected override IEnumerator Collapse()
    {
        isCollapsed = true;
        RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("(", System.StringComparison.Ordinal), 7) : name)}_Collapse", transform.position);

        collider2D.enabled = false;
        Rigidbody2D.velocity = Vector2.zero;
        Rigidbody2D.bodyType = RigidbodyType2D.Static;
        Animator.SetTrigger(collapse);
        GameManager.Instance.enemyRunTimeSet.Remove(gameObject);
        onMonsterCollapse.Raise(transform);
        int randCount = Random.Range(0, 5);
        for (int i = 0; i < randCount; i++) ObjectManager.Instance.PopObject("ExpOrb", transform);

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);
        yield return StartCoroutine(UIManager.Instance.SpeedWagon_BossOff(this));
        yield return new WaitForSeconds(3f);

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);

        (StageManager.Instance.CurrentRoom as BossRoom)?.RoomClear();
        gameObject.SetActive(false);
    }
}
