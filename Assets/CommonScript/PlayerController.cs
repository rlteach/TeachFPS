using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using RL_Helpers;

namespace CoreCode {
    [RequireComponent(typeof(CharacterController))]		//This ensures we have a character controller, if needed will add one
    public class PlayerController : Entity {

        #region Stats
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
					if (Input.GetKey (KeyCode.LeftArrow)) {
						transform.Rotate (0, -Time.deltaTime * 360f, 0);
					}
					if (Input.GetKey (KeyCode.RightArrow)) {
						transform.Rotate (0, Time.deltaTime * 360f, 0);
					}
					if (Input.GetKey (KeyCode.UpArrow)) {
						mMoveDirection.z = Time.deltaTime;
					}
					if (Input.GetKey (KeyCode.UpArrow)) {
						mMoveDirection.z = -Time.deltaTime;
					}
                    mMoveDirection.x = 0f;
                    mMoveDirection.y = 0f;
                    mMoveDirection = transform.TransformDirection(mMoveDirection);      //Move in direction character is facing
                    mMoveDirection *= MoveSpeed;
                    if (Input.GetKey(KeyCode.Return)) {
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
        }

        // Update is called once per frame
        void Update() {
            MoveCharacter();            //Move player
        }
		public override TypeID ID {     //get Type of Enity
            get {
				return TypeID.Player;
            }
        }
        #endregion


        #region Interaction
        protected override void Collision(Entity vOther, bool vIsTrigger) {     //This means Player collided
            if (vOther.Type == EType.Pickup) {
				DB.MsgFormat("Player {0} with {1}",(vIsTrigger)?"Triggered":"Collided", vOther.ID);
            }
        }
        #endregion

    }
}