#if (UNITY_EDITOR || EXPERIMENTAL_ENABLED)
using UnityEngine;
using System.Collections.Generic;

namespace TiltBrush {
  public partial class FreePaintTool {
    [SerializeField] private List<Transform> m_GuideMarkers;
    // [SerializeField] private List<Transform> m_GuideCubeOutlines;

    public class GuideMarker {
      public Transform markerTx;
      public Renderer markerRenderer;

      public GuideMarker(Transform transform) {
        markerTx = transform;
        markerRenderer = transform.GetComponent<Renderer>();
      }

      private bool _enabled;
      public bool enabled {
        get {
          return _enabled;
        }
        set {
          _enabled = value;
          markerRenderer.enabled = _enabled;
        }
      }

      public enum PathModeID {
        Orbit,
        Trail
      }

      public PathModeID pathMode;
      public TrTransform transformTr;
      public TrTransform goalTr;
      public Quaternion tilt;

      private TrTransform Orbit(float radius) {

        TrTransform result = new TrTransform();

        Vector3 spindleAxis = transformTr.rotation * Vector3.up;

        Quaternion spindleRotation = Quaternion.AngleAxis(lerpT * -360, spindleAxis);
        Quaternion radialLookRot = transformTr.rotation;

        Vector3 radialOffset = spindleRotation * radialLookRot * Vector3.forward;
        result.translation = transformTr.translation + radialOffset * radius;

        Quaternion pointerRotation = spindleRotation * radialLookRot * tilt;        

        result.rotation = pointerRotation;

        result.scale = transformTr.scale;

        return result;
      }

      private TrTransform Trail() {

        return LazyLerp(transformTr, goalTr, lerpT);
      }

      private float _lerpT;
      public float lerpT {
        get {
          return _lerpT;
        }
        set {
          if (value == 1)
            _lerpT = value;
          else
          {
            _lerpT = value % 1;
            if (_lerpT < 0)
              _lerpT += 1;
          }
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

            result.ToTransform(markerTx);

            break;
          case PathModeID.Trail:
            result = Trail();

            result.ToTransform(markerTx);

            break;
          default:
            break;
        }
      }
    }

    private List<GuideMarker> _guideMarkers;
    private void InitGuideMarkers() {
      if (_guideMarkers != null)
        return;

      _guideMarkers = new List<GuideMarker>();

      for (int i = 0; i < m_GuideMarkers.Count; i++) {
        _guideMarkers.Add(new GuideMarker(m_GuideMarkers[i]));
      }
    }

    private void BeginGuideMarkers(GuideMarker.PathModeID pathMode) {
      for (int i = 0; i < _guideMarkers.Count; i++) {
        _guideMarkers[i].enabled = true;
        _guideMarkers[i].pathMode = pathMode;
      }
    }

    private void EndGuideMarkers() {
      for (int i = 0; i < _guideMarkers.Count; i++) {
        _guideMarkers[i].enabled = false;
      }
    }

    private float _guideMarkerOrbitalRadius;
    private float GuideMarkerOrbitalRadius {
      get {
        return _guideMarkerOrbitalRadius;
      }
      set {
        if (_guideMarkerOrbitalRadius != value) {
          _guideMarkerOrbitalRadius = value;
          for (int i = 0; i < _guideMarkers.Count; i++) {
            _guideMarkers[i].radius = _guideMarkerOrbitalRadius;
          }
        }

      }
    }

    private TrTransform _guideMarkerTransform;
    private TrTransform GuideMarkerTransform {
      get {
        return _guideMarkerTransform;
      }
      set {
        if (_guideMarkerTransform != value) {
          _guideMarkerTransform = value;
          for (int i = 0; i < _guideMarkers.Count; i++) {
            _guideMarkers[i].transformTr = _guideMarkerTransform;
          }
        }

      }
    }

    private TrTransform _guideMarkerGoal;
    private TrTransform GuideMarkerGoal {
      get {
        return _guideMarkerGoal;
      }
      set {
        if (_guideMarkerGoal != value) {
          _guideMarkerGoal = value;
          for (int i = 0; i < _guideMarkers.Count; i++) {
            _guideMarkers[i].goalTr = _guideMarkerGoal;
          }
        }

      }
    }

    private Quaternion _guideMarkerTilt;
    private Quaternion GuideMarkerTilt {
      get {
        return _guideMarkerTilt;
      }
      set {
        if (_guideMarkerTilt != value) {
          _guideMarkerTilt = value;
          for (int i = 0; i < _guideMarkers.Count; i++) {
            _guideMarkers[i].tilt = _guideMarkerTilt;
          }
        }

      }
    }

    private float _guideMarkerLerpT;
    private float GuideMarkerLerpT {
      get {
        return _guideMarkerLerpT;
      }
      set {
        if (_guideMarkerLerpT != value) {
          _guideMarkerLerpT = value;

          switch (_guideMarkers[0].pathMode) {
            case GuideMarker.PathModeID.Orbit: {
                float incr = 1f / _guideMarkers.Count;

                for (int i = 0; i < _guideMarkers.Count; i++) {
                  _guideMarkers[i].lerpT = i * incr + _guideMarkerLerpT;
                }

              }
              break;
            case GuideMarker.PathModeID.Trail: {
                // reserve last marker for 1.0 to show final position
                float incr = 1f / (_guideMarkers.Count - 1);

                for (int i = 0; i < _guideMarkers.Count - 1; i++) {
                  _guideMarkers[i].lerpT = i * incr + _guideMarkerLerpT;
                }
                _guideMarkers[_guideMarkers.Count - 1].lerpT = 1;
              }

              break;
            default:
              break;
          }

        }

      }
    }

  }
}
#endif