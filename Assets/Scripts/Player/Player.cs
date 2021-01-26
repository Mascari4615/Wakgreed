using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class Player : MonoBehaviour
{
    [Header("Player Components")]
    public Rigidbody2D playerRB = null;
    [SerializeField] private GameObject attackPosParent = null;
    [SerializeField] private Transform attackPos = null;
    [SerializeField] private Transform weaponPos = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private SpriteRenderer spriteRenderer = null;

    // [Header("Player Stats")]
    [HideInInspector] public int ad = 0;
    [HideInInspector] public int criticalChance = 0;
    private int hp = 0;
    private int hpMax = 0;
    private int exp = 0;
    private int necessaryExp = 0;
    private int level = 0;
    private float moveSpeed = 0;
    private float attackCoolTime = 0;

    [Header("Stuff")]
    [SerializeField] private AudioClip[] audioClips = new AudioClip[3];
    [SerializeField] private JoyStick joyStick = null;
    [SerializeField] private CinemachineTargetGroup cinemachineTargetGroup = null;
    [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource = null;
    [SerializeField] private float attackPosGap = 1f;
    private bool isInputAttack;
    private float t = 0;
    private float t1 = 0;
    private bool isReadyToAttack = false;
    private bool isBleeding = false;
    private int targetIndex = 4444;
    private int weaponRot = 0;

    public List<Item> inventory;

    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        hp = 100;
        hpMax = hp;
        ad = 30;
        attackCoolTime = 0.3f;
        moveSpeed = 8;
        necessaryExp = 100;
        exp = 0;
        level = 0;
        criticalChance = 5;
        
        playerRB.bodyType = RigidbodyType2D.Dynamic;
        GameManager.Instance.nearInteractionObject = GameManager.InteractionObjext.None;

        animator.SetBool("Run", false);

        GameManager.Instance.hpBar.fillAmount = (float)hp / hpMax;
        GameManager.Instance.hpText.text = hp + " / " + hpMax;

        GameManager.Instance.expBar.fillAmount = (float)exp / necessaryExp;
        GameManager.Instance.levelText.text = "Lv. " + level;
        GameManager.Instance.expText.text = Mathf.Floor((float)exp / necessaryExp * 100) + "%";
    }

    void Update()
    {
        if (Time.timeScale != 1)
        {
            GameManager.Instance.mapPanel.SetActive(false);
            return;
        }
        
        Move();
        Attack();
        Targeting();

        if (Input.GetKeyDown(KeyCode.F))
        {
            GameManager.Instance.Interaction();
        }

        if (isBleeding)
        {
            if (t1 < 1f)
            {
                spriteRenderer.color = new Color(1, 1, 1, (float)100 / 255);
                t1 += Time.deltaTime;
            }
            else
            {
                spriteRenderer.color = new Color(1, 1, 1, 1);
                isBleeding = false;
                t1 = 0;
            }
        }
    }

    private void Move()
    {
        float h = joyStick.inputValue.x;
        float v = joyStick.inputValue.y;

        if (joyStick.isDraging)
        {
            playerRB.velocity = new Vector2(h, v) * moveSpeed;      
            animator.SetBool("Run", true);       
        }
        else
        {
            playerRB.velocity = Vector2.zero;
            animator.SetBool("Run", false);
        }

        if (h > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            weaponPos.transform.rotation = Quaternion.Euler(0, 0,  joyStick.inputValue.y * 90);
        }
        else if (h < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            weaponPos.transform.rotation = Quaternion.Euler(0, 0, joyStick.inputValue.y * 90 * -1);
        }
    }

    public void AttackButtonDown()
    {
        isInputAttack = true;
    }
    public void AttackButtonUp()
    {
        isInputAttack = false;
    }
    private void Attack()
    {
        if (isReadyToAttack == false)
        {
            t += Time.deltaTime;
            if (t >= attackCoolTime)
            {
                t = 0;
                isReadyToAttack = true;
            }
        }

        if ((isInputAttack || Input.GetKey(KeyCode.Space)) && isReadyToAttack)
        {
            if (weaponRot == 0)
            {
                weaponRot = 90;
                weaponPos.transform.localPosition = new Vector3(0, -0.5f, -0.5f);
                weaponPos.transform.localScale = new Vector3(-1, -1, 1);
            }
            else if (weaponRot == 90)
            {
                weaponRot = 0;
                weaponPos.transform.localPosition = new Vector3(0.7f, -0.35f, 0.5f);
                weaponPos.transform.localScale = new Vector3(1, 1, 1);
            }
            ObjectManager.Instance.GetQueue(PoolType.DefaultAttack, attackPos);
            audioSource.clip = audioClips[Random.Range(0,audioClips.Length)];
            audioSource.Play();
            cinemachineImpulseSource.GenerateImpulse();
            isReadyToAttack = false;
        }
    }

    private void Targeting()
    {
        if (GameManager.Instance.monsters.Count > 0)
        {
            targetIndex = 4444;
            float currentDist = 0;
            float targetDist = 15;
            
            for (int i = 0; i < GameManager.Instance.monsters.Count; i++)
            {
                currentDist = Vector2.Distance(transform.position, GameManager.Instance.monsters[i].transform.position);
                RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, GameManager.Instance.monsters[i].transform.position - transform.position, currentDist, LayerMask.NameToLayer("Everything"));
                for (int j = 0; j < hit.Length; j++)
                {
                    if (hit[j].transform.CompareTag("Wall"))
                    {
                        break;
                    }
                    else if (hit[j].transform.CompareTag("Monster") && (targetDist >= currentDist))
                    {                     
                        targetIndex = i;
                        targetDist = currentDist; 
                    }
                }
            }
        }
        else
        {
            targetIndex = 4444; // 타겟팅 안됨
        }

        if (targetIndex == 4444)
        {
            cinemachineTargetGroup.m_Targets[1].target = null;
            attackPosParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(joyStick.inputValue.y, joyStick.inputValue.x) * Mathf.Rad2Deg - 90);
            attackPos.transform.localPosition = new Vector3(0, attackPosGap, 0);
        }
        else
        {
            Debug.DrawRay(transform.position, GameManager.Instance.monsters[targetIndex].transform.position - transform.position, Color.red);

            cinemachineTargetGroup.m_Targets[1].target = GameManager.Instance.monsters[targetIndex].transform;
            attackPosParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y - GameManager.Instance.monsters[targetIndex].transform.position.y, transform.position.x - GameManager.Instance.monsters[targetIndex].transform.position.x) * Mathf.Rad2Deg + 90);

            /* if (GameManager.Instance.monsters[targetIndex].transform.position.x >= transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
                weaponPos.transform.rotation = Quaternion.Euler(0, 0,  joyStick.inputValue.y * 90);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
                weaponPos.transform.rotation = Quaternion.Euler(0, 0, joyStick.inputValue.y * 90 * -1);
            } */
        }
    }

    public void ReceiveDamage(int damage)
    {
        if (isBleeding)
        {
            return;
        }

        GameManager.Instance.bloodingPanel.SetActive(false);
        GameManager.Instance.bloodingPanel.SetActive(true);
        isBleeding = true;
        // ObjectManager.Instance.GetQueue(PoolType.animatedText, transform.position, damage + "", Color.red);
        hp -= damage;
        GameManager.Instance.hpBar.fillAmount = (float)hp / hpMax;
        GameManager.Instance.hpText.text = hp + " / " + hpMax;

        if (hp <= 0)
        {
            GameManager.Instance.hpBar.fillAmount = 0;
            GameManager.Instance.hpText.text = 0 + " / " + hpMax;

            animator.SetTrigger("Died");
            
            playerRB.bodyType = RigidbodyType2D.Static;

            StartCoroutine(GameManager.Instance.GameOver());
            this.enabled = false;
        }
    }

    public void ReceiveExp(int expValue)
    {
        exp += expValue;
        
        if (exp >= necessaryExp)
        {
            GameManager.Instance.LevelUp();
            level += 1;
            exp = 0;
            necessaryExp += 150;  
        }

        GameManager.Instance.expBar.fillAmount = (float)exp / necessaryExp;
        GameManager.Instance.levelText.text = "Lv. " + level;
        GameManager.Instance.expText.text = Mathf.Floor((float)exp / necessaryExp * 100) + "%";
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            ReceiveDamage(other.gameObject.GetComponent<Monster>().ad);
            // Debug.Log(other.gameObject.name +", " + other.gameObject.GetComponent<Monster>().ad + "데미지 피격");
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            ReceiveDamage(other.GetComponentInParent<Monster>().ad);
            // Debug.Log(other.gameObject.name +", " + other.gameObject.GetComponent<Monster>().ad + "데미지 피격");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.tag;
        switch (tag)
        {
            case "Portal":
                GameManager.Instance.attackButton.SetActive(false);
                GameManager.Instance.interactionButton.SetActive(true);
                GameManager.Instance.nearInteractionObject = GameManager.InteractionObjext.Portal;  
                break;
            case "UpperDoor":
                StartCoroutine(GameManager.Instance.MigrateRoom("Up"));    
                break;
            case "LowerDoor":
                StartCoroutine(GameManager.Instance.MigrateRoom("Down"));   
                break;
            case "LeftDoor":
                StartCoroutine(GameManager.Instance.MigrateRoom("Left"));
                break;
            case "RightDoor":
                StartCoroutine(GameManager.Instance.MigrateRoom("Right"));
                break;
            case "Exp":
                ObjectManager.Instance.InsertQueue(PoolType.Exp, other.gameObject);
                ReceiveExp(Random.Range(20, 31));
                break;
            case "Nyang":
                ObjectManager.Instance.InsertQueue(PoolType.Nyang, other.gameObject);
                GameManager.Instance.ReceiveValue("Nyang", Random.Range(10, 51));         
                break;
            case "Item":
                ObjectManager.Instance.InsertQueue(PoolType.Item, other.gameObject);
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i] == GameManager.Instance.itemBuffer.items[other.GetComponent<ItemGameObject>().itemID])
                    {
                        inventory[i].itemCount++;
                        return;
                    }
                }
                inventory.Add(GameManager.Instance.itemBuffer.items[other.GetComponent<ItemGameObject>().itemID]);
                GameManager.Instance.itemBuffer.items[other.GetComponent<ItemGameObject>().itemID].Effect();
                break;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Portal")
        {
            GameManager.Instance.interactionButton.SetActive(false);
            GameManager.Instance.attackButton.SetActive(true);
            GameManager.Instance.nearInteractionObject = GameManager.InteractionObjext.None; 
        }
    }
}