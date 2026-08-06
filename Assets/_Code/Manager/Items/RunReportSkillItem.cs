using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunReportSkillItem : MonoBehaviour
{
    public Image skillImage;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillLevelText;
    public TextMeshProUGUI skillBuffText;

    public void Setup(GameStat_SO.SkillData skillData)
    {
        skillImage.sprite = skillData.image;

        // Temporary dummy values
        skillNameText.text = "Skill";
        skillLevelText.text = "Lv. " + skillData.level;
        skillBuffText.text = "+10% Effect";
    }
}