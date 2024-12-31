using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
    
public class TimeLineManager : MonoBehaviour
{
    public static TimeLineManager instance;

    private PlayableDirector pd;
    public TimelineAsset[] timelineAsset;

    public bool isCutScene; // ÇöÀç ÄÆ¾À ÁßÀÎ°¡
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
        pd = GetComponent<PlayableDirector>();
    }
    public void StartCutScene(int index) // ÄÆ¾À ½ÃÀÛ
    {
        pd.Play(timelineAsset[index]);

        isCutScene = true;
    }

    public void EndCutScene()
    {
        isCutScene = false;
    }
}
