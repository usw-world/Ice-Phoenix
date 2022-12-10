using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pool;
using TMPro;

public class DamageTextGenerator : MonoBehaviour {
    ParticlePool damagePooler;
    [SerializeField] GameObject damageTmp;
    [SerializeField] Transform drawingCanvas;

    private void Start() {
        damagePooler = new ParticlePool("Damage Text", damageTmp, 20, 10, drawingCanvas);
    }
    public void ShowDamageText(string number, Vector3 point, Color color) {
        GameObject text = damagePooler.OutPool(point, drawingCanvas);
        text.GetComponent<TextMeshProUGUI>().text = number;
        text.GetComponent<TextMeshProUGUI>().color = color;
        StartCoroutine(FadeOutUpCoroutine(text));
    }
    private IEnumerator FadeOutUpCoroutine(GameObject target) {
        float offset = 0;
        TextMeshProUGUI tmp = target.GetComponent<TextMeshProUGUI>();
        Color originColor = tmp.color;
        while(offset < 1) {
            offset += Time.deltaTime;
            // target.GetComponent<RectTransform>().position += new Vector3(0, 100 * Time.deltaTime, 0);
            target.transform.position += new Vector3(0, Time.deltaTime, 0);
            float alpha = Mathf.Lerp(originColor.a, 0, offset);
            tmp.color = new Color(originColor.r, originColor.g, originColor.b, alpha);
            yield return null;
        }
        tmp.color = originColor;
        damagePooler.InPool(target);
    }
}
