# APS 與 MES API 通訊協議 (v1.2)

## 1. 基本規則
*   **協定**：HTTP/REST
*   **格式**：JSON
*   **授權**：NTLM/Windows 驗證 (視環境設定)

## 2. 核心 API 端點

### 批量 WIP 查詢
*   **URL**: `GET /api/wip/batch?ids=EQP01,EQP02`
*   **用途**: 取得多台機台的當前 WIP 數量與最大容量。
*   **關鍵欄位**: `current_wip_qty`, `max_wip_qty`

### 機台狀態查詢
*   **URL**: `GET /api/equipment/status/batch?ids=EQP01,EQP02`
*   **用途**: 取得機台即時狀態 (RUN, IDLE, DOWN)。
*   **關鍵欄位**: `status`, `duration`

### 工單資訊查詢
*   **URL**: `GET /api/order/batch?ids=LOT01,LOT02`
*   **用途**: 取得卡匣所在的當前站點、下一站點及流程編號。
*   **關鍵欄位**: `step_id`, `next_step_id`, `route_id`, `prev_out_time`

### Q-Time 規則載入
*   **URL**: `GET /api/qtime/all`
*   **用途**: 系統啟動時同步全局 Q-Time 限制。

---

## 3. 同步機制
*   **自動同步**：根據 `AppConfig.SyncIntervalSec` 設定（預設 30秒）自動循環。
*   **即時觸發**：當收到硬體 `ENTER` 指令時，會立即執行針對特定 Lot 的同步以加速 Transit 移除。
