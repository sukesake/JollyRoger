
class QuickStartMainScriptJS extends MonoBehaviour {

private var m_layer:blindGUILayer;
private var m_startAnimation:boolean = false;

function Start() {
	m_layer = this.gameObject.GetComponent(blindGUILayer);
	m_layer.m_alpha = 0.0f;
	var buttongo:GameObject = GameObject.Find("OKButton");
	var button:blindGUIButton = buttongo.GetComponent(blindGUIButton);
	if (button != null) {
		button.m_buttonClickDelegate = this.OnButtonClick;	
	}
}

function Update () {
	
	if (m_layer && Time.time >= 1.0f && !m_startAnimation) {
		var targetShowAnimation:blindGUIAnimationState = new blindGUIAnimationState( m_layer );
		targetShowAnimation.alpha = 1.0f;
		m_layer.AnimateTo( targetShowAnimation, 1.0f);
		m_startAnimation = true;
	}
}
	
public function OnButtonClick( sender:blindGUIButton ):void {
	if ((sender.name == "OKButton") && m_layer) {
		var targetShowAnimation:blindGUIAnimationState = new blindGUIAnimationState( m_layer );
		targetShowAnimation.alpha = 0.0f;
		m_layer.AnimateTo( targetShowAnimation, 1.0f);	
	}
}

}