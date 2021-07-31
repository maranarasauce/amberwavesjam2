using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopup : MonoBehaviour
{
    private Text Text;
    private Color color;
    private Transform PlayerTransform;


    public float dissapearTimer = 0.5f;
    public float fadeOutSpeed = 5f;
    public float moveYSpeed = 1f;


    public void SetUp(int amount)
    {
        Text = gameObject.GetComponent<Text>();
        PlayerTransform = Camera.main.transform;
        color = Text.color;
        Text.text = amount.ToString();
    }

    private void LateUpdate()
    {
        transform.LookAt(2 * transform.position - PlayerTransform.position);
        transform.position += new Vector3(0f, moveYSpeed * Time.deltaTime, 0f);

        dissapearTimer = Time.deltaTime;
        if(dissapearTimer <= 0f)
        {
            color.a -= fadeOutSpeed * Time.deltaTime;
            Text.color = color;
            if(Text.color.a <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
