using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private static int sortingOrder;

    private const float DISAPPEAR_TIMER_MAX = 0.6f;
    private TextMeshPro textmesh;
    private Action updateDel;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;
 



    public static DamagePopup Create (Vector3 position, int damageAmount,bool isCritical) 
    {
        Transform damagePopupTransform = Instantiate(_GameAssets.Instance.pfDamagePopup, position, Quaternion . identity) ;

        DamagePopup damagePopup  = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup. Setup (damageAmount,isCritical) ;
        return damagePopup ;
    }

    private void Awake() {
        textmesh = transform.GetComponent<TextMeshPro>();
        var renderer = GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "Effects"; // create this layer in project if needed
        renderer.sortingOrder = 10; 
    }

    public void Setup(int damageAmount,bool isCritical) 
    {

        textmesh.SetText(damageAmount.ToString());
        disappearTimer = DISAPPEAR_TIMER_MAX;
        updateDel=ActivateUpdate;
        if(isCritical)
        {
            textColor = _GameAssets.Instance.ciriticalHitColor;
            textmesh.fontSize=45f;
        }else{
            textColor= _GameAssets.Instance.normalHitColor;
            textmesh.fontSize=36f;
        }

        textmesh.color = textColor;

        sortingOrder++;
        textmesh.sortingOrder = sortingOrder;

        moveVector = new Vector3(.3f, 2f) * 2f;
        
        updateDel=ActivateUpdate;

    }
    void SetForward()
    {
        //transform.localPosition+=damagePopupOffset;
        updateDel=ActivateUpdate;

    }

    void Update ()=>updateDel?.Invoke();
   

    void ActivateUpdate()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f) {
            // First half of the popup lifetime
            float increaseScaleAmount = 0.4f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        } else {
            // Second half of the popup lifetime
            float decreaseScaleAmount = 0.4f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer <= 0) {
            // Start disappearing
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textmesh.color = textColor;
            if (textColor.a <= 0) {
                // updateDel=new util().NullFun;
                // this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
        }
   
    }
}
