using UnityEngine;
using System.Collections;

public class QuickStartMainScriptCS : MonoBehaviour {
	
	private blindGUILayer m_layer;
	private bool m_startAnimation = false;
	
	// Use this for initialization
	void Start () {
		m_layer = this.gameObject.GetComponent<blindGUILayer>();
		m_layer.m_alpha = 0.0f;
		GameObject buttongo = GameObject.Find("OKButton");
		
		blindGUIButton button = buttongo.GetComponent<blindGUIButton>();
		if (button != null) {
			button.m_buttonClickDelegate = this.OnButtonClick;	
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (m_layer && Time.time >= 1.0f && !m_startAnimation) {
			blindGUIAnimationState targetShowAnimation = new blindGUIAnimationState( m_layer );
			targetShowAnimation.alpha = 1.0f;
			m_layer.AnimateTo( targetShowAnimation, 1.0f);
			m_startAnimation = true;
		}
	}
	
	public void OnButtonClick( blindGUIButton sender ) {
		if ((sender.name == "OKButton") && m_layer) {
			blindGUIAnimationState targetShowAnimation = new blindGUIAnimationState( m_layer );
			targetShowAnimation.alpha = 0.0f;
			m_layer.AnimateTo( targetShowAnimation, 1.0f);	
		}
	}
}
