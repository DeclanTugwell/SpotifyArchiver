# SpotifyArchiver

Used as a University Project. Involves experimenting with different Ai CLI practices for software development. The outcome of which will produce a console application with the functionality of archiving Spotify Playlists to local json files.

## Experiment Trace

|Practice ID| Practice| Branch Name | Contextual Link (ID) | Completion Time (Hrs) | Error Count | Layer Separation Score | CQRS Compliance | Outcome Summary | Classification|
|-|-|-|-|-|-|-|-|-|-|
|QueryPlaylists-ControlA| N/A |  |  |  |  |  |  |  | |
|QueryPlaylists-ControlB| N/A |  |  |  |  |  |  |  | |
|QueryPlaylists-Experimental| Persistent Context File for Multi-Turn Accuracy |  |  |  |  |  |  |  | |
|ArchivePlaylist-ControlA| N/A |  |  |  |  |  |  |  | |
|ArchivePlaylist-ControlB| N/A |  |  |  |  |  |  |  | |
|ArchivePlaylist-Experimental| Prompt Scaffolding for Complex Feature Delivery |  |  |  |  |  |  |  | |
|FetchPlaylistDetails-ControlA| N/A |  |  |  |  |  |  |  | |
|FetchPlaylistDetails-ControlB| N/A |  |  |  |  |  |  |  | |
|FetchPlaylistDetails-Experimental| Code Explanation and Variant Generation |  |  |  |  |  |  |  | |
|ListSongsFromArchive-ControlA| N/A |  |  |  |  |  |  |  | |
|ListSongsFromArchive-ControlB| N/A |  |  |  |  |  |  |  | |
|ListSongsFromArchive-Experimental| Assertion-First Prompting for Test-Driven Development |  |  |  |  |  |  |  | |
|DeleteArchivedPlaylist-ControlA| N/A |  |  |  |  |  |  |  | |
|DeleteArchivedPlaylist-ControlB| N/A |  |  |  |  |  |  |  | |
|DeleteArchivedPlaylist-Experimental| Agile-Integrated Branching and Review Flow |  |  |  |  |  |  |  | |
|SpotifyAuthenticationFlow-ControlA| N/A |  |  |  |  |  |  |  | |
|SpotifyAuthenticationFlow-ControlB| N/A | 11-SpotifyAuthenticationFlow-ControlB | N/A | 1 hour 23 minutes | 1 error encountered at runtime | FAIL | PASS | Layer Separation Compliance failed due to SpotifyArchiver.Application.Abstraction containing the third party dependancy SpotifyAPI.Web even though this is specific to the implementation. This is because DTOs were not created to represent the models used by the dependancy. | FAIL |
|SpotifyAuthenticationFlow-Experimental| Layer-Scoped Sub-Tasking for Abstraction Enforcement |  |  |  |  |  |  |  | |

## Implementation Checklist

### 1. Pre-Run Setup

- **Git Branch Reset:**  
  Fork from Control A's previous feature branch to the base branch for that feature (no contamination).  
- **Environment Parity Check:**  
  Verify .NET version, tool versions, OS match fixed variables.  

### 2. Implementation Phase

- **Start Completion Time Clock** (from first line of code written).
- **Log Prompt/Turn Counts** (if Experimental run).
- **Capture Prompt & Output Screenshots** for evidence.

### 3. Compilation & Tests

- **Stop Clock** after first full compile+run with working feature.  
- **Record Error Count** (compile & runtime, even if resolved).
- **Save Unit Test Run Output**.

### 4. Static Analysis - Automated Layer Separation

- Run automated layer separation check  
  (we'll set up NDepend / Roslyn Analyzer later - produces **Layer Separation Score %**).

### 5. Manual CQRS PR Review

Performed during Pull Request peer review stage.

- **Command Handlers contain no read operations** (no query repo use, no SELECTs).  
- **Query Handlers contain no write operations** (no command repo, no inserts/updates).  
- **Correct Interface Use:**  
  - `CommandHandler` implements `ICommandHandler`  
  - `QueryHandler` implements `IQueryHandler`
- **No violation of CQRS read/write separation patterns**.
- Record judgment in experiment log as `CQRS Pass` or `CQRS Fail` plus notes.

### 6. Final Outcome Summary

- Observations on ease/difficulty, AI code suggestions, integration pain points.
- PR review comments.
- Store artefacts in `/evidence/<feature>` (code, prompts, outputs, test results, static analysis report).

---

### 7. TAM Survey Post-Review

- Perceived Usefulness score (1-5)
- Perceived Ease of Use score (1-5)
- Optional comments.

### 8. Pass/Fail Classification

- Pass if:  
  **Layer Separation Score ≥ 90%**  
  **Manual CQRS review: Pass**  
  Error count ≤ 2 non-blocking.