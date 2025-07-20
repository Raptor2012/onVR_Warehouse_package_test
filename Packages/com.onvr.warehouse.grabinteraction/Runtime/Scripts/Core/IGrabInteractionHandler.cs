namespace OnVR.Warehouse.GrabInteraction.Core
{
    /// <summary>
    /// Interface for platform-specific grab interaction handlers
    /// </summary>
    public interface IGrabInteractionHandler
    {
        /// <summary>
        /// Initialize the handler with the interaction manager
        /// </summary>
        void Initialize(InteractionManager manager);

        /// <summary>
        /// Enable the handler (called when platform becomes active)
        /// </summary>
        void Enable();

        /// <summary>
        /// Disable the handler (called when switching platforms)
        /// </summary>
        void Disable();

        /// <summary>
        /// Update method called every frame when handler is active
        /// </summary>
        void UpdateHandler();

        /// <summary>
        /// Called when a new grabbable object is registered
        /// </summary>
        void OnObjectRegistered(GrabbableObject grabbable);

        /// <summary>
        /// Called when a grabbable object is unregistered
        /// </summary>
        void OnObjectUnregistered(GrabbableObject grabbable);

        /// <summary>
        /// Check if this handler is currently enabled
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Get the platform this handler supports
        /// </summary>
        InteractionPlatform SupportedPlatform { get; }
    }
}
