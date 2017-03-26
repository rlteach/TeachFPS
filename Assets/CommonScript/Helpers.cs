using UnityEngine;
using System.Collections;


namespace RL_Helpers {
	
public class DB  {

	#region DebugMessages
		static public bool Show = true;    //Show Debug messages

		//Allows debug string to be output, but allows this to turned off anythere in code by clearing ShowDebug 
		public static void MsgFormat(string vFormat, params object[] vArgs) {
			if (Show) {
				Debug.LogFormat(vFormat,vArgs);
			}
		}
		public static void Msg(string vMessage) {
			if (Show) {
				Debug.Log(vMessage);
			}
		}
		public static void Error(string vMessage) {
			if (Show) {
				Debug.LogError(vMessage);
			}
		}
		public static void ErrorFormat(string vFormat, params object[] vArgs) {
			if (Show) {
				Debug.LogErrorFormat(vFormat,vArgs);
			}
		}
    }
	#endregion

    public  class Util  {

        //Helper code which check a refernece to a compenent exsists and if not crates one, also shows error if Component can't be found
        //Will only find first component of that type
        public static bool LinkComponentFromChildrenIfNeeded<T>(GameObject vGO, ref T vComponent) where T : Component {
            if (vComponent != null) {       //If component already linked just return
                return true;
            }
            vComponent = vGO.GetComponentInChildren<T>();       //Try to find the component
            if (vComponent != null) {           //If  found then assing it and return true
                return true;
            }
            DB.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + "<" + typeof(T).Name + "> not found");
            return false;
        }
    }
}
