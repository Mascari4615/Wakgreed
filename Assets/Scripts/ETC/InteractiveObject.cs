using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    // 상속받는 오브젝트가 반드시 초기화 시켜줘야 함
    protected InteractiveObjectType interactiveObjectType;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            InteractionManager.Instance.interactionIcon.SetActive(true);
            InteractionManager.Instance.attackIcon.SetActive(false);
            InteractionManager.Instance.nearInteractionObject = interactiveObjectType;
        }  
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InteractionManager.Instance.interactionIcon.SetActive(true);
            InteractionManager.Instance.attackIcon.SetActive(false);
            InteractionManager.Instance.nearInteractionObject = interactiveObjectType;
        }  
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            InteractionManager.Instance.interactionIcon.SetActive(false);
            InteractionManager.Instance.attackIcon.SetActive(true);
            InteractionManager.Instance.nearInteractionObject = InteractiveObjectType.None;
        }     
    }

    protected virtual void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InteractionManager.Instance.interactionIcon.SetActive(false);
            InteractionManager.Instance.attackIcon.SetActive(true);
            InteractionManager.Instance.nearInteractionObject = InteractiveObjectType.None;
        }  
    }
}
