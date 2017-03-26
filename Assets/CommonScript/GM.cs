using UnityEngine;
using System.Collections;
using RL_Helpers;
using UnityEngine.UI;

namespace CoreCode {						//So we can keep supplied code seperate
	public class GM : Singleton {	//Derive from Singleton to get single instance static call ability

		static	GM	sGM;		//Our Singleton reference

		void Awake () {
			if (CreateSingleton<GM> (out sGM)) {
			}
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}