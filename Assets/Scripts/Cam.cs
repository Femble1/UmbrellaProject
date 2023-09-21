using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public int framerate = 120;
    int m_frameCounter = 0;
    float m_timeCounter = 0.0f;
    public float m_lastFramerate = 0.0f;
    public float m_refreshTime = 0.000000007f;
    public static Cam instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = framerate;
        frame_counter();
    }

    void frame_counter()
    {
        if( m_timeCounter < m_refreshTime )
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
            m_lastFramerate = (float)m_frameCounter/m_timeCounter;
            m_frameCounter = 0;
            m_timeCounter = 0.0f;
        }
    }
}
