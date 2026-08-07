using Unity.Entities;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class HybridMarbleSyncSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // SystemAPI.Query fully supports managed components (classes) unlike Entities.ForEach
        foreach (var (stateRef, link) in
                 SystemAPI.Query<RefRW<MarbleStateData>, MarbleGameObjectLink>())
        {
            // If the GameObject link hasn't been assigned by MarbleLoader yet, skip
            if (link.View == null) continue;

            // Read-only access to our struct data
            var state = stateRef.ValueRO;

            // Push ECS state to the GameObject
            if (link.View.IsWishlisted != state.IsWishlisted)
            {
                link.View.IsWishlisted = state.IsWishlisted;
                // Update your UI visuals here if needed
            }

            // Example of pulling data from GameObject back to ECS:
            // stateRef.ValueRW.IsImageDownloaded = link.View.texture != null;
        }
    }
}