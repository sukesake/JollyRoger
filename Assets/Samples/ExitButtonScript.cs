using UnityEngine;
using System.Collections;

public class ExitButtonScript : MonoBehaviour {
	
	private blindGUIButton m_connectedButton;
	
	// Use this for initialization
	void Start () {
		m_connectedButton = this.gameObject.GetComponent<blindGUIButton>();
		if (m_connectedButton != null) {
			m_connectedButton.m_buttonClickDelegate = this.OnButtonClick;	
		}
	}
	
	void OnButtonClick( blindGUIButton sender ) {
		if (sender == m_connectedButton) {
			Application.Quit();
		}
	}
}
