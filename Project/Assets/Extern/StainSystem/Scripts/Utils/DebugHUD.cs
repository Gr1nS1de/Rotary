using UnityEngine;
using UnityEngine.UI;

namespace SplatterSystem {
    
    [RequireComponent(typeof(Text))]
    public class DebugHUD : MonoBehaviour {
        const float fpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        const string display1 = "{0} FPS";
        const string display2 = "{0} FPS\n{1} particles";
        private Text m_Text;

        private SplatterParticleProvider m_Particles;
        private bool m_ShowNumParticles;

        private void Start() {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
            m_Text = GetComponent<Text>();
            m_Particles = GameObject.FindObjectOfType<SplatterParticleProvider>();
            m_ShowNumParticles = m_Particles != null;
        }

        private void Update() {
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod) {
                m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;

                if (m_ShowNumParticles) {
                    m_Text.text = string.Format(display2, m_CurrentFps, m_Particles.GetNumParticlesActive());
                } else {
                    m_Text.text = string.Format(display1, m_CurrentFps);
                }
            }
        }
    }

}