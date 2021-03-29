#if (UNITY_EDITOR || EXPERIMENTAL_ENABLED)
using UnityEngine;

namespace TiltBrush {
  public partial class FreePaintTool {
    private bool m_RevolverActive;
    private float m_RevolverRadius;
    private float m_RevolverAngle;
    private float m_RevolverVelocity;
    private Quaternion m_RevolverBrushRotationOffset;

    private void BeginRevolver() {
      if (m_RevolverActive)
        return;

      m_RevolverActive = true;
      m_RevolverAngle = 0;
      m_RevolverVelocity = 0;

      SetRevolverRadius(1);
    }

    private void SetRevolverRadius(float lerpRate) {
      Transform brushAttachTransform = InputManager.m_Instance.GetBrushControllerAttachPoint();
      Vector3 brushDelta = m_btIntersectGoal - brushAttachTransform.position;
      m_RevolverRadius = Mathf.Lerp(m_RevolverRadius, brushDelta.magnitude, lerpRate);
    }

    private void ApplyRevolver(ref Vector3 pos, ref Quaternion rot) {
      if (!m_RevolverActive)
        return;

      Transform lAttachPoint = InputManager.m_Instance.GetWandControllerAttachPoint();
      Vector3 lPos = lAttachPoint.position;

      Transform rAttachPoint = InputManager.m_Instance.GetBrushControllerAttachPoint();
      Vector3 rPos = rAttachPoint.position;

      Vector3 guideDelta = lPos - m_btCursorPos;
      Vector3 radialDelta = Vector3.ProjectOnPlane(rPos - m_btIntersectGoal, guideDelta.normalized);

      Quaternion radialLookRot = Quaternion.LookRotation(radialDelta.normalized, guideDelta);

      m_RevolverAngle = m_RevolverAngle + m_RevolverVelocity * 720 * Time.deltaTime;

      if (m_brushTrigger) {
				if (InputManager.m_Instance.IsBrushScrollActive()) {

          float turnRate = InputManager.m_Instance.GetBrushScrollAmount();
          // apply a cubic exponential speed curve to make joystick handling easier
          turnRate = Mathf.Sign(turnRate) * Mathf.Pow(Mathf.Abs(turnRate), 3);

          m_RevolverVelocity = Mathf.MoveTowards(m_RevolverVelocity, -turnRate, Time.deltaTime * (turnRate == 0 ? 0.15f : 0.333f));
        }
      }
      else {
        Transform brushAttachTransform = InputManager.m_Instance.GetBrushControllerAttachPoint();

        Quaternion RevolverBrushRotation = brushAttachTransform.rotation * sm_OrientationAdjust;

        m_RevolverBrushRotationOffset = radialLookRot.TrueInverse() * RevolverBrushRotation;

        if (m_RevolverVelocity == 0)
          m_RevolverAngle = 0;
      }



      Quaternion deltaRot = Quaternion.AngleAxis(m_RevolverAngle, guideDelta.normalized);

      if (m_brushUndoButton)
        SetRevolverRadius(m_brushTrigger ? m_lazyInputRate : 1);

      Vector3 revolverOffset = deltaRot * radialDelta.normalized * m_RevolverRadius;
      Quaternion btCursorRotGoal = deltaRot * radialLookRot * m_RevolverBrushRotationOffset;
      m_btCursorRot = btCursorRotGoal;

      pos = (m_brushTrigger && !m_lazyInput ? m_btCursorPos : m_btIntersectGoal) + revolverOffset;
      rot = m_btCursorRot;
    }
  }
}

#endif // #if (UNITY_EDITOR || EXPERIMENTAL_ENABLED)
