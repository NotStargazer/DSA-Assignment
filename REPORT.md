# REPORT.md - Individual Assignment

Keep this concise and specific to your implementation.

## 1. Initial analysis

- How are locations and connections represented in the baseline?
  - Both Locations and Connections are represented as Unsorted Arrays, however when data is fetched from the WorldData it returns as
  a ReadOnlyList.
- Which operations appear frequent?
  - Linear search appears the most frequent, appearing in both FindLocationById and FindOutgoingConnections.
- Which operation(s) do you predict may scale poorly?
  - Linear search on average has an time complexity of O(n) thus will scale poorly as the data set increases.
- What will you investigate first?
  - I will investigate how the world is stored and plan how to improve the efficiency of fetching data

## 2. Data representation

Describe important changes/retained choices, operations affected, and time/memory trade-offs.

> ### World Data
> Storing Nodes as a lists is requiring the program to search each object in the list in order to find a spesific
node. Similarly, when we want to find a connection to an outgoing node we find that we need to perform another linear
search on the connections list. I plan to improve the lookup of nodes and connections in the following ways:
> - Change the list of nodes to a dictionary 
>   - This changes the average lookup time complexity from O(n) to O(1)
>   - incurs miniscule additional cost to memory per data point.
>   - Lookups for non ID data points, such as cost, may incur a performance hit.
> - Change the list of connections to instead have a dictionary with lists in it with the outgoing connections.
>   - Given that connections have the same lookup operation as nodes. Placing all the outgoing connections in a
>   dictionary allows us to find each outgoing connection at O(n) time using only one lookup.
>   - The trade-off for this is that creating new connections requires a lookup for the corresponding 'from' node 
>   before adding an outgoing connection.
>   - Putting lists on the nodes incours a miniscule overhead in memory, but the amount of data being stored remains
>   the same.

## 3. Algorithms and adaptation

Explain your route finder at a useful high level and how the travel-budget query adapts the same ideas.



## 4. Complexity

| Operation | Your representation/algorithm | Complexity | Short explanation |
| --- |-------------------------------| --- | --- |
| Find location by ID | Dictionary Lookup | O(1) | Using the ID as a hashed key we can very quickly find the correct node |
| Get outgoing connections | Dictionary Lookup | O(1) | For the same reason as Finding Location, we can return an array of outgoing connections |
| Priority queue enqueue | Insert at top, sift toward the root | O(log n) | Searches up the tree to find position in heap branch |
| Priority queue dequeue-min | Swap last and root, sift towards leaves | O(log n) | Swaps the top to maintain invarience, Searches down the tree to find position of swapped entry |
| Route finding |                               | | |

## 5. Correctness and testing

Important tests/edge cases and what they demonstrate.



## 6. Performance investigation

Record reproducible before/after measurements for loading/preparation, repeated runtime work, and relevant allocation behavior. Explain what changed and why.

## 7. Development reflection

Describe one bug, wrong assumption, failed approach, or surprising result and how you investigated it.

## 8. Tools and AI assistance

Briefly describe relevant assistance, if any.

## 9. VG extension

Only if attempting VG: heuristic, traversal constraints, correctness, and Dijkstra/A* comparison.
