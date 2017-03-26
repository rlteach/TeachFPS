using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using RL_Helpers;

namespace SinglePlayer {
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : Entity {

        #region Stats
        int mWater = 3;
        int mFood = 3;
        bool mDead = false;


        float mJumpHeight = 10f;

        public float JumpHeight {
            set {
                if (value > 0f && value < 30f) {
                    mJumpHeight = value;
                } else {
                    mJumpHeight = 10f;
                }
            }
            get {
                return mJumpHeight;
            }
        }

        public bool Dead {      //Setter to deal with Dead flag, prints message to console on death
            get {
                return mDead;
            }
            set {
                mDead = value;
                if (mDead) {
                    GM.DebugText = "You are dead";
                }
            }
        }

        public int Water {      //Setter to deal with Water, will not allow it to go below 0 and trigger death
            get {
                return mWater;
            }
            set {
                mWater = value;
                if (mWater < 0) {
                    mWater = 0;
                    Dead = true;
                }
            }
        }

        public int Food {       //Setter to deal with Food, will not allow it to go below 0 and trigger death
            get {
                return mFood;
            }
            set {
                mFood = value;
                if (mFood < 0) {
                    mFood = 0;
                    Dead = true;
                }
            }
        }
        #endregion

        #region PlayerModifiers

        List<Pickup> mModifiers = new List<Pickup>();       //List of modifiers applied to player



        public bool AcceptPickup(Pickup vPickup) {      //Add a new modifier to player
            mModifiers.Add(vPickup);
            GM.DebugText = PickupItems();
            return true;
        }

        public string PickupItems() {                   //Get string with current modifiers
            string tDebug = "";
            foreach (var tM in mModifiers) {
                tDebug += tM.Name + "\n";
            }
            return tDebug;
        }

        void ProcessModifiers() {                   //Process all modifiers attached to player
            List<Pickup> tToDelete = new List<Pickup>();        //Keep a list of ones to delete, needed as you cannot deleter items from list while iterating it
            foreach (var tM in mModifiers) {
                tM.UpdatePlayer(this, Time.deltaTime);      //Ask modifer to apply its effect
                if (tM.Delete) {        //If modifier sets Delete flag, schedule its deletion, by adding to Delete List
                    tToDelete.Add(tM);
                }
            }
            foreach (var tM in tToDelete) {     //Delete all the modifers which were marked for deletion, using delete list
                mModifiers.Remove(tM);
            }
        }


        #endregion


        #region Movement
        //Set the speed of movement in inspector;
        public float MoveSpeed = 10f;

        CharacterController mCC;

        Vector3 mMoveDirection = Vector3.zero;

        void MoveCharacter() {          //Move Character with controller
            if (!Dead) {
                if (mCC.isGrounded) {
                    transform.Rotate(0, IC.GetInput(IC.Directions.MoveX), 0);
                    mMoveDirection.x = 0f;
                    mMoveDirection.y = 0f;
                    mMoveDirection.z = IC.GetInput(IC.Directions.MoveY);
                    mMoveDirection = transform.TransformDirection(mMoveDirection);      //Move in direction character is facing
                    mMoveDirection *= MoveSpeed;
                    if (IC.GetInput(IC.Directions.Jump) > 0f) {
                        mMoveDirection.y = mJumpHeight;        //Jump
                    }
                }
                mMoveDirection.y += Physics.gravity.y * Time.deltaTime;
                mCC.Move(mMoveDirection * Time.deltaTime);
            }
        }

        #endregion


        #region Housekeeping

        protected override void Start() {
            base.Start();       //Process base class Startup
            mCC = GetComponent<CharacterController>();
            GM.AddPlayer(this);     //Add to player list
        }

        // Update is called once per frame
        void Update() {
            ProcessModifiers();     //Apply modifiers to player
            MoveCharacter();            //Move player
        }
        public override EType Type {     //get Type of Enity
            get {
                return EType.Player;
            }
        }
        #endregion


        #region Interaction
        protected override void Collision(Entity vOther, bool vIsTrigger) {     //This means Player collided
            if (vOther.Type == EType.Pickup) {
                GM.DebugMsg("Player Collided with " + vOther.name);
            }
        }
        #endregion

    }
}