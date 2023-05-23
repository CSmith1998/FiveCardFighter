using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected GameObject Tooltip;

    private GameObject temp;

    void Start() {
        
    }

    public void OnPointerEnter(PointerEventData eventData) {
        temp = Instantiate(Tooltip);
        temp.SetActive(false);

        var title = temp.transform.Find("txtCardName").GetComponent<TextMeshProUGUI>();
        var txt = temp.transform.Find("txtContainer").GetComponent<TextMeshProUGUI>();
        var img = temp.transform.Find("imgContainer").GetComponent<Image>();
        var name = gameObject.name;

        var mgr = GameObject.Find("Main Camera").GetComponent<GameManager>();
        temp.transform.SetParent(mgr.Canvas.transform);
        temp.transform.SetAsLastSibling();

        title.SetText(name);
        
        switch(name) {
            case "Attack - Smash":
                img.sprite = mgr.SmashSprite;
                txt.SetText(string.Format("This card, characterized by its mace image, will always deal {0} damage. The smash card is unique in its unique chance to stun the enemy for up to a total of {1} turn(s)! Sunning the enemy is not a guarantee however, so weigh your options when choosing this card.", mgr.PlayerSmashDamage, mgr.MaxStunTurns));
                break;
            case "Attack - Slash":
                img.sprite = mgr.SlashSprite;
                txt.SetText(string.Format("This card, characterized by its sword image, will always deal a minimum of {0} damage. As the Hero's primary source for damage, the card has the chance to deal more damage upon a critical hit. The chance for additional damage is not guaranteed however.", mgr.PlayerSlashDamage));
                break;
            case "Special - Parry":
                img.sprite = mgr.ParrySprite;
                txt.SetText("This card, characterized by its crossed sword image, turns back the enemy's damage upon them. This card is unique in its ability to fully protect the Hero from damage regardless of circumstance, but relies upon the enemy's hit chance to deal damage, making this card useless if the enemy is stunned.");
                break;
            case "Defence - Shield":
                txt.SetText(string.Format("This card, characterized by its shield image, reduces the enemy's damage by a minimum of {0} damage with a chance at reducing more. The shield card has the additional chance to stun the enemy if it critically protects the Hero for more damage than the enemy deals. This, and the chance to protect for more than {0} damage is not guaranteed however.", mgr.PlayerDefenseAmount));
                img.sprite = mgr.DefendSprite;
                break;
            case "Special - Heal":
                txt.SetText(string.Format("This card, characterized by its familiar '+' image, will always heal the Hero by a minimum of {0} damage, with the chance to heal for more. These cards are a limited resource however, so use with caution! Additional healing is subject to random chance.", mgr.PlayerHealAmount));
                img.sprite = mgr.HealingSprite;
                break;
            case "Detriment - Exhaustion":
                txt.SetText(string.Format("Heros beware! This card, characterized by its spiral image, is every great Hero's bane! These cards cannot be utilized and, if too many appear in the hero's hand, will have devastating effect! When a Hero draws 3 exhaustion, their turn will be skipped; drawing a full 5 exhaustion will result in the Hero taking {0} damage and losing some of their cards!", mgr.ExhaustionDamage));
                img.sprite = mgr.ExhaustionSprite;
                break;
        }

        temp.transform.localPosition = Vector3.zero;
        temp.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        DestroyImmediate(temp);
    }
}