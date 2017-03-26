using UnityEngine;
using System.Collections;
namespace CoreCode {

    public class Zombie : Entity {

        protected override void Start() {
            base.Start();       //Process base class Startup
        }

        public override TypeID ID {     //get Type of Enity
            get {
                return TypeID.Zombie1;
                }
            }
    }
}
