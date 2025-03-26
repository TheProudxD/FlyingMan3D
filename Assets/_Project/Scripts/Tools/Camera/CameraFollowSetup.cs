/* 
    ------------------- Code Monkey -------------------
    
    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Tools.Camera {

    /*
     * Easy set up for CameraFollow, it will follow the transform with zoom
     * */
    public class CameraFollowSetup : MonoBehaviour {

        [FormerlySerializedAs("cameraFollow")] [SerializeField] private CameraFollow _cameraFollow;
        [FormerlySerializedAs("followTransform")] [SerializeField] private Transform _followTransform;
        [FormerlySerializedAs("zoom")] [SerializeField] private float _zoom;

        private void Start() {
            if (_followTransform == null) {
                Debug.LogError("followTransform is null! Intended?");
                _cameraFollow.Setup(() => Vector3.zero, () => _zoom, true, true);
            } else {
                _cameraFollow.Setup(() => _followTransform.position, () => _zoom, true, true);
            }
        }
    }

}