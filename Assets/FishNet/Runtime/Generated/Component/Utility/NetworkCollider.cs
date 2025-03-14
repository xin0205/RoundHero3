using FishNet.Managing;
using FishNet.Object;
using GameKit.Dependencies.Utilities;
using GameKit.Dependencies.Utilities.Types;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TimeManagerCls = FishNet.Managing.Timing.TimeManager;

public abstract class NetworkCollider : NetworkBehaviour
{
#if PREDICTION_V2
    #region Types.
    private struct ColliderData : IResettable
    {
        /// <summary>
        /// Tick which the collisions happened.
        /// </summary>
        public uint Tick;
        /// <summary>
        /// Hits for Tick.
        /// </summary>
        public HashSet<Collider> Hits;

        public ColliderData(uint tick, HashSet<Collider> hits)
        {
            Tick = tick;
            Hits = hits;
        }

        public void InitializeState() { }
        public void ResetState()
        {
            Tick = TimeManagerCls.UNSET_TICK;
            CollectionCaches<Collider>.StoreAndDefault(ref Hits);
        }
    }
    #endregion

    /// <summary>
    /// Called when another collider enters this collider.
    /// </summary>
    public event Action<Collider> OnEnter;
    /// <summary>
    /// Called when another collider stays in this collider.
    /// </summary>
    public event Action<Collider> OnStay;
    /// <summary>
    /// Called when another collider exits this collider.
    /// </summary>
    public event Action<Collider> OnExit;
    /// <summary>
    /// True to run collisions for colliders which are triggers, false to run collisions for colliders which are not triggers.
    /// </summary>
    [HideInInspector]
    protected bool IsTrigger;
    /// <summary>
    /// The maximum number of simultaneous hits to check for.
    /// </summary>
    [SerializeField]
    private ushort _maximumSimultaneousHits = 16;

    /// <summary>
    /// The duration of the history.
    /// </summary>
    [SerializeField]
    private float _historyDuration = 0.5f;

    /// <summary>
    /// The colliders on this object.
    /// </summary>
    private Collider[] _colliders;
    /// <summary>
    /// The hits from the last check.
    /// </summary>
    private Collider[] _hits;
    /// <summary>
    /// The history of collider data.
    /// </summary>
    private ResettableRingBuffer<ColliderData> _colliderDataHistory;
    /// <summary>
    /// True if colliders have been searched for at least once.
    /// We cannot check the null state on _colliders because Unity has a habit of initializing collections on it's own.
    /// </summary>
    private bool _collidersFound;
    /// <summary>
    /// True to cache collision histories for comparing start and exits.
    /// </summary>
    private bool _useCache => (OnEnter != null || OnExit != null);

    protected virtual void Awake()
    {
        _colliderDataHistory = ResettableCollectionCaches<ColliderData>.RetrieveRingBuffer();
        _hits = CollectionCaches<Collider>.RetrieveArray();
        if (_hits.Length < _maximumSimultaneousHits)
            _hits = new Collider[_maximumSimultaneousHits];
    }

    private void OnDestroy()
    {
        ResettableCollectionCaches<ColliderData>.StoreAndDefault(ref _colliderDataHistory);
        CollectionCaches<Collider>.StoreAndDefault(ref _hits, -_hits.Length);
    }

    public override void OnStartNetwork()
    {
        FindColliders();

        //Initialize the ringbuffer. Server only needs 1 tick worth of history.
        uint historyTicks = (base.IsServerStarted) ? 1 : TimeManager.TimeToTicks(_historyDuration);
        _colliderDataHistory.Initialize((int)historyTicks);

        //Events needed by server and client.
        TimeManager.OnPostPhysicsSimulation += TimeManager_OnPostPhysicsSimulation;
    }

    public override void OnStartClient()
    {
        //Events only needed by the client.
        PredictionManager.OnPostPhysicsTransformSync += PredictionManager_OnPostPhysicsTransformSync;
        PredictionManager.OnPostReplicateReplay += PredictionManager_OnPostReplicateReplay;
    }

    public override void OnStopClient()
    {
        //Events only needed by the client.
        PredictionManager.OnPostPhysicsTransformSync -= PredictionManager_OnPostPhysicsTransformSync;
        PredictionManager.OnPostReplicateReplay -= PredictionManager_OnPostReplicateReplay;

    }

    public override void OnStopNetwork()
    {
        TimeManager.OnPostPhysicsSimulation -= TimeManager_OnPostPhysicsSimulation;
    }

    /// <summary>
    /// Called after Physics SyncTransforms are run after a reconcile.
    /// This will only invoke if physics are set to TimeManager, within the TimeManager inspector.
    /// </summary>
    private void PredictionManager_OnPostPhysicsTransformSync(uint clientTick, uint serverTick)
    {
        /* This callback will only occur when client only.
         * SInce this is the case remove histories prior
         * to clientTick. */
        if (clientTick > 0)
            CleanHistory(clientTick - 1);
        CheckColliders(clientTick, true);
    }

    /// <summary>
    /// When using TimeManager for physics timing, this is called immediately after the physics simulation has occured for the tick.
    /// While using Unity for physics timing, this is called during Update, only if a physics frame.
    /// This may be useful if you wish to run physics differently for stacked scenes.
    private void TimeManager_OnPostPhysicsSimulation(float delta)
    {
        CheckColliders(TimeManager.LocalTick, false);
    }

    /// <summary>
    /// Called after physics is simulated when replaying a replicate method.
    /// </summary>
    private void PredictionManager_OnPostReplicateReplay(uint clientTick, uint serverTick)
    {
        CheckColliders(clientTick, true);
    }

    /// <summary>
    /// Cleans history up to, while excluding tick.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CleanHistory(uint tick)
    {
        if (_useCache)
        {
            int removeCount = 0;
            int historyCount = _colliderDataHistory.Count;
            for (int i = 0; i < historyCount; i++)
            {
                if (_colliderDataHistory[i].Tick >= tick)
                    break;
                removeCount++;
            }

            for (int i = 0; i < removeCount; i++)
                _colliderDataHistory[i].ResetState();
            _colliderDataHistory.RemoveRange(true, removeCount);
        }
        //Cache is not used.
        else
        {
            ClearColliderDataHistory();
        }
    }

    /// <summary>
    /// Checks for any trigger changes;
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckColliders(uint tick, bool replay)
    {
        //Should not be possible as tick always starts on 1.
        if (tick == TimeManagerCls.UNSET_TICK)
            return;

        const int INVALID_HISTORY_VALUE = -1;

        HashSet<Collider> current = CollectionCaches<Collider>.RetrieveHashSet();
        HashSet<Collider> previous = null;

        int previousHitsIndex = INVALID_HISTORY_VALUE;
        /* Server only keeps 1 history so
         * if server is started then
         * simply clean one. When the server is
         * started replay will never be true, so this
         * will only call once per tick. */
        if (base.IsServerStarted && tick > 0)
            CleanHistory(tick - 1);

        if (_useCache)
        {
            if (replay)
            {
                previousHitsIndex = GetHistoryIndex(tick - 1, false);
                if (previousHitsIndex != -1)
                    previous = _colliderDataHistory[previousHitsIndex].Hits;
            }
            //Not replaying.
            else
            {
                if (_colliderDataHistory.Count > 0)
                {
                    ColliderData cd = _colliderDataHistory[_colliderDataHistory.Count - 1];
                    /* If the hit tick one before current then it can be used, otherwise
                    * use a new collection for previous. */
                    if (cd.Tick == (tick - 1))
                        previous = cd.Hits;
                }
            }
        }
        //Not using history, clear it all.
        else
        {
            ClearColliderDataHistory();
        }

        /* Previous may not be set here if there were
         * no collisions during the previous tick. */

        // The rotation of the object for box colliders.
        Quaternion rotation = transform.rotation;
        // The layer mask for this object.
        int layerMask = 1 << gameObject.layer;

        // Check each collider for triggers.
        foreach (Collider col in _colliders)
        {
            if (!col.enabled)
                continue;
            if (IsTrigger != col.isTrigger)
                continue;

            //Number of hits from the checks.
            int hits;
            if (col is SphereCollider sphereCollider)
                hits = GetSphereColliderHits(sphereCollider, layerMask);
            else if (col is CapsuleCollider capsuleCollider)
                hits = GetCapsuleColliderHits(capsuleCollider, layerMask);
            else if (col is BoxCollider boxCollider)
                hits = GetBoxColliderHits(boxCollider, rotation, layerMask);
            else
                hits = 0;

            // Check the hits for triggers.
            for (int i = 0; i < hits; i++)
            {
                Collider hit = _hits[i];
                if (hit == null || hit == col)
                    continue;

                /* If not in previous then add and
                 * invoke enter. */
                if (previous == null || !previous.Contains(hit))
                    OnEnter?.Invoke(hit);

                //Also add to current hits.
                current.Add(hit);
                OnStay?.Invoke(hit);
            }
        }

        if (previous != null)
        {
            //Check for stays and exits.
            foreach (Collider col in previous)
            {
                //If it was in previous but not current, it has exited.
                if (!current.Contains(col))
                    OnExit?.Invoke(col);
            }
        }

        //If not using the cache then clean up collections.
        if (_useCache)
        {
            //If not replaying add onto the end. */
            if (!replay)
            {
                AddToEnd();
            }
            /* If a replay then set current colliders
             * to one entry past historyIndex. If the next entry
             * beyond historyIndex is for the right tick it can be
             * updated, otherwise a result has to be inserted. */
            else
            {
                /* Previous hits was not found in history so we
                 * cannot assume current results go right after the previousIndex.
                 * Find whichever index is the closest to tick and return it. 
                 * 
                 * If an exact match is not found for tick then the entry just after
                 * tick will be returned. This will let us insert current hits right
                 * before that entry. */
                if (previousHitsIndex == -1)
                {
                    int currentIndex = GetHistoryIndex(tick, true);
                    AddDataToIndex(currentIndex);
                }
                //If previous hits are known then the index to update is right after previous index.
                else
                {
                    int insertIndex = (previousHitsIndex + 1);
                    /* InsertIndex is out of bounds which means
                     * to add onto the end. */
                    if (insertIndex >= _colliderDataHistory.Count)
                        AddToEnd();
                    //Not the last entry to insert in the middle.
                    else
                        AddDataToIndex(insertIndex);
                }

                /* Adds data to an index. If the tick
                 * matches on index with the current tick then
                 * replace the entry. Otherwise insert to the
                 * correct location. */
                void AddDataToIndex(int index)
                {
                    ColliderData colliderData = new ColliderData(tick, current);
                    /* If insertIndex is the same tick then replace, otherwise
                     * put in front of. */
                    //Replace.
                    if (_colliderDataHistory[index].Tick == tick)
                    {
                        _colliderDataHistory[index].ResetState();
                        _colliderDataHistory[index] = colliderData;
                    }
                    //Insert before.
                    else
                    {
                        _colliderDataHistory.Insert(index, colliderData);
                    }
                }
            }

            void AddToEnd()
            {
                ColliderData colliderData = new ColliderData(tick, current);
                _colliderDataHistory.Add(colliderData);
            }

        }
        /* If not using caching then store results from this run. */
        else
        {
            CollectionCaches<Collider>.Store(current);
        }

        //Returns history index for a tick.
        /* GetClosest will return the closest match which is
         * past lTick if lTick could not be found. */
        int GetHistoryIndex(uint lTick, bool getClosest)
        {
            for (int i = 0; i < _colliderDataHistory.Count; i++)
            {
                uint localTick = _colliderDataHistory[i].Tick;
                if (localTick == lTick)
                    return i;
                /* Tick is too high, any further results
                 * will also be too high. */
                if (localTick > tick)
                {
                    if (getClosest)
                        return i;
                    else
                        return INVALID_HISTORY_VALUE;
                }
            }

            //Fall through.
            return INVALID_HISTORY_VALUE;
        }
    }

    /// <summary>
    /// Checks for Sphere collisions.
    /// </summary>
    /// <returns>Number of colliders hit.</returns>
    private int GetSphereColliderHits(SphereCollider sphereCollider, int layerMask)
    {
        float scaledRadius = (sphereCollider.radius * transform.lossyScale.magnitude);
        return Physics.OverlapSphereNonAlloc(sphereCollider.bounds.center, scaledRadius, _hits, layerMask);
    }

    /// <summary>
    /// Checks for Capsule collisions.
    /// </summary>
    /// <returns>Number of colliders hit.</returns>
    private int GetCapsuleColliderHits(CapsuleCollider capsuleCollider, int layerMask)
    {
        Vector3 center = capsuleCollider.bounds.center;
        float halfHeight = (capsuleCollider.height * 0.5f);
        Vector3 offset;

        int direction = capsuleCollider.direction;
        if (direction == 0)
        {
            offset = new Vector3(halfHeight, 0.0f, 0.0f);
        }
        else if (direction == 1)
        {
            offset = new Vector3(0.0f, halfHeight, 0.0f);
        }
        else if (direction == 2)
        {
            offset = new Vector3(0.0f, 0.0f, halfHeight);
        }
        else
        {
            base.NetworkManager.LogError("Invalid capsule collider direction.");
            return 0;
        }

        Vector3 start = (center + offset);
        Vector3 end = (center - offset);
        float scaledRadius = (capsuleCollider.radius * transform.lossyScale.magnitude);
        return Physics.OverlapCapsuleNonAlloc(start, end, scaledRadius, _hits, layerMask);
    }

    /// <summary>
    /// Checks for Box collisions.
    /// </summary>
    /// <returns>Number of colliders hit.</returns>
    private int GetBoxColliderHits(BoxCollider boxCollider, Quaternion rotation, int layerMask)
    {
        Bounds bounds = boxCollider.bounds;
        return Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, _hits, rotation, layerMask);
    }

    /// <summary>
    /// Finds colliders to use.
    /// <paramref name="rebuild"/>True to rebuild the colliders even if they are already populated.
    /// </summary>
    public void FindColliders(bool rebuild = false)
    {
        if (_collidersFound && !rebuild)
            return;
        _collidersFound = true;

        _colliders = GetComponents<Collider>();
    }

    /// <summary>
    /// Resets this NetworkBehaviour so that it may be added to an object pool.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void ResetState()
    {
        base.ResetState();
        ClearColliderDataHistory();
    }

    /// <summary>
    /// Resets datas in collider data history and clears collection.
    /// </summary>
    private void ClearColliderDataHistory()
    {
        foreach (ColliderData cd in _colliderDataHistory)
            cd.ResetState();
        _colliderDataHistory.Clear();
    }
#endif
}
