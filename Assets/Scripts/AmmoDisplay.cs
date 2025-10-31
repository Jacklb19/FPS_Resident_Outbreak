using UnityEngine;
using TMPro;

public class AmmoDisplay : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public WeaponController weapon;
    
    void Update()
    {
        if (weapon != null && ammoText != null)
        {
            ammoText.text = weapon.currentAmmo + " / " + weapon.reserveAmmo;
        }
    }
}
