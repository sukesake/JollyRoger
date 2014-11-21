import UnityEngine

class QuickStartMainScriptBoo (MonoBehaviour): 

	private m_layer as blindGUILayer
	private m_startAnimation as bool = false

	def Start ():
		m_layer = GetComponent[of blindGUILayer]()
		m_layer.m_alpha = 0.0f	
		buttongo as GameObject = GameObject.Find("OKButton")
		button as blindGUIButton = buttongo.GetComponent[of blindGUIButton]()
		if not (button == null):
			button.m_buttonClickDelegate = self.OnButtonClick		
	
	def Update ():
		if m_layer and (Time.time >= 1.0f) and not m_startAnimation :
			targetShowAnimation as blindGUIAnimationState = blindGUIAnimationState( m_layer )
			targetShowAnimation.alpha = 1.0f
			m_layer.AnimateTo( targetShowAnimation, 1.0f)
			m_startAnimation = true

	def OnButtonClick ( sender as blindGUIButton ):
		if sender.name == "OKButton" and m_layer :
			targetShowAnimation as blindGUIAnimationState = blindGUIAnimationState( m_layer )
			targetShowAnimation.alpha = 0.0f
			m_layer.AnimateTo( targetShowAnimation, 1.0f)
