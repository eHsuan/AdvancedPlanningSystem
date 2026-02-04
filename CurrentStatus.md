# AdvancedPlanningSystem Project Analysis Report

## 1. Project Overview
*   **Project Name:** AdvancedPlanningSystem
*   **Target Framework:** .NET Framework 4.5.2
*   **Application Type:** Windows Forms (WinForms)
*   **Status:** Functional Prototype (Mock Data)

## 2. Architecture & Design
The application follows a monolithic WinForms architecture, utilizing User Controls for reusable UI components.

*   **Main Window (`FormMain.cs`):**
    *   Acts as the central dashboard.
    *   Dynamically generates a grid of storage shelves based on configuration (`AppConfig.cs`).
    *   Hosts `PortControl` instances to visualize individual storage locations.
*   **UI Components:**
    *   `PortControl.cs`: A custom UserControl representing a physical port/cassette slot. Handles its own rendering and state (Empty, Occupied, Dispatching, Error).
    *   `GlobalMonitorForm.cs`: A system-wide monitoring dashboard displaying a "Leaderboard" of dispatch tasks.
    *   `CassetteDetailForm.cs`: A detailed view for a specific cassette, showing scoring breakdown and queue status.
*   **Data Layer:**
    *   `MockDataService.cs`: Currently serves hardcoded mock data for demonstration and UI testing.
    *   `LeaderboardItem.cs`: Data model for the dispatching leaderboard.
*   **Configuration:**
    *   `AppConfig.cs`: Static configuration class (e.g., Grid dimensions).

## 3. Documentation (`doc/`)
The project contains design documentation which should be the guide for future implementation:
*   **Architecture:** `系統架構v2.png`, `資料庫架構v3.png`
*   **Algorithms:**
    *   `派貨演算法V2.docx` (Dispatching Algorithm)
    *   `批次與強制派貨決策邏輯.png` (Batch & Forced Dispatch Logic)
    *   `優先序權重v2.png` (Priority Weights)
    *   `派貨流程圖v2.png` (Dispatch Workflow)

## 4. Current Functionality
*   **Visual Dashboard:** Renders a 4x5 grid (configurable) of ports.
*   **Status Indication:** Ports change color based on status (Empty=Gray, Occupied=Blue, Dispatching=Green).
*   **Interactivity:** Clicking a port opens the Detail View.
*   **Monitoring:** The Global Monitor displays a sorted list of tasks with visual highlighting for top priority items.

## 5. Observations & Recommendations
*   **Mock Data Dependency:** The system is currently fully decoupled from real data sources. The `MockDataService` needs to be replaced with a real database access layer implementing the logic found in `doc/資料庫架構v3.png`.
*   **Algorithm Implementation:** The core logic for dispatching and scoring is simulated. The algorithms described in `doc/演算法相關` need to be implemented in C#.
*   **WinForms Best Practices:** The project uses `DoubleBuffered` to reduce flickering, which is good.
*   **Build Status:** The project builds successfully with `dotnet build`.

## 6. Next Steps
1.  **Database Integration:** Create the database schema based on the design docs and implement a Data Access Layer (DAL).
2.  **Algorithm Implementation:** Translate the "Dispatching Algorithm V2" and "Priority Weights" into a C# logic engine.
3.  **Real-time Updates:** Implement a mechanism to refresh data (e.g., Timer or Events) instead of static loading.
