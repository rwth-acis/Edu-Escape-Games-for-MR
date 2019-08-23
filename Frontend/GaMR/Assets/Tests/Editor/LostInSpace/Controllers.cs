using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace Tests {
    
    public class GrabControllerTests {

        [UnityTest]
        public IEnumerator SimpleGrabTest() {
            var grabObject = new GameObject().AddComponent<GrabController>();
            GrabController controller = grabObject.GetComponent<GrabController>();
            controller.isMoveable = true;
            controller.grabbedDistance = 2;

            controller.OnInputClicked(null);

            yield return null;      // Skip frame

            Assert.IsTrue(controller.IsGrabbed());
        }

        [UnityTest]
        public IEnumerator GrabAndPlaceTest() {
            var grabObject = new GameObject().AddComponent<GrabController>();
            GrabController controller = grabObject.GetComponent<GrabController>();
            controller.isMoveable = true;
            controller.grabbedDistance = 2;

            controller.OnInputClicked(null);

            yield return null;      // Skip frame

            Assert.IsTrue(controller.IsGrabbed());

            controller.OnInputClicked(null);

            yield return null;      // Skip frame

            Assert.IsFalse(controller.IsGrabbed());
        }

        [UnityTest]
        public IEnumerator GrabNonMoveableObjectTest() {
            var grabObject = new GameObject().AddComponent<GrabController>();
            GrabController controller = grabObject.GetComponent<GrabController>();
            controller.isMoveable = false;
            controller.grabbedDistance = 2;

            controller.OnInputClicked(null);

            yield return null;      // Skip frame

            Assert.IsFalse(controller.IsGrabbed());
        }

        [UnityTest]
        public IEnumerator GrabTest() {
            var grabObject = new GameObject().AddComponent<GrabController>();
            GrabController controller = grabObject.GetComponent<GrabController>();
            controller.isMoveable = false;
            controller.grabbedDistance = 2;

            controller.OnInputClicked(null);

            yield return null;      // Skip frame

            Assert.IsFalse(controller.IsGrabbed());

            controller.isMoveable = true;
            controller.OnInputClicked(null);

            yield return null;      // Skip frame

            Assert.IsTrue(controller.IsGrabbed());

            controller.isMoveable = false;
            controller.StopGrabbing();

            yield return null;      // Skip frame

            Assert.IsFalse(controller.IsGrabbed());
        }

    }

    public class LookAtControllerTests {

        [UnityTest]
        public IEnumerator SimpleLookAtTest() {
            var lookObject = new GameObject().AddComponent<LookAtController>();
            LookAtController controller = lookObject.GetComponent<LookAtController>();
            controller.lookAtDistance = 1;
            controller.speed = 1;

            Assert.IsFalse(controller.IsLookingAt());

            controller.OnInputClicked(null);

            yield return null;      // Skip frame

            Assert.IsTrue(controller.IsLookingAt());
        }

    }

    public class CablieModification {

        [UnityTest]
        public IEnumerator CableClickTest() {
            var cableObject = new GameObject().AddComponent<CableModification>();
            CableModification controller = cableObject.GetComponent<CableModification>();
            controller.tileType = 0;
            controller.x = 0;
            controller.y = 0;
            controller.rotation = 0;

            yield return null;

            Assert.IsTrue(controller.rotation == 0);

            controller.OnInputClicked(null);

            yield return null;

            Assert.IsFalse(controller.rotation == 0);
        }

        [UnityTest]
        public IEnumerator ThreeSixtyTurnTest() {
            var cableObject = new GameObject().AddComponent<CableModification>();
            CableModification controller = cableObject.GetComponent<CableModification>();
            controller.tileType = 0;
            controller.x = 0;
            controller.y = 0;
            controller.rotation = 0;

            yield return null;

            Assert.IsTrue(controller.rotation == 0);

            for (int i = 0; i < 4; i++) {   // Turn 4 times
                controller.OnInputClicked(null);
                yield return null;
            }

            Assert.IsTrue(controller.rotation == 0);
        }

    }
}
