using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BlockAnim : MonoBehaviour
{
    [SerializeField]
    private float startDlayTime;
    [SerializeField]
    private float toY;
    

    public void VerticalWave()
    {
        LeanTween.moveY(this.gameObject, toY, 3f).setEase(LeanTweenType.easeShake).setLoopCount(0);
    }
    private void Start()
    {
        LeanTween.delayedCall(startDlayTime, VerticalWave);
    }





}
