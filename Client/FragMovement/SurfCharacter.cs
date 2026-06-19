using UnityEngine;
using System.Linq;
using System.Collections.Generic;
//using Fragsurf.Actors;

namespace Fragsurf.Movement
{
    [AddComponentMenu("Fragsurf/Surf Character")]
    public class SurfCharacter : MonoBehaviour, ISurfControllable
    {
        [Header("Physics Settings")]
        public int TickRate = 100;
        public bool FixedTickRate;
        public Vector3 ColliderSize = new Vector3(1, 1.83f, 1);

        [Header("View Settings")]
        public Camera Camera;
        public Vector3 ViewOffset = new Vector3(0, 1.64f, 0);
        public Vector3 DuckedViewOffset = new Vector3(0, 1.21f, 0);
        public int FieldOfView = 75;

        [Header("Input Settings")]
        public float XSens = 25;
        public float YSens = 25;
        public KeyCode JumpButton = KeyCode.Space;
        public KeyCode DuckButton = KeyCode.LeftControl;
        public KeyCode MoveLeft = KeyCode.A;
        public KeyCode MoveRight = KeyCode.D;
        public KeyCode MoveForward = KeyCode.W;
        public KeyCode MoveBack = KeyCode.S;
        public KeyCode Noclip = KeyCode.N;
        public KeyCode Restart = KeyCode.R;
        public KeyCode YawLeft = KeyCode.Mouse4;
        public KeyCode YawRight = KeyCode.Mouse3;
        public int YawSpeed = 260;

        [Header("Movement Config")]
        [SerializeField]
        private MovementConfig _moveConfig = new MovementConfig();
        private Vector3 _startPosition;
        public SurfController _controller = new SurfController();
        public MoveType MoveType { get; set; } = MoveType.Walk;

        public MovementConfig MoveConfig => _moveConfig;
        public MoveData MoveData { get; set; } = new MoveData();
        public BoxCollider Collider { get; private set; }
        public GameObject GroundObject { get; set; }
        public Vector3 BaseVelocity { get; }
        public Quaternion Orientation => Quaternion.identity;
        public Vector3 Forward => transform.forward;
        public Vector3 Right => transform.right;
        public Vector3 Up => transform.up;
        public Vector3 StandingExtents => ColliderSize * 0.5f;
        private float _alpha;
        private float _accumulator;
        private float _elapsedTime;
        private bool _hasCursor = true;

        [HideInInspector] public bool active = true;
        [HideInInspector] public bool fixedBodyRotation = false;

        [Header("Camera Config")]
        public float camZRotation = 0f;

        [Header("Other Vars")]
        public Vector4 directionalKeysMultipliers = Vector4.one;
        public Vector2 turnSpeed;
        public static bool resetOriginBindActive;
        public static bool noclipToggleBindActive;

        /*
        private void OnDestroy()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        */

        private void Start()
        {
            if (Application.isEditor)
            {
                noclipToggleBindActive = true;
            }

            //Cursor.lockState = CursorLockMode.Locked;
            CursorManager.RefreshCursorState();

            if (!FixedTickRate) { Time.fixedDeltaTime = 1f / TickRate; }
            var mr = GetComponent<Renderer>();
            if (mr)
            {
                mr.enabled = false;
            }

            if (Camera == null)
            {
                Camera = Camera.main;
            }

            Camera.fieldOfView = FieldOfView;
            Camera.transform.parent.SetParent(null);

            ///Why ?
            /*
            foreach (var collider in gameObject.GetComponentsInChildren<Collider>())
            {
                GameObject.Destroy(collider);
            }
            */

            Collider = gameObject.AddComponent<BoxCollider>();
            Collider.size = ColliderSize;
            Collider.center = new Vector3(0, ColliderSize.y * 0.5f, 0);
            Collider.isTrigger = true;

            var rbody = gameObject.GetComponent<Rigidbody>();
            if (!rbody) rbody = gameObject.AddComponent<Rigidbody>();
            rbody.isKinematic = true;
            rbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            MoveData.Origin = transform.position;
            MoveData.ViewAngles = transform.rotation.eulerAngles;
            _moveConfig.NoclipCollide = false;
            _startPosition = transform.position;

            Physics.autoSimulation = true;
            Physics.autoSyncTransforms = true;

            _controller.SpawnDummyGroundObject();
        }

        private void Update()
        {
            MoveData.PreJumpGroundNormal = MoveData.GroundNormal;
            /*
            if(_hasCursor&&!Terminal.terminalActive)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if(Input.GetKey(KeyCode.Escape))
            {
                _hasCursor = !_hasCursor;
            }
            */

            UpdateTestBinds();
            UpdateRotation();
            UpdateMoveData();

            var dt = Time.fixedDeltaTime * (1f / Time.timeScale);
            if (!FixedTickRate)
            {
                Time.fixedDeltaTime = 1f / TickRate;
                dt = Time.fixedDeltaTime * (1f / Time.timeScale);
                var newTime = Time.realtimeSinceStartup;
                var frameTime = newTime - _elapsedTime;
                if (frameTime > Time.fixedDeltaTime)
                    frameTime = Time.fixedDeltaTime;
                Time.maximumDeltaTime = Time.fixedDeltaTime * (1f / Time.timeScale);
                _elapsedTime = newTime;
                _accumulator += frameTime;

                while (_accumulator >= dt)
                {
                    _accumulator -= dt;
                    Tick();
                }
            }
            _alpha = _accumulator / dt;

            if (Camera)
            {
                if (active) { Camera.transform.position = Vector3.Lerp(MoveData.PreviousOrigin, MoveData.Origin, _alpha) + (MoveData.Ducked ? DuckedViewOffset : ViewOffset); }
                else { Camera.transform.position = transform.position + (MoveData.Ducked ? DuckedViewOffset : ViewOffset); }
            }
        }
        public static int TICK_ID;
        void FixedUpdate()
        {
            Tick();
        }

        private void UpdateTestBinds()
        {
            if (InputManager.GetRestartKeyDown() && resetOriginBindActive)
            {
                MoveData.Velocity = Vector3.zero;
                MoveData.Origin = _startPosition;
            }
            if (InputManager.GetNoClipKeyDown() && noclipToggleBindActive)
            {
                MoveType = MoveType == MoveType.Noclip ? MoveType.Walk : MoveType.Noclip;
            }
        }

        private List<GameObject> _touchingLastFrame = new List<GameObject>();
        private void Tick()
        {
            if (active)
            {
                _controller.CalculateMovement(this, _moveConfig, Time.fixedDeltaTime);
                //TICK_ID++;
                transform.localPosition = MoveData.Origin;
            }

            var prevOrigin = MoveData.PreviousOrigin;
            var newOrigin = MoveData.Origin;
            var center = prevOrigin;
            center.y += Collider.bounds.extents.y;
            var dir = (newOrigin - prevOrigin).normalized;
            var currentDistance = Vector3.Distance(prevOrigin, newOrigin);

            var touchedThisFrame = Physics.BoxCastAll(center: center,
                halfExtents: Collider.bounds.extents,
                direction: dir,
                orientation: Quaternion.identity,
                maxDistance: currentDistance,
                layerMask: SurfPhysics.GroundLayerMask,
                queryTriggerInteraction: QueryTriggerInteraction.Collide).ToList();

            for (int i = _touchingLastFrame.Count - 1; i >= 0; i--)
            {
                foreach (var hit in touchedThisFrame)
                {
                    if (hit.transform.gameObject == _touchingLastFrame[i].gameObject)
                    {
                        //_touchingLastFrame[i].GetComponent<FSMTrigger>().OnEndTouch(0, true);
                        _touchingLastFrame.RemoveAt(i);
                        break;
                    }
                }
            }

            foreach (var raycastHit in touchedThisFrame)
            {
                if (!raycastHit.collider.isTrigger)
                {
                    continue;
                }
                /*
                var fsmTrigger = raycastHit.transform.GetComponent<FSMTrigger>();
                if (fsmTrigger != null)
                {
                    if (!_touchingLastFrame.Contains(raycastHit.transform.gameObject))
                    {
                        fsmTrigger.OnStartTouch(0, true);
                        _touchingLastFrame.Add(raycastHit.transform.gameObject);
                    }
                    else
                    {
                        fsmTrigger.OnTouch(0, true);
                    }
                }
                */
            }
        }

        private void UpdateMoveData()
        {
            if (active)
            {
                var moveLeft = InputManager.GetMoveLeftKey();
                var moveRight = InputManager.GetMoveRightKey();
                var moveFwd = InputManager.GetMoveForwardKey();
                var moveBack = InputManager.GetMoveBackKey();
                var jump = InputManager.GetJumpKey() && !Death.dead && MoveData.canJump; //Hard coded teehee
                var duck = Input.GetKey(DuckButton);

                if (!moveLeft && !moveRight)
                    MoveData.SideMove = 0;
                else if (moveLeft)
                    MoveData.SideMove = -MoveConfig.Accelerate * directionalKeysMultipliers.x;
                else if (moveRight)
                    MoveData.SideMove = MoveConfig.Accelerate * directionalKeysMultipliers.y;
                if (!moveFwd && !moveBack)
                    MoveData.ForwardMove = 0;
                else if (moveFwd)
                    MoveData.ForwardMove = MoveConfig.Accelerate * directionalKeysMultipliers.z;
                else if (moveBack)
                    MoveData.ForwardMove = -MoveConfig.Accelerate * directionalKeysMultipliers.w;

                if (jump)
                    MoveData.Buttons |= InputActions.Jump;
                else
                    MoveData.Buttons &= ~InputActions.Jump;

                if (duck)
                    MoveData.Buttons |= InputActions.Duck;
                else
                    MoveData.Buttons &= ~InputActions.Duck;

                if (moveLeft)
                    MoveData.Buttons |= InputActions.MoveLeft;
                else
                    MoveData.Buttons &= ~InputActions.MoveLeft;

                if (moveRight)
                    MoveData.Buttons |= InputActions.MoveRight;
                else
                    MoveData.Buttons &= ~InputActions.MoveRight;

                if (moveFwd)
                    MoveData.Buttons |= InputActions.MoveForward;
                else
                    MoveData.Buttons &= ~InputActions.MoveForward;

                if (moveBack)
                    MoveData.Buttons |= InputActions.MoveBack;
                else
                    MoveData.Buttons &= ~InputActions.MoveBack;

                MoveData.OldButtons = MoveData.Buttons;
            }
            MoveData.ViewAngles = Camera.transform.localRotation.eulerAngles;
            if (!fixedBodyRotation) { transform.rotation = Quaternion.Euler(0, MoveData.ViewAngles.y, MoveData.ViewAngles.z); }

            Collider.size = ColliderSize;
            Collider.center = new Vector3(0, ColliderSize.y * 0.5f, 0);
        }

        private void UpdateRotation()
        {
            var angles = MoveData.ViewAngles;
            float mx = (InputManager.GetMouseMoveX() * .02200f);
            float my = InputManager.GetMouseMoveY() * .02200f;
            turnSpeed = new Vector2(mx, my);
            var rot = angles + new Vector3(-my, mx, 0f);
            rot.x = SurfPhysics.ClampAngle(rot.x, -89f, 89f);

            var yaw = 0;
            if (InputManager.GetYawLeftKey())
            {
                yaw = -YawSpeed;
            }
            else if (InputManager.GetYawRightKey())
            {
                yaw = YawSpeed;
            }

            rot.y += yaw * Time.deltaTime;
            rot.z = camZRotation;
            Camera.transform.localRotation = Quaternion.Euler(rot);
            MoveData.RawLookDirection = rot;
        }

    }
}

