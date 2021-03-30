#if (UNITY_EDITOR || EXPERIMENTAL_ENABLED)
using UnityEngine;
using System.Collections.Generic;

namespace TiltBrush {
  public partial class FreePaintTool {
    [SerializeField] private List<Transform> m_GuideCubes;
    [SerializeField] private List<Transform> m_GuideCubeOutlines;

    public class GuideCube {
      public Transform cubeTx;
      public Renderer cubeRenderer;

      public Transform cubeOutlineTx;
      public Renderer cubeOutlineRenderer;

      public const float OutlineWidth = 0.1f;
      public const float BaseThickness = 0.025f;

      public GuideCube(Transform transform, Transform transformOutline) {
        this.cubeTx = transform;
        cubeRenderer = transform.GetComponent<Renderer>();

        this.cubeOutlineTx = transformOutline;
        cubeOutlineRenderer = transformOutline.GetComponent<Renderer>();
      }

      private bool _enabled;
      public bool enabled {
        get {
          return _enabled;
        }
        set {
          _enabled = value;
          cubeRenderer.enabled = _enabled;
          cubeOutlineRenderer.enabled = _enabled;
        }
      }

      public enum PathModeID {
        Orbit
      }

      public PathModeID pathMode;
      public TrTransform transform;
      public Quaternion tilt;

      private TrTransform Orbit(float radius) {

        TrTransform result = new TrTransform();

        Vector3 spindleAxis = transform.rotation * Vector3.up;

        Quaternion spindleRotation = Quaternion.AngleAxis(lerpT * -360, spindleAxis);
        Quaternion radialLookRot = transform.rotation;

        Vector3 radialOffset = spindleRotation * radialLookRot * Vector3.forward;
        result.translation = transform.translation + radialOffset * radius;

        Quaternion pointerRotation = spindleRotation * radialLookRot * tilt;
        result.rotation = pointerRotation;

        result.scale = transform.scale;

        return result;
      }

      private float _lerpT;
      public float lerpT {
        get {
          return _lerpT;
        }
        set {
          _lerpT = value;
          Update();
        }
      }
      private float _radius;
      public float radius {
        get {
          return _radius;
        }
        set {
          _radius = value;
          Update();
        }
      }


    private void Update() {
        TrTransform result;
        switch (pathMode) {
          case PathModeID.Orbit:
            result = Orbit(radius);

            cubeTx.transform.position = result.translation;
            cubeTx.transform.rotation = result.rotation;
            cubeTx.transform.localScale = new Vector3(BaseThickness, BaseThickness, result.scale);

            
            cubeOutlineTx.transform.position = result.translation;
            cubeOutlineTx.transform.rotation = result.rotation;

            float OutlineThickness = OutlineWidth + BaseThickness;
            float OutlineLength = OutlineWidth * Mathf.Min(1.0f, 1.0f / result.scale) + result.scale;
            cubeOutlineTx.transform.localScale = new Vector3(OutlineThickness, OutlineThickness, OutlineLength);
            break;
          default:
            break;
        }
      }

    }

    private List<GuideCube> _guideCubes;
    private void InitGuideCubes() {
      if (_guideCubes != null)
        return;

      _guideCubes = new List<GuideCube>();

      for (int i = 0; i < m_GuideCubes.Count; i++) {
        _guideCubes.Add(new GuideCube(m_GuideCubes[i], m_GuideCubeOutlines[i]));
      }
    }

    private void GuideCubesBegin() {
      for (int i = 0; i < _guideCubes.Count; i++) {
        _guideCubes[i].enabled = true;
      }
    }

    private void GuideCubesEnd() {
      for (int i = 0; i < _guideCubes.Count; i++) {
        _guideCubes[i].enabled = false;
      }
    }

    private float _guideCubeOrbitalRadius;
    private float GuideCubeOrbitalRadius {
      get {
        return _guideCubeOrbitalRadius;
      }
      set {
        if (_guideCubeOrbitalRadius != value) {
          _guideCubeOrbitalRadius = value;
          for (int i = 0; i < _guideCubes.Count; i++) {
            _guideCubes[i].radius = _guideCubeOrbitalRadius;
          }
        }

      }
    }

    private TrTransform _guideCubeTransform;
    private TrTransform GuideCubeTransform {
      get {
        return _guideCubeTransform;
      }
      set {
        if (_guideCubeTransform != value) {
          _guideCubeTransform = value;
          for (int i = 0; i < _guideCubes.Count; i++) {
            _guideCubes[i].transform = _guideCubeTransform;
          }
        }

      }
    }

    private Quaternion _guideCubeTilt;
    private Quaternion GuideCubeTilt {
      get {
        return _guideCubeTilt;
      }
      set {
        if (_guideCubeTilt != value) {
          _guideCubeTilt = value;
          for (int i = 0; i < _guideCubes.Count; i++) {
            _guideCubes[i].tilt = _guideCubeTilt;
          }
        }

      }
    }

    private float _guideCubeLerpT;
    private float GuideCubeLerpT {
      get {
        return _guideCubeLerpT;
      }
      set {
        if (_guideCubeLerpT != value) {
          _guideCubeLerpT = value;

          float incr = 1f / _guideCubes.Count;

          for (int i = 0; i < _guideCubes.Count; i++) {
            _guideCubes[i].lerpT = (i * incr + _guideCubeLerpT) % 1f;
          }
        }

      }
    }

  }
}
#endif