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
        public float RotationSpeed = 360f;

        CharacterController mCC;                //Cached copy of controller

        Vector3 mMoveDirection = Vector3.zero;          //Next movement

        Vector3 mLastMousePosition = Vector3.zero;      //Used to calculate moves

        Camera mCamera;      //Assumes camara is parented to player

        public  RectTransform Sights;

        float mCamXRotation=0f;     //Camara inclination


        //Assumes camera is parented to player
        //Uses input to move and rotate player
        //However up and down acts only on camera to similate elevation
        void MoveCharacter() {          //Move Character with controller
            if (!Dead) {
                if (mCC.isGrounded) {   //Only take control if on ground, this is set by Character Controller
                    Vector3 tMouse = Camera.main.ScreenToViewportPoint(Input.mousePosition);        //Calculate mouse move delta
                    Vector3 tMove = mLastMousePosition - tMouse;
                    mLastMousePosition = tMouse;
                    if (Mathf.Abs(tMove.x)>0f) {        //Only do this if we moved X controls, ie rotate
						transform.Rotate (0, -Time.deltaTime * RotationSpeed * tMove.x*100f, 0);
					}

                    if (Mathf.Abs(tMove.y) > 0f) {      //Deal with up and down
                        if(tMove.y<0f) {
                            mCamXRotation = Mathf.Max(mCamXRotation + tMove.y * Time.deltaTime * RotationSpeed*100f,-20f);      //Clamp down
                        } else if (tMove.y>0f) {
                            mCamXRotation = Mathf.Min(mCamXRotation + tMove.y * Time.deltaTime * RotationSpeed*100f, 20f);      //Clamp up
                        }
                        mCamera.transform.rotation = Quaternion.Euler(mCamXRotation, mCamera.transform.rotation.eulerAngles.y, mCamera.transform.rotation.eulerAngles.z); // Rotate camera up/down
                    }

                    if (Input.GetKey (KeyCode.UpArrow)) {               //Move player
						mMoveDirection.z = Time.deltaTime * MoveSpeed;
					} else if (Input.GetKey (KeyCode.DownArrow)) {
						mMoveDirection.z = -Time.deltaTime * MoveSpeed;
					} else {
                        mMoveDirection.z = 0f;      //If no movement stop
                    }
                    if (Input.GetKey(KeyCode.LeftArrow)) {
                        mMoveDirection.x = Time.deltaTime * MoveSpeed;
                    } else if (Input.GetKey(KeyCode.RightArrow)) {
                        mMoveDirection.x = -Time.deltaTime * MoveSpeed;
                    } else {
                        mMoveDirection.x = 0f;  //If no movement stop
                    }
                    mMoveDirection.y = 0f;
                    mMoveDirection = transform.TransformDirection(mMoveDirection);      //Move in direction character is facing
                    mMoveDirection *= MoveSpeed;
                    if (Input.GetKey(KeyCode.Return)) {         //Deal with jumping
                        mMoveDirection.y = mJumpHeight;        //Jump
                    }
                }
                mMoveDirection.y += Physics.gravity.y * Time.deltaTime;     //Apply gravity for jum
                mCC.Move(mMoveDirection * Time.deltaTime);
            }
        }

        #endregion


        #region Housekeeping

        protected override void Start() {
            base.Start();       //Process base class Startup
            mCC = GetComponent<CharacterController>();      //Cache character controller
            GM.PC = this;       //Link player to Game Manager       
            mCamera = GetComponentInChildren<Camera>();     //Cache child camera
            mLastMousePosition = mCamera.ScreenToViewportPoint(Input.mousePosition);        //Get a mouse postion to update does not jerk on first run
        }


        // Update is called once per frame
        void Update() {
            MoveCharacter();            //Move player
            if(mCooldown.Cool(Time.deltaTime) ) {       //Restrict fire rate
                DoFire();
            }
        }
        public override TypeID ID {     //get Type of Enity
            get {
                return TypeID.Player;
                }
            }
        #endregion

        #region Fire

        Cooldown mCooldown = new Cooldown(0.1f);        //10 shots per second


        public GameObject BulletPrefab;

        void DoFire() {
            if(Input.GetMouseButton(0)) {           //Simulate fire
                Vector3 tScreen = Sights.TransformPoint(Vector3.zero);              //Get acreen position of UI target sprite
                Vector3 tGunDirection = Sights.TransformVector(Vector3.forward).normalized;      //Find out forward vector for gun based on that sprite
                Vector3 tWorld = mCamera.ScreenToWorldPoint(tScreen);           //Convert UI position (Screen) to World using our camera 
                tGunDirection = mCamera.transform.rotation * tGunDirection;           //use Camera rotation to angle, gun as camera is parented to player, this is liek aiming a gun
                Debug.DrawLine(tWorld, tWorld+tGunDirection*10f, Color.red,1f);      //Draw line of fire for debug
                GameObject tGO = Instantiate<GameObject>(BulletPrefab);
                tGO.transform.position = tWorld+Vector3.back;
                tGO.GetComponent<Rigidbody>().velocity = tGunDirection * 50f;
                Destroy(tGO, 2f);
            }
        }
        #endregion


        #region Interaction
        protected override void Collision(Entity vOther, bool vIsTrigger) {     //This means Player collided
            DB.MsgFormat("Player {0} with {1}",(vIsTrigger)?"Triggered":"Collided", vOther.ID);
        }
        #endregion

    }
}