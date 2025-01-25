using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] private RawImage _skillIcon1;
    [SerializeField] private RawImage _skillIcon2;

    public void MakeSkill1Able()
    {
        _skillIcon1.gameObject.SetActive(true);
        _skillIcon2.gameObject.SetActive(false);
    }

    public void MakeSkill2Able()
    {
        _skillIcon1.gameObject.SetActive(false);
        _skillIcon2.gameObject.SetActive(true);
    }
}
