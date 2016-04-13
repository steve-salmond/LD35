using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PhysicsQuality : MonoBehaviour
{

    // Properties
    // ----------------------------------------------------------------------------

    [System.Serializable]
    public struct Config
    {
        public QualityThreshold.Level Level;
        public float FixedDeltaTime;
        public float MaxDeltaTime;
        public int SolverIterationCount;
    }

    /** Configuration entries. */
    public List<Config> Configuration;


    // Unity Implementation
    // ----------------------------------------------------------------------------

    /** Initialize the component. */
    protected virtual void Awake()
    {
        var level = QualitySettings.GetQualityLevel();
        var config = Configuration.FirstOrDefault(c => (int) c.Level == level);

        Time.fixedDeltaTime = config.FixedDeltaTime;
        Time.maximumDeltaTime = config.MaxDeltaTime;
        Physics.solverIterationCount = config.SolverIterationCount;
    }

}
