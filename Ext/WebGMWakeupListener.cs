using UnityEngine;
using System.Collections;

public class WebGMWakeupListener : MonoBehaviour
{

    private const float TIME_LIMIT = 0.3f;
    private const int NEED_COUNT = 10;
    private int count = 0;
    private float previousTouchTime = 0f;
    private const float AREA_LIMIT = 100f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var x = Input.mousePosition.x;
            var y = Input.mousePosition.y;

            //Debug.Log("x:" + x + ", y:" + y);

            if (x < AREA_LIMIT && y < AREA_LIMIT)
            {
                Debug.Log("activate touch");

                if ((Time.realtimeSinceStartup - previousTouchTime) > TIME_LIMIT)
                {
                    count = 1;
                }
                else
                {
                    count++;
                }

                previousTouchTime = Time.realtimeSinceStartup;

                if (count >= NEED_COUNT)
                {
                    WakeupWebGM();
                    count = 0;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            WakeupWebGM();
        }
    }

    public static string kWebGMAutoStart = "__WebGMAutoStart";

    private static bool ShouldAutoStartWebGMService()
    {
        return PlayerPrefsX.GetBool(kWebGMAutoStart);
    }

    private static bool m_hasWakeup = false;
    public static void WakeupWebGM()
    {
        if (m_hasWakeup)
        {
            Debug.LogWarning("WebGM has already wakeup");
        }
        else
        {
            var console = new GameObject("CURLD Console");
            GameObject.DontDestroyOnLoad(console);

            console.AddComponent<CUDLR.Server>();

            Debug.Log("WebGM service started");
            
            var tipSys = GameRoot.GetGameSystemS<MsgInfoSystem>();
            if (tipSys != null)
            {
                tipSys.ShowMsgInfo("WebGM service started");
            }
            
        }
    }

    public static void Setup()
    {
        var go = new GameObject("WebGMWakeup");
        go.hideFlags = HideFlags.HideInHierarchy;
        go.AddComponent<WebGMWakeupListener>();
        GameObject.DontDestroyOnLoad(go);

        if (ShouldAutoStartWebGMService())
        {
            WakeupWebGM();
        }
    }
}
