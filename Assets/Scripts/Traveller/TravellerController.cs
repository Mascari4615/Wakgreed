using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.EventSystems;

public enum TravellerClass
{
    Knight,
    Archer
}

public class TravellerController : MonoBehaviour
{
    private static TravellerController instance;
    [HideInInspector] public static TravellerController Instance { get { return instance; } }

    public Traveller traveller;
    public TravellerClass travellerClass;

    public Transform attackPositionParent;
    public Transform attackPosition;
    public Transform weaponPosition;
    public JoyStick joyStick;
    public CinemachineTargetGroup cinemachineTargetGroup;
    public Image hpBar;
    public Text hpText;
    public Image expBar;
    public Text expText;
    public Text levelText;  
    public GameObject bloodingPanel;
    public GameObject interactionIcon = null;
    public GameObject attackIcon = null;
    public EventTrigger attackButtonEventTrigger;
    public EventTrigger skill0ButtonEventTrigger;
    public EventTrigger skill1ButtonEventTrigger;
    public EventTrigger skill2ButtonEventTrigger;

    public Traveller[] travellers;

    public void ChangeTraveller(int index)
    {
        traveller = travellers[index];
        traveller._Awake(this);
        traveller.Initialize(this);
    }

    private void Awake()
    {
        Debug.Log($"{name} : Awake");
        instance = this;
        traveller._Awake(this);
    }

    private void OnEnable()
    {
        Debug.Log(name + " : OnEnable");
        Initialize();
    }

    public void Initialize()
    {
        Debug.Log(name + " : Initialize");
        traveller.Initialize(this);
        
        transform.position = Vector3.zero;
    }

    public void Update()
    {
        traveller._Update(this);
    }

    public void Collapse()
    {
        traveller.Collapse();
        this.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        traveller.OnTriggerEnter2D(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        traveller.OnTriggerExit2D(other);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        traveller.OnCollisionEnter2D(other);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        traveller.OnCollisionExit2D(other);
    }
}
