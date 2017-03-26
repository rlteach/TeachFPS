using UnityEngine;
using System.Collections;
using System;

//Collsion controller base class, abstracts Unity Collsion behaviour
//Processes triggers and colliders


//Makes sure this component is present as its needed by the script
//[RequireComponent(typeof(Rigidbody))]
namespace SinglePlayer {
    public class Entity : MonoBehaviour {

        //Get References to most used components

        public enum EType {     //Possible Enity types, used instead of tags
            None
            , Player
            , Pickup
        }

        protected virtual void Start() {
            GM.DebugMsg(GetType().Name + " Started");
        }

        public virtual EType Type {     //get Type of Enity
            get {
                return EType.None;
            }
        }

        //Override default string conversion to print useful object information
        public override string ToString() {
            return string.Format("{0}({1}) {2:f2}", GetType().Name, name, transform.position);
        }

        private void NonEntityCollisionError(GameObject vOther) {       //error in case this is not an entity
            GM.Msg("Ignoring non entity collision " + gameObject.name + " with " + vOther.gameObject.name);
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
                GM.Msg(gameObject.name + " trigger with " + vOther.gameObject.name + "Type " + vOther.Type);
            } else {
                GM.Msg(gameObject.name + " collided with " + vOther.gameObject.name + "Type " + vOther.Type);
            }
        }

        void OnDestroy() {
            GM.Msg(gameObject.name + " about to be destroyed");
        }

        //Default destroy
        public virtual void Die() {
            Destroy(gameObject);
        }

    }
}