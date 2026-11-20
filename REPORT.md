# REPORT.md - Individual Assignment

Keep this concise and specific to your implementation.

## 1. Initial analysis

- How are locations and connections represented in the baseline?
  - Both Locations and Connections are represented as Unsorted Arrays, however when data is fetched from the WorldData it returns as
  a ReadOnlyList.
- Which operations appear frequent?
  - Linear search appears the most frequent, appearing in both FindLocationById and FindOutgoingConnections.
- Which operation(s) do you predict may scale poorly?
  - Linear search on average has a time complexity of O(n) thus will scale poorly as the data set increases.
- What will you investigate first?
  - I will investigate how the world is stored and plan how to improve the efficiency of fetching data

## 2. Data representation

Describe important changes/retained choices, operations affected, and time/memory trade-offs.

> ### World Data
> Storing Nodes as a lists is requiring the program to search each object in the list in order to find a specific node. Similarly, when we want to find a connection to an outgoing node we find that we need to perform another linear search on the connections list. I plan to improve the lookup of nodes and connections in the following ways:
> - Change the list of nodes to a dictionary 
>   - This changes the average lookup time complexity from O(n) to O(1)
>   - incurs miniscule additional cost to memory per data point.
>   - Lookups for non ID data points, such as cost, may incur a performance hit.
> - Change the list of connections to instead have a dictionary with lists in it with the outgoing connections.
>   - Given that connections have the same lookup operation as nodes. Placing all the outgoing connections in a
>   dictionary allows us to find each outgoing connection at O(n) time using only one lookup.
>   - The trade-off for this is that creating new connections requires a lookup for the corresponding 'from' node 
>   before adding an outgoing connection.
>   - Putting lists on the nodes incurs a miniscule overhead in memory, but the amount of data being stored remains
>   the same.

## 3. Algorithms and adaptation

Explain your route finder at a useful high level and how the travel-budget query adapts the same ideas.

> ### Route Finder | A*
> The route finder uses A* to traverse between each node and calculate their lowest cost. We do this with the following steps.
> - Assign a map for the nodes' G-Cost and set the starting node's cost to 0. 
> - Assign a Min Priority Queue and Enqueue the starting node with a priority of 0.
> - Assign a map that links the lowest cost route back to their parents.
> - Begin a loop that breaks there are no nodes left to check:
>   - Take the lowest F-Cost node from the priority queue.
>   - If the G-Cost of this check is greater than the existing node, we skip this node.
>   - Check if the current node is our goal node.
>     - If it is our goal node, reconstruct the route and return a success.
>   - Search each neighboring node of the current node and calculate their corresponding G-Costs.
>   - Check if the node is in the block list and skip if it is.
>   - If the G-Cost of the neighboring node is unassigned or less then the current cost. Replace the G-Cost and assign the parent node to the route map.
>   - Calculate the F-Cost of the neighboring node using the G-Cost + the distance to the goal node.
>   - Use the F-Cost as a priority and add the node to the Priority Queue and also assign the G-Cost of the node as it was added to check for relaxation later.
>   - If it already exists in the queue then reassign the priority of that node.
> - If the goal node is never found, return a failure.
> 
> **Reconstructing the Route**<br>
> - Create a new list and add the goal node to it.
> - Loop through the route map and trace the nodes back to the start node from the goal node. Add the nodes to the list as it does.
> - Add the start node and reverse the list.
>
> **Adaptation for travel-budget.**<br>
> - When adapting to the travel-budget query. The only change we need to make is skipping nodes that are outside the travel-budget.
> - Eventually the frontier will run out of nodes to check outside the travel budget or will find all nodes.
> - Once we do that we can simply return the G-Cost map.

## 4. Complexity

| Operation | Your representation/algorithm | Complexity | Short explanation |
| --- |-------------------------------| --- | --- |
| Find location by ID | Key Value Table Lookup | O(1) on average, collisions may result in O(n) in worst case| Using the ID as a hashed key we can very quickly find the correct node |
| Get outgoing connections | Key Value Table Lookup | O(1) on average, collisions may result in O(n) in worst case | For the same reason as Finding Location, we can return an array of outgoing connections |
| Priority queue enqueue | Insert at top, sift toward the root node| O(log n) using binary heap| Searches up the tree to find position in heap branch |
| Priority queue dequeue-min | Swap last and root, sift towards leaf nodes | O(log n) using binary heap | Swaps the top to maintain invarience, Searches down the tree to find position of swapped entry |
| Route finding| A* (A-star)                              | O(\|V\|)log(\|E\|) with an adjacency graph. Each expanded edge is on average O(log(n)).| Gets outgoing connections from a node and calculates their cost. Then repeats by choosing the lowest cost path until the goal node is reached. A-star introduces a heuristic that allows for a closer aproximation on which node is closer to the goal, thus reducing the amount of total work.|

## 5. Correctness and testing

Important tests/edge cases and what they demonstrate.

> ### Priority Queue
> - `PriorityQueue_EnqueueAndDequeue_ReturnsCorrectOrder()` Ensures that as I add elements to the queue, the queue would not break invariance, regardless of the order I insert them in.
> - `PriorityQueue_EnqueueAndDequeue_HandlesSingleElement()` Ensures the queue does not break if there's only one element remaining in the queue.
> - `PriorityQueue_EnqueueAndDequeue_ThrowsWhenEmpty()` Ensures that the queue cannot be Peeked or Dequeued if the queue is empty.
> ### A* Route Finder
> - I first implemented some basic tests to ensure it would find the specified route I supplied it. `AStar_FindsGoal_CorrectPathAndCost()` makes sure that the path and cost in the supplied data-set match expectations.
> - Edge cases for if the route is the same was important to test as well, so `AStar_StartEqualsGoal_ReturnsZeroCostRoute()` eliminates that uncertainty.
> - I had to ensure that A* would match the cost of Dijkstra. But in order to do that I had to make sure that A* would be able to find the route as if it was Dijkstra first. The `AStar_FindsGoal_BadHeuristicReturnsSameAsDijkstra()` ensures that if A* runs on its own without heuristic it would still function as Dijkstra.
> - I needed to ensure that A* was always taking expanding fewer nodes than Dijkstra to ensure the heuristic was being enforced. `AStar_DijkstraComparison_MatchingPathWithLessExpandedNodes()` Ensures that by checking if the resulting Dijkstra path has more expanded nodes then A*.
> - Lastly I needed to check if A* was actually avoiding blocked nodes. So `AStar_BlocksNodes_DoesNotTraverseBlockedNodes()` would test to see if a specific node was not in the resulting route.
> 

## 6. Performance investigation

Record reproducible before/after measurements for loading/preparation, repeated runtime work, and relevant allocation behavior. Explain what changed and why.

> ### World Data
> Dictionary Lookup significantly improved the lookup time for relevant world data.
>
> Linear Search
> ```json
> Locations: 2500
> Connections: 9800
> Iterations: 5000
> Elapsed: 449 ms
> ```
> Dictionary Lookup
> ```json
> Locations: 2500
> Connections: 9800
> Iterations: 5000
> Elapsed: 0 ms (~4500 ticks)
> ```
> ### Path Finding Differences
> 
> I've been sitting and racking my brain over why the performance metrics for A* and Dijkstra seem to not want to vary. The amount of expanded nodes seems to evaluate to A* being correct, ergo on average O(|V|log|E|) but the difference in performance seems to be on average only a tiny bit slower. Despite the total checked nodes being significantly lower.
> 
> Upon further reflection I've concluded that it might be because of Csharp's Garbage collector. I decided to try turning it off for the tests and got much more favourable results.
> 
> Dijkstra performance metrics<br>
> **Iterations Per World: 100 \| Checksum: 2220012**
> | World Size | Small World | Medium World | Large World |
> |---------------|--------------|-------------|------------|
>| Locations      | 25          | 2500         | 40000       |
>| Connections    | 80          | 9800         | 159200      |
>| Avg. Expanded  | 11          | 1261         | 20173       |
>| Total Cost     | 1379        | 14958        | 58957       |
>| Elapsed Time   | 2        ms | 34        ms | 784      ms |
>
> A-Star performance metrics<br>
> **Iterations Per World: 100 \| Checksum: 1811799**
> | World Size | Small World | Medium World | Large World |
> |---------------|--------------|-------------|------------|
> | Locations      | 25          | 2500         | 40000       |
> | Connections    | 80          | 9800         | 159200      |
> | Avg. Expanded  | 9           | 981          | 15335       |
> | Avg. Relaxed   | 0           | 11           | 1026        |
> | Avg. Checked   | 9           | 992          | 16361       |
> | Total Cost     | 1379        | 14958        | 58957       |
> | Elapsed Time   | 2        ms | 31        ms | 647      ms |
> 
> Even though the difference between Dijkstra and A-Star is only ~17% compared to the estimated up to ~44% it saves on average. Due to the chaotic nature of the built worlds the amount of time A-Star can save isn't actually as much as a more contextual example. 
> 
> After running the test with less chaotic data I can safely determine that my pathfinding algorithm was not the cause for the slow-down. But rather that the data was pathological. The reason for this is probably that the heuristic was not taking enough president when determining the path to the goal due to graph being set on a world grid. This caused only minor increases in cost when traversing away from the goal and resulted in the gCost being more prevalent. 
> 
> Here are the results when using a more spread out node graph. Using a Euclidean Distance Heuristic.
> 
>Dijkstra performance metrics<br>
> **Iterations Per World: 5000 | Checksum: 4495000**
> |                | Small World | Medium World | Large World |
> |---------------|--------------|-------------|------------|
> | Locations      | 8           | 20           | 2500        |
> | Connections    | 18          | 66           | 19404       |
> | Avg. Expanded  | 6           | 22           | 3712        |
> | Total Cost     | 70000       | 275000       | 3430000     |
> | Elapsed Time   | 6 ms | 33 ms | 5298 ms |
>
> A-Star performance metrics<br>
> **Iterations Per World: 5000 | Checksum: 893000**
>| World Size | Small World | Medium World | Large World |
>|---------------|--------------|-------------|------------|
>|               | Small World | Medium World | Large World |
>|Locations      | 8           | 20           | 2500        |
>|Connections    | 18          | 66           | 19404       |
>|Avg. Expanded  | 5           | 13           | 554         |
>|Avg. Relaxed   | 0           | 2            | 247         |
>|Avg. Checked   | 5           | 15           | 801         |
>|Total Cost     | 70000       | 275000       | 3430000     |
>|Elapsed Time   | 6 ms | 18 ms | 1457 ms |
> 
> A* has a very clear ~72% improvement in performance in large worlds in this case compared to Dijkstra. This dataset did require Euclidean distance.
> 
> ### World Creation
> 
> The initial metrics for world creation look like this.
> 
> **Large World Creation Metrics**
> ```json
> Locations: 40000
> Connections: 159200
> Memory Allocation (Bytes): 20,372,624 bytes
> Memory Allocation (Mb): 20 Mb
> Creation Time: 32.9709 ms
> ```
> 
> The main thing that stood out to me is that connections do not need a `FromLocationId`. This is because the `FromLocationId` is already on the node when performing pathfinding algorithms. So instead of creating all the connections separately and adding them to the world data post creation would be more efficient.
> 
> **Large World Creation Metrics**
> ```json
>Locations: 40000
>Connections: 159200
>Memory Allocation (Bytes): 14,904,256 bytes
>Memory Allocation (Mb): 15 Mb
>Creation Time: 15,5484 ms
> ```
> 
> Not only did this create significantly less data than before, it also improved performance by a very noticeable amount too. Due to us not having to go through the step of populating `WorldData` with connections anymore. This saves space without losing any information since if we need to retrieve the connections, we already know the `FromLocationId`. And if we need to iterate to find by any other information. We have the key already saved on the Dictionary KeyValue pair.
> 
> Knowing this we can also extend this logic to `Location`. We can remove the ID from location to save additional data, and we can instead store the Id as the Key in the KeyValue pair in the dictionary. Saving additional space.
> 
> **Large World Creation Metrics**
> ```json
>Locations: 40000
>Connections: 159200
>Memory Allocation (Bytes): 14,264,160 bytes
>Memory Allocation (Mb): 14 Mb
>Creation Time: 14,6792 ms
> ```
> 
> With this I've managed to get the total down to roughly 14 mb while also giving a tiny bit more performance due to not having to populate the lists in the dictionary.

## 7. Development reflection

Describe one bug, wrong assumption, failed approach, or surprising result and how you investigated it.

> ### World Data
> I initially tried to add the outgoing connections on the nodes themselves. However, when trying to add connections. 
> I realized I would have to break encapsulation of the node and world data and allowed others to add connections to
> what was supposed to be a complete set of data. I backtracked a bit and decided to place the data as separate lists
> a dictionary instead.
> ### A* Pathfinding
> When attempting to implement A* pathfinding I first attempted to create a MinHeap combined with Dictionary, or a HashHeap if you will. In Theory, the benefit of this would be able to skip relaxing nodes if the gCost exceeded the existing node's gCost. I could simply update the priority of the existing frontier nodes. However, through performance testing this didn't pan out. The extra Dictionary lookups and having to resift the MinHeap node ended up only costing the algorithm more. I could potentially see this working with a better implementation of a PriorityQueue.

## 8. Tools and AI assistance

Briefly describe relevant assistance, if any.
> I mostly used it was to describe unfamiliar terms to me that I couldn't find a concise explanation on in either Wikipedia or other websites.<br>
> Other cases were for helping describe common edge cases for A* or Dijkstra for testing implementation.

## 9. VG extension

Only if attempting VG: heuristic, traversal constraints, correctness, and Dijkstra/A* comparison.

> ### A-Star Implementation
> 
> **Heuristic**<br>
> The heuristic I used depended on the graph supplied. Using manhatten distance made sense as the performance of this calculation was fast. But often only worked on a grid in which you could only traverse in cardinal directions. When attempting the same heuristic on a more open-ended world, the heuristic would fail to retrieve the optimal path. In that case Euclidean distance would have to be used. However, if the world data supplied had a lot of branching paths with more random costs, the Euclidean distance heuristic may end up becoming slower than the brute forcing of Dijkstra.
> 
> **Traversal Constraints**<br>
> Traversal constrains was easy to implement. Whenever we encounter the node when checking the neighboring nodes, we simply skip the node instead and do not add it to the frontier.
> 
> **Correctness**<br>
> To test correctness of A-Star, the bulk of testing was to ensure the resulting route that it retrieved had a matching cost to that of Dijkstra. While the  route could vary to that of Dijkstra's in some circumstances due to A-Star prioritizing the heuristic, the overall cost remained the same in all tested cases. Ensuring it took a different route when a specific node was blocked was also important to test.
> 
> **Comparison to A-Star**<br>
> In almost all cases A* performed better in 1 metric. The amount of expanded nodes was a lot lower. However, because of the cost of traversing the neighboring nodes is higher due to the heuristic. There is a chance that in a pathological data-set, A* can perform worse than Dijkstra. In cases like this the heuristic would either be pointless in the worst case or need reevaluation in others.