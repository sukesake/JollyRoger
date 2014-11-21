using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SampleFractal: MonoBehaviour {
	
	blindGUIParentElement m_parent;
	public List<blindGUIParentElement> elements;
	bool m_first = true;
	
	public Vector2 m_anchorPoint = new Vector2(-0.5f,-0.5f);
	
	// Use this for initialization
	void Start () {
		m_parent = this.gameObject.GetComponent<blindGUIParentElement>();
		Add(0,this.gameObject);
		m_parent.UpdateLayout();
	}
	
	void Add(int it, GameObject whereTo) {
		if (it == 30) return;
	
		GameObject go = new GameObject("Fractal_ "+it.ToString());
		blindGUIColorTexturedContainer pe = go.AddComponent<blindGUIColorTexturedContainer>();
		pe.m_size = new Vector2(100,100);
		if (!m_first) {
			pe.m_offset = new Vector2(pe.m_size.x*(-m_anchorPoint.x)*2,pe.m_size.y*(-m_anchorPoint.y)*2);
		}
		m_first = false;
		pe.m_anchorPoint = m_anchorPoint;//new Vector2(-0.5f,-0.5f);
		pe.m_scale = 0.9f;
		pe.m_angle = 0;
		pe.m_horizontalAlign  = blindGUIParentElement.HALIGN.free;
		pe.m_verticalAlign = blindGUIParentElement.VALIGN.free;
		pe.m_textureColor = new Color(0,0,0,(float)it/20.0f);
		
		elements.Add(pe);
		
		go.transform.parent = whereTo.transform;
			
		Add(++it,go);
	}
	
	void Update() {
		int id = 0;
		foreach( blindGUIParentElement element in elements ) {
			element.m_angle = Time.time*40+id;
			id++;
		}
	}
	
	
}
