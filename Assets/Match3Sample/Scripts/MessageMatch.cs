/* Copyright (c) Vander Amaral
 * This code holds the Messages Animation that appears on the Screen
 * I tried to make it the best and easy to change.
 * You can polish it even more, and add functions to it if you bought.
 */ 

using UnityEngine;
using System.Collections;

public class MessageMatch : MonoBehaviour {

	public float speed = 0f;
	public bool wait = false;
	public float howLong = 1;
	public int direction = 0;  //1 Up, 2 Down, 3 left, 4 right
	public float directionSpeed = 0.3f;
	public float finalSize = 1.9f;
	public bool expand = true;
	//public string 
	
	
	// Update is called once per frame
	void Start (){
		if(wait) StartCoroutine(WaitForSecond(howLong));
	}
	
	void LateUpdate () {
		
		if(expand){
		if(transform.localScale.x < finalSize) 
		{ 
			transform.localScale += transform.localScale * (0.15f+speed); 
		}
		else
		{ 
			if(!wait){
				if(direction != 0) gotoDirection();
				transform.localScale += transform.localScale * (0.008f+speed);
				GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f,GetComponent<Renderer>().material.color.a-(0.08f+speed));
				if(GetComponent<Renderer>().material.color.a < 0.01f) Destroy(gameObject);
			}
		}
			
		}else{
			if(!wait){
				if(direction != 0) gotoDirection();
				GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b,GetComponent<Renderer>().material.color.a-(0.08f+speed));
				if(GetComponent<Renderer>().material.color.a < 0.01f) Destroy(gameObject);
			}
		}
		
	}
	
	IEnumerator WaitForSecond(float a) {
 		yield return new WaitForSeconds(a);
		wait = false;
	}
	
	void gotoDirection(){
		if(direction == 1) transform.Translate(0,directionSpeed,0);
		if(direction == 2) transform.Translate(0,-directionSpeed,0);
		if(direction == 3) transform.Translate(directionSpeed,0,0);
		if(direction == 4) transform.Translate(-directionSpeed,0,0);
	}
}
