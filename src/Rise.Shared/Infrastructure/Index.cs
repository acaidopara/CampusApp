using Rise.Shared.Resto;

namespace Rise.Shared.Infrastructure;

/// <summary>
/// Static utility class containing response models for infrastructure operations.
/// These models define the structure of outgoing responses from service methods.
/// Optimized to include only actively used models (e.g., Index for paginated results).
/// </summary>
public static partial class CampusResponse
{
    /// <summary>
    /// Response model for paginated index of campuses.
    /// Includes the list of campuses and total count for pagination.
    /// </summary>
    public class Index
    {
        /// <summary>
        /// The collection of campus DTOs in the current page.
        /// </summary>
        public IEnumerable<CampusDto.Index> Campuses { get; set; } = [];
        
        /// <summary>
        /// The total number of campuses matching the query (for pagination).
        /// </summary>
        public int TotalCount { get; set; }
    }
    
    public class Detail
    {
        public required CampusDto.Detail Campus { get; set; }
    }
}

public static partial class BuildingResponse
{
    /// <summary>
    /// Response model for paginated index of buildings.
    /// Includes the list of buildings and total count for pagination.
    /// </summary>
    public class Index
    {
        /// <summary>
        /// The collection of building DTOs in the current page.
        /// </summary>
        public IEnumerable<BuildingDto.Index> Buildings { get; set; } = [];
        
        /// <summary>
        /// The total number of buildings matching the query (for pagination).
        /// </summary>
        public int TotalCount { get; set; }
    }
    
    public class Detail
    {
        public required BuildingDto.Detail Building { get; set; }
    }
}

public static partial class ClassroomResponse
{
    /// <summary>
    /// Response model for paginated index of classrooms.
    /// Includes the list of classrooms and total count for pagination.
    /// </summary>
    public class Index
    {
        /// <summary>
        /// The collection of classroom DTOs in the current page.
        /// </summary>
        public IEnumerable<ClassroomDto.Index> Classrooms { get; set; } = [];
        
        /// <summary>
        /// The total number of classrooms matching the query (for pagination).
        /// </summary>
        public int TotalCount { get; set; }
    }
}

public static partial class InfrastructureResponse
{
    public class Index
    {
            public CampusResponse.Index Campuses { get; set; } = new();
            public BuildingResponse.Index Buildings { get; set; } = new();
            public ClassroomResponse.Index Classrooms { get; set; } = new();
            public RestoResponse.Index Restos { get; set; } = new();
    }
}