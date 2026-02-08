# APS 與 模擬器通訊協議 (v2.5)

## 1. 基本規則
*   **傳輸層**：TCP/IP
*   **埠號**：預設 5000 (APS 為 Server, 模擬器為 Client)
*   **結束符號**：`;` (Semicolon)
*   **資料格式**：CSV 字串 (Comma Separated)

## 2. 模擬器 -> APS (指令)

### 入庫請求 (自動分配)
*   **格式**：`IN,{CarrierID};`
*   **說明**：模擬器刷 Barcode 後請求入庫。APS 會自動尋找空位並回傳 `ASSIGNED_PORT`。

### 入庫請求 (指定位置)
*   **格式**：`IN,{PortID},{CarrierID};`
*   **說明**：手動模式使用，將卡匣放入指定 Port。

### 卡匣取出 (Pick)
*   **格式**：`PICK,{PortID};`
*   **說明**：模擬人員或手臂將卡匣從貨架拿走。APS 會將狀態移至 Transit。

### 進入機台 (Enter)
*   **格式**：`ENTER,{EqpID};`
*   **說明**：卡匣抵達機台並進站。APS 接收到此指令後會立即觸發 Transit 移除檢查。

---

## 3. APS -> 模擬器 (指令)

### 開啟門鎖 (Open / Dispatch)
*   **格式**：`OPEN,{PortID},{TargetEqpID};`
*   **說明**：APS 決策引擎下達指令，要求人員取走卡匣送往目標機台。
*   **特殊目標**：`Pass99` 代表該站點未定義機台，要求模擬器自動跳過。

### 分配儲位回饋 (Assigned Port)
*   **格式**：`ASSIGNED_PORT,{PortID},{CarrierID};`
*   **說明**：回應模擬器的自動入庫請求，告知最終分配的 Port 位置。

### 心跳包 (Heartbeat)
*   **格式**：`HEART;`
*   **說明**：每 5 秒維持連線狀態。
