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

## 3. Algorithms and adaptation

Explain your route finder at a useful high level and how the travel-budget query adapts the same ideas.

## 4. Complexity

| Operation | Your representation/algorithm | Complexity | Short explanation |
| --- |-------------------------------| --- | --- |
| Find location by ID |                               | | |
| Get outgoing connections |                               | | |
| Priority queue enqueue |                               | | |
| Priority queue dequeue-min |                               | | |
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
