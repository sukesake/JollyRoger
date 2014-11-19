using UnityEngine;
using System.Collections;

public class ClockArrowController : MonoBehaviour {
	
	public blindGUITexturedContainer m_hoursArrow;
	public blindGUITexturedContainer m_minutesArrow;
	public blindGUITexturedContainer m_secondsArrow;
	public blindGUITexturedContainer m_hoursArrowShadow;
	public blindGUITexturedContainer m_minutesArrowShadow;
	public blindGUITexturedContainer m_secondsArrowShadow;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (m_hoursArrow != null) {
			m_hoursArrow.m_angle = (System.DateTime.Now.Hour/12.0f+System.DateTime.Now.Minute/(12.0f*60.0f)+System.DateTime.Now.Second/(12.0f*60.0f*60.0f)+System.DateTime.Now.Millisecond/(12.0f*60.0f*60.0f*1000.0f))*360.0f;	
		}
		if (m_minutesArrow != null) {
			m_minutesArrow.m_angle = (System.DateTime.Now.Minute/60.0f+System.DateTime.Now.Second/(60.0f*60.0f)+System.DateTime.Now.Millisecond/(60.0f*60.0f*1000.0f))*360.0f;
		}
		if (m_secondsArrow != null) {
			m_secondsArrow.m_angle = (System.DateTime.Now.Second/60.0f+System.DateTime.Now.Millisecond/(60*1000.0f))*360.0f;	
		}
		
		if (m_hoursArrowShadow != null) {
			m_hoursArrowShadow.m_angle = (System.DateTime.Now.Hour/12.0f+System.DateTime.Now.Minute/(12.0f*60.0f)+System.DateTime.Now.Second/(12.0f*60.0f*60.0f)+System.DateTime.Now.Millisecond/(12.0f*60.0f*60.0f*1000.0f))*360.0f;	
		}
		if (m_minutesArrowShadow != null) {
			m_minutesArrowShadow.m_angle = (System.DateTime.Now.Minute/60.0f+System.DateTime.Now.Second/(60.0f*60.0f)+System.DateTime.Now.Millisecond/(60.0f*60.0f*1000.0f))*360.0f;	
		}
		if (m_secondsArrowShadow != null) {
			m_secondsArrowShadow.m_angle = (System.DateTime.Now.Second/60.0f+System.DateTime.Now.Millisecond/(60*1000.0f))*360.0f;	
		}
	}
}
