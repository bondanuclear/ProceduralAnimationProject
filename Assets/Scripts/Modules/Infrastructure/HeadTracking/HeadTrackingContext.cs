using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Modules.Infrastructure.HeadTracking
{
    public class HeadTrackingContext
    {
        private MultiAimConstraint _headConstraint;
        private Transform _targetTransform;
        private Transform _headTransform;
        private Quaternion _originalHeadRotation;
        private float _maxTrackingAngle;
        private float _trackingSpeed;
        private float _returnSpeed;
        private float _detectionRadius;
        private string _trackableLayerName;
        private int _trackableLayerMask;
        private GameObject _currentTrackingTarget;
        private float _trackingWeight;

        public HeadTrackingContext(MultiAimConstraint headConstraint, Transform headTransform, 
                                  float maxTrackingAngle, float trackingSpeed, float returnSpeed,
                                  float detectionRadius, string trackableLayerName)
        {
            _headConstraint = headConstraint;
            _headTransform = headTransform;
            _originalHeadRotation = headTransform.localRotation;
            _maxTrackingAngle = maxTrackingAngle;
            _trackingSpeed = trackingSpeed;
            _returnSpeed = returnSpeed;
            _detectionRadius = detectionRadius;
            _trackableLayerName = trackableLayerName;
            _trackableLayerMask = LayerMask.GetMask(trackableLayerName);
            _trackingWeight = 0f;

            // Create a target transform for the head to look at
            GameObject targetObj = new GameObject("HeadTrackingTarget");
            _targetTransform = targetObj.transform;
            _targetTransform.SetParent(_headTransform.parent);
            _targetTransform.localPosition = Vector3.forward;
            
            // Set the constraint source
            WeightedTransformArray newSources = new WeightedTransformArray { new WeightedTransform(_targetTransform, 1f) };
            var constraintData = _headConstraint.data;
            constraintData.sourceObjects = newSources;
            _headConstraint.data = constraintData;
            
            // Start with weight at zero (no influence)
            _headConstraint.weight = 0;
        }

        public MultiAimConstraint HeadConstraint { get => _headConstraint; }
        public Transform TargetTransform { get => _targetTransform; }
        public Transform HeadTransform { get => _headTransform; }
        public Quaternion OriginalHeadRotation { get => _originalHeadRotation; }
        public float MaxTrackingAngle { get => _maxTrackingAngle; }
        public float TrackingSpeed { get => _trackingSpeed; }
        public float ReturnSpeed { get => _returnSpeed; }
        public float DetectionRadius { get => _detectionRadius; }
        public int TrackableLayerMask { get => _trackableLayerMask; }
        public GameObject CurrentTrackingTarget { get => _currentTrackingTarget; set => _currentTrackingTarget = value; }
        public float TrackingWeight { get => _trackingWeight; set => _trackingWeight = value; }
    }
} 