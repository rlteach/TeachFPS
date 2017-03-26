using UnityEngine;
using System.Collections;
using System;
using RL_Helpers;

//Collsion controller base class, abstracts Unity Collsion behaviour
//Processes triggers and colliders


//Makes sure this component is present as its needed by the script
//[RequireComponent(typeof(Rigidbody))]
namespace CoreCode {						//So we can keep supplied code seperate
    public class Entity : MonoBehaviour {

        //Get References to most used components

        public enum TypeID {     //Possible Enity types, used instead of tags
            None
            , Player		//ID for playey
            , Zombie1		//ID for Zombies
			, Zombie2
			, Zombie3
			, Zombie4
			, Zombie5
			, Zombie6
        }

        protected virtual void Start() {
			DB.MsgFormat("{0} Started",GetType().Name);
        }

		public virtual TypeID ID {     //get Type of Enity
            get {
				return TypeID.None;
            }
        }

        //Override default string conversion to print useful object information
        public override string ToString() {
            return string.Format("{0}({1}) {2:f2}", GetType().Name, name, transform.position);
        }

        private void NonEntityCollisionError(GameObject vOther) {       //error in case this is not an entity
			DB.MsgFormat("Ignoring non entity collision {0} with {1}",gameObject.name,vOther.gameObject.name);
        }

        //Wrapper for collsion
        void OnCollisionEnter(Collision vCollision) {
            Entity tCCOther = vCollision.gameObject.GetComponent<Entity>(); //Find collided with Entity
            if (tCCOther != null) {
                Collision(tCCOther, false);
            } else {
                NonEntityCollisionError(vCollision.gameObject);
            }
        }

        //Wrapper for trigger
        void OnTriggerEnter(Collider vCollider) {
            Entity tCCOther = vCollider.gameObject.GetComponent<Entity>();
            if (tCCOther != null) {
                Collision(tCCOther, true);       //Pass collision on
            } else {
                NonEntityCollisionError(vCollider.gameObject);
            }
        }

        //Default Collsion method prints message
        protected virtual void Collision(Entity vOther, bool vIsTrigger) {
            if (vIsTrigger) {        //Print appropriate message
				DB.MsgFormat("{0} trigger with {1} Type:{2}",gameObject.name ,vOther.gameObject.name,vOther.Type);
            } else {
				DB.MsgFormat("{0} collided with {1} Type:{2}",gameObject.name ,vOther.gameObject.name,vOther.Type);
            }
        }

        void OnDestroy() {
			DB.MsgFormat("{0} about to be destroyed",gameObject.name);
        }

        //Default destroy
        public virtual void Die() {
            Destroy(gameObject);
        }

    }
}