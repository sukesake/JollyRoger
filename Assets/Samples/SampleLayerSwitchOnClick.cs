using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SampleLayerSwitchOnClick : MonoBehaviour {
	
	public blindGUILayer m_switchLayer;
	private blindGUILayer m_currentLayer;
	private blindGUIButton m_connectedButton;
	
	// Use this for initialization
	void Start () {
		m_connectedButton = this.gameObject.GetComponent<blindGUIButton>();
		if (m_connectedButton != null) {
			m_connectedButton.m_buttonClickDelegate = this.OnButtonClick;	
		}
		GameObject currentGO = this.gameObject;
		while (currentGO.transform.parent != null) {
			m_currentLayer = currentGO.GetComponent<blindGUILayer>();
			if (m_currentLayer != null) {
				break;
			}
			currentGO = currentGO.transform.parent.gameObject;
		}	
	}
	
	void OnButtonClick( blindGUIButton sender ) {
		if (sender == m_connectedButton) {
			if (m_currentLayer != null) {
				blindGUIAnimationState targetState = new blindGUIAnimationState( m_currentLayer );
				targetState.alpha = 0.0f;
				m_currentLayer.AnimateTo(targetState, 1.0f);
			}
			if (m_switchLayer != null) {
				blindGUIAnimationState targetState = new blindGUIAnimationState( m_switchLayer );
				targetState.alpha = 1.0f;
				m_switchLayer.AnimateTo(targetState, 1.0f,null,iTweenInBlindGUI.EaseType.linear,0.25f);
			}
		}
	}
}
