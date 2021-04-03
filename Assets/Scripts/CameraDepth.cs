using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TiltBrush {


  public class CameraDepth : MonoBehaviour {
#if (UNITY_EDITOR || EXPERIMENTAL_ENABLED)
    private Camera _Camera;
    public Camera c_Camera {
      get {
        if (!_Camera)
          _Camera = GetComponent<Camera>();
        return _Camera;
      }
    }

    public DepthTextureMode depthTextureMode;

    void OnEnable() {
      if (!Config.IsExperimental) {
        enabled = false;
        return;
      }
      c_Camera.depthTextureMode = depthTextureMode;
    }
    void OnDisable() {
      c_Camera.depthTextureMode = DepthTextureMode.None;
    }
#endif
  }
}
