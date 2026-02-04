# AdvancedPlanningSystem 專案進度報告

## 1. 專案概況
*   **專案名稱**：AdvancedPlanningSystem (APS)
*   **目標框架**：.NET Framework 4.5.2
*   **應用類型**：Windows Forms (WinForms)
*   **目前階段**：**功能完整的資料驅動系統 (Data-Driven)**

## 2. 核心架構進度 (Architecture Status)

### A. 資料庫與儲存 (Data Layer) - [100% 完成]
*   **APSLocalDB.db**：實作機台配置、Port 狀態、帳料綁定資料表。
*   **APSCloudDB.db**：實作歷史 Log 紀錄。
*   **ExternalDB.db**：實作外部資料查詢（Barcode 轉 WorkNo）。
*   **Repositories**：已完成 `ApsLocalDbRepository` 與 `ApsCloudDbRepository`，不再依賴 Mock 資料。

### B. 核心服務 (Service Layer) - [90% 完成]
*   **DataSyncService**：
    *   ✅ MES 資料定期同步。
    *   ✅ 評分演算法 (Scoring)：實作 QTime 剩餘時間計算、優先序權重。
    *   ✅ 系統狀態恢復 (Recovery Logic)。
*   **DispatchService**：
    *   ✅ 派貨決策邏輯：實作滿批 (Full Batch) 與 強制派貨 (Forced Dispatch) 判斷。
    *   ✅ 硬體控制串接：透過 TCP 發送 `OPEN` 等指令。
*   **ExternalDataService**：
    *   ✅ 完成外部資料對照查詢功能。

### C. 外部整合 (Integration) - [85% 完成]
*   **MES 介面**：實作 `IMesService`，支援 HTTP 實際串接與 Mock 模擬。
*   **機台通訊**：`TcpServerModule` 已具備發送指令至 AGV/機台的能力。
*   **模擬器 (APS_Simulator)**：可獨立運行，模擬硬體與 MES 行為。

## 3. UI 介面進度 (UI Status) - [95% 完成]
*   **主畫面 (`FormMain`)**：動態生成 Port 網格，支援即時狀態顏色顯示。
*   **全域監控 (`GlobalMonitorForm`)**：即時顯示派貨清單與排行榜。
*   **詳細資訊 (`CassetteDetailForm`)**：顯示單一 Cassette 的評分細節。

## 4. 待處理事項 (Remaining Tasks)
1.  **整合壓力測試**：驗證在大量 Port (例如 100+) 下的評分效能。
2.  **異常處理強化**：針對 TCP 通訊中斷或 MES 回傳異常的自動恢復機制。
3.  **UI 繁體中文語系優化**：確保所有提示與狀態皆符合使用者語系。

---
*最後更新日期：2026年2月4日*