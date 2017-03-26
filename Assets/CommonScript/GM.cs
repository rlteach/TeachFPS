using UnityEngine;
using System.Collections;
using RL_Helpers;
using UnityEngine.UI;

namespace CoreCode {                        //So we can keep supplied code seperate
    public class GM : Singleton {   //Derive from Singleton to get single instance static call ability

    #region SingletonBehaviour
        static GM sGM;      //Our Singleton reference
        void Awake() {
            if (CreateSingleton<GM>(ref sGM)) {
            }
        }
    #endregion

    #region Player    
        PlayerController mPC;

        public static PlayerController PC {
            get {
                return  sGM.mPC;
            }
            set {
                    sGM.mPC = value;
            }
        }
    #endregion
	}
}