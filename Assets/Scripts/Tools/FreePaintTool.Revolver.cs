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

      if (m_brushTrigger) {
        m_RevolverVelocity = Mathf.Clamp(m_RevolverVelocity + -InputManager.m_Instance.GetBrushScrollAmount() * Time.deltaTime * 120, -360, 360); // 10 revolutions/sec
        m_RevolverAngle = m_RevolverAngle + m_RevolverVelocity * Time.deltaTime / Mathf.Max(0.25f, Mathf.Abs(m_RevolverRadius));
      }
      else {
        Transform brushAttachTransform = InputManager.m_Instance.GetBrushControllerAttachPoint();

        Quaternion RevolverBrushRotation = brushAttachTransform.rotation * sm_OrientationAdjust;

        m_RevolverBrushRotationOffset = radialLookRot.TrueInverse() * RevolverBrushRotation;

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
