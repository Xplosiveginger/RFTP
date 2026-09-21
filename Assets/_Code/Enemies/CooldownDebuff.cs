using System.Collections;
using UnityEngine;

public class CooldownDebuff : MonoBehaviour
{

    private float CooldownDebuffDuration = 2f;
    private float lifetime;
    private WeaponBase weapon;

    public void Init(float cooldownDebuffDuration, float lifetime)
    {
        this.CooldownDebuffDuration = cooldownDebuffDuration;
        this.lifetime = lifetime;
        StartCoroutine(AddDebuff());
    }
    IEnumerator AddDebuff()
    {
        weapon=GetComponent<WeaponBase>();
        if (weapon != null)
        {
            weapon.AddCooldownDebuff(CooldownDebuffDuration);
        }
        yield return new WaitForSeconds(lifetime);
        weapon.RemoveCooldownDebuff(CooldownDebuffDuration);
        Destroy(this);
    }



}
