using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    /// <summary>
    /// Describes the result of a route-search operation.
    /// </summary>
    public sealed class RouteResult
    {
        public RouteResult(bool found, IReadOnlyList<int>? locationIds, int totalCost, int expandedCount, int relaxedNodes = 0)
        {
            Found = found;
            LocationIds = locationIds;
            TotalCost = totalCost;
            ExpandedCount = expandedCount;
            RelaxedNodes = relaxedNodes;
        }

        /// <summary>
        /// True when a route to the requested goal was found.
        /// </summary>
        public bool Found { get; }

        /// <summary>
        /// Location IDs making up the resulting route, in traversal order.
        /// For a successful route, this includes the start and goal.
        /// </summary>
        public IReadOnlyList<int>? LocationIds { get; }

        /// <summary>
        /// Sum of the connection travel costs along the resulting route.
        /// </summary>
        public int TotalCost { get; }

        /// <summary>
        /// Number of locations expanded by the search.
        ///
        /// This can be useful when comparing search behavior independently
        /// of wall-clock timing.
        /// </summary>
        public int ExpandedCount { get; }
        public int RelaxedNodes { get; }

        public static RouteResult NotFound(int expandedCount) => 
            new RouteResult(false, null, int.MaxValue, expandedCount);
        public static RouteResult Success(IReadOnlyList<int> route, int expandedCount, int totalCost) => 
            new RouteResult(true, route, totalCost, expandedCount);
        public static RouteResult SuccessAStar(IReadOnlyList<int> route, int expandedCount, int relaxedNodes, int totalCost) => 
            new RouteResult(true, route, totalCost, expandedCount, relaxedNodes);
    }
}
