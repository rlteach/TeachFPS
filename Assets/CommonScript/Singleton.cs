using UnityEngine;
using System.Collections;

namespace RL_Helpers {
	abstract public class Singleton : MonoBehaviour {

		#region Singleton
		protected bool CreateSingleton<T>(ref T sGM) where T : Singleton {  //Set Up singleton for a Type
			if (sGM == null) {
				sGM = (T)this;
				DontDestroyOnLoad(gameObject);
				DB.MsgFormat("First time creation of:{0}", this.GetType ().Name);
				return true;        //Signal back if this is the first time this has been created
			} else if (sGM != this) {
				Destroy(gameObject);
				DB.MsgFormat("Subsequent creation of: {0} ignored",this.GetType().Name);
			}
			return false;   //Don't do it twice
		}
		#endregion
	}
}