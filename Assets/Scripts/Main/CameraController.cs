namespace SAE.RoguePG.Main
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    ///     Will make the Camera that this is attached to follow the referenced GameObject
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        /// <summary> Distance from the camera in which <seealso cref="LimitedRangeObjects"/> are active </summary>
        public const float ActiveRange = 40.0f;

        /// <summary> The delay in seconds between updates in <seealso cref="UpdateRangeActivity"/> </summary>
        private const float ActivityUpdateRate = 0.5f;

        /// <summary>
        ///     The <seealso cref="Transform"/> of the GameObject to follow
        /// </summary>
        [HideInInspector]
        public Transform following;

        /// <summary> How fast the camera approaches the target position. Lower values increase the speed. </summary>
        [Range(0.0f, 1.0f)]
        public float movementSpeedBase = 0.1f;

        /// <summary> How fast the camera approaches the target rotation. Lower values increase the speed. </summary>
        [Range(0.0f, 1.0f)]
        public float rotationSpeedBase = 0.01f;

        /// <summary> How fast is the manual camera control </summary>
        public float manualControlSpeed = 5.0f;

        /// <summary>
        ///     How far away should the camera be from <see cref="following"/> out of battle
        /// </summary>
        [SerializeField]
        private float preferredDistance = 4.0f;

        /// <summary>
        ///     How much higher should the camera be than the pivot of <see cref="following"/> out of battle
        /// </summary>
        [SerializeField]
        private float preferredHeight = 1.0f;

        /// <summary>
        ///     How far away should the camera be from <see cref="following"/> during a battle
        /// </summary>
        [SerializeField]
        private float preferredDistanceBattle = 4.0f;

        /// <summary>
        ///     How much higher should the camera be than the pivot of <see cref="following"/> during a battle
        /// </summary>
        [SerializeField]
        private float preferredHeightBattle = 1.0f;

        /// <summary> Layer used by entity collision </summary>
        private int opaqueLayerMask;

        /// <summary> GameObjects which are only enabled in a limited range </summary>
        public List<GameObject> LimitedRangeObjects { get; set; }

        /// <summary> Behaviours which are only enabled in a limited range </summary>
        public List<Behaviour> LimitedRangeBehaviours { get; set; }

        /// <summary>
        ///     How far away should the camera be from <see cref="following"/>
        /// </summary>
        public float PreferedDistance
        {
            get
            {
                return BattleManager.IsBattleActive ? this.preferredDistanceBattle : this.preferredDistance;
            }
        }

        /// <summary>
        ///     How much higher should the camera be than the pivot of <see cref="following"/>
        /// </summary>
        public float PreferedHeight
        {
            get
            {
                return BattleManager.IsBattleActive ? this.preferredHeightBattle : this.preferredHeight;
            }
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="CameraController"/> whether it is enabled or not.
        /// </summary>
        private void Awake()
        {
            this.LimitedRangeObjects = new List<GameObject>();
            this.LimitedRangeBehaviours = new List<Behaviour>();

            this.opaqueLayerMask = LayerMask.GetMask("Default");
        }

        /// <summary>
        ///     Called by Unity to initialize the <see cref="CameraController"/> when it is first enabled.
        /// </summary>
        private void Start()
        {
            this.StartCoroutine(this.UpdateRangeActivity());
        }

        /// <summary>
        ///     Updates which objects and behaviour in <seealso cref="LimitedRangeObjects"/> and
        ///     <seealso cref="LimitedRangeBehaviours"/> are active.
        /// </summary>
        /// <returns>An iterator</returns>
        private IEnumerator UpdateRangeActivity()
        {
            Func<bool> waitWhileBattleActive = delegate
            {
                return BattleManager.IsBattleActive;
            };

            while (true)
            {
                yield return new WaitForSecondsRealtime(CameraController.ActivityUpdateRate);
                yield return new WaitWhile(waitWhileBattleActive);

                foreach (GameObject gameObject in this.LimitedRangeObjects)
                {
                    if (gameObject == null)
                    {
                        continue;
                    }

                    bool shouldBeActive = this.IsInActiveRange(gameObject.transform);

                    if (gameObject.activeSelf != shouldBeActive)
                    {
                        gameObject.SetActive(shouldBeActive);
                    }
                }

                foreach (Behaviour behaviour in this.LimitedRangeBehaviours)
                {
                    if (behaviour == null)
                    {
                        continue;
                    }

                    bool shouldBeEnabled = this.IsInActiveRange(behaviour.transform);

                    if (behaviour.enabled != shouldBeEnabled)
                    {
                        behaviour.enabled = shouldBeEnabled;
                    }
                }
            }
        }

        /// <summary>
        ///     Returns whether the given transform is in the active range
        /// </summary>
        /// <param name="transform">The transform to check</param>
        /// <returns>Whether it is in the active range</returns>
        private bool IsInActiveRange(Transform transform)
        {
            return (this.transform.position - transform.position).sqrMagnitude < CameraController.ActiveRange * CameraController.ActiveRange;
        }

        /// <summary>
        ///     Called by Unity once every frame after all Updates and FixedUpdates have been executed.
        /// </summary>
        private void FixedUpdate()
        {
            if (this.following != null)
            {
                float distance = this.PreferedDistance;

                RaycastHit raycastHit;
                Vector3 rayCastDirection = this.transform.position - this.following.position;
                rayCastDirection.y = 0.0f;

                if (Physics.Raycast(
                    this.following.position,
                    rayCastDirection,
                    out raycastHit,
                    distance,
                    this.opaqueLayerMask))
                {
                    distance = (raycastHit.point - this.following.position).magnitude;
                }

                Vector3 currentRotation = VariousCommon.WrapDegrees(this.transform.eulerAngles);
                this.transform.LookAt(this.following);
                this.transform.position += this.transform.right * Input.GetAxis("CameraHorizontal") * Time.fixedDeltaTime * this.manualControlSpeed;
                Vector3 targetRotation = VariousCommon.WrapDegrees(this.transform.eulerAngles);

                Vector3 thisToFollowing = this.transform.forward;
                thisToFollowing.y = 0.0f;
                thisToFollowing.Normalize();

                Vector3 newPosition = VariousCommon.ExponentialLerp(
                    this.transform.position,
                    this.following.position - thisToFollowing * distance,
                    this.movementSpeedBase,
                    Time.fixedDeltaTime);

                newPosition.y = this.following.position.y + this.PreferedHeight;
                this.transform.position = newPosition;

                this.transform.eulerAngles = VariousCommon.ExponentialLerpRotation(
                    currentRotation,
                    targetRotation,
                    this.rotationSpeedBase,
                    Time.fixedDeltaTime);
            }
        }
    }
}