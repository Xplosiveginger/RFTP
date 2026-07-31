using UnityEngine;

public class IDCardManager : MonoBehaviour
{
    public GameObject weaponsItemContainer;
    public GameObject weaponItemPrefab;

    public GameStat_SO gameStat;
    
    public GameObject skillsItemContainer;
    public GameObject skillItemPrefab;
    private void Start()
    {
        PopulateWeapons();
        PopulateSkills();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PopulateWeapons();
            PopulateSkills();
        }
    }
    private void PopulateWeapons()
    {
        foreach (Transform child in weaponsItemContainer.transform)
        {
            Destroy(child.gameObject);
        }

        var weapons = gameStat.GetAllActiveWeapons();

        foreach (var weapon in weapons)
        {
            GameObject item = Instantiate(
                weaponItemPrefab,
                weaponsItemContainer.transform);

            item.GetComponent<IDCardWeaponItem>().Setup(weapon);
        }
    }
    private void PopulateSkills()
    {
        foreach (Transform child in skillsItemContainer.transform)
        {
            Destroy(child.gameObject);
        }

        GameStat_SO.SkillData[] skills =
        {
            gameStat.skill1,
            gameStat.skill2,
            gameStat.skill3,
            gameStat.skill4
        };

        foreach (var skill in skills)
        {
            if (skill.image == null)
                continue;

            GameObject item = Instantiate(
                skillItemPrefab,
                skillsItemContainer.transform);

            item.GetComponent<IDCardSkillItem>().Setup(skill);
        }
    }
}