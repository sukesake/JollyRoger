using UnityEngine;
using System.Collections;

public class LoadIndicatorScript : MonoBehaviour {
	
	public blindGUILayer m_startupLayer;
	private blindGUILayer m_currentLayer;
	
	private bool m_startAnimation = false;
	
	// Use this for initialization
	void Start () {
		m_currentLayer = GetComponent<blindGUILayer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_startupLayer && m_currentLayer && Time.time >= 2.0f && !m_startAnimation) {
			
			blindGUIAnimationState targetShowAnimation = new blindGUIAnimationState( m_startupLayer );
			targetShowAnimation.alpha = 1.0f;
			m_startupLayer.AnimateTo( targetShowAnimation, 1.0f);
			m_startAnimation = true;
			
			blindGUIAnimationState targetHideAnimation = new blindGUIAnimationState( m_currentLayer );
			targetHideAnimation.alpha = 0.0f;
			m_currentLayer.AnimateTo( targetHideAnimation, 1.0f);
		}
		
		if (Time.time <= 3.0f) {
			GameObject loadIndicatorGO = GameObject.Find("LoadIndicator");
			if (loadIndicatorGO) {
				blindGUITexturedContainer loadIndicator	= loadIndicatorGO.GetComponent<blindGUITexturedContainer>();
				if (loadIndicator) {
					loadIndicator.m_angle = 30*(int)(Time.time*10.0f);
				}
			}
		}
	}
}
