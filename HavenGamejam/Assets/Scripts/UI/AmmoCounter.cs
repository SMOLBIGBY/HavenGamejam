using UnityEngine;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text ammoText;

    WeaponScript weaponScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponScript = FindAnyObjectByType<WeaponScript>();
    }

    // Update is called once per frame
    void Update()
    {
        ammoText.text = "Ammo: " + weaponScript.AmmoCount.ToString() + "/" + weaponScript.MaxAmmoCount.ToString();
    }
}
